-- Add Sample Orders
-- Adaugă comenzi de test în baza de date

USE OrderProcessingDb;
GO

-- Order 1: Tech Enthusiast
DECLARE @Order1Id UNIQUEIDENTIFIER = NEWID();
INSERT INTO Orders (Id, CustomerName, CustomerEmail, TotalAmount, PlacedAt)
VALUES (@Order1Id, 'Ion Popescu', 'ion.popescu@email.com', 7249.93, GETDATE());

INSERT INTO OrderLines (OrderId, ProductCode, Quantity, Price, LineTotal)
VALUES
    (@Order1Id, 'LAPTOP-001', 2, 3499.99, 6999.98),
    (@Order1Id, 'MOUSE-USB-05', 5, 49.99, 249.95);
GO

-- Order 2: Office Setup
DECLARE @Order2Id UNIQUEIDENTIFIER = NEWID();
INSERT INTO Orders (Id, CustomerName, CustomerEmail, TotalAmount, PlacedAt)
VALUES (@Order2Id, 'Maria Ionescu', 'maria@company.ro', 3199.96, GETDATE());

INSERT INTO OrderLines (OrderId, ProductCode, Quantity, Price, LineTotal)
VALUES
    (@Order2Id, 'MONITOR-27', 2, 1299.99, 2599.98),
    (@Order2Id, 'KEYBOARD-MECH', 2, 299.99, 599.98);
GO

-- Order 3: Gamer Setup
DECLARE @Order3Id UNIQUEIDENTIFIER = NEWID();
INSERT INTO Orders (Id, CustomerName, CustomerEmail, TotalAmount, PlacedAt)
VALUES (@Order3Id, 'Alex Georgescu', 'alex.gamer@gmail.com', 5149.94, GETDATE());

INSERT INTO OrderLines (OrderId, ProductCode, Quantity, Price, LineTotal)
VALUES
    (@Order3Id, 'LAPTOP-001', 1, 3499.99, 3499.99),
    (@Order3Id, 'HEADSET-WIRELESS', 1, 199.99, 199.99),
    (@Order3Id, 'MONITOR-27', 1, 1299.99, 1299.99),
    (@Order3Id, 'WEBCAM-HD', 1, 149.99, 149.99);
GO

-- Order 4: Home Office
DECLARE @Order4Id UNIQUEIDENTIFIER = NEWID();
INSERT INTO Orders (Id, CustomerName, CustomerEmail, TotalAmount, PlacedAt)
VALUES (@Order4Id, 'Elena Dumitrescu', 'elena.d@remote.com', 1279.96, GETDATE());

INSERT INTO OrderLines (OrderId, ProductCode, Quantity, Price, LineTotal)
VALUES
    (@Order4Id, 'DESK-LAMP', 2, 79.99, 159.98),
    (@Order4Id, 'CHAIR-ERGONOMIC', 1, 899.99, 899.99),
    (@Order4Id, 'MOUSE-USB-05', 4, 49.99, 199.96);
GO

PRINT 'Added 4 sample orders successfully!';
GO

-- Verify
SELECT COUNT(*) AS TotalOrders FROM Orders;
SELECT COUNT(*) AS TotalOrderLines FROM OrderLines;
GO
