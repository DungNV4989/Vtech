BEGIN TRANSACTION;
GO

CREATE TABLE [BillCustomerProductBonus] (
    [Id] uniqueidentifier NOT NULL,
    [BillCustomerProductId] uniqueidentifier NULL,
    [ProductId] uniqueidentifier NULL,
    [Quantity] int NOT NULL,
    [IsDebt] bit NULL,
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
    CONSTRAINT [PK_BillCustomerProductBonus] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [BillCustomerProducts] (
    [Id] uniqueidentifier NOT NULL,
    [ProductId] uniqueidentifier NULL,
    [BillCustomerId] uniqueidentifier NULL,
    [Price] decimal(18,2) NULL,
    [Quantity] int NOT NULL,
    [TablePriceId] uniqueidentifier NULL,
    [DiscountValue] decimal(18,2) NULL,
    [DiscountUnit] int NULL,
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
    CONSTRAINT [PK_BillCustomerProducts] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [BillCustomers] (
    [Id] uniqueidentifier NOT NULL,
    [CustomerId] uniqueidentifier NULL,
    [StoreId] uniqueidentifier NULL,
    [EmployeeNote] nvarchar(max) NULL,
    [TablePriceId] uniqueidentifier NULL,
    [EmployeeCare] uniqueidentifier NULL,
    [EmployeeSell] uniqueidentifier NULL,
    [VatValue] decimal(18,2) NULL,
    [VatUnit] int NULL,
    [DiscountValue] decimal(18,2) NULL,
    [DiscountUnit] int NULL,
    [CustomerBillPayStatus] int NULL,
    [Cash] decimal(18,2) NULL,
    [Banking] decimal(18,2) NULL,
    [Coupon] decimal(18,2) NULL,
    [PayNote] nvarchar(max) NULL,
    [TransportForm] int NOT NULL,
    [TransportDate] datetime2 NULL,
    [COD] bit NOT NULL,
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
    CONSTRAINT [PK_BillCustomers] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230426041309_table BillCustomer', N'6.0.5');
GO

COMMIT;
GO
