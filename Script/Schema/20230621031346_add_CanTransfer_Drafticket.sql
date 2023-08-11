BEGIN TRANSACTION;
GO

ALTER TABLE [DraftTicketProducts] ADD [CanTransfer] int NOT NULL DEFAULT 0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230621031346_add_CanTransfer_Drafticket', N'6.0.5');
GO

COMMIT;
GO

