EXEC sp_rename N'[Products].[Lenght]', N'Length', N'COLUMN';
GO

EXEC sp_rename N'[Products].[BranchWhosalePrice]', N'BranchWholeSalePrice', N'COLUMN';
GO

INSERT INTO [__EFMigrationsHistory] ([MigrationId], [ProductVersion])
VALUES (N'20230321082347_rename_ColumninProduct', N'6.0.5');