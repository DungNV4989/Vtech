BEGIN TRANSACTION;
GO

DROP TABLE [Districts];
GO

DROP TABLE [Nationals];
GO

DROP TABLE [Provinces];
GO

DROP TABLE [Wards];
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230321040954_droptable_HCVN', N'6.0.5');
GO

COMMIT;
GO