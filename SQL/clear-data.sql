-- Clear All Data (Keep Structure)
-- Șterge toate datele dar păstrează tabelele

USE OrderProcessingDb;
GO

-- Disable foreign key constraints
ALTER TABLE OrderLines NOCHECK CONSTRAINT ALL;
GO

-- Delete all data
DELETE FROM OrderLines;
DELETE FROM Orders;
GO

-- Re-enable foreign key constraints
ALTER TABLE OrderLines CHECK CONSTRAINT ALL;
GO

-- Reset identity on OrderLines
DBCC CHECKIDENT ('OrderLines', RESEED, 0);
GO

PRINT 'All data cleared successfully! Tables structure preserved.';
GO

-- Verify
SELECT COUNT(*) AS OrdersCount FROM Orders;
SELECT COUNT(*) AS OrderLinesCount FROM OrderLines;
GO
