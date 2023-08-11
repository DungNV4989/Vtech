BEGIN TRANSACTION;
GO

CREATE SEQUENCE [DraftTicketNumbers] AS int START WITH 1 INCREMENT BY 1 NO MINVALUE NO MAXVALUE NO CYCLE;
GO

CREATE TABLE [DraftTicketProducts] (
    [Id] uniqueidentifier NOT NULL,
    [DraftTicketId] uniqueidentifier NOT NULL,
    [ProductId] uniqueidentifier NOT NULL,
    [Quantity] int NOT NULL,
    [ConfirmQuatity] int NULL,
    [CostPrice] decimal(18,2) NULL,
    [Note] nvarchar(max) NULL,
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
    CONSTRAINT [PK_DraftTicketProducts] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [DraftTickets] (
    [Id] uniqueidentifier NOT NULL,
    [Code] nvarchar(max) NULL DEFAULT (FORMAT(NEXT VALUE FOR DraftTicketNumbers,'0000000000')),
    [SourceStoreId] uniqueidentifier NOT NULL,
    [DestinationStoreId] uniqueidentifier NOT NULL,
    [Note] nvarchar(max) NULL,
    [TransferStatus] int NOT NULL,
    [CreatedFrom] int NOT NULL,
    [TransferBillType] int NULL,
    [DraftApprovedUserId] uniqueidentifier NULL,
    [DraftApprovedDate] datetime2 NULL,
    [DeliveryConfirmedUserId] uniqueidentifier NULL,
    [DeliveryConfirmedDate] datetime2 NULL,
    [TranferDate] datetime2 NULL,
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
    CONSTRAINT [PK_DraftTickets] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230426082424_Add_DraftTicket_DraftTicketProduct', N'6.0.5');
GO

COMMIT;
GO

