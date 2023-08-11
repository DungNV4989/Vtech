BEGIN TRANSACTION;
GO

ALTER TABLE [BillCustomers] ADD [AmountAfterDiscount] decimal(18,2) NULL;
GO

ALTER TABLE [BillCustomers] ADD [AmountCustomerPay] decimal(18,2) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230509091158_add col AmountCustomerPay, AmountAfterDiscount tab BillCustomer', N'6.0.5');
GO

COMMIT;
GO

