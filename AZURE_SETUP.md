# Configurare Azure Service Bus

Pentru ca aplicația să funcționeze, trebuie să configurezi connection string-ul pentru Azure Service Bus.

## Connection String-ul este salvat în User Secrets (nu pe GitHub)

După ce clonezi acest repository, trebuie să configurezi connection string-ul local folosind User Secrets:

### Pasul 1: Configurează User Secrets pentru cele 3 proiecte

```bash
# Pentru API
dotnet user-secrets set "ServiceBus:ConnectionString" "PASTE_CONNECTION_STRING_HERE" --project src/OrderProcessing.Api/OrderProcessing.Api.csproj

# Pentru Invoicing Worker
dotnet user-secrets set "ServiceBus:ConnectionString" "PASTE_CONNECTION_STRING_HERE" --project src/OrderProcessing.Invoicing.Worker/OrderProcessing.Invoicing.Worker.csproj

# Pentru Shipping Worker
dotnet user-secrets set "ServiceBus:ConnectionString" "PASTE_CONNECTION_STRING_HERE" --project src/OrderProcessing.Shipping.Worker/OrderProcessing.Shipping.Worker.csproj
```

### Pasul 2: Obține Connection String-ul din Azure

1. Deschide [Azure Portal](https://portal.azure.com)
2. Mergi la Service Bus Namespace: **orderprocessing-pssc-aleksandru** (France Central)
3. În meniul din stânga, selectează **Shared access policies**
4. Selectează **RootManageSharedAccessKey**
5. Copiază **Primary Connection String**
6. Înlocuiește `PASTE_CONNECTION_STRING_HERE` din comenzile de mai sus cu connection string-ul copiat

### Pasul 3: Verifică configurația

Connection string-ul va arăta cam așa:
```
Endpoint=sb://orderprocessing-pssc-aleksandru.servicebus.windows.net/;SharedAccessKeyName=RootManageSharedAccessKey;SharedAccessKey=...
```

**IMPORTANT**: Nu adăuga niciodată connection string-ul direct în fișierele appsettings.json pentru că GitHub va bloca push-ul (secret protection).

## Resurse Azure

- **Namespace**: orderprocessing-pssc-aleksandru
- **Region**: France Central
- **Tier**: Standard (pentru Topic support)
- **Topic**: orders
- **Subscriptions**: invoicing-subscription, shipping-subscription

## Cost estimat

- ~€9.17/lună pentru Standard tier
- ~€0.30 pentru perioada de testare
- Șterge namespace-ul după evaluare pentru a opri costurile
