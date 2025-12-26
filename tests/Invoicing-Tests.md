# Invoicing Microservice - Manual Testing Guide

**Port:** 5260  
**Base URL:** http://localhost:5260  
**Swagger UI:** http://localhost:5260/swagger

## Informații Generale

**Calcul TVA:** 19% (România)
- **Subtotal:** suma tuturor liniilor de comandă
- **Tax (TVA):** subtotal × 0.19
- **Total:** subtotal + tax

**Invoice Status:**
- `0` - Draft (Ciornă)
- `1` - Generated (Generată)
- `2` - Sent (Trimisă)
- `3` - Paid (Plătită)

**Format Număr Factură:** INV-2025-XXXXXX (secvențial, pornind de la 001001)

---

## Test 1: Generare Factură - Comandă Simplă

**Metodă:** POST  
**Endpoint:** http://localhost:5260/api/invoice/generate  
**Content-Type:** application/json

**JSON pentru copiat:**
```json
{
  "orderId": "123e4567-e89b-12d3-a456-426614174000",
  "customerName": "Ion Popescu",
  "customerEmail": "ion.popescu@example.com",
  "orderLines": [
    {
      "productCode": "LAPTOP-001",
      "quantity": 1,
      "unitPrice": 3499.99
    },
    {
      "productCode": "MOUSE-USB-05",
      "quantity": 2,
      "unitPrice": 49.99
    }
  ],
  "totalAmount": 3599.97
}
```

**Rezultat așteptat:**
- Status: 201 Created
- `invoiceId`: GUID nou generat
- `invoiceNumber`: INV-2025-001001 (sau următorul număr)
- `subtotal`: 3599.97
- `tax`: 683.99 (19% din subtotal)
- `total`: 4283.96
- `status`: 1 (Generated)
- Salvează `invoiceId` pentru testele următoare

---

## Test 2: Generare Factură - Comandă Mare (Office Setup)

**Metodă:** POST  
**Endpoint:** http://localhost:5260/api/invoice/generate

**JSON pentru copiat:**
```json
{
  "orderId": "223e4567-e89b-12d3-a456-426614174001",
  "customerName": "Compania SRL",
  "customerEmail": "office@compania.ro",
  "orderLines": [
    {
      "productCode": "MONITOR-24",
      "quantity": 10,
      "unitPrice": 899.99
    },
    {
      "productCode": "KEYBOARD-MEC",
      "quantity": 10,
      "unitPrice": 299.99
    },
    {
      "productCode": "MOUSE-USB-05",
      "quantity": 10,
      "unitPrice": 49.99
    }
  ],
  "totalAmount": 12499.70
}
```

**Rezultat așteptat:**
- `subtotal`: 12499.70
- `tax`: 2374.94
- `total`: 14874.64

---

## Test 3: Generare Factură - Gaming Setup

**Metodă:** POST  
**Endpoint:** http://localhost:5260/api/invoice/generate

**JSON pentru copiat:**
```json
{
  "orderId": "323e4567-e89b-12d3-a456-426614174002",
  "customerName": "Alexandru Gamer",
  "customerEmail": "alex.gamer@yahoo.com",
  "orderLines": [
    {
      "productCode": "LAPTOP-001",
      "quantity": 1,
      "unitPrice": 3499.99
    },
    {
      "productCode": "HEADSET-G502",
      "quantity": 1,
      "unitPrice": 249.99
    },
    {
      "productCode": "MONITOR-24",
      "quantity": 1,
      "unitPrice": 899.99
    },
    {
      "productCode": "WEBCAM-HD",
      "quantity": 1,
      "unitPrice": 199.99
    }
  ],
  "totalAmount": 4849.96
}
```

**Rezultat așteptat:**
- `subtotal`: 4849.96
- `tax`: 921.49
- `total`: 5771.45

---

## Test 4: Listare Toate Facturile

**Metodă:** GET  
**Endpoint:** http://localhost:5260/api/invoice

**Fără JSON - request simplu GET**

**Rezultat așteptat:**
- Status: 200 OK
- Array cu toate facturile generate
- Ordonate descrescător după `generatedAt`

---

## Test 5: Obținere Factură Specifică

**Metodă:** GET  
**Endpoint:** http://localhost:5260/api/invoice/{invoiceId}

**Instrucțiuni:**
1. Ia un `invoiceId` din răspunsul testului 1, 2 sau 3
2. Înlocuiește `{invoiceId}` în URL cu GUID-ul real

**Exemplu URL:**
```
http://localhost:5260/api/invoice/8b85cf55-7eb3-41ca-9ae2-9db541f1f517
```

**Rezultat așteptat:**
- Status: 200 OK dacă există
- Status: 404 Not Found dacă nu există

---

## Test 6: Găsire Facturi pentru o Comandă

**Metodă:** GET  
**Endpoint:** http://localhost:5260/api/invoice/order/{orderId}

**Instrucțiuni:**
1. Folosește un `orderId` din testele 1-3
2. Înlocuiește `{orderId}` în URL

**Exemplu URL:**
```
http://localhost:5260/api/invoice/order/123e4567-e89b-12d3-a456-426614174000
```

**Rezultat așteptat:**
- Array cu toate facturile pentru comanda respectivă
- Poate fi gol dacă nu există facturi

---

## Test 7: Update Status - Draft → Generated

**Metodă:** PUT  
**Endpoint:** http://localhost:5260/api/invoice/{invoiceId}/status?newStatus=1

**Instrucțiuni:**
1. Înlocuiește `{invoiceId}` cu un GUID real
2. Query parameter: `newStatus=1`

**Exemplu URL:**
```
http://localhost:5260/api/invoice/8b85cf55-7eb3-41ca-9ae2-9db541f1f517/status?newStatus=1
```

**Rezultat așteptat:**
- Status: 200 OK
- Factura returnată cu `status: 1`

---

## Test 8: Update Status - Generated → Sent

**Metodă:** PUT  
**Endpoint:** http://localhost:5260/api/invoice/{invoiceId}/status?newStatus=2

**Exemplu URL:**
```
http://localhost:5260/api/invoice/8b85cf55-7eb3-41ca-9ae2-9db541f1f517/status?newStatus=2
```

**Rezultat așteptat:**
- `status: 2` (Sent)

---

## Test 9: Update Status - Sent → Paid

**Metodă:** PUT  
**Endpoint:** http://localhost:5260/api/invoice/{invoiceId}/status?newStatus=3

**Exemplu URL:**
```
http://localhost:5260/api/invoice/8b85cf55-7eb3-41ca-9ae2-9db541f1f517/status?newStatus=3
```

**Rezultat așteptat:**
- `status: 3` (Paid - Plătită)

---

## Test 10: Verificare Factură După Update

**Metodă:** GET  
**Endpoint:** http://localhost:5260/api/invoice/{invoiceId}

**Instrucțiuni:**
După testele 7-9, verifică că statusul s-a actualizat corect.

---

## Exemple Calcul TVA

### Exemplu 1: Comandă mică
```
Subtotal: 3599.97 RON
TVA (19%): 3599.97 × 0.19 = 683.99 RON
Total: 3599.97 + 683.99 = 4283.96 RON
```

### Exemplu 2: Comandă mare
```
Subtotal: 12499.70 RON
TVA (19%): 12499.70 × 0.19 = 2374.94 RON
Total: 12499.70 + 2374.94 = 14874.64 RON
```

### Exemplu 3: Gaming setup
```
Subtotal: 4849.96 RON
TVA (19%): 4849.96 × 0.19 = 921.49 RON
Total: 4849.96 + 921.49 = 5771.45 RON
```

---

## Flux Complet de Testare

1. **Generează factură** (Test 1) → salvează `invoiceId`
2. **Verifică factura** (Test 5) → status = 1 (Generated)
3. **Update la Sent** (Test 8) → status = 2
4. **Update la Paid** (Test 9) → status = 3
5. **Verifică finală** (Test 10) → confirmă status = 3

---

## Notițe pentru Testare în Swagger

1. Deschide http://localhost:5260/swagger
2. Toate endpoint-urile sunt listate cu documentație
3. Click "Try it out" pe endpoint-ul dorit
4. Copiază JSON-ul din acest document
5. Click "Execute"
6. Verifică răspunsul și salvează ID-urile necesare
