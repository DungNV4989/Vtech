BEGIN TRANSACTION;
GO

CREATE TABLE [GroupTransportInformation] (
    [Id] uniqueidentifier NOT NULL,
    [ParentTransportInformationId] uniqueidentifier NULL,
    [ChildTransportInformationId] uniqueidentifier NULL,
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
    CONSTRAINT [PK_GroupTransportInformation] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [TransporstBills] (
    [Id] uniqueidentifier NOT NULL,
    [TransportInformationId] uniqueidentifier NULL,
    [BillCustomerId] uniqueidentifier NULL,
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
    CONSTRAINT [PK_TransporstBills] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230602084002_addTableTransporstBills', N'6.0.5');
GO

COMMIT;
GO

