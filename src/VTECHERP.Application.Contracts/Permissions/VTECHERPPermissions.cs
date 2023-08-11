using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Volo.Abp.Identity;

namespace VTECHERP.Permissions;

public static class VTECHERPPermissions
{
    public const string Base = "VTECHERP";

    public static List<(string ModuleName, string GroupName, string PermissionName, string PermissionCode)> GetAllPermissions()
    {
        var permissions = new List<(string ModuleName, string GroupName, string PermissionName, string PermissionCode)>();
        var modules = typeof(VTECHERPPermissions).GetNestedTypes().Where(p => p.Name != "Common").ToList();
        foreach (var module in modules)
        {
            var groups = module.GetNestedTypes();
            foreach (var group in groups)
            {
                var fields = group.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.Name != "GroupName")
                    .Select(p =>
                    {
                        var value = (
                            module.Name,
                            group.Name,
                            p.Name,
                            p.GetRawConstantValue().ToString()
                        );
                        return value;
                    })
                    .ToList();
                permissions.AddRange(fields);
            }
        }

        return permissions;
    }

    public static List<string> GetAllModule()
    {
        var modules = typeof(VTECHERPPermissions).GetNestedTypes().Where(p => p.Name != "Common").Select(p => p.Name).ToList();

        return modules;
    }

    public static List<(string ModuleName, string GroupName, string GroupCode)> GetAllGroups()
    {
        var groups = new List<(string ModuleName, string GroupName, string GroupCode)>();
        var modules = typeof(VTECHERPPermissions).GetNestedTypes().Where(p => p.Name != "Common").ToList();
        foreach (var module in modules)
        {
            var moduleGroups = module.GetNestedTypes().Select(p =>
                {
                    var value = (
                    module.Name,
                    p.Name,
                    p.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                        .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.Name == "GroupName")
                        .Select(p => p.GetRawConstantValue().ToString())
                        .FirstOrDefault()
                    );
                    return value;
                }).ToList();
            groups.AddRange(moduleGroups);
        }

        return groups;
    }

    public static List<(string ModuleName, string GroupName, string PermissionName, string PermissionCode)> GetGroupPermission(string moduleName, string groupName)
    {
        var permissions = new List<(string ModuleName, string GroupName, string PermissionName, string PermissionCode)>();
        var modules = typeof(VTECHERPPermissions).GetNestedTypes().Where(p => p.Name == moduleName).ToList();
        foreach (var module in modules)
        {
            var groups = module.GetNestedTypes().Where(p => p.Name == groupName).ToList();
            foreach (var group in groups)
            {
                var fields = group.GetFields(BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy)
                    .Where(fi => fi.IsLiteral && !fi.IsInitOnly && fi.Name != "GroupName")
                    .Select(p =>
                    {
                        var value = (
                            module.Name,
                            group.Name,
                            p.Name,
                            p.GetRawConstantValue().ToString()
                        );
                        return value;
                    })
                    .ToList();
                permissions.AddRange(fields);
            }
        }

        return permissions;
    }

    public class Common
    {
        public const string List = "List";
        public const string Detail = "Detail";
        public const string Create = "Create";
        public const string Update = "Update";
        public const string UpdateNote = "UpdateNote";
        public const string Delete = "Delete";
        public const string Export = "Export";
        public const string Import = "Import";
        public const string Print = "Print";
        public const string Upload = "Upload";
        public const string ViewUpload = "ViewUpload";
        public const string Complete = "Complete";
        public const string Approve = "Approve";
        public const string ChangeStatus = "ChangeStatus";
        public const string RemoveStore = "RemoveStore";
    }

    public class AbpIdentity
    {
        public const string ModuleName = "AbpIdentity";
        public class Users
        {
            public const string GroupName = $"{ModuleName}.Users";
            public const string List = $"{GroupName}.{Common.List}";
            public const string Export = $"{GroupName}.{Common.Export}";
            public const string Import = $"{GroupName}.{Common.Import}";
            public const string ChangeStatus = $"{GroupName}.{Common.ChangeStatus}";
            public const string RemoveStore = $"{GroupName}.{Common.RemoveStore}";
        }
        public class Roles
        {
            public const string GroupName = $"{ModuleName}.Roles";
            public const string List = $"{GroupName}.{Common.List}";
        }
    }

    public class Product
    {
        public const string ModuleName = $"{Base}.Product";
        public class Management
        {
            public const string GroupName = $"{ModuleName}.Management";
            public const string List = $"{GroupName}.{Common.List}";
            public const string Detail = $"{GroupName}.{Common.Detail}";
            public const string Create = $"{GroupName}.{Common.Create}";
            public const string Update = $"{GroupName}.{Common.Update}";
            public const string Delete = $"{GroupName}.{Common.Delete}";
            public const string Export = $"{GroupName}.{Common.Export}";
            // xem giá vốn
            public const string ViewEntryPrice = $"{GroupName}.ViewEntryPrice";
            // quản lý giá spa
            public const string SpaPriceMManagement = $"{GroupName}.SpaPriceMManagement";
            // chuyển mã
            public const string CodeChange = $"{GroupName}.CodeChange";
        }

        public class ProductHistory
        {
            public const string GroupName = $"{ModuleName}.ProductHistory";
            public const string View = $"{GroupName}.{Common.List}";
            public const string Export = $"{GroupName}.{Common.Export}";
        }

        public class ProductStock
        {
            public const string GroupName = $"{ModuleName}.ProductStock";
            public const string View = $"{GroupName}.{Common.List}";
            public const string Export = $"{GroupName}.{Common.Export}";
            // tạo giá vốn
            public const string Create = $"{GroupName}.{Common.Create}";
            // sửa giá vốn
            public const string Update = $"{GroupName}.{Common.Update}";
        }

        public class ProductCategory
        {
            public const string GroupName = $"{ModuleName}.ProductCategory";
            public const string Create = $"{GroupName}.{Common.Create}";
            public const string List = $"{GroupName}.{Common.List}";
            public const string Update = $"{GroupName}.{Common.Update}";
            public const string Delete = $"{GroupName}.{Common.Delete}";
            public const string Export = $"{GroupName}.{Common.Export}";
            public const string Import = $"{GroupName}.{Common.Import}";
            public const string ChangeStatus = $"{GroupName}.ChangeStatus";
            public const string ChangeProductToOtherCategory = $"{GroupName}.ChangeProductToDifferentCategory";
            public const string RemoveProductFromCategory = $"{GroupName}.RemoveProductFromCategory";
        }
    }

    public class Supplier
    {
        public const string ModuleName = $"{Base}.Supplier";
        public class Management
        {
            public const string GroupName = $"{ModuleName}.Management";
            public const string List = $"{GroupName}.{Common.List}";
            public const string Detail = $"{GroupName}.{Common.Detail}";
            public const string Create = $"{GroupName}.{Common.Create}";
            public const string Update = $"{GroupName}.{Common.Update}";
            public const string Delete = $"{GroupName}.{Common.Delete}";
            public const string Import = $"{GroupName}.{Common.Import}";
        }
    }

    public class PriceTable
    {
        public const string ModuleName = $"{Base}.PriceTable";

        public class Management
        {
            public const string GroupName = $"{ModuleName}.Management";
            public const string List = $"{GroupName}.{Common.List}";
            public const string Detail = $"{GroupName}.{Common.Detail}";
            public const string Create = $"{GroupName}.{Common.Create}";
            public const string Update = $"{GroupName}.{Common.Update}";
            public const string Delete = $"{GroupName}.{Common.Delete}";
        }

        public class Product
        {
            public const string GroupName = $"{ModuleName}.Product";
            public const string List = $"{GroupName}.{Common.List}";
            public const string Import = $"{GroupName}.{Common.Import}";
            public const string Update = $"{GroupName}.{Common.Update}";
            public const string Delete = $"{GroupName}.{Common.Delete}";
        }
    }

    public class Customer
    {
        public const string ModuleName = $"{Base}.Customer";
        public class Management
        {
            public const string GroupName = $"{ModuleName}.Management";
            public const string List = $"{GroupName}.{Common.List}";
            public const string SpaCustomerManagement = $"{GroupName}.SpaCustomerManagement";
            //public const string Detail = $"{GroupName}.{Common.Detail}";
            public const string Create = $"{GroupName}.{Common.Create}";
            public const string Update = $"{GroupName}.{Common.Update}";
            public const string Delete = $"{GroupName}.{Common.Delete}";
            public const string Import = $"{GroupName}.{Common.Import}";
        }
        public class TransactionHistory
        {
            public const string GroupName = $"{ModuleName}.TransactionHistory";
            public const string List = $"{GroupName}.{Common.List}";
        }
    }

    public class Warehousing
    {
        public const string ModuleName = $"{Base}.Warehousing";
        public class Management
        {
            public const string GroupName = $"{ModuleName}.Management";

            public const string List = $"{GroupName}.{Common.List}";
            public const string Detail = $"{GroupName}.{Common.Detail}";
            public const string CreateExportBill = $"{GroupName}.CreateExportBill";
            public const string CreateImportBill = $"{GroupName}.CreateImportBill";
            public const string Update = $"{GroupName}.{Common.Update}";
            public const string Delete = $"{GroupName}.{Common.Delete}";
            public const string Print = $"{GroupName}.{Common.Print}";
            public const string Upload = $"{GroupName}.{Common.Upload}";
            public const string UpdateNote = $"{GroupName}.{Common.UpdateNote}";
            public const string Export = $"{GroupName}.{Common.Export}";
        }

        public class History
        {
            public const string GroupName = $"{ModuleName}.History";
            public const string List = $"{GroupName}.{Common.List}";
            public const string Detail = $"{GroupName}.{Common.Detail}";
            public const string Export = $"{GroupName}.{Common.Export}";
        }

        public class Product
        {
            public const string GroupName = $"{ModuleName}.Product";
            public const string List = $"{GroupName}.{Common.List}";
            public const string Update = $"{GroupName}.{Common.Update}";
            public const string Delete = $"{GroupName}.{Common.Delete}";
            public const string Export = $"{GroupName}.{Common.Export}";
        }
    }

    public class SupplierOrder
    {
        public const string ModuleName = $"{Base}.SupplierOrder";

        public class Management
        {
            public const string GroupName = $"{ModuleName}.Management";

            public const string List = $"{GroupName}.{Common.List}";
            public const string Detail = $"{GroupName}.{Common.Detail}";
            public const string Create = $"{GroupName}.{Common.Create}";
            public const string Update = $"{GroupName}.{Common.Update}";
            public const string Delete = $"{GroupName}.{Common.Delete}";
            public const string Approve = $"{GroupName}.{Common.Approve}";
            public const string Complete = $"{GroupName}.{Common.Complete}";
            public const string Upload = $"{GroupName}.{Common.Upload}";
            public const string Export = $"{GroupName}.{Common.Export}";
            public const string UpdateNote = $"{GroupName}.{Common.UpdateNote}";
            public const string ViewUpload = $"{GroupName}.{Common.ViewUpload}";
        }

        public class Product
        {
            public const string GroupName = $"{ModuleName}.Product";

            public const string List = $"{GroupName}.{Common.List}";
            public const string UpdatePrice = $"{GroupName}.UpdatePrice";
            public const string Export = $"{GroupName}.{Common.Export}";
        }

        public class Transport
        {
            public const string GroupName = $"{ModuleName}.Transport";

            public const string List = $"{GroupName}.{Common.List}";
            public const string Create = $"{GroupName}.{Common.Create}";
            public const string Update = $"{GroupName}.{Common.Update}";
        }
    }

    public class WarehouseTransfer
    {
        public const string ModuleName = $"{Base}.WarehouseTransfer";

        public class Draft
        {
            public const string GroupName = $"{ModuleName}.Draft";

            public const string List = $"{GroupName}.{Common.List}";
            public const string Detail = $"{GroupName}.{Common.Detail}";
            public const string Create = $"{GroupName}.{Common.Create}";
            public const string Update = $"{GroupName}.{Common.Update}";
            public const string Delete = $"{GroupName}.{Common.Delete}";
            public const string Approve = $"{GroupName}.{Common.Approve}";
            public const string Print = $"{GroupName}.{Common.Print}";
            public const string Export = $"{GroupName}.{Common.Export}";
        }

        public class Management
        {
            public const string GroupName = $"{ModuleName}.Management";

            public const string List = $"{GroupName}.{Common.List}";
            public const string Detail = $"{GroupName}.{Common.Detail}";
            public const string Create = $"{GroupName}.{Common.Create}";
            public const string Update = $"{GroupName}.{Common.Update}";
            public const string Delete = $"{GroupName}.{Common.Delete}";
            public const string Approve = $"{GroupName}.{Common.Approve}";
            public const string Print = $"{GroupName}.{Common.Print}";
            public const string Export = $"{GroupName}.{Common.Export}";
            public const string ViewUpload = $"{GroupName}.{Common.ViewUpload}";
            public const string Upload = $"{GroupName}.{Common.Upload}";
        }

        public class Moving
        {
            public const string GroupName = $"{ModuleName}.Moving";

            public const string List = $"{GroupName}.{Common.List}";
            public const string Detail = $"{GroupName}.{Common.Detail}";
            public const string Delete = $"{GroupName}.{Common.Delete}";
            public const string Print = $"{GroupName}.{Common.Print}";
            public const string Export = $"{GroupName}.{Common.Export}";
        }

        public class Coming
        {
            public const string GroupName = $"{ModuleName}.Coming";

            public const string List = $"{GroupName}.{Common.List}";
            public const string Detail = $"{GroupName}.{Common.Detail}";
            public const string Print = $"{GroupName}.{Common.Print}";
            public const string Export = $"{GroupName}.{Common.Export}";
            public const string Approve = $"{GroupName}.{Common.Approve}";
        }
    }

    public class CustomerSale
    {
        public const string ModuleName = $"{Base}.CustomerSale";

        public class Sale
        {
            public const string GroupName = $"{ModuleName}.Sale";

            public const string List = $"{GroupName}.{Common.List}";
            public const string Detail = $"{GroupName}.{Common.Detail}";
            public const string Create = $"{GroupName}.{Common.Create}";
            public const string Update = $"{GroupName}.{Common.Update}";
            public const string Delete = $"{GroupName}.{Common.Delete}";
            public const string Export = $"{GroupName}.{Common.Export}";
            public const string AddGift = $"{GroupName}.AddGift";
            public const string AddChildProduct = $"{GroupName}.AddChildProduct";
            public const string Print = $"{GroupName}.{Common.Print}";
            public const string PrintWarehousingBill = $"{GroupName}.PrintWarehousingBill";
            public const string Check = $"{GroupName}.Check";
            public const string DefaultSuccessStatus = $"{GroupName}.DefaultSuccessStatus";
            public const string Delivery = $"{GroupName}.Delivery";
        }


        public class Return
        {
            public const string GroupName = $"{ModuleName}.Return";

            public const string List = $"{GroupName}.{Common.List}";
            public const string Detail = $"{GroupName}.{Common.Detail}";
            public const string Create = $"{GroupName}.{Common.Create}";
            public const string Update = $"{GroupName}.{Common.Update}";
            public const string Delete = $"{GroupName}.{Common.Delete}";
            public const string Export = $"{GroupName}.{Common.Export}";
            // đổi trả ngang
            public const string Exchange = $"{GroupName}.Exchange";
            public const string Check = $"{GroupName}.Check";
            public const string Print = $"{GroupName}.{Common.Print}";
            public const string PrintWarehousingBill = $"{GroupName}.PrintWarehousingBill";
            public const string Approve = $"{GroupName}.{Common.Approve}";
        }
    }

    public class InternalTransport
    {
        public const string ModuleName = $"{Base}.InternalTransport";

        public class Management
        {
            public const string GroupName = $"{ModuleName}.Management";

            public const string List = $"{GroupName}.{Common.List}";
            public const string Detail = $"{GroupName}.{Common.Detail}";
            public const string Create = $"{GroupName}.{Common.Create}";
            public const string Update = $"{GroupName}.{Common.Update}";
            public const string Delete = $"{GroupName}.{Common.Delete}";
            public const string Export = $"{GroupName}.{Common.Export}";
            public const string Print = $"{GroupName}.{Common.Print}";
            public const string Upload = $"{GroupName}.{Common.Upload}";

            public const string UpdateTransporter = $"{GroupName}.UpdateTransporter";
            public const string SendCodeToExternal = $"{GroupName}.SendCodeToExternal";
            public const string UpdateStatus = $"{GroupName}.UpdateStatus";
        }
    }

    public class TransportHistory
    {
        public const string ModuleName = $"{Base}.TransportHistory";

        public class Management
        {
            public const string GroupName = $"{ModuleName}.Management";

            public const string List = $"{GroupName}.{Common.List}";
            public const string Update = $"{GroupName}.{Common.Update}";
        }
    }

    public class ExternalTransport
    {
        public const string ModuleName = $"{Base}.ExternalTransport";

        public class Management
        {
            public const string GroupName = $"{ModuleName}.Management";

            public const string List = $"{GroupName}.{Common.List}";
            public const string Export = $"{GroupName}.{Common.Export}";
            public const string Upload = $"{GroupName}.{Common.Upload}";
            public const string UpdateCODStatus = $"{GroupName}.UpdateCODStatus";
        }
    }

    public class Accounting
    {
        public const string ModuleName = $"{Base}.Accounting";

        public class Account
        {
            public const string GroupName = $"{Base}.Account";

            public const string List = $"{GroupName}.{Common.List}";
            public const string Create = $"{GroupName}.{Common.Create}";
            public const string Update = $"{GroupName}.{Common.Update}";
            public const string Delete = $"{GroupName}.{Common.Delete}";
            public const string Export = $"{GroupName}.{Common.Export}";
        }

        public class Entry
        {
            public const string GroupName = $"{Base}.Entry";

            public const string List = $"{GroupName}.{Common.List}";
            public const string Detail = $"{GroupName}.{Common.Detail}";
            public const string Create = $"{GroupName}.{Common.Create}";
            public const string Update = $"{GroupName}.{Common.Update}";
            public const string Delete = $"{GroupName}.{Common.Delete}";
            public const string Export = $"{GroupName}.{Common.Export}";
            public const string Upload = $"{GroupName}.{Common.Upload}";
        }
    }

    public class Debt
    {
        public const string ModuleName = $"{Base}.Debt";

        public class Supplier
        {
            public const string GroupName = $"{Base}.Supplier";

            public const string List = $"{GroupName}.{Common.List}";
            public const string Detail = $"{GroupName}.{Common.Detail}";
        }

        public class Customer
        {
            public const string GroupName = $"{Base}.Customer";

            public const string List = $"{GroupName}.{Common.List}";
            public const string Detail = $"{GroupName}.{Common.Detail}";
            public const string CreateReminder = $"{GroupName}.CreateReminder";
        }

        public class ReminderHistory
        {
            public const string GroupName = $"{Base}.ReminderHistory";

            public const string List = $"{GroupName}.{Common.List}";
            public const string Create = $"{GroupName}.{Common.Create}";
            public const string Update = $"{GroupName}.{Common.Update}";
            public const string Delete = $"{GroupName}.{Common.Delete}";
            public const string Export = $"{GroupName}.{Common.Export}";
        }
    }

    public class PaymentSummary
    {
        public const string ModuleName = $"{Base}.PaymentSummary";

        public class Management
        {
            public const string GroupName = $"{ModuleName}.Management";

            public const string List = $"{GroupName}.{Common.List}";
            public const string Detail = $"{GroupName}.{Common.Detail}";
            public const string Create = $"{GroupName}.{Common.Create}";
            public const string Update = $"{GroupName}.{Common.Update}";
            public const string Delete = $"{GroupName}.{Common.Delete}";
            public const string Export = $"{GroupName}.{Common.Export}";
            public const string Approve = $"{GroupName}.{Common.Approve}";
        }
    }
}
