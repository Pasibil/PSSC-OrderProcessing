# Order Processing System - DDD Lab

## Echipa
- Aleksandru Demchuchen

## Domeniul Ales
**Order Processing System** - Sistem pentru preluarea de comenzi, facturare și expediere

## Descriere
Sistem software complex pentru gestionarea completă a procesului de comandă într-un magazin online. Implementat folosind principiile Domain-Driven Design (DDD) și arhitectură microservices, sistemul acoperă întregul flux de la plasarea comenzii până la expedierea produselor către client.

## Bounded Contexts Identificate

1. **Order Taking Context**: Contextul de preluare și procesare comenzi
   - Primirea comenzilor de la clienți
   - Validarea datelor comenzii (email, coduri produse, cantități)
   - Verificarea disponibilității produselor
   - Calculul prețurilor și sumei totale
   - Plasarea comenzii în sistem
   - Generarea ID-ului unic pentru comandă

2. **Invoicing Context**: Contextul de facturare
   - Ascultarea evenimentelor de comandă plasată
   - Generarea automată a facturilor
   - Calculul taxelor fiscale (TVA)
   - Stocarea facturilor în sistem
   - Trimiterea facturilor către clienți

3. **Shipping Context**: Contextul de expediere
   - Primirea notificărilor pentru comenzi de expediat
   - Generarea etichetelor de transport
   - Alocarea curierilor pentru livrare
   - Sistem de tracking pentru colete
   - Actualizarea statusului de livrare

## Event Storming Results
Vezi [docs/EventStorming.md](docs/EventStorming.md) pentru diagrame complete și analiza evenimentelor de business.

**Evenimente principale identificate:**
- `OrderPlaced` - Comandă plasată cu succes
- `OrderValidationFailed` - Validare comandă eșuată
- `InvoiceGenerated` - Factură generată
- `ShippingLabelCreated` - Etichetă transport creată
- `OrderShipped` - Comandă expediată
- `OrderDelivered` - Comandă livrată

## Implementare

### Arhitectură Event-Driven
Sistemul folosește **Azure Service Bus** pentru comunicare asincronă între microservices:
- **Topic**: `orders` - pentru publicarea evenimentelor
- **Subscriptions**: `invoicing-subscription`, `shipping-subscription`
- **Pattern**: Pub/Sub cu CloudEvents format
- **Worker Services**: Background services care ascultă și procesează evenimente

### Proiecte Implementate

#### 1. OrderProcessing.Events (Abstraction Layer)
Interfețe și abstractions pentru comunicare event-driven:
- `IEventSender` - pentru publicarea evenimentelor
- `IEventListener` - pentru ascultarea evenimentelor
- `IEventHandler<T>` - pentru procesarea evenimentelor
- `AbstractEventHandler<T>` - bază pentru handlers cu deserializare JSON

#### 2. OrderProcessing.Events.ServiceBus (Implementation Layer)
Implementare Azure Service Bus:
- `ServiceBusTopicEventSender` - publică evenimente în topic-uri
- `ServiceBusTopicEventListener` - ascultă subscriptions și rutează la handlers
- Folosește **CloudEvents** standard pentru format evenimente

#### 3. OrderProcessing.Dto (Shared Contracts)
Contracte partajate între microservices:
- `OrderPlacedEvent` - eveniment declanșat când o comandă este plasată
- Conține: OrderId, CustomerName, CustomerEmail, OrderLines, TotalAmount

#### 4. OrderProcessing.Invoicing.Worker (Background Service)
Worker Service care ascultă evenimente și generează facturi automat:
- Ascultă subscription: `invoicing-subscription`
- Procesează: `OrderPlacedEvent`
- Apelează Invoicing API pentru generare factură

#### 5. OrderProcessing.Shipping.Worker (Background Service)
Worker Service care ascultă evenimente și creează shipments automat:
- Ascultă subscription: `shipping-subscription`
- Procesează: `OrderPlacedEvent`
- Apelează Shipping API pentru creare shipment

### Value Objects
- `OrderId`: Identificator unic pentru comandă (GUID), generat automat
- `ProductCode`: Cod produs validat (3-20 caractere, uppercase)
- `Quantity`: Cantitate validată (trebuie să fie > 0)
- `Price`: Preț produs (nu poate fi negativ)
- `Amount`: Sumă totală calculată (nu poate fi negativă)
- `CustomerInfo`: Informații client (nume și email validat prin regex)

### Entity States
Comanda trece prin următoarele stări în procesul de transformare:

- `UnvalidatedOrder`: Comandă primită inițial, datele nu sunt verificate
- `ValidatedOrder`: Comandă cu date validate (email corect, produse valide, cantități > 0)
- `PricedOrder`: Comandă cu prețuri calculate pentru fiecare linie și total
- `PlacedOrder`: Comandă finalizată și salvată în sistem (stare finală)

### Operations
Operațiile de business care transformă comanda:

1. `ValidateOrderOperation`: Validează datele comenzii
   - Verifică formatul email-ului
   - Validează codurile produselor
   - Verifică cantitățile (> 0)
   - Creează CustomerInfo și OrderId

2. `PriceOrderOperation`: Calculează prețurile
   - Interogează repository-ul de produse pentru prețuri
   - Calculează prețul fiecărei linii (preț × cantitate)
   - Calculează suma totală a comenzii

3. `PlaceOrderOperation`: Finalizează comanda
   - Adaugă timestamp (PlacedAt)
   - Creează starea finală PlacedOrder
   - Pregătește pentru salvare în repository

### Workflow
`PlaceOrderWorkflow`: Pipeline complet care orchestrează întregul proces

**Fluxul workflow-ului:**
```
PlaceOrderCommand 
    → UnvalidatedOrder 
    → ValidateOrderOperation 
    → ValidatedOrder 
    → PriceOrderOperation 
    → PricedOrder 
    → PlaceOrderOperation 
    → PlacedOrder 
    → SaveToRepository 
    → PublishOrderPlacedEvent (Azure Service Bus)
    → OrderPlacedSuccessEvent
```

**Comunicare asincronă:**
- După salvare, API publică `OrderPlacedEvent` în Azure Service Bus topic
- Invoicing Worker și Shipping Worker procesează independent evenimentul
- Fiecare worker apelează propriul API pentru a crea resurse (Invoice/Shipment)
- Eventual consistency - workers procesează când sunt disponibili

**Gestionarea erorilor:**
- Orice excepție în pipeline → `OrderPlacedFailedEvent`
- Retry logic în workers (max 5 încercări, apoi dead-letter queue)
- Logging pentru fiecare pas important
- Validări la nivel de Value Object previne stări invalide

## Rulare

### Configurare Azure Service Bus (OBLIGATORIU)

Înainte de rulare, configurează connection string-ul folosind User Secrets:

```bash
# Pentru API
dotnet user-secrets set "ServiceBus:ConnectionString" "YOUR_CONNECTION_STRING" --project src/OrderProcessing.Api/OrderProcessing.Api.csproj

# Pentru Invoicing Worker
dotnet user-secrets set "ServiceBus:ConnectionString" "YOUR_CONNECTION_STRING" --project src/OrderProcessing.Invoicing.Worker/OrderProcessing.Invoicing.Worker.csproj

# Pentru Shipping Worker
dotnet user-secrets set "ServiceBus:ConnectionString" "YOUR_CONNECTION_STRING" --project src/OrderProcessing.Shipping.Worker/OrderProcessing.Shipping.Worker.csproj
```

**Vezi [AZURE_SETUP.md](AZURE_SETUP.md) pentru instrucțiuni complete de configurare Azure Service Bus.**

### Pornire Servicii

```bash
# Compilare proiect
dotnet build

# Terminal 1 - Order Processing API (port 5259)
dotnet run --project src/OrderProcessing.Api

# Terminal 2 - Invoicing API (port 5260)
dotnet run --project src/OrderProcessing.Invoicing

# Terminal 3 - Shipping API (port 5261)
dotnet run --project src/OrderProcessing.Shipping

# Terminal 4 - Invoicing Worker (background service)
dotnet run --project src/OrderProcessing.Invoicing.Worker

# Terminal 5 - Shipping Worker (background service)
dotnet run --project src/OrderProcessing.Shipping.Worker
```

### Testare Flow Complet

```powershell
# Plasează o comandă
$body = @{
    customerName = "John Doe"
    customerEmail = "john@example.com"
    orderLines = @(
        @{ productCode = "LAPTOP-001"; quantity = 1 }
    )
} | ConvertTo-Json

Invoke-RestMethod -Uri "http://localhost:5259/api/orders" -Method Post -Body $body -ContentType "application/json"

# Verifică factura generată automat
Invoke-RestMethod -Uri "http://localhost:5260/api/invoices" -Method Get

# Verifică shipment-ul creat automat
Invoke-RestMethod -Uri "http://localhost:5261/api/shipping" -Method Get
```

## Lecții Învățate

### Ce a funcționat bine cu AI
- Generarea rapidă a structurii de proiect și fișierelor boilerplate
- Sugestii pentru Value Objects și validări
- Identificarea pattern-urilor DDD potrivite pentru domeniu
- Crearea documentației clare și structurate
- Explicații detaliate despre concepte DDD complexe

### Limitări ale AI identificate
- Necesitatea de clarificare a cerințelor de business specifice
- Validarea manuală a logicii de domeniu pentru cazuri edge
- Testarea integrării între componente necesită atenție umană
- Configurația infrastructurii (DB, messaging) necesită expertise specific

### Prompturi Utile
```
"Creează un Value Object pentru ProductCode cu validare între 3-20 caractere"
"Implementează workflow-ul de plasare comandă folosind pattern railway-oriented programming"
"Generează documentație Event Storming pentru contextul Order Taking"
"Explică cum funcționează comunicarea asincronă între bounded contexts"
```

## Design Decisions

### Arhitectură și Patterns
- **Value Objects pentru validare**: Imutabilitate și validare la nivel de tip previne stări invalide
- **Record types în C#**: Ușurează crearea de Value Objects imutabile
- **Repository Pattern**: Abstractizare completă a persistenței de logica de business
- **Railway Oriented Programming**: Gestionare erori prin tipuri (Success/Failure events)
- **CQRS lite**: Separare read/write pentru scalabilitate viitoare

### Comunicare între Microservices
- **Event-Driven Architecture**: ✅ Implementat - comunicare asincronă prin evenimente
- **Azure Service Bus**: ✅ Implementat - Standard tier, France Central region
- **CloudEvents Standard**: ✅ Folosit pentru format standardizat evenimente (CNCF)
- **Worker Services**: ✅ Background services care procesează evenimente independent
- **Retry Logic**: ✅ Implementat - max 5 încercări, apoi dead-letter queue
- **Typed HttpClient**: ✅ Workers folosesc HttpClient pentru apeluri către API-uri

### Persistență
- **Entity Framework Core**: ORM pentru simplificare CRUD
- **SQL Server**: Bază de date relațională pentru consistență
- **Repository Implementation**: Separare preocupări infrastructură/domeniu

### Logging și Monitoring
- **ILogger injection**: Logging structurat în toate componentele
- **Structured logging**: Contextualizare evenimente pentru debugging
- **Health checks** (planificat): Monitorizare stare microservices

Pentru detalii complete, vezi [docs/DesignDecisions.md](docs/DesignDecisions.md)

---

**Tehnologii utilizate:** .NET 8.0, C# 12, Entity Framework Core, ASP.NET Core Web API  
**Pattern-uri implementate:** DDD, CQRS, Repository, Value Object, Railway-Oriented Programming  
**Status proiect:** 🚀 În dezvoltare activă
