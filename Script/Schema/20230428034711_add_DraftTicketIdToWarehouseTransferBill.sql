BEGIN TRANSACTION;
GO

ALTER TABLE [WarehouseTransferBills] ADD [DraftTicketId] uniqueidentifier NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230428034711_add_DraftTicketIdToWarehouseTransferBill', N'6.0.5');
GO

COMMIT;
GO

