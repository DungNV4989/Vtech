BEGIN TRANSACTION;
GO

ALTER TABLE [DraftTickets] ADD [Status] int NOT NULL DEFAULT 0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230427091903_add_Status_DraftTicket', N'6.0.5');
GO

COMMIT;
GO

