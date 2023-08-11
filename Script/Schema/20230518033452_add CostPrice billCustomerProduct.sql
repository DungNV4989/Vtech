BEGIN TRANSACTION;
GO

ALTER TABLE [BillCustomerProducts] ADD [CostPrice] decimal(18,2) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230518033452_add CostPrice billCustomerProduct', N'6.0.5');
GO

COMMIT;
GO