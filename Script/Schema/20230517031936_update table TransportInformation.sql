BEGIN TRANSACTION;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[TransportInfomations]') AND [c].[name] = N'ActionSource');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [TransportInfomations] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [TransportInfomations] ALTER COLUMN [ActionSource] int NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230517031936_update table TransportInformation', N'6.0.5');
GO

COMMIT;
GO
