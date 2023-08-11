BEGIN TRANSACTION;
GO

ALTER TABLE [AbpUsers] ADD [IsVTech] bit NOT NULL DEFAULT CAST(1 AS bit);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230315074009_addIsVTechIdentityUser', N'6.0.5');
GO

COMMIT;
GO

