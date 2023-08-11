BEGIN TRANSACTION;
GO

CREATE SEQUENCE [DebtReminderLogCode] AS int START WITH 1 INCREMENT BY 1 NO MINVALUE NO MAXVALUE NO CYCLE;
GO

CREATE TABLE [DebtReminderLogs] (
    [Id] uniqueidentifier NOT NULL,
    [Code] nvarchar(max) NULL DEFAULT (FORMAT(NEXT VALUE FOR DebtReminderLogCode,'0000000000')),
    [PayDate] datetime2 NULL,
    [Content] nvarchar(max) NULL,
    [CustomerId] uniqueidentifier NULL,
    [CustomerName] nvarchar(max) NULL,
    [HandlerEmployeeId] uniqueidentifier NULL,
    [HandlerEmployeeName] nvarchar(max) NULL,
    [SupportEmployeeId] uniqueidentifier NULL,
    [SupportEmployeeName] nvarchar(max) NULL,
    [HandlerStoreId] uniqueidentifier NULL,
    [HandlerStoreName] nvarchar(max) NULL,
    [ExtraProperties] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(40) NULL,
    [CreationTime] datetime2 NOT NULL,
    [CreatorId] uniqueidentifier NULL,
    [LastModificationTime] datetime2 NULL,
    [LastModifierId] uniqueidentifier NULL,
    [IsDeleted] bit NOT NULL DEFAULT CAST(0 AS bit),
    [DeleterId] uniqueidentifier NULL,
    [DeletionTime] datetime2 NULL,
    [TenantId] uniqueidentifier NULL,
    [IsActive] bit NOT NULL,
    CONSTRAINT [PK_DebtReminderLogs] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230505020834_Add_DebtReminderLogs', N'6.0.5');
GO

COMMIT;
GO

