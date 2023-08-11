BEGIN TRANSACTION;
GO

ALTER TABLE [WarehouseTransferBillProducts] ADD [CanTransfer] int NOT NULL DEFAULT 0;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230621024800_add_CanTransfer_WarehouseTransferBill', N'6.0.5');
GO

COMMIT;
GO

