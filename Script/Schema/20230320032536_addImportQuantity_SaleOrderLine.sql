BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[SaleOrders]') AND [c].[name] = N'IsConfirm');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [SaleOrders] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [SaleOrders] DROP COLUMN [IsConfirm];
GO

ALTER TABLE [Stores] ADD [Address] nvarchar(max) NULL;
GO

ALTER TABLE [SaleOrders] ADD [Confirm] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [SaleOrders] ADD [Note] nvarchar(max) NULL;
GO

ALTER TABLE [SaleOrders] ADD [Package] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [SaleOrderLines] ADD [ImportQuantity] int NULL;
GO

ALTER TABLE [SaleOrderLines] ADD [Note] nvarchar(max) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230320032536_addImportQuantity_SaleOrderLine', N'6.0.5');
GO

COMMIT;
GO

