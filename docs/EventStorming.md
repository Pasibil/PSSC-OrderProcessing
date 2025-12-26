# Event Storming - Order Processing System

## 📋 Introducere

Acest document prezintă rezultatele sesiunii de Event Storming pentru sistemul Order Processing. Event Storming este o tehnică de modelare colaborativă care ne ajută să înțelegem fluxul de business prin identificarea evenimentelor cheie care apar în sistem.

## 🎯 Bounded Contexts Identificate

### 1. Order Taking Context
**Responsabilitate**: Gestionarea procesului de primire și validare a comenzilor

**Actori**:
- Client (Customer)
- Sistem de validare
- Catalog produse

**Responsabilități**:
- Primirea comenzilor de la clienți
- Validarea datelor comenzii
- Verificarea disponibilității produselor
- Calcularea prețurilor
- Plasarea comenzii în sistem

---

### 2. Invoicing Context
**Responsabilitate**: Generarea și gestionarea facturilor

**Actori**:
- Sistem de facturare
- Departament financiar
- Client

**Responsabilități**:
- Generarea automată a facturilor
- Calculul taxelor (TVA)
- Stocarea facturilor
- Trimiterea facturilor către clienți

---

### 3. Shipping Context
**Responsabilitate**: Gestionarea expedierii și livrării comenzilor

**Actori**:
- Departament logistică
- Curier
- Client

**Responsabilități**:
- Crearea etichetelor de transport
- Alocarea curierilor
- Tracking expedieri
- Actualizarea statusului de livrare

---

## 🔄 Evenimente de Domeniu

### Order Taking Context - Evenimente

#### 1. OrderReceived
**Când**: Clientul trimite o comandă către sistem
```
Trigger: Client Request
Data: {
  CustomerName: string
  CustomerEmail: string
  OrderLines: [{
    ProductCode: string
    Quantity: int
  }]
}
```

#### 2. OrderValidated
**Când**: Datele comenzii au fost validate cu succes
```
Trigger: ValidateOrderOperation
Data: {
  OrderId: Guid
  CustomerInfo: CustomerInfo
  ValidatedLines: List<ValidatedOrderLine>
}
```

#### 3. OrderValidationFailed
**Când**: Validarea comenzii a eșuat
```
Trigger: ValidateOrderOperation
Data: {
  Reason: string
  ValidationErrors: List<string>
}
```

#### 4. OrderPriced
**Când**: Prețurile au fost calculate pentru comandă
```
Trigger: PriceOrderOperation
Data: {
  OrderId: Guid
  PricedLines: List<PricedOrderLine>
  TotalAmount: decimal
}
```

#### 5. OrderPlaced
**Când**: Comanda a fost plasată cu succes în sistem
```
Trigger: PlaceOrderOperation
Data: {
  OrderId: Guid
  CustomerInfo: CustomerInfo
  OrderLines: List<PricedOrderLine>
  TotalAmount: decimal
  PlacedAt: DateTime
}
Status: SUCCESS
```

#### 6. OrderPlacementFailed
**Când**: Plasarea comenzii a eșuat
```
Trigger: PlaceOrderWorkflow Exception
Data: {
  Reason: string
  ErrorDetails: string
}
Status: FAILURE
```

---

### Invoicing Context - Evenimente

#### 7. InvoiceRequested
**Când**: O comandă plasată necesită o factură
```
Trigger: OrderPlaced event
Data: {
  OrderId: Guid
  CustomerInfo: CustomerInfo
  TotalAmount: decimal
}
```

#### 8. InvoiceGenerated
**Când**: Factura a fost generată cu succes
```
Trigger: GenerateInvoiceOperation
Data: {
  InvoiceId: Guid
  OrderId: Guid
  InvoiceNumber: string
  TotalAmount: decimal
  TaxAmount: decimal (TVA)
  GeneratedAt: DateTime
}
```

#### 9. InvoiceGenerationFailed
**Când**: Generarea facturii a eșuat
```
Trigger: GenerateInvoiceOperation Exception
Data: {
  OrderId: Guid
  Reason: string
}
```

#### 10. InvoiceSent
**Când**: Factura a fost trimisă către client
```
Trigger: SendInvoiceOperation
Data: {
  InvoiceId: Guid
  SentTo: string (email)
  SentAt: DateTime
}
```

---

### Shipping Context - Evenimente

#### 11. ShippingRequested
**Când**: O comandă necesită expediere
```
Trigger: InvoiceGenerated event
Data: {
  OrderId: Guid
  CustomerInfo: CustomerInfo
  ShippingAddress: Address
  OrderLines: List<OrderLine>
}
```

#### 12. ShippingLabelCreated
**Când**: Eticheta de transport a fost creată
```
Trigger: CreateShippingLabelOperation
Data: {
  ShippingId: Guid
  OrderId: Guid
  LabelUrl: string
  TrackingNumber: string
  CreatedAt: DateTime
}
```

#### 13. CourierAssigned
**Când**: Un curier a fost alocat pentru livrare
```
Trigger: AssignCourierOperation
Data: {
  ShippingId: Guid
  CourierName: string
  EstimatedDelivery: DateTime
}
```

#### 14. OrderShipped
**Când**: Comanda a fost expediată
```
Trigger: ShipOrderOperation
Data: {
  ShippingId: Guid
  OrderId: Guid
  TrackingNumber: string
  ShippedAt: DateTime
  Courier: string
}
```

#### 15. OrderDelivered
**Când**: Comanda a fost livrată la client
```
Trigger: Courier Confirmation
Data: {
  ShippingId: Guid
  OrderId: Guid
  DeliveredAt: DateTime
  ReceivedBy: string
}
```

#### 16. DeliveryFailed
**Când**: Livrarea a eșuat
```
Trigger: Courier Report
Data: {
  ShippingId: Guid
  OrderId: Guid
  Reason: string
  RetryScheduled: DateTime?
}
```

---

## 📊 Flow Diagrama Completă

```
[Client] --submits--> OrderReceived
                         |
                         v
                    Validate Order
                    /           \
                   /             \
          OrderValidated    OrderValidationFailed
                |                     |
                v                     v
           Price Order            [END - Error]
                |
                v
          OrderPriced
                |
                v
           Place Order
           /          \
          /            \
    OrderPlaced    OrderPlacementFailed
         |                  |
         v                  v
  InvoiceRequested    [END - Error]
         |
         v
  Generate Invoice
    /          \
   /            \
InvoiceGenerated  InvoiceGenerationFailed
   |                      |
   v                      v
Send Invoice         [END - Error]
   |
   v
InvoiceSent
   |
   v
ShippingRequested
   |
   v
Create Shipping Label
   |
   v
ShippingLabelCreated
   |
   v
Assign Courier
   |
   v
CourierAssigned
   |
   v
Ship Order
   |
   v
OrderShipped
   |
   v
[Courier delivers]
   |
   v
OrderDelivered
   |
   v
[END - Success]
```

---

## 🎨 Color Coding (Standard Event Storming)

- 🟠 **Orange (Events)**: OrderPlaced, InvoiceGenerated, OrderShipped
- 🔵 **Blue (Commands)**: PlaceOrder, GenerateInvoice, ShipOrder
- 💛 **Yellow (Actors)**: Client, Courier, System
- 💚 **Green (Read Models)**: Order Details, Invoice, Tracking Info
- 🟣 **Purple (Policies)**: "When OrderPlaced then GenerateInvoice"
- 🔴 **Red (Concerns)**: Validation errors, Payment failures, Delivery issues

---

## 🔗 Comunicarea Între Contexte

### Event-Driven Communication

```
Order Taking Context
    |
    | publishes: OrderPlaced
    v
Message Bus (Events)
    |
    +---> Invoicing Context (subscribes: OrderPlaced)
    |         |
    |         | publishes: InvoiceGenerated
    |         v
    +---> Shipping Context (subscribes: InvoiceGenerated)
              |
              | publishes: OrderShipped, OrderDelivered
              v
          [Notifications]
```

### Tipuri de Mesaje

1. **Events** (Event-uri) - Fapte care s-au întâmplat în trecut
   - `OrderPlaced`
   - `InvoiceGenerated`
   - `OrderShipped`

2. **Commands** (Comenzi) - Cereri de a efectua o acțiune
   - `PlaceOrder`
   - `GenerateInvoice`
   - `ShipOrder`

3. **Queries** (Interogări) - Cereri de informații
   - `GetOrderDetails`
   - `GetInvoiceByOrderId`
   - `GetShippingStatus`

---

## 📝 Domain Stories

### Story 1: Comandă cu Succes
```
1. Ion plasează o comandă pentru 2 produse
2. Sistemul validează datele și produsele
3. Se calculează prețul total: 150 RON
4. Comanda este plasată cu ID: abc-123
5. Se generează factura cu numărul: INV-2025-001
6. Factura este trimisă pe email la ion@email.com
7. Se creează etichetă de transport cu tracking: TR123456
8. Curierul "FastDelivery" preia comanda
9. Comanda este expediată
10. După 2 zile, comanda este livrată cu succes
```

### Story 2: Validare Eșuată
```
1. Maria încearcă să plaseze o comandă
2. Email-ul introdus este invalid: "maria@invalid"
3. Sistemul returnează OrderValidationFailed
4. Maria primește mesaj de eroare
5. Maria corectează email-ul și reîncearcă
6. De data aceasta validarea reușește
7. Procesul continuă normal
```

### Story 3: Product Indisponibil
```
1. Alex comandă produsul "LAPTOP-999" 
2. Sistemul verifică disponibilitatea
3. Produsul nu există în catalog
4. OrderValidationFailed cu motivul: "Product code LAPTOP-999 not found"
5. Alex este notificat și poate alege alt produs
```

---

## 🎯 Agregați Identificați

### Order Aggregate
**Root Entity**: Order (PlacedOrder)
**Value Objects**: 
- OrderId
- CustomerInfo
- OrderLine
- PricedOrderLine
- Amount

**Operations**:
- Validate
- Price
- Place

---

### Invoice Aggregate
**Root Entity**: Invoice
**Value Objects**:
- InvoiceId
- InvoiceNumber
- TaxAmount

**Operations**:
- Generate
- Calculate Tax
- Send

---

### Shipping Aggregate
**Root Entity**: Shipment
**Value Objects**:
- ShippingId
- TrackingNumber
- ShippingAddress

**Operations**:
- Create Label
- Assign Courier
- Ship
- Track

---

## 💡 Insight-uri și Observații

### Anti-Corruption Layers
Fiecare bounded context are propriul său model de date și nu depinde direct de celelalte:
- Order Taking nu știe despre facturi sau expedieri
- Invoicing primește doar evenimentul OrderPlaced, nu întreaga comandă
- Shipping lucrează independent cu propriul său tracking system

### Event Sourcing Potential
Evenimentele pot fi stocate într-un event store pentru:
- Audit trail complet
- Replay events pentru debugging
- Reconstruirea stării sistemului
- Analytics și reporting

### Eventual Consistency
Sistemul acceptă eventual consistency:
- O comandă poate fi plasată chiar dacă factura nu e generată instant
- Shipping poate începe chiar dacă email-ul cu factura întârzie
- Fiecare context își menține propria consistență

---

**Data creării**: 2025-12-26  
**Versiune**: 1.0  
**Autor**: Aleksandru Demchuchen
