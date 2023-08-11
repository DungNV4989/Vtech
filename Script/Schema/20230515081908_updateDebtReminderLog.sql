BEGIN TRANSACTION;
GO

ALTER TABLE [DebtReminderLogs] DROP COLUMN [CustomerName];
GO

ALTER TABLE [DebtReminderLogs] DROP COLUMN [HandlerEmployeeId];
GO

ALTER TABLE [DebtReminderLogs] DROP COLUMN [HandlerEmployeeName];
GO

ALTER TABLE [DebtReminderLogs] DROP COLUMN [HandlerStoreId];
GO

ALTER TABLE [DebtReminderLogs] DROP COLUMN [HandlerStoreIds];
GO

ALTER TABLE [DebtReminderLogs] DROP COLUMN [HandlerStoreName];
GO

ALTER TABLE [DebtReminderLogs] DROP COLUMN [SupportEmployeeId];
GO

ALTER TABLE [DebtReminderLogs] DROP COLUMN [SupportEmployeeName];
GO


INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230515081908_updateDebtReminderLog', N'6.0.5');
GO

COMMIT;
GO

