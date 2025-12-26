-- Search and Query Examples
-- Exemple de căutare și interogări utile

USE OrderProcessingDb;
GO

-- 1. Find orders by customer email
DECLARE @Email NVARCHAR(200) = 'ion.popescu@email.com'; -- Change this
SELECT o.*, ol.*
FROM Orders o
INNER JOIN OrderLines ol ON o.Id = ol.OrderId
WHERE o.CustomerEmail LIKE '%' + @Email + '%';
GO

-- 2. Find orders by date range
DECLARE @StartDate DATETIME2 = '2025-12-01';
DECLARE @EndDate DATETIME2 = '2025-12-31';
SELECT *
FROM Orders
WHERE PlacedAt BETWEEN @StartDate AND @EndDate
ORDER BY PlacedAt DESC;
GO

-- 3. Find orders containing specific product
DECLARE @ProductCode NVARCHAR(50) = 'LAPTOP-001'; -- Change this
SELECT DISTINCT o.*
FROM Orders o
INNER JOIN OrderLines ol ON o.Id = ol.OrderId
WHERE ol.ProductCode = @ProductCode;
GO

-- 4. Find orders above certain amount
DECLARE @MinAmount DECIMAL(18,2) = 5000.00; -- Change this
SELECT *
FROM Orders
WHERE TotalAmount >= @MinAmount
ORDER BY TotalAmount DESC;
GO

-- 5. Find recent orders (last 7 days)
SELECT *
FROM Orders
WHERE PlacedAt >= DATEADD(day, -7, GETDATE())
ORDER BY PlacedAt DESC;
GO

-- 6. Get order details by Order ID
DECLARE @OrderId UNIQUEIDENTIFIER = 'YOUR-GUID-HERE'; -- Replace with actual GUID
SELECT 
    o.Id,
    o.CustomerName,
    o.CustomerEmail,
    o.PlacedAt,
    ol.ProductCode,
    ol.Quantity,
    ol.Price,
    ol.LineTotal
FROM Orders o
LEFT JOIN OrderLines ol ON o.Id = ol.OrderId
WHERE o.Id = @OrderId;
GO

-- 7. Top 5 customers by spending
SELECT TOP 5
    CustomerName,
    CustomerEmail,
    COUNT(*) AS OrderCount,
    SUM(TotalAmount) AS TotalSpent
FROM Orders
GROUP BY CustomerName, CustomerEmail
ORDER BY TotalSpent DESC;
GO

-- 8. Orders with multiple items
SELECT 
    o.Id,
    o.CustomerName,
    COUNT(ol.Id) AS ItemCount,
    o.TotalAmount
FROM Orders o
INNER JOIN OrderLines ol ON o.Id = ol.OrderId
GROUP BY o.Id, o.CustomerName, o.TotalAmount
HAVING COUNT(ol.Id) > 1
ORDER BY ItemCount DESC;
GO
