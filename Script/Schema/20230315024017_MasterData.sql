BEGIN TRANSACTION;
GO

CREATE TABLE [ProductCategories] (
    [Id] uniqueidentifier NOT NULL,
    [TenantId] uniqueidentifier NULL,
    [Code] nvarchar(max) NULL,
    [Name] nvarchar(max) NULL,
    [Insuarance] nvarchar(max) NULL,
    [Quantity] int NULL,
    [Status] int NOT NULL,
    [ManagerId] uniqueidentifier NOT NULL,
    [Ratio] float NULL,
    [ExtraProperties] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(40) NULL,
    [CreationTime] datetime2 NOT NULL,
    [CreatorId] uniqueidentifier NULL,
    [LastModificationTime] datetime2 NULL,
    [LastModifierId] uniqueidentifier NULL,
    CONSTRAINT [PK_ProductCategories] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Products] (
    [Id] uniqueidentifier NOT NULL,
    [TenantId] uniqueidentifier NULL,
    [Name] nvarchar(max) NULL,
    [ParentId] uniqueidentifier NOT NULL,
    [ParentCode] nvarchar(max) NULL,
    [ParentName] nvarchar(max) NULL,
    [Type] int NOT NULL,
    [BarCode] nvarchar(max) NULL,
    [Enterprise] nvarchar(max) NULL,
    [OtherName] nvarchar(max) NULL,
    [ImageLink] nvarchar(max) NULL,
    [Unit] int NOT NULL,
    [Weight] float NULL,
    [MonthOfWarranty] int NOT NULL,
    [EntryPrice] decimal(18,2) NULL,
    [SalePrice] decimal(18,2) NULL,
    [VAT] decimal(18,2) NULL,
    [OldPrice] decimal(18,2) NULL,
    [Profit] decimal(18,2) NULL,
    [CostPrice] decimal(18,2) NULL,
    [WholeSalePrice] decimal(18,2) NULL,
    [Status] int NOT NULL,
    [CategoryId] uniqueidentifier NOT NULL,
    [SupplierId] uniqueidentifier NOT NULL,
    [WebsiteLink] nvarchar(max) NULL,
    [Height] float NULL,
    [Width] float NULL,
    [Lenght] float NULL,
    [BranchSalePrice] decimal(18,2) NULL,
    [BranchWhosalePrice] decimal(18,2) NULL,
    [ExtraProperties] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(40) NULL,
    [CreationTime] datetime2 NOT NULL,
    [CreatorId] uniqueidentifier NULL,
    [LastModificationTime] datetime2 NULL,
    [LastModifierId] uniqueidentifier NULL,
    CONSTRAINT [PK_Products] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [SaleOrderLines] (
    [Id] uniqueidentifier NOT NULL,
    [TenantId] uniqueidentifier NULL,
    [OrderId] uniqueidentifier NOT NULL,
    [ProductId] uniqueidentifier NOT NULL,
    [RequestQuantity] int NULL,
    [RequestPrice] decimal(18,2) NULL,
    [SuggestedPrice] decimal(18,2) NULL,
    [TotalYuan] decimal(18,2) NULL,
    [ExtraProperties] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(40) NULL,
    [CreationTime] datetime2 NOT NULL,
    [CreatorId] uniqueidentifier NULL,
    [LastModificationTime] datetime2 NULL,
    [LastModifierId] uniqueidentifier NULL,
    CONSTRAINT [PK_SaleOrderLines] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [SaleOrders] (
    [Id] uniqueidentifier NOT NULL,
    [TenantId] uniqueidentifier NULL,
    [Code] nvarchar(max) NULL,
    [StoreId] uniqueidentifier NOT NULL,
    [SupplierId] uniqueidentifier NOT NULL,
    [InvoiceNumber] nvarchar(max) NULL,
    [Rate] float NULL,
    [Status] int NOT NULL,
    [IsConfirm] bit NOT NULL,
    [ExtraProperties] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(40) NULL,
    [CreationTime] datetime2 NOT NULL,
    [CreatorId] uniqueidentifier NULL,
    [LastModificationTime] datetime2 NULL,
    [LastModifierId] uniqueidentifier NULL,
    CONSTRAINT [PK_SaleOrders] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Stores] (
    [Id] uniqueidentifier NOT NULL,
    [TenantId] uniqueidentifier NULL,
    [Code] nvarchar(max) NULL,
    [Name] nvarchar(max) NULL,
    [ProvinceId] uniqueidentifier NOT NULL,
    [DistricId] uniqueidentifier NOT NULL,
    [WardId] uniqueidentifier NOT NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [ExpriDate] datetime2 NULL,
    [ExtraProperties] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(40) NULL,
    [CreationTime] datetime2 NOT NULL,
    [CreatorId] uniqueidentifier NULL,
    [LastModificationTime] datetime2 NULL,
    [LastModifierId] uniqueidentifier NULL,
    CONSTRAINT [PK_Stores] PRIMARY KEY ([Id])
);
GO

CREATE TABLE [Suppliers] (
    [Id] uniqueidentifier NOT NULL,
    [TenantId] uniqueidentifier NULL,
    [Code] nvarchar(max) NULL,
    [Name] nvarchar(max) NULL,
    [Address] nvarchar(max) NULL,
    [Type] int NOT NULL,
    [Email] nvarchar(max) NULL,
    [PhoneNumber] nvarchar(max) NULL,
    [BankName] nvarchar(max) NULL,
    [BankBrand] nvarchar(max) NULL,
    [BankNumberAccount] nvarchar(max) NULL,
    [BankAccountHolder] nvarchar(max) NULL,
    [Status] int NOT NULL,
    [ExtraProperties] nvarchar(max) NULL,
    [ConcurrencyStamp] nvarchar(40) NULL,
    [CreationTime] datetime2 NOT NULL,
    [CreatorId] uniqueidentifier NULL,
    [LastModificationTime] datetime2 NULL,
    [LastModifierId] uniqueidentifier NULL,
    CONSTRAINT [PK_Suppliers] PRIMARY KEY ([Id])
);
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230315024017_MasterData', N'6.0.5');
GO

COMMIT;
GO

