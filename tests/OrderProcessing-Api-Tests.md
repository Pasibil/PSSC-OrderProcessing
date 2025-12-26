# Order Processing API - Manual Testing Guide

**Port:** 5259  
**Base URL:** http://localhost:5259  
**Swagger UI:** http://localhost:5259/swagger

## Produse Disponibile

| Cod Produs | Denumire | Preț (RON) |
|------------|----------|------------|
| LAPTOP-001 | Laptop | 3499.99 |
| MOUSE-USB-05 | Mouse USB | 49.99 |
| KEYBOARD-MEC | Tastatură Mecanică | 299.99 |
| MONITOR-24 | Monitor 24" | 899.99 |
| HEADSET-G502 | Căști Gaming | 249.99 |
| WEBCAM-HD | Webcam HD | 199.99 |
| USB-HUB-7 | Hub USB 7 porturi | 89.99 |
| DESK-LAMP | Lampă Birou | 129.99 |
| OFFICE-CHAIR | Scaun Birou | 999.99 |
| MOUSEPAD-XL | Mousepad XL | 79.99 |

---

## Test 1: Creare Comandă - Laptop + Mouse

**Metodă:** POST  
**Endpoint:** http://localhost:5259/api/orders  
**Content-Type:** application/json

**JSON pentru copiat:**
```json
{
  "customerName": "Ion Popescu",
  "customerEmail": "ion.popescu@example.com",
  "orderLines": [
    {
      "productCode": "LAPTOP-001",
      "quantity": 1
    },
    {
      "productCode": "MOUSE-USB-05",
      "quantity": 2
    }
  ]
}
```

**Rezultat așteptat:**
- Status: 201 Created
- `success: true`
- `orderId`: GUID-ul comenzii create
- `orderDetails` cu total calculat: 3599.97 RON
- Salvează `orderId` pentru testele următoare

---

## Test 2: Creare Comandă - Monitor + Keyboard

**Metodă:** POST  
**Endpoint:** http://localhost:5259/api/orders

**JSON pentru copiat:**
```json
{
  "customerName": "Maria Ionescu",
  "customerEmail": "maria.ionescu@gmail.com",
  "orderLines": [
    {
      "productCode": "MONITOR-24",
      "quantity": 2
    },
    {
      "productCode": "KEYBOARD-MEC",
      "quantity": 1
    }
  ]
}
```

**Rezultat așteptat:**
- Total: 2099.97 RON (2 × 899.99 + 1 × 299.99)

---

## Test 3: Creare Comandă - Gaming Setup

**Metodă:** POST  
**Endpoint:** http://localhost:5259/api/orders

**JSON pentru copiat:**
```json
{
  "customerName": "Alexandru Gamer",
  "customerEmail": "alex.gamer@yahoo.com",
  "orderLines": [
    {
      "productCode": "LAPTOP-001",
      "quantity": 1
    },
    {
      "productCode": "HEADSET-G502",
      "quantity": 1
    },
    {
      "productCode": "MONITOR-24",
      "quantity": 1
    },
    {
      "productCode": "WEBCAM-HD",
      "quantity": 1
    }
  ]
}
```

**Rezultat așteptat:**
- Total: 4849.96 RON (3499.99 + 249.99 + 899.99 + 199.99)

---

## Test 4: Creare Comandă - Office Setup (Comandă Mare)

**Metodă:** POST  
**Endpoint:** http://localhost:5259/api/orders

**JSON pentru copiat:**
```json
{
  "customerName": "Compania SRL",
  "customerEmail": "office@compania.ro",
  "orderLines": [
    {
      "productCode": "DESK-LAMP",
      "quantity": 5
    },
    {
      "productCode": "KEYBOARD-MEC",
      "quantity": 5
    },
    {
      "productCode": "MOUSE-USB-05",
      "quantity": 5
    },
    {
      "productCode": "MONITOR-24",
      "quantity": 5
    }
  ]
}
```

**Rezultat așteptat:**
- Total: 6899.75 RON (cinci seturi complete pentru birou)

---

## Test 5: Creare Comandă - Produs Singular

**Metodă:** POST  
**Endpoint:** http://localhost:5259/api/orders

**JSON pentru copiat:**
```json
{
  "customerName": "Test User",
  "customerEmail": "test@test.com",
  "orderLines": [
    {
      "productCode": "OFFICE-CHAIR",
      "quantity": 1
    }
  ]
}
```

**Rezultat așteptat:**
- Total: 999.99 RON

---

## Test 6: Listare Toate Comenzile

**Metodă:** GET  
**Endpoint:** http://localhost:5259/api/orders

**Fără JSON - request simplu GET**

**Rezultat așteptat:**
- Status: 200 OK
- Array cu toate comenzile create
- Fiecare comandă include `orderLines` expandate

---

## Test 7: Obținere Comandă Specifică

**Metodă:** GET  
**Endpoint:** http://localhost:5259/api/orders/{orderId}

**Instrucțiuni:**
1. Ia un `orderId` din răspunsul testului 1 sau 6
2. Înlocuiește `{orderId}` în URL cu GUID-ul real

**Exemplu URL:**
```
http://localhost:5259/api/orders/123e4567-e89b-12d3-a456-426614174000
```

**Rezultat așteptat:**
- Status: 200 OK dacă există
- Status: 404 Not Found dacă nu există

---

## Test 8: Eroare - Produs Inexistent

**Metodă:** POST  
**Endpoint:** http://localhost:5259/api/orders

**JSON pentru copiat:**
```json
{
  "customerName": "Test Error",
  "customerEmail": "error@test.com",
  "orderLines": [
    {
      "productCode": "INVALID-PRODUCT",
      "quantity": 1
    }
  ]
}
```

**Rezultat așteptat:**
- Status: 200 OK (workflow returnează succes/eroare în body)
- `success: false`
- `errorMessage`: mesaj despre produs inexistent

---

## Test 9: Eroare - Email Invalid

**Metodă:** POST  
**Endpoint:** http://localhost:5259/api/orders

**JSON pentru copiat:**
```json
{
  "customerName": "Test Error",
  "customerEmail": "email-invalid",
  "orderLines": [
    {
      "productCode": "LAPTOP-001",
      "quantity": 1
    }
  ]
}
```

**Rezultat așteptat:**
- `success: false`
- `errorMessage`: eroare validare email (trebuie format valid)

---

## Test 10: Eroare - Cantitate Zero

**Metodă:** POST  
**Endpoint:** http://localhost:5259/api/orders

**JSON pentru copiat:**
```json
{
  "customerName": "Test Error",
  "customerEmail": "error@test.com",
  "orderLines": [
    {
      "productCode": "LAPTOP-001",
      "quantity": 0
    }
  ]
}
```

**Rezultat așteptat:**
- `success: false`
- `errorMessage`: cantitatea trebuie să fie mai mare decât 0

---

## Notițe pentru Testare în Swagger

1. Deschide http://localhost:5259/swagger
2. Click pe endpoint-ul dorit (POST /api/orders sau GET /api/orders)
3. Click "Try it out"
4. Copiază JSON-ul din acest document
5. Click "Execute"
6. Verifică răspunsul în secțiunea "Response body"

## Verificare în Baza de Date

După creare comenzi, poți verifica în SQL Server:

```sql
USE OrderProcessingDb;

-- Vezi toate comenzile
SELECT * FROM Orders ORDER BY PlacedAt DESC;

-- Vezi liniile de comandă
SELECT o.OrderId, o.CustomerName, ol.ProductCode, ol.Quantity, ol.Price
FROM Orders o
JOIN OrderLines ol ON o.Id = ol.OrderId
ORDER BY o.PlacedAt DESC;
```
