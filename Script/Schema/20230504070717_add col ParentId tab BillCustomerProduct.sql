BEGIN TRANSACTION;
GO

ALTER TABLE [BillCustomerProducts] ADD [ParentId] uniqueidentifier NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230504070717_add col ParentId tab BillCustomerProduct', N'6.0.5');
GO

COMMIT;
GO
