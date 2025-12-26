# Shipping Microservice - Manual Testing Guide

**Port:** 5261  
**Base URL:** http://localhost:5261  
**Swagger UI:** http://localhost:5261/swagger

## Informații Generale

**Shipping Status:**
- `0` - Pending (În așteptare)
- `1` - Processing (În procesare)
- `2` - Shipped (Expediat) - *setează automat ShippedAt*
- `3` - InTransit (În tranzit)
- `4` - OutForDelivery (În curs de livrare)
- `5` - Delivered (Livrat) - *setează automat DeliveredAt*
- `6` - Failed (Eșuat)

**Curieri disponibili** (assignment random):
- Fan Courier
- DHL
- GLS
- Cargus
- Sameday

**Format Tracking Number:** RO + 7 cifre (ex: RO1000001, RO1000002)

---

## Test 1: Creare Transport - Timișoara

**Metodă:** POST  
**Endpoint:** http://localhost:5261/api/shipping  
**Content-Type:** application/json

**JSON pentru copiat:**
```json
{
  "orderId": "123e4567-e89b-12d3-a456-426614174000",
  "customerName": "Ion Popescu",
  "customerEmail": "ion.popescu@example.com",
  "shippingAddress": "Str. Mihai Viteazu nr. 25, Bl. A3, Sc. 2, Ap. 15",
  "city": "Timișoara",
  "postalCode": "300001",
  "country": "Romania"
}
```

**Rezultat așteptat:**
- Status: 201 Created
- `shippingId`: GUID nou generat
- `trackingNumber`: RO1000001 (sau următorul număr)
- `courier`: unul din cei 5 curieri (random)
- `status`: 1 (Processing)
- Salvează `shippingId` și `trackingNumber` pentru testele următoare

---

## Test 2: Creare Transport - București

**Metodă:** POST  
**Endpoint:** http://localhost:5261/api/shipping

**JSON pentru copiat:**
```json
{
  "orderId": "223e4567-e89b-12d3-a456-426614174001",
  "customerName": "Maria Ionescu",
  "customerEmail": "maria.ionescu@gmail.com",
  "shippingAddress": "Bulevardul Unirii nr. 45",
  "city": "București",
  "postalCode": "030167",
  "country": "Romania"
}
```

**Rezultat așteptat:**
- Tracking number diferit: RO1000002
- Curier posibil diferit (random)

---

## Test 3: Creare Transport - Cluj-Napoca

**Metodă:** POST  
**Endpoint:** http://localhost:5261/api/shipping

**JSON pentru copiat:**
```json
{
  "orderId": "323e4567-e89b-12d3-a456-426614174002",
  "customerName": "Alexandru Pop",
  "customerEmail": "alex.pop@yahoo.com",
  "shippingAddress": "Str. Memorandumului nr. 28",
  "city": "Cluj-Napoca",
  "postalCode": "400114",
  "country": "Romania"
}
```

---

## Test 4: Creare Transport - Brașov

**Metodă:** POST  
**Endpoint:** http://localhost:5261/api/shipping

**JSON pentru copiat:**
```json
{
  "orderId": "423e4567-e89b-12d3-a456-426614174003",
  "customerName": "Elena Dumitrescu",
  "customerEmail": "elena.dumitrescu@outlook.com",
  "shippingAddress": "Str. Republicii nr. 62",
  "city": "Brașov",
  "postalCode": "500030",
  "country": "Romania"
}
```

---

## Test 5: Creare Transport - Iași

**Metodă:** POST  
**Endpoint:** http://localhost:5261/api/shipping

**JSON pentru copiat:**
```json
{
  "orderId": "523e4567-e89b-12d3-a456-426614174004",
  "customerName": "Andrei Vasilescu",
  "customerEmail": "andrei.v@gmail.com",
  "shippingAddress": "Bulevardul Ștefan cel Mare și Sfânt nr. 8",
  "city": "Iași",
  "postalCode": "700064",
  "country": "Romania"
}
```

---

## Test 6: Listare Toate Transporturile

**Metodă:** GET  
**Endpoint:** http://localhost:5261/api/shipping

**Fără JSON - request simplu GET**

**Rezultat așteptat:**
- Status: 200 OK
- Array cu toate transporturile create
- Ordonate descrescător după `createdAt`

---

## Test 7: Obținere Transport Specific

**Metodă:** GET  
**Endpoint:** http://localhost:5261/api/shipping/{shippingId}

**Instrucțiuni:**
1. Ia un `shippingId` din răspunsul testului 1-5
2. Înlocuiește `{shippingId}` în URL cu GUID-ul real

**Exemplu URL:**
```
http://localhost:5261/api/shipping/a1b2c3d4-e5f6-7890-abcd-ef1234567890
```

**Rezultat așteptat:**
- Status: 200 OK dacă există
- Status: 404 Not Found dacă nu există

---

## Test 8: Tracking Colet

**Metodă:** GET  
**Endpoint:** http://localhost:5261/api/shipping/track/{trackingNumber}

**Instrucțiuni:**
1. Folosește un `trackingNumber` din testele 1-5
2. Înlocuiește `{trackingNumber}` în URL

**Exemplu URL:**
```
http://localhost:5261/api/shipping/track/RO1000001
```

**Rezultat așteptat:**
- Informații complete despre transport
- Include status curent, curier, adresă

---

## Test 9: Găsire Transporturi pentru o Comandă

**Metodă:** GET  
**Endpoint:** http://localhost:5261/api/shipping/order/{orderId}

**Instrucțiuni:**
1. Folosește un `orderId` din testele 1-5
2. Înlocuiește `{orderId}` în URL

**Exemplu URL:**
```
http://localhost:5261/api/shipping/order/123e4567-e89b-12d3-a456-426614174000
```

**Rezultat așteptat:**
- Array cu toate transporturile pentru comanda respectivă
- Poate fi gol dacă nu există

---

## Test 10: Update Status - Pending → Processing

**Metodă:** PUT  
**Endpoint:** http://localhost:5261/api/shipping/{shippingId}/status?newStatus=1

**Instrucțiuni:**
1. Înlocuiește `{shippingId}` cu un GUID real
2. Query parameter: `newStatus=1`

**Exemplu URL:**
```
http://localhost:5261/api/shipping/a1b2c3d4-e5f6-7890-abcd-ef1234567890/status?newStatus=1
```

**Rezultat așteptat:**
- Status: 200 OK
- Transport returnat cu `status: 1` (Processing)

---

## Test 11: Update Status - Processing → Shipped

**Metodă:** PUT  
**Endpoint:** http://localhost:5261/api/shipping/{shippingId}/status?newStatus=2

**Exemplu URL:**
```
http://localhost:5261/api/shipping/a1b2c3d4-e5f6-7890-abcd-ef1234567890/status?newStatus=2
```

**Rezultat așteptat:**
- `status: 2` (Shipped)
- **`shippedAt`: timestamp automat setat!**

---

## Test 12: Update Status - Shipped → InTransit

**Metodă:** PUT  
**Endpoint:** http://localhost:5261/api/shipping/{shippingId}/status?newStatus=3

**Exemplu URL:**
```
http://localhost:5261/api/shipping/a1b2c3d4-e5f6-7890-abcd-ef1234567890/status?newStatus=3
```

**Rezultat așteptat:**
- `status: 3` (InTransit - coletul este în drum)

---

## Test 13: Update Status - InTransit → OutForDelivery

**Metodă:** PUT  
**Endpoint:** http://localhost:5261/api/shipping/{shippingId}/status?newStatus=4

**Exemplu URL:**
```
http://localhost:5261/api/shipping/a1b2c3d4-e5f6-7890-abcd-ef1234567890/status?newStatus=4
```

**Rezultat așteptat:**
- `status: 4` (OutForDelivery - curierul este în zonă)

---

## Test 14: Update Status - OutForDelivery → Delivered

**Metodă:** PUT  
**Endpoint:** http://localhost:5261/api/shipping/{shippingId}/status?newStatus=5

**Exemplu URL:**
```
http://localhost:5261/api/shipping/a1b2c3d4-e5f6-7890-abcd-ef1234567890/status?newStatus=5
```

**Rezultat așteptat:**
- `status: 5` (Delivered - livrat cu succes)
- **`deliveredAt`: timestamp automat setat!**

---

## Test 15: Verificare Transport După Livrare

**Metodă:** GET  
**Endpoint:** http://localhost:5261/api/shipping/{shippingId}

**Instrucțiuni:**
După testele 10-14, verifică că:
- `status` este 5 (Delivered)
- `shippedAt` are valoare (de la Test 11)
- `deliveredAt` are valoare (de la Test 14)

---

## Test 16: Update Status - Mark as Failed

**Metodă:** PUT  
**Endpoint:** http://localhost:5261/api/shipping/{shippingId}/status?newStatus=6

**Exemplu URL:**
```
http://localhost:5261/api/shipping/a1b2c3d4-e5f6-7890-abcd-ef1234567890/status?newStatus=6
```

**Rezultat așteptat:**
- `status: 6` (Failed - livrare eșuată)

**Notă:** Folosește un transport diferit decât cel din testele 10-15

---

## Scenarii Complete de Testare

### Scenariul 1: Livrare cu Succes (Timișoara)

1. **Creare transport** (Test 1)
   - Salvează `shippingId` și `trackingNumber`
   - Status inițial: Processing (1)

2. **Verifică tracking** (Test 8)
   - Folosește tracking number-ul

3. **Update la Shipped** (Test 11)
   - Status devine 2
   - `shippedAt` este setat automat

4. **Update la InTransit** (Test 12)
   - Status devine 3

5. **Update la OutForDelivery** (Test 13)
   - Status devine 4

6. **Update la Delivered** (Test 14)
   - Status devine 5
   - `deliveredAt` este setat automat

7. **Verificare finală** (Test 15)
   - Confirmă că ambele timestamp-uri sunt setate

---

### Scenariul 2: Livrare Eșuată (București)

1. **Creare transport** (Test 2)
   - Salvează `shippingId`

2. **Update la Processing** (Test 10)
   - Status: 1

3. **Update la Failed** (Test 16)
   - Status: 6
   - Livrare nereușită

4. **Verificare** (Test 7)
   - Confirmă status Failed

---

### Scenariul 3: Tracking Client

1. **Client cere tracking** pentru RO1000001 (Test 8)
2. Vezi status curent, curier, adresă
3. Client verifică din nou mai târziu
4. Status s-a schimbat (dacă ai rulat update-uri)

---

## Diagrama Flux Status

```
0 (Pending)
    ↓
1 (Processing)
    ↓
2 (Shipped) ← setează ShippedAt
    ↓
3 (InTransit)
    ↓
4 (OutForDelivery)
    ↓
5 (Delivered) ← setează DeliveredAt

Alternative:
Orice status → 6 (Failed)
```

---

## Notițe pentru Testare în Swagger

1. Deschide http://localhost:5261/swagger
2. Toate endpoint-urile sunt listate cu documentație
3. Pentru POST: Click "Try it out", copiază JSON
4. Pentru GET/PUT: Înlocuiește parametrii în URL
5. Click "Execute" și verifică răspunsul

---

## Timestamp-uri Automate

**Important:** Două câmpuri se setează automat:

1. **`shippedAt`** - se setează când status devine 2 (Shipped)
2. **`deliveredAt`** - se setează când status devine 5 (Delivered)

Nu trebuie să le trimiți manual, sistemul le adaugă automat!
