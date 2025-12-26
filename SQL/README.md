# SQL Scripts pentru OrderProcessing Database

Acest folder conține scripturi SQL utile pentru administrarea bazei de date OrderProcessingDb.

## 📄 Scripturi disponibile:

### 1. **create-db.sql** - Creare bază de date
**Scop**: Creează baza de date de la zero cu structura completă și date de test.
- Șterge baza existentă (dacă există)
- Creează tabele Orders și OrderLines
- Configurează relații și indecși
- Adaugă date de test inițiale

**Când să folosești**: Prima dată când configurezi proiectul sau pentru resetare completă.

### 2. **view-orders.sql** - Vizualizare date
**Scop**: Afișează toate comenzile cu detalii complete.
- Lista tuturor comenzilor cu liniile lor
- Statistici generale (total comenzi, revenue)
- Comenzi grupate pe clienți
- Produse cele mai populare

**Când să folosești**: Pentru a verifica datele existente în baza de date.

### 3. **add-sample-orders.sql** - Adăugare date de test
**Scop**: Adaugă 4 comenzi de exemplu cu profile diferite.
- Tech Enthusiast (LAPTOP + MOUSE)
- Office Setup (MONITOR + KEYBOARD)
- Gamer Setup (LAPTOP + HEADSET + MONITOR + WEBCAM)
- Home Office (DESK-LAMP + CHAIR + MOUSE)

**Când să folosești**: Pentru a popula baza de date cu date de test realiste.

### 4. **search-queries.sql** - Exemple de căutare
**Scop**: Interogări utile pentru căutarea și filtrarea comenzilor.
- Căutare după email client
- Căutare după interval de date
- Căutare după produs specific
- Filtrare după sumă minimă
- Comenzi recente (ultimele 7 zile)
- Top 5 clienți după spending
- Comenzi cu multiple produse

**Când să folosești**: Pentru a găsi comenzi specifice sau a face analize.

### 5. **clear-data.sql** - Curățare date
**Scop**: Șterge toate datele dar păstrează structura tabelelor.
- Șterge Orders și OrderLines
- Resetează identity counter
- Păstrează tabele și relații

**Când să folosești**: Pentru a reseta datele fără a recrea baza de date.

### 6. **delete-orders.sql** - Ștergere comenzi specifice
**Scop**: Șterge comenzi individuale sau în grup.
- Ștergere după Order ID
- Ștergere după email client
- Ștergere comenzi vechi (peste X zile)

**Când să folosești**: Pentru a șterge comenzi specifice (comenzile de test, comenzi eronate, etc.).

## 🔧 Cum să folosești scripturile:

### În SQL Server Management Studio (SSMS):
1. Deschide SSMS
2. Conectează-te la `.\SQLEXPRESS`
3. File → Open → File... și selectează scriptul
4. Apasă F5 sau click pe Execute

### În Azure Data Studio:
1. Deschide Azure Data Studio
2. Conectează-te la server
3. File → Open File și selectează scriptul
4. Click pe Run sau F5

## ⚠️ Avertismente:

- **create-db.sql**: Va șterge baza de date existentă! Fă backup dacă e nevoie.
- **clear-data.sql**: Șterge toate datele! Nu poate fi recuperat.
- **delete-orders.sql**: Verifică ÎNTOTDEAUNA ce ștergi înainte să decomentezi DELETE.

## 📊 Verificare rapidă:

```sql
USE OrderProcessingDb;
SELECT COUNT(*) AS Orders FROM Orders;
SELECT COUNT(*) AS OrderLines FROM OrderLines;
```

## 🔗 Connection String:

```
Server=.\SQLEXPRESS;Database=OrderProcessingDb;Trusted_Connection=true;MultipleActiveResultSets=true;TrustServerCertificate=true
```
