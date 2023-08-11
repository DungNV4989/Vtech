
BEGIN TRANSACTION;
GO

CREATE SEQUENCE [BillCustomerCode] AS int START WITH 1 INCREMENT BY 1 NO MINVALUE NO MAXVALUE NO CYCLE;
GO

CREATE SEQUENCE [BillCustomerProductBonusCode] AS int START WITH 1 INCREMENT BY 1 NO MINVALUE NO MAXVALUE NO CYCLE;
GO

CREATE SEQUENCE [BillCustomerProductCode] AS int START WITH 1 INCREMENT BY 1 NO MINVALUE NO MAXVALUE NO CYCLE;
GO

ALTER TABLE [BillCustomers] ADD [Code] nvarchar(max) NULL DEFAULT (FORMAT(NEXT VALUE FOR BillCustomerCode,'0000000000'));
GO

ALTER TABLE [BillCustomerProducts] ADD [Code] nvarchar(max) NULL DEFAULT (FORMAT(NEXT VALUE FOR BillCustomerProductCode,'0000000000'));
GO

ALTER TABLE [BillCustomerProductBonus] ADD [Code] nvarchar(max) NULL DEFAULT (FORMAT(NEXT VALUE FOR BillCustomerProductBonusCode,'0000000000'));
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230427024030_add code column tb BillCustomer', N'6.0.5');
GO

COMMIT;
GO
