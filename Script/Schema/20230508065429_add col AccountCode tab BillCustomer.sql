BEGIN TRANSACTION;
GO

ALTER TABLE [BillCustomers] ADD [AccountCode] nvarchar(max) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230508065429_add col AccountCode tab BillCustomer', N'6.0.5');
GO

COMMIT;
GO