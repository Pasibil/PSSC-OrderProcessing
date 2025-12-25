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

Modulele nu comunică direct între ele, ci folosesc un sistem de **evenimente asincrone** (mesaje). De exemplu:
- Când Order Taking plasează o comandă → trimite eveniment "OrderPlaced"
- Invoicing primește acest eveniment → generează factura → trimite eveniment "InvoiceCreated"
- Shipping primește acest eveniment → pregătește expedierea

Acest mod de comunicare face ca sistemul să fie:
- **Rezistent** - dacă un modul se blochează, celelalte continuă să funcționeze
- **Scalabil** - fiecare modul poate fi rulat pe servere diferite
- **Ușor de întreținut** - poți modifica un modul fără să afectezi celelalte

## 🛠️ Tehnologii Folosite

- **Limbaj**: C# cu .NET 8.0 (framework modern Microsoft)
- **API**: ASP.NET Core Web API (pentru comunicarea cu aplicații externe)
- **Bază de date**: SQL Server cu Entity Framework Core (pentru stocare date)
- **Reziliență**: Polly (pentru gestionarea erorilor și retry-uri automate)
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
├── README.md                          # Acest fișier - introducere în proiect
├── IMPLEMENTATION_GUIDE.md            # Ghid detaliat de implementare
├── .gitignore                         # Fișiere excluse din Git
├── OrderProcessing.sln                # Solution Visual Studio
├── docs/                              # Documentație
│   ├── EventStorming.md              # Diagrame și evenimente de business
│   └── DesignDecisions.md            # Decizii de arhitectură
├── src/                               # Cod sursă
│   ├── OrderProcessing.Domain/       # Domain Layer - logica de business
│   │   ├── Models/                   # Value Objects și Entities
│   │   ├── Operations/               # Operații de domeniu
│   │   ├── Workflows/                # Workflow-uri complete
│   │   ├── Repositories/             # Interfețe pentru date
│   │   └── Exceptions/               # Excepții specifice domeniului
│   ├── OrderProcessing.Api/          # API pentru Order Taking
│   ├── OrderProcessing.InvoicingApi/ # API pentru Invoicing
│   └── OrderProcessing.ShippingApi/  # API pentru Shipping
└── sql/                               # Script-uri bază de date
    └── create-db.sql
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

```bash
# Compilare proiect
dotnet build

# Rulare API principal
dotnet run --project src/OrderProcessing.Api

# Rulare toate microservices
dotnet run --project src/OrderProcessing.Api &
dotnet run --project src/OrderProcessing.InvoicingApi &
dotnet run --project src/OrderProcessing.ShippingApi &

# Testare
dotnet test

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
