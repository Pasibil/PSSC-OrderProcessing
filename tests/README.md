# Test Examples - Order Processing System

Acest folder conține ghiduri de testare manuală pentru toate cele 3 microservicii.

## Structură Fișiere

- **OrderProcessing-Api-Tests.md** - Testare API principal (Order Taking)
- **Invoicing-Tests.md** - Testare microserviciu Facturare
- **Shipping-Tests.md** - Testare microserviciu Transport
- **README.md** - Acest fișier (ghid general)

---

## Cum să Testezi

### Metoda Recomandată: Swagger UI

Cea mai simplă metodă este prin interfețele Swagger:

1. **Order Processing API**: http://localhost:5259/swagger
2. **Invoicing API**: http://localhost:5260/swagger
3. **Shipping API**: http://localhost:5261/swagger

**Pași pentru testare:**
1. Deschide URL-ul Swagger în browser
2. Click pe endpoint-ul dorit (ex: POST /api/orders)
3. Click "Try it out"
4. Copiază JSON-ul din fișierele `.md` corespunzătoare
5. Lipește în câmpul "Request body"
6. Click "Execute"
7. Vezi răspunsul în secțiunea "Response body"
8. Salvează ID-urile (orderId, invoiceId, shippingId) pentru teste ulterioare

---

### Metoda Alternativă: PowerShell/cURL

Dacă preferi linia de comandă, poți adapta exemplele din fișierele `.md`.

**Exemplu PowerShell:**
```powershell
# Creare comandă
$orderData = @{
    customerName = "Test User"
    customerEmail = "test@example.com"
    orderLines = @(
        @{
            productCode = "LAPTOP-001"
            quantity = 1
        }
    )
} | ConvertTo-Json -Depth 10

$response = Invoke-RestMethod -Uri "http://localhost:5259/api/orders" `
    -Method POST `
    -Body $orderData `
    -ContentType "application/json"

$response | ConvertTo-Json
```

---

## Ordinea Recomandată de Testare

### Flux Complet: Comandă → Factură → Transport

#### 1. Creare Comandă (API Principal)

Fișier: `OrderProcessing-Api-Tests.md` → Test 1

```
POST http://localhost:5259/api/orders
```

**Salvează din răspuns:**
- `orderId` (ex: 123e4567-e89b-12d3-a456-426614174000)
- `totalAmount` (ex: 3599.97)

---

#### 2. Generare Factură (Invoicing)

Fișier: `Invoicing-Tests.md` → Test 1

```
POST http://localhost:5260/api/invoice/generate
```

**Folosește datele din pasul 1:**
- Copiază `orderId`
- Copiază detaliile comenzii (customerName, email, orderLines)
- Folosește `totalAmount` din comanda

**Salvează din răspuns:**
- `invoiceId`
- `invoiceNumber` (ex: INV-2025-001001)
- Verifică că `tax` = totalAmount × 0.19
- Verifică că `total` = totalAmount + tax

---

#### 3. Creare Transport (Shipping)

Fișier: `Shipping-Tests.md` → Test 1

```
POST http://localhost:5261/api/shipping
```

**Folosește datele din pasul 1:**
- Copiază `orderId`
- Copiază datele clientului

**Salvează din răspuns:**
- `shippingId`
- `trackingNumber` (ex: RO1000001)
- `courier` (ex: Fan Courier)

---

#### 4. Verificare Completă

**4a. Verifică comanda:**
```
GET http://localhost:5259/api/orders/{orderId}
```

**4b. Verifică factura:**
```
GET http://localhost:5260/api/invoice/{invoiceId}
```

**4c. Tracking colet:**
```
GET http://localhost:5261/api/shipping/track/{trackingNumber}
```

---

#### 5. Update Status Factură

Fișier: `Invoicing-Tests.md` → Teste 7-9

```
PUT http://localhost:5260/api/invoice/{invoiceId}/status?newStatus=1  # Draft → Generated
PUT http://localhost:5260/api/invoice/{invoiceId}/status?newStatus=2  # Generated → Sent
PUT http://localhost:5260/api/invoice/{invoiceId}/status?newStatus=3  # Sent → Paid
```

---

#### 6. Update Status Transport

Fișier: `Shipping-Tests.md` → Teste 10-14

```
PUT http://localhost:5261/api/shipping/{shippingId}/status?newStatus=1  # → Processing
PUT http://localhost:5261/api/shipping/{shippingId}/status?newStatus=2  # → Shipped (setează ShippedAt)
PUT http://localhost:5261/api/shipping/{shippingId}/status?newStatus=3  # → InTransit
PUT http://localhost:5261/api/shipping/{shippingId}/status?newStatus=4  # → OutForDelivery
PUT http://localhost:5261/api/shipping/{shippingId}/status?newStatus=5  # → Delivered (setează DeliveredAt)
```

---

## Scenarii de Testare Avansate

### Scenariul 1: Comandă Mare (Office Setup)

1. Creare comandă cu 5 produse × 5 bucăți fiecare
2. Factură cu TVA pentru sumă mare (12499.70 → Total: 14874.64)
3. Transport cu adresă completă pentru firmă
4. Tracking până la livrare

### Scenariul 2: Erori de Validare

1. Email invalid → vezi eroare validare
2. Produs inexistent → vezi eroare produs
3. Cantitate 0 → vezi eroare cantitate

### Scenariul 3: Multiple Comenzi Același Client

1. Crează 3 comenzi cu același email
2. Listează toate comenzile (GET /api/orders)
3. Generează facturi pentru fiecare
4. Crează transport pentru fiecare
5. Verifică toate facturile: GET /api/invoice (vezi toate)
6. Tracking pe fiecare transport

---

## Date de Test Utile

### Produse Disponibile (API Principal)

| Cod | Produs | Preț (RON) |
|-----|--------|------------|
| LAPTOP-001 | Laptop | 3499.99 |
| MOUSE-USB-05 | Mouse USB | 49.99 |
| KEYBOARD-MEC | Tastatură | 299.99 |
| MONITOR-24 | Monitor 24" | 899.99 |
| HEADSET-G502 | Căști Gaming | 249.99 |
| WEBCAM-HD | Webcam HD | 199.99 |
| USB-HUB-7 | Hub USB | 89.99 |
| DESK-LAMP | Lampă Birou | 129.99 |
| OFFICE-CHAIR | Scaun Birou | 999.99 |
| MOUSEPAD-XL | Mousepad XL | 79.99 |

### Orașe pentru Testare (Shipping)

- **Timișoara**: 300001
- **București**: 030167
- **Cluj-Napoca**: 400114
- **Brașov**: 500030
- **Iași**: 700064

### Email-uri pentru Test

- ion.popescu@example.com
- maria.ionescu@gmail.com
- alex.gamer@yahoo.com
- office@compania.ro
- test@test.com

---

## Verificare în Baza de Date

Dacă ai acces la SQL Server Management Studio:

```sql
USE OrderProcessingDb;

-- Vezi toate comenzile cu linii
SELECT 
    o.OrderId, 
    o.CustomerName, 
    o.CustomerEmail,
    o.PlacedAt,
    ol.ProductCode,
    ol.Quantity,
    ol.Price,
    ol.Amount
FROM Orders o
JOIN OrderLines ol ON o.Id = ol.OrderId
ORDER BY o.PlacedAt DESC;

-- Statistici comenzi
SELECT 
    COUNT(*) as TotalOrders,
    SUM(TotalAmount) as TotalRevenue,
    AVG(TotalAmount) as AverageOrderValue
FROM Orders;
```

---

## Checklist de Testare Completă

### API Principal (5259)
- [ ] Creare comandă simplă (1 produs)
- [ ] Creare comandă complexă (4+ produse)
- [ ] Creare comandă mare (cantități mari)
- [ ] Listare toate comenzile
- [ ] Obținere comandă specifică
- [ ] Test eroare: produs inexistent
- [ ] Test eroare: email invalid
- [ ] Test eroare: cantitate zero

### Invoicing (5260)
- [ ] Generare factură pentru comandă simplă
- [ ] Generare factură pentru comandă mare
- [ ] Verificare calcul TVA (19%)
- [ ] Verificare format număr factură (INV-2025-XXXXXX)
- [ ] Listare toate facturile
- [ ] Găsire facturi după orderId
- [ ] Update status: Draft → Generated
- [ ] Update status: Generated → Sent
- [ ] Update status: Sent → Paid

### Shipping (5261)
- [ ] Creare transport pentru 5 orașe diferite
- [ ] Verificare generare tracking number (RO + cifre)
- [ ] Verificare assignment curier random
- [ ] Tracking cu tracking number
- [ ] Găsire transporturi după orderId
- [ ] Update status: flow complet (Pending → Delivered)
- [ ] Verificare setare automată ShippedAt
- [ ] Verificare setare automată DeliveredAt
- [ ] Test transport eșuat (Failed status)

### Integrare
- [ ] Flux complet: Comandă → Factură → Transport
- [ ] Verificare sincronizare între microservicii
- [ ] Test multiple comenzi pentru același client
- [ ] Verificare persistență date (restart servicii)

---

## Note Importante

1. **ID-uri**: Salvează GUID-urile pentru a le folosi în teste ulterioare
2. **Timestamp-uri**: ShippedAt și DeliveredAt se setează automat
3. **TVA**: Calculată automat la 19% pentru România
4. **Tracking**: Format RO + 7 cifre, secvențial
5. **Invoice Number**: Format INV-YEAR-XXXXXX, secvențial
6. **Thread Safety**: Toate microserviciile folosesc ConcurrentDictionary

---

## Troubleshooting

**Serviciile nu răspund?**
- Verifică că toate cele 3 servicii rulează
- Check terminale pentru erori
- Verifică porturile: 5259, 5260, 5261

**Erori 404?**
- Verifică că endpoint-ul este corect
- Verifică că GUID-ul există (folosește GET pentru liste)

**Date nu persistă?**
- API Principal: folosește SQL Server (datele persistă)
- Invoicing/Shipping: folosesc In-Memory (datele se pierd la restart)

**Calcule greșite?**
- TVA trebuie să fie exact 19% din subtotal
- Verifică că prețurile produselor sunt corecte
