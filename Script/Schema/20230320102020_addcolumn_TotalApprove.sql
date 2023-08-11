ALTER TABLE [SaleOrders] ADD [TotalApprove] int NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230320102020_addcolumn_TotalApprove', N'6.0.5');