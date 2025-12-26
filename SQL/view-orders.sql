-- View All Orders with Details
-- Afișează toate comenzile cu liniile lor

USE OrderProcessingDb;
GO

-- Show all orders with their lines
SELECT 
    o.Id AS OrderId,
    o.CustomerName,
    o.CustomerEmail,
    o.TotalAmount,
    o.PlacedAt,
    ol.ProductCode,
    ol.Quantity,
    ol.Price,
    ol.LineTotal
FROM Orders o
INNER JOIN OrderLines ol ON o.Id = ol.OrderId
ORDER BY o.PlacedAt DESC, ol.Id;
GO

-- Summary: Total orders and revenue
SELECT 
    COUNT(*) AS TotalOrders,
    SUM(TotalAmount) AS TotalRevenue,
    AVG(TotalAmount) AS AverageOrderValue
FROM Orders;
GO

-- Orders by customer
SELECT 
    CustomerEmail,
    COUNT(*) AS NumberOfOrders,
    SUM(TotalAmount) AS TotalSpent
FROM Orders
GROUP BY CustomerEmail
ORDER BY TotalSpent DESC;
GO

-- Most popular products
SELECT 
    ProductCode,
    SUM(Quantity) AS TotalQuantitySold,
    COUNT(*) AS TimesOrdered,
    SUM(LineTotal) AS TotalRevenue
FROM OrderLines
GROUP BY ProductCode
ORDER BY TotalQuantitySold DESC;
GO
