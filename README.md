# Order Processing System - Sistem de Procesare Comenzi

## 📋 Despre Proiect

Acesta este un sistem software modern pentru gestionarea completă a comenzilor online, de la preluarea inițială până la expedierea produselor către client. Proiectul este conceput folosind principiile **Domain-Driven Design (DDD)** și o arhitectură bazată pe **microservices**, asigurând scalabilitate, mentenabilitate și separare clară a responsabilităților.

## 🎯 Ce Rezolvă Acest Sistem?

Sistemul automatizează întregul flux al unei comenzi online:

1. **Preluarea Comenzii** - Clientul plasează o comandă cu produsele dorite
2. **Validarea** - Sistemul verifică dacă datele sunt corecte și produsele sunt disponibile
3. **Calculul Prețului** - Se calculează automat prețul total inclusiv toate taxele
4. **Facturarea** - Se generează automat o factură fiscală
5. **Expedierea** - Se pregătește și se trimite comanda către client

## 🏗️ Arhitectura Sistemului

Sistemul este împărțit în **trei contexte independente** (bounded contexts), fiecare având responsabilități clare:

### 1. Order Taking Context (Contextul Preluării Comenzilor)
Acest modul se ocupă de partea inițială a procesului:
- Primește datele comenzii de la client (produse, cantități, date personale)
- **Validează** informațiile: verifică dacă email-ul este corect, dacă codurile produselor există, dacă cantitățile sunt pozitive
- **Verifică disponibilitatea** produselor în stoc
- **Calculează prețul total** al comenzii
- **Plasează comanda** în sistem și îi atribuie un număr unic
- Anunță celelalte module că o comandă nouă a fost plasată

### 2. Invoicing Context (Contextul Facturării)
După ce o comandă este plasată, acest modul:
- Primește notificarea că o comandă a fost finalizată
- **Generează automat o factură** cu toate detaliile comenzii
- Calculează **taxele fiscale** (TVA, alte taxe)
- Stochează factura în sistem pentru evidență contabilă
- Trimite factura către clientul și către modulul de expediere

### 3. Shipping Context (Contextul Expedierii)
Ultimul pas în proces:
- Primește informația că o comandă trebuie expediată
- **Creează o etichetă de transport** cu adresa clientului
- **Alocă un curier** pentru livrare
- Oferă **tracking pentru urmărirea coletului**
- Actualizează statusul expedierii (în curs, livrat, etc.)

## 🔄 Comunicarea Între Module

Modulele nu comunică direct între ele, ci folosesc **Azure Service Bus** pentru evenimente asincrone:

### Arhitectură Implementată
```
Order Processing API (port 5259)
    |
    | publică OrderPlacedEvent
    v
Azure Service Bus Topic: "orders"
    |
    ├─> Subscription: invoicing-subscription
    |       |
    |       v
    |   Invoicing Worker (background service)
    |       |
    |       | apelează
    |       v
    |   Invoicing API (port 5260) → generează Invoice
    |
    └─> Subscription: shipping-subscription
            |
            v
        Shipping Worker (background service)
            |
            | apelează
            v
        Shipping API (port 5261) → creează Shipment
```

### Beneficii
- **Rezistent** - dacă un worker se oprește, mesajele rămân în queue și sunt procesate când revine
- **Scalabil** - poți rula multiple instanțe ale fiecărui worker pentru procesare paralelă
- **Ușor de întreținut** - fiecare worker este independent și poate fi actualizat separat
- **Eventual Consistency** - workers procesează când sunt disponibili, fără să blocheze API-ul
- **CloudEvents Standard** - folosim format standardizat CNCF pentru evenimente

## 🛠️ Tehnologii Folosite

- **Limbaj**: C# cu .NET 9.0 (framework modern Microsoft)
- **API**: ASP.NET Core Web API (pentru comunicarea cu aplicații externe)
- **Message Broker**: Azure Service Bus Standard tier (pentru evenimente asincrone)
- **Event Format**: CloudEvents 2.8.0 (standard CNCF pentru evenimente)
- **Worker Services**: .NET Background Services (pentru procesare evenimente)
- **Bază de date**: SQL Server cu Entity Framework Core (pentru stocare date)
- **Securitate**: User Secrets (pentru connection strings sensibile)
- **Documentație API**: Swagger/OpenAPI (interfață grafică pentru testare)

## 📊 Domain-Driven Design

Proiectul folosește DDD, ceea ce înseamnă că:

### Value Objects (Obiecte de Valoare)
Reprezintă concepte din domeniu care sunt identificate prin valoarea lor:
- `OrderId` - număr unic al comenzii (ex: "550e8400-e29b-41d4-a716-446655440000")
- `ProductCode` - cod produs (ex: "PROD-123", validat automat)
- `Quantity` - cantitate (ex: 5, trebuie să fie > 0)
- `Price` - preț (ex: 99.99 RON, nu poate fi negativ)
- `Amount` - sumă totală (ex: 499.95 RON)
- `CustomerInfo` - date client (nume și email validat)

### Entity States (Stările Entității)
O comandă trece prin mai multe stări pe măsură ce este procesată:
- `UnvalidatedOrder` - comanda tocmai primită, datele nu sunt verificate
- `ValidatedOrder` - datele au fost verificate și sunt corecte
- `PricedOrder` - prețul total a fost calculat
- `PlacedOrder` - comanda a fost finalizată și plasată

### Operations (Operații de Business)
Funcțiile care transformă comanda dintr-o stare în alta:
- `ValidateOrderOperation` - verifică și validează datele
- `PriceOrderOperation` - calculează prețurile
- `PlaceOrderOperation` - finalizează și salvează comanda

### Workflow
Întregul proces compus din operații:
- `PlaceOrderWorkflow` - orchestrează toate operațiile pentru a procesa o comandă de la început la sfârșit

## 🚀 Exemplu de Utilizare

### Fluxul unei Comenzi

```
1. Client plasează comandă:
   POST /api/orders
   {
     "customerName": "Ion Popescu",
     "customerEmail": "ion@example.com",
     "orderLines": [
       { "productCode": "LAPTOP-001", "quantity": 1 },
       { "productCode": "MOUSE-USB-05", "quantity": 2 }
     ]
   }

2. Sistem validează datele:
   ✓ Email-ul este valid
   ✓ Codurile produselor există
   ✓ Cantitățile sunt > 0

3. Sistem calculează prețurile:
   - LAPTOP-001: 3499.99 RON × 1 = 3499.99 RON
   - MOUSE-USB-05: 49.99 RON × 2 = 99.98 RON
   - TOTAL: 3599.97 RON

4. Comandă plasată cu succes:
   - OrderId: 550e8400-e29b-41d4-a716-446655440000
   - Status: Placed
   - PlacedAt: 2025-12-25 10:30:00

5. Eveniment trimis → Invoicing generează factură
6. Eveniment trimis → Shipping pregătește expedierea
```

## 📁 Structura Proiect

Proiectul este organizat modular pentru claritate și separare a responsabilităților:

```
Proiect-Implementare/
├── README.md                                  # Acest fișier - introducere
├── IMPLEMENTATION_GUIDE.md                    # Ghid implementare
├── AZURE_SETUP.md                             # ⭐ Setup Azure Service Bus
├── .gitignore                                 # Fișiere excluse din Git
├── OrderProcessing.sln                        # Solution Visual Studio
├── docs/                                      # Documentație
│   ├── EventStorming.md                      # Evenimente de business
│   ├── DesignDecisions.md                    # Decizii arhitectură
│   ├── AZURE_SERVICE_BUS_SETUP.md            # Setup Azure detaliat
│   ├── EVENT_ARCHITECTURE_SUMMARY.md         # Arhitectură evenimente
│   └── TESTE_FINALE.md                       # Ghid testare completă
├── src/                                       # Cod sursă
│   ├── OrderProcessing.Domain/               # Domain Layer
│   │   ├── Models/                           # Value Objects
│   │   ├── Operations/                       # Operații domeniu
│   │   ├── Workflows/                        # Workflows
│   │   ├── Repositories/                     # Interfețe persistență
│   │   └── Exceptions/                       # Excepții domeniu
│   ├── OrderProcessing.Events/               # ⭐ Event abstractions
│   │   ├── IEventSender.cs
│   │   ├── IEventListener.cs
│   │   ├── IEventHandler.cs
│   │   └── AbstractEventHandler.cs
│   ├── OrderProcessing.Events.ServiceBus/    # ⭐ Azure Service Bus impl
│   │   ├── ServiceBusTopicEventSender.cs
│   │   └── ServiceBusTopicEventListener.cs
│   ├── OrderProcessing.Dto/                  # ⭐ Event contracts
│   │   └── OrderPlacedEvent.cs
│   ├── OrderProcessing.Api/                  # Order Taking API (5259)
│   ├── OrderProcessing.Invoicing/            # Invoicing API (5260)
│   ├── OrderProcessing.Invoicing.Worker/     # ⭐ Invoicing Worker
│   ├── OrderProcessing.Shipping/             # Shipping API (5261)
│   └── OrderProcessing.Shipping.Worker/      # ⭐ Shipping Worker
└── sql/                                       # Script-uri DB
    └── create-db.sql

⭐ = Proiecte noi pentru Event-Driven Architecture
```

## 🎓 Concepte Învățate

Acest proiect demonstrează:

### 1. Domain-Driven Design (DDD)
- **Separarea responsabilităților** în bounded contexts
- **Value Objects** pentru date imutabile
- **Entity States** pentru lifecycle management
- **Domain Operations** pentru logica de business pură

### 2. Arhitectură Microservices
- Fiecare context = un microservice independent
- Comunicare asincronă prin evenimente
- Scalabilitate și reziliență

### 3. Type-Driven Development
- Sistemul de tipuri C# previne erori
- Validare la compilare, nu doar la runtime
- Imposibilitatea de a crea stări invalide

### 4. Functional Programming în C#
- Records pentru imutabilitate
- Pattern matching
- Evitarea null-urilor prin design

## 🔧 Comenzi Utile

### Configurare Inițială
```bash
# Configurare Azure Service Bus connection string
# Vezi AZURE_SETUP.md pentru detalii complete
dotnet user-secrets set "ServiceBus:ConnectionString" "YOUR_CONNECTION_STRING" --project src/OrderProcessing.Api/OrderProcessing.Api.csproj
dotnet user-secrets set "ServiceBus:ConnectionString" "YOUR_CONNECTION_STRING" --project src/OrderProcessing.Invoicing.Worker/OrderProcessing.Invoicing.Worker.csproj
dotnet user-secrets set "ServiceBus:ConnectionString" "YOUR_CONNECTION_STRING" --project src/OrderProcessing.Shipping.Worker/OrderProcessing.Shipping.Worker.csproj
```

### Build și Run
```bash
# Compilare proiect
dotnet build

# Rulare Order Processing API (Terminal 1)
dotnet run --project src/OrderProcessing.Api

# Rulare Invoicing API (Terminal 2)
dotnet run --project src/OrderProcessing.Invoicing

# Rulare Shipping API (Terminal 3)
dotnet run --project src/OrderProcessing.Shipping

# Rulare Invoicing Worker (Terminal 4)
dotnet run --project src/OrderProcessing.Invoicing.Worker

# Rulare Shipping Worker (Terminal 5)
dotnet run --project src/OrderProcessing.Shipping.Worker
```

### Testare
```powershell
# Plasare comandă
$body = @{
    customerName = "John Doe"
    customerEmail = "john@example.com"
    orderLines = @(@{ productCode = "LAPTOP-001"; quantity = 1 })
} | ConvertTo-Json
Invoke-RestMethod -Uri "http://localhost:5259/api/orders" -Method Post -Body $body -ContentType "application/json"

# Verificare facturi
Invoke-RestMethod -Uri "http://localhost:5260/api/invoices" -Method Get

# Verificare shipments
Invoke-RestMethod -Uri "http://localhost:5261/api/shipping" -Method Get
```

### Altele
```bash
# Verificare formatare cod
dotnet format

# Creare migrare bază de date
dotnet ef migrations add InitialCreate -p src/OrderProcessing.Api
```

## 📚 Resurse Utile

- **Domain-Driven Design**: "Domain Modeling Made Functional" de Scott Wlaschin
- **.NET Documentation**: https://docs.microsoft.com/dotnet
- **Microservices Patterns**: https://microservices.io/patterns/
- **Event Storming**: https://www.eventstorming.com/

## 📝 Licență

Acest proiect este realizat în scop educațional pentru cursul de PSSC (Proiectarea Sistemelor Software Complexe).

---

**Dezvoltat cu**: Visual Studio Code, .NET 8.0, și AI Copilot  
**An Academic**: 2025  
**Universitate**: Universitatea Politehnica Timișoara
