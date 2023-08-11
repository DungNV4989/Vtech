BEGIN TRANSACTION;
GO

ALTER TABLE [Products] ADD [Code] nvarchar(max) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230315072850_addCodeProduct', N'6.0.5');
GO

COMMIT;
GO

