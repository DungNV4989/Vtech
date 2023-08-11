BEGIN TRANSACTION;
GO

ALTER TABLE [OrderTransports] ADD [DateArrive] datetime2 NULL;
GO

ALTER TABLE [OrderTransports] ADD [DateTransport] datetime2 NULL;
GO

ALTER TABLE [OrderTransports] ADD [TotalPrice] decimal(18,2) NOT NULL DEFAULT 0.0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230414015137_update table orderTransport', N'6.0.5');
GO

COMMIT;
GO