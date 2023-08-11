BEGIN TRANSACTION;
GO

ALTER TABLE [HistoryChangeCostPriceProducts] ADD [Type] int NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230524025121_add col Type table HistoryChangeCostPriceProduct', N'6.0.5');
GO

COMMIT;
GO
