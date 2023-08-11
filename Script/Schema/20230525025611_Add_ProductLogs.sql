BEGIN TRANSACTION;
GO

CREATE SEQUENCE [ProductLogs] AS int START WITH 1 INCREMENT BY 1 NO MINVALUE NO MAXVALUE NO CYCLE;
GO

ALTER TABLE [PaymentReceipts] ADD [IsLiquidity] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

CREATE TABLE [ProductLogss] (
    [Id] uniqueidentifier NOT NULL,
    [Code] nvarchar(max) NULL DEFAULT (FORMAT(NEXT VALUE FOR ProductLogs,'0000000000')),
    [ActionId] uniqueidentifier NOT NULL,
    [ProductId] uniqueidentifier NOT NULL,
    [FromValue] nvarchar(max) NULL,
    [ToValue] nvarchar(max) NULL,
    [Action] int NOT NULL,
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
    CONSTRAINT [PK_ProductLogss] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230525025611_Add_ProductLogs', N'6.0.5');
GO

COMMIT;
GO

