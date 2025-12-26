-- Delete Specific Order
-- Șterge o comandă specifică (cascade delete va șterge și liniile)

USE OrderProcessingDb;
GO

-- Method 1: Delete by Order ID (recommended)
-- Replace 'YOUR-GUID-HERE' with actual Order ID
DECLARE @OrderId UNIQUEIDENTIFIER = 'YOUR-GUID-HERE';

-- Show order before deletion
SELECT 'Order to be deleted:' AS Info;
SELECT * FROM Orders WHERE Id = @OrderId;
SELECT * FROM OrderLines WHERE OrderId = @OrderId;
GO

-- Uncomment to delete:
-- DELETE FROM Orders WHERE Id = @OrderId;
-- PRINT 'Order deleted successfully!';
GO

-- Method 2: Delete by customer email (deletes ALL orders for that customer)
-- Be careful with this!
DECLARE @CustomerEmail NVARCHAR(200) = 'test@example.com';

-- Show orders before deletion
SELECT 'Orders to be deleted:' AS Info;
SELECT * FROM Orders WHERE CustomerEmail = @CustomerEmail;
GO

-- Uncomment to delete:
-- DELETE FROM Orders WHERE CustomerEmail = @CustomerEmail;
-- PRINT 'All orders for customer deleted successfully!';
GO

-- Method 3: Delete old orders (older than X days)
DECLARE @DaysOld INT = 30;

-- Show orders before deletion
SELECT 'Orders older than ' + CAST(@DaysOld AS NVARCHAR) + ' days:' AS Info;
SELECT * FROM Orders WHERE PlacedAt < DATEADD(day, -@DaysOld, GETDATE());
GO

-- Uncomment to delete:
-- DELETE FROM Orders WHERE PlacedAt < DATEADD(day, -@DaysOld, GETDATE());
-- PRINT 'Old orders deleted successfully!';
GO
