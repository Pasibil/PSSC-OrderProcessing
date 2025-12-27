# Pași pentru Testare Finală

## 1. Configurare Azure Service Bus (OBLIGATORIU)

### Opțiunea A: Azure Portal (Recomandat - 10 minute)

1. **Intră în Azure Portal**: https://portal.azure.com

2. **Creează Service Bus Namespace**:
   - Click "Create a resource"
   - Caută "Service Bus"
   - Click "Create"
   - **Subscription**: Alege subscription-ul tău
   - **Resource Group**: Creează nou sau folosește existent
   - **Namespace name**: `orderprocessing-pssc` (sau alt nume unic)
   - **Location**: West Europe
   - **Pricing tier**: **Standard** (necesar pentru Topics, ~€9.17/lună, șterge după curs)
   - Click "Review + Create" → "Create"
   - **Așteaptă 2-3 minute** până se creează

3. **Creează Topic**:
   - Deschide namespace-ul creat
   - În meniul stâng: "Topics"
   - Click "+ Topic"
   - **Name**: `orders` (exact așa, lowercase!)
   - Lasă restul default
   - Click "Create"

4. **Creează Subscriptions**:
   - Click pe topic-ul "orders"
   - Click "+ Subscription"
   - **Name**: `invoicing-subscription` (exact așa!)
   - Click "Create"
   - Click "+ Subscription" din nou
   - **Name**: `shipping-subscription` (exact așa!)
   - Click "Create"

5. **Copiază Connection String**:
   - În namespace (nu în topic!), click "Shared access policies" în stânga
   - Click pe "RootManageSharedAccessKey"
   - Copiază **Primary Connection String**
   - Ar trebui să arate așa: `Endpoint=sb://orderprocessing-pssc.servicebus.windows.net/;SharedAccessKeyName=...`

### Opțiunea B: Azure CLI (Rapid - 2 minute)

```bash
# Login
az login

# Creează resource group (dacă nu există)
az group create --name pssc-lab --location westeurope

# Creează Service Bus namespace
az servicebus namespace create \
  --resource-group pssc-lab \
  --name orderprocessing-pssc \
  --location westeurope \
  --sku Standard

# Creează topic
az servicebus topic create \
  --resource-group pssc-lab \
  --namespace-name orderprocessing-pssc \
  --name orders

# Creează subscriptions
az servicebus topic subscription create \
  --resource-group pssc-lab \
  --namespace-name orderprocessing-pssc \
  --topic-name orders \
  --name invoicing-subscription

az servicebus topic subscription create \
  --resource-group pssc-lab \
  --namespace-name orderprocessing-pssc \
  --topic-name orders \
  --name shipping-subscription

# Obține connection string
az servicebus namespace authorization-rule keys list \
  --resource-group pssc-lab \
  --namespace-name orderprocessing-pssc \
  --name RootManageSharedAccessKey \
  --query primaryConnectionString \
  --output tsv
```

## 2. Configurează Connection String cu User Secrets

**IMPORTANT**: Pentru securitate, connection string-ul este salvat în **User Secrets**, NU în appsettings.json.

### Configurare automată (recomandat)

Rulează aceste comenzi pentru cele **3 proiecte**:

```bash
# Pentru API
dotnet user-secrets set "ServiceBus:ConnectionString" "PASTE_YOUR_CONNECTION_STRING_HERE" --project src/OrderProcessing.Api/OrderProcessing.Api.csproj

# Pentru Invoicing Worker
dotnet user-secrets set "ServiceBus:ConnectionString" "PASTE_YOUR_CONNECTION_STRING_HERE" --project src/OrderProcessing.Invoicing.Worker/OrderProcessing.Invoicing.Worker.csproj

# Pentru Shipping Worker
dotnet user-secrets set "ServiceBus:ConnectionString" "PASTE_YOUR_CONNECTION_STRING_HERE" --project src/OrderProcessing.Shipping.Worker/OrderProcessing.Shipping.Worker.csproj
```

**Înlocuiește `PASTE_YOUR_CONNECTION_STRING_HERE`** cu connection string-ul tău real din Azure (din pasul 1).

### De ce User Secrets?

- ✅ Connection string-ul nu apare pe GitHub (securitate)
- ✅ GitHub Push Protection nu mai blochează commit-urile
- ✅ Fiecare developer își configurează local propriile credențiale
- ✅ Production folosește Azure Key Vault sau variabile de mediu

**Vezi [AZURE_SETUP.md](../AZURE_SETUP.md) pentru detalii complete.**

## 3. Pornește Toate Serviciile

Deschide **5 terminale PowerShell** separate:

### Terminal 1 - OrderProcessing API
```powershell
cd "d:\Laboratoare PSSC CSharp\Proiect-Implementare\src\OrderProcessing.Api"
dotnet run
```
**Port**: 5259  
**Status așteptat**: "Now listening on: http://localhost:5259"

### Terminal 2 - Invoicing API
```powershell
cd "d:\Laboratoare PSSC CSharp\Proiect-Implementare\src\OrderProcessing.Invoicing"
dotnet run
```
**Port**: 5260  
**Status așteptat**: "Now listening on: http://localhost:5260"

### Terminal 3 - Shipping API
```powershell
cd "d:\Laboratoare PSSC CSharp\Proiect-Implementare\src\OrderProcessing.Shipping"
dotnet run
```
**Port**: 5261  
**Status așteptat**: "Now listening on: http://localhost:5261"

### Terminal 4 - Invoicing Worker
```powershell
cd "d:\Laboratoare PSSC CSharp\Proiect-Implementare\src\OrderProcessing.Invoicing.Worker"
dotnet run
```
**Status așteptat**: 
```
info: OrderProcessing.Invoicing.Worker.EventProcessorWorker[0]
      Invoicing Worker starting...
info: OrderProcessing.Invoicing.Worker.EventProcessorWorker[0]
      Invoicing Worker started and listening for events
```

### Terminal 5 - Shipping Worker
```powershell
cd "d:\Laboratoare PSSC CSharp\Proiect-Implementare\src\OrderProcessing.Shipping.Worker"
dotnet run
```
**Status așteptat**:
```
info: OrderProcessing.Shipping.Worker.EventProcessorWorker[0]
      Shipping Worker starting...
info: OrderProcessing.Shipping.Worker.EventProcessorWorker[0]
      Shipping Worker started and listening for events
```

## 4. Testează Flow-ul Complet

### Test 1: Plasează o Comandă

În **Terminal 6** (nou):
```powershell
$body = @{
    customerName = "John Doe"
    customerEmail = "john@example.com"
    orderLines = @(
        @{
            productCode = "WIDGET"
            quantity = 2
        }
    )
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5259/api/orders" -Method Post -Body $body -ContentType "application/json"
```

**Răspuns așteptat**:
```json
{
  "success": true,
  "orderId": "guid-aici",
  "orderDetails": {
    "orderId": "guid-aici",
    "customerName": "John Doe",
    "customerEmail": "john@example.com",
    "orderLines": [...],
    "totalAmount": 20.0
  }
}
```

### Test 2: Verifică Log-urile

**Terminal 1 (API)** - Ar trebui să vezi:
```
info: OrderProcessing.Api.Controllers.OrdersController[0]
      Published OrderPlacedEvent for Order {OrderId}
```

**Terminal 4 (Invoicing Worker)** - Ar trebui să vezi:
```
info: OrderProcessing.Events.ServiceBus.ServiceBusTopicEventListener[0]
      Received cloud event {EventId} of type OrderPlacedEvent
info: OrderProcessing.Invoicing.Worker.OrderPlacedEventHandler[0]
      Processing OrderPlacedEvent for Order {OrderId}
info: OrderProcessing.Invoicing.Worker.OrderPlacedEventHandler[0]
      Successfully generated invoice for Order {OrderId}
```

**Terminal 5 (Shipping Worker)** - Ar trebui să vezi:
```
info: OrderProcessing.Events.ServiceBus.ServiceBusTopicEventListener[0]
      Received cloud event {EventId} of type OrderPlacedEvent
info: OrderProcessing.Shipping.Worker.OrderPlacedEventHandler[0]
      Processing OrderPlacedEvent for Order {OrderId}
info: OrderProcessing.Shipping.Worker.OrderPlacedEventHandler[0]
      Successfully created shipment for Order {OrderId}
```

### Test 3: Verifică Factura Generată

```powershell
Invoke-RestMethod -Uri "http://localhost:5260/api/invoices" -Method Get
```

**Așteptat**: Factura nou creată ar trebui să apară în listă.

### Test 4: Verifică Shipping-ul Generat

```powershell
Invoke-RestMethod -Uri "http://localhost:5261/api/shipping" -Method Get
```

**Așteptat**: Shipping-ul nou creat ar trebui să apară în listă.

## 5. Verificare Completă

✅ **Checklist Final**:
- [ ] API returnează success la POST /api/orders
- [ ] API loghează "Published OrderPlacedEvent"
- [ ] Invoicing Worker loghează "Processing OrderPlacedEvent"
- [ ] Invoicing Worker loghează "Successfully generated invoice"
- [ ] Shipping Worker loghează "Processing OrderPlacedEvent"
- [ ] Shipping Worker loghează "Successfully created shipment"
- [ ] GET /api/invoices returnează factura nouă
- [ ] GET /api/shipping returnează shipping-ul nou

## 6. Troubleshooting

### Eroare: "Could not connect to Service Bus"
- Verifică connection string-ul în appsettings.json
- Verifică că namespace-ul există în Azure Portal
- Verifică că topic-ul "orders" există

### Eroare: "Subscription not found"
- Verifică că "invoicing-subscription" și "shipping-subscription" există în topic
- Numele trebuie să fie **exact** cum sunt în cod (lowercase, cu cratimă)

### Worker-ul nu pornește
- Verifică că ServiceBusClient este injectat corect în Program.cs
- Verifică că există reference la OrderProcessing.Events.ServiceBus

### Event-urile nu ajung la workers
- Verifică că toate 5 serviciile rulează
- Verifică log-urile API-ului - trebuie să vadă "Published OrderPlacedEvent"
- Verifică Azure Portal → Topic → Subscriptions → Message count

### Worker loghează erori la procesare
- Verifică că Invoicing API (5260) și Shipping API (5261) rulează
- Verifică că endpoint-urile sunt corecte în OrderPlacedEventHandler

## 7. Demo pentru Curs

Când prezinți:
1. Arată toate 5 terminalele rulând
2. Trimite un POST /api/orders
3. Arată cum log-urile apar **instant** în workers
4. Arată factura și shipping-ul generate automat
5. Explică: "Comunicarea este **asincronă** prin Azure Service Bus - API-ul nu așteaptă workers, ei procesează independent"

## 8. Curățenie (După Testare)

Pentru a opri toate serviciile:
- `Ctrl+C` în fiecare terminal

Pentru a șterge resursele Azure (ca să nu plătești):
```bash
az group delete --name pssc-lab --yes
```

Sau din Portal: Delete resource group "pssc-lab"

---

**Cost estimat Azure**: ~€0.30 pentru testare (câteva ore cu Standard tier)  
**Timp total setup**: 10-15 minute  
**Status cerință curs**: ✅ Comunicare asincronă implementată și testată cu succes  
**Status implementare**: ✅ Event-Driven Architecture funcțională cu Azure Service Bus  
**Status securitate**: ✅ Connection strings în User Secrets, nu pe GitHub
