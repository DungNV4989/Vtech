BEGIN TRANSACTION;
GO

ALTER TABLE [BillCustomerProductBonus] ADD [CostPrice] decimal(18,2) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230606071146_add col CostPrice table BillCustomerProductBonus', N'6.0.5');
GO

COMMIT;
GO
