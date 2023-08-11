BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[WarehousingBillProducts]') AND [c].[name] = N'Unit');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [WarehousingBillProducts] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [WarehousingBillProducts] ALTER COLUMN [Unit] int NOT NULL;
ALTER TABLE [WarehousingBillProducts] ADD DEFAULT 0 FOR [Unit];
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230403044025_chageColumnTypeUnit', N'6.0.5');
GO

COMMIT;
GO
