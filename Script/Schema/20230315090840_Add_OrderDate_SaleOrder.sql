BEGIN TRANSACTION;
GO

ALTER TABLE [SaleOrders] ADD [OrderDate] datetime2 NOT NULL DEFAULT '0001-01-01T000000.0000000';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230315090840_Add_OrderDate_SaleOrder', N'6.0.5');
GO

COMMIT;
GO
