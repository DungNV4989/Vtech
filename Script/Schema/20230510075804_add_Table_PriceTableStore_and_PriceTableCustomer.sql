BEGIN TRANSACTION;
GO

ALTER TABLE [BillCustomers] ADD [AccountCode] nvarchar(max) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230508065429_add col AccountCode tab BillCustomer', N'6.0.5');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE SEQUENCE [ShipperCode] AS int START WITH 1 INCREMENT BY 1 NO MINVALUE NO MAXVALUE NO CYCLE;
GO

DECLARE @var0 sysname;
SELECT @var0 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[TransportInfomations]') AND [c].[name] = N'ToStoreId');
IF @var0 IS NOT NULL EXEC(N'ALTER TABLE [TransportInfomations] DROP CONSTRAINT [' + @var0 + '];');
ALTER TABLE [TransportInfomations] ALTER COLUMN [ToStoreId] nvarchar(max) NULL;
GO

CREATE TABLE [Shippers] (
    [Id] uniqueidentifier NOT NULL,
    [Code] nvarchar(max) NULL DEFAULT (FORMAT(NEXT VALUE FOR ShipperCode,'0000000000')),
    [Name] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [Password] nvarchar(max) NULL,
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
    CONSTRAINT [PK_Shippers] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230508090140_editTable', N'6.0.5');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Debts] ADD [StoreId] uniqueidentifier NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230508105744_debtcustomer', N'6.0.5');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

CREATE SEQUENCE [CustomerCodes] AS int START WITH 1 INCREMENT BY 1 NO MINVALUE NO MAXVALUE NO CYCLE;
GO

DECLARE @var1 sysname;
SELECT @var1 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[Customers]') AND [c].[name] = N'Code');
IF @var1 IS NOT NULL EXEC(N'ALTER TABLE [Customers] DROP CONSTRAINT [' + @var1 + '];');
ALTER TABLE [Customers] ADD DEFAULT (FORMAT(NEXT VALUE FOR CustomerCodes,'0000000000')) FOR [Code];
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230509035915_add_customer_code_seq', N'6.0.5');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var2 sysname;
SELECT @var2 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DraftTickets]') AND [c].[name] = N'CreatedFrom');
IF @var2 IS NOT NULL EXEC(N'ALTER TABLE [DraftTickets] DROP CONSTRAINT [' + @var2 + '];');
ALTER TABLE [DraftTickets] DROP COLUMN [CreatedFrom];
GO

DECLARE @var3 sysname;
SELECT @var3 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[DraftTickets]') AND [c].[name] = N'TransferStatus');
IF @var3 IS NOT NULL EXEC(N'ALTER TABLE [DraftTickets] DROP CONSTRAINT [' + @var3 + '];');
ALTER TABLE [DraftTickets] DROP COLUMN [TransferStatus];
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230509044905_updatetransferdraft', N'6.0.5');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [TransportInfomations] ADD [IsCOD] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

ALTER TABLE [TransportInfomations] ADD [TotalAmount] decimal(18,2) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230509072752_addCOD', N'6.0.5');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [BillCustomers] ADD [AmountAfterDiscount] decimal(18,2) NULL;
GO

ALTER TABLE [BillCustomers] ADD [AmountCustomerPay] decimal(18,2) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230509091158_add col AmountCustomerPay, AmountAfterDiscount tab BillCustomer', N'6.0.5');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

ALTER TABLE [Debts] ADD [HasCod] bit NOT NULL DEFAULT CAST(0 AS bit);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230510035811_updatedebt', N'6.0.5');
GO

COMMIT;
GO

BEGIN TRANSACTION;
GO

DECLARE @var4 sysname;
SELECT @var4 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PriceTables]') AND [c].[name] = N'CustomerId');
IF @var4 IS NOT NULL EXEC(N'ALTER TABLE [PriceTables] DROP CONSTRAINT [' + @var4 + '];');
ALTER TABLE [PriceTables] DROP COLUMN [CustomerId];
GO

DECLARE @var5 sysname;
SELECT @var5 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PriceTables]') AND [c].[name] = N'CustomerType');
IF @var5 IS NOT NULL EXEC(N'ALTER TABLE [PriceTables] DROP CONSTRAINT [' + @var5 + '];');
ALTER TABLE [PriceTables] DROP COLUMN [CustomerType];
GO

DECLARE @var6 sysname;
SELECT @var6 = [d].[name]
FROM [sys].[default_constraints] [d]
INNER JOIN [sys].[columns] [c] ON [d].[parent_column_id] = [c].[column_id] AND [d].[parent_object_id] = [c].[object_id]
WHERE ([d].[parent_object_id] = OBJECT_ID(N'[PriceTables]') AND [c].[name] = N'StoreId');
IF @var6 IS NOT NULL EXEC(N'ALTER TABLE [PriceTables] DROP CONSTRAINT [' + @var6 + '];');
ALTER TABLE [PriceTables] DROP COLUMN [StoreId];
GO

ALTER TABLE [PriceTables] ADD [Status] int NOT NULL DEFAULT 0;
GO

CREATE TABLE [PriceTableCustomer] (
    [Id] uniqueidentifier NOT NULL,
    [PriceTableId] uniqueidentifier NOT NULL,
    [CustomerId] uniqueidentifier NOT NULL,
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
    CONSTRAINT [PK_PriceTableCustomer] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [PriceTableStore] (
    [Id] uniqueidentifier NOT NULL,
    [PriceTableId] uniqueidentifier NOT NULL,
    [StoreId] uniqueidentifier NOT NULL,
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
    CONSTRAINT [PK_PriceTableStore] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230510075804_add_Table_PriceTableStore_and_PriceTableCustomer', N'6.0.5');
GO

COMMIT;
GO

