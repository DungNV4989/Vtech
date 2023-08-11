BEGIN TRANSACTION;
GO

CREATE TABLE [WarehousingBillLogs] (
    [Id] uniqueidentifier NOT NULL,
    [ActionId] uniqueidentifier NOT NULL,
    [WarehousingBillId] uniqueidentifier NOT NULL,
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
    CONSTRAINT [PK_WarehousingBillLogs] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230329075647_add_warehousing_log', N'6.0.5');
GO

COMMIT;
GO

