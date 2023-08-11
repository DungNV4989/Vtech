BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Wards]') AND [c].[name] = N'OldId');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [Wards] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [Wards] DROP COLUMN [OldId];
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Provinces]') AND [c].[name] = N'OldId');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Provinces] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [Provinces] DROP COLUMN [OldId];
GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Nationals]') AND [c].[name] = N'OldId');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [Nationals] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [Nationals] DROP COLUMN [OldId];
GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Districts]') AND [c].[name] = N'OldId');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [Districts] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [Districts] DROP COLUMN [OldId];
GO

ALTER TABLE [Wards] ADD [IdInt] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [Provinces] ADD [IdInt] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [Nationals] ADD [IdInt] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [Districts] ADD [IdInt] int NOT NULL DEFAULT 0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230315092757_changeColumnTypeHCVN', N'6.0.5');
GO

COMMIT;
GO

