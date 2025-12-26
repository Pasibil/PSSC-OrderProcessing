-- Create OrderProcessing Database
USE master;
GO

-- Drop database if exists
IF EXISTS (SELECT name FROM sys.databases WHERE name = N'OrderProcessingDb')
BEGIN
    ALTER DATABASE OrderProcessingDb SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE OrderProcessingDb;
END
GO

-- Create new database
CREATE DATABASE OrderProcessingDb;
GO

USE OrderProcessingDb;
GO

-- Create Orders table
CREATE TABLE Orders (
    Id UNIQUEIDENTIFIER PRIMARY KEY,
    CustomerName NVARCHAR(200) NOT NULL,
    CustomerEmail NVARCHAR(200) NOT NULL,
    TotalAmount DECIMAL(18,2) NOT NULL,
    PlacedAt DATETIME2 NOT NULL
);
GO

-- Create OrderLines table
CREATE TABLE OrderLines (
    Id INT PRIMARY KEY IDENTITY(1,1),
    OrderId UNIQUEIDENTIFIER NOT NULL,
    ProductCode NVARCHAR(50) NOT NULL,
    Quantity INT NOT NULL,
    Price DECIMAL(18,2) NOT NULL,
    LineTotal DECIMAL(18,2) NOT NULL,
    CONSTRAINT FK_OrderLines_Orders FOREIGN KEY (OrderId) REFERENCES Orders(Id) ON DELETE CASCADE
);
GO

-- Create indexes for better performance
CREATE INDEX IX_OrderLines_OrderId ON OrderLines(OrderId);
CREATE INDEX IX_Orders_PlacedAt ON Orders(PlacedAt DESC);
CREATE INDEX IX_Orders_CustomerEmail ON Orders(CustomerEmail);
GO

-- Insert sample data for testing
INSERT INTO Orders (Id, CustomerName, CustomerEmail, TotalAmount, PlacedAt)
VALUES 
    (NEWID(), 'Test User', 'test@example.com', 3999.97, GETDATE());
GO

DECLARE @OrderId UNIQUEIDENTIFIER = (SELECT TOP 1 Id FROM Orders);

INSERT INTO OrderLines (OrderId, ProductCode, Quantity, Price, LineTotal)
VALUES
    (@OrderId, 'LAPTOP-001', 1, 3499.99, 3499.99),
    (@OrderId, 'MOUSE-USB-05', 10, 49.99, 499.98);
GO

-- Verify data
SELECT * FROM Orders;
SELECT * FROM OrderLines;
GO

PRINT 'Database OrderProcessingDb created successfully!';
GO
