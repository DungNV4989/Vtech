BEGIN TRANSACTION;
GO

ALTER TABLE [TransportInfomations] ADD [ActionSource] int NOT NULL DEFAULT 0;
GO

ALTER TABLE [TransportInfomations] ADD [SourceId] uniqueidentifier NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230517025709_add col SourceId table TransportInformation', N'6.0.5');
GO

COMMIT;
GO