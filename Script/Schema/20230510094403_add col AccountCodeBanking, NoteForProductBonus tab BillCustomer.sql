BEGIN TRANSACTION;
GO

ALTER TABLE [BillCustomers] ADD [AccountCodeBanking] nvarchar(max) NULL;
GO

ALTER TABLE [BillCustomers] ADD [NoteForProductBonus] nvarchar(max) NULL;
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230510094403_add col AccountCodeBanking, NoteForProductBonus tab BillCustomer', N'6.0.5');
GO

COMMIT;
GO

