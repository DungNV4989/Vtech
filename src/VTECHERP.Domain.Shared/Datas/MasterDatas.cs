using System;
using System.Collections.Generic;
using System.Text;
using VTECHERP.DTOs.Base;
using VTECHERP.Enums;
using VTECHERP.Enums.Product;

namespace VTECHERP.Datas
{
    public class MasterDatas
    {
        /// <summary>
        /// Đơn vị tính
        /// </summary>
        public static readonly List<EnumMasterData<ProductUnit>> ProductUnits = new()
        {
            new EnumMasterData<ProductUnit>
            {
                Id = ProductUnit.Once,
                Name = "Cái"
            },
            new EnumMasterData<ProductUnit>
            {
                Id = ProductUnit.Lot,
                Name = "Lô"
            }
        };
        /// <summary>
        /// Loại phiếu
        /// </summary>
        public static readonly List<EnumMasterData<TicketTypes>> TicketTypes = new()
        {
            new()
            {
                Id = Enums.TicketTypes.DebitNote,
                Name = "Báo nợ"
            },
            new()
            {
                Id = Enums.TicketTypes.CreditNote,
                Name = "Báo có"
            },
            new()
            {
                Id = Enums.TicketTypes.Receipt,
                Name = "Phiếu thu"
            },
            new()
            {
                Id = Enums.TicketTypes.PaymentVoucher,
                Name = "Phiếu chi"
            },
            new()
            {
                Id = Enums.TicketTypes.Import,
                Name = "Phiếu nhập"
            },
            new()
            {
                Id = Enums.TicketTypes.Export,
                Name = "Phiếu xuất"
            },
            new()
            {
                Id = Enums.TicketTypes.Sales,
                Name = "Phiếu bán hàng"
            },
            new()
            {
                Id = Enums.TicketTypes.Return,
                Name = "Phiếu trả hàng"
            },
            new()
            {
                Id = Enums.TicketTypes.ClosingEntry,
                Name = "kết chuyển"
            },
            new()
            {
                Id = Enums.TicketTypes.Other,
                Name = "Khác"
            },
            new()
            {
                Id = Enums.TicketTypes.FundTransfer,
                Name = "Chuyển quỹ"
            }
        };

        /// <summary>
        /// Loại hạch toán
        /// </summary>
        public static readonly List<EnumMasterData<AccountingTypes>> AccountingTypes = new()
        {
            new()
            {
                Id = Enums.AccountingTypes.Auto,
                Name = "Tự động"
            },
            new()
            {
                Id = Enums.AccountingTypes.Manual,
                Name = "Thủ công"
            }
        };

        /// <summary>
        /// Loại đối tượng
        /// </summary>
        public static readonly List<EnumMasterData<AudienceTypes>> AudienceTypes = new()
        {
            new()
            {
                Id = Enums.AudienceTypes.Customer,
                Name = "Khách hàng"
            },
            new()
            {
                Id = Enums.AudienceTypes.SupplierVN,
                Name = "Nhà cung cấp VN"
            },
            new()
            {
                Id = Enums.AudienceTypes.SupplierCN,
                Name = "Nhà cung cấp TQ"
            },
            new()
            {
                Id = Enums.AudienceTypes.Employee,
                Name = "Nhân viên"
            },
            new()
            {
                Id = Enums.AudienceTypes.Other,
                Name = "Khác"
            }
        };

        public static readonly List<EnumMasterData<DocumentTypes>> DocumentTypes = new()
        {
            new()
            {
                Id = Enums.DocumentTypes.SupplierOrder,
                Name = "Đơn hàng NCC"
            },
            new()
            {
                Id = Enums.DocumentTypes.InventoryImport,
                Name = "Phiếu nhập kho"
            },
            new()
            {
                Id = Enums.DocumentTypes.InventoryExport,
                Name = "Phiếu xuất kho"
            },
            new()
            {
                Id = Enums.DocumentTypes.ShippingNote,
                Name = "Đơn vận chuyển"
            },
            new()
            {
                Id = Enums.DocumentTypes.Other,
                Name = "Chứng từ ngoài"
            },
            new()
            {
                Id = Enums.DocumentTypes.DebitNote,
                Name = "Báo nợ"
            },
            new()
            {
                Id = Enums.DocumentTypes.CreditNote,
                Name = "Báo có"
            },
            new()
            {
                Id = Enums.DocumentTypes.Receipt,
                Name = "Phiếu thu"
            },
            new()
            {
                Id = Enums.DocumentTypes.PaymentVoucher,
                Name = "Phiếu chi"
            },
            new()
            {
                Id = Enums.DocumentTypes.FundTransfer,
                Name = "Phiếu chuyển quỹ"
            },
            new()
            {
                Id = Enums.DocumentTypes.BillCustomer,
                Name = "Bán hàng"
            },
            new()
            {
                Id = Enums.DocumentTypes.ReturnProduct,
                Name = "Trả hàng"
            }
        };

        public static readonly List<EnumMasterData<CustomerType>> CustomerTypes = new()
        {
            new()
            {
                Id = CustomerType.RetailCustomer,
                Name = "Khách lẻ"
            },
            new()
            {
                Id = CustomerType.SPACustomer,
                Name = "Khách SPA"
            },
            new()
            {
                Id = CustomerType.Agency,
                Name = "Đại lý"
            }
        };

        public static readonly List<EnumMasterData<DebtGroup>> DebtGroups = new()
        {
            new()
            {
                Id = DebtGroup.Normal,
                Name = "Bình thường"
            },
            new()
            {
                Id = DebtGroup.LimitedSale,
                Name = "Giới hạn mua"
            },
            new()
            {
                Id = DebtGroup.NoSale,
                Name = "Không bán"
            }
        };

        public static readonly List<EnumMasterData<Gender>> Genders = new()
        {
            new()
            {
                Id = Gender.Male,
                Name = "Nam"
            },
            new()
            {
                Id = Gender.Female,
                Name = "Nữ"
            },
            new()
            {
                Id = Gender.Other,
                Name = "Khác"
            }
        };

        public static readonly List<EnumMasterData<DocumentDetailType>> DocumentDetailTypes = new()
        {
            new()
            {
                Id = DocumentDetailType.ImportCustomer,
                Name = "Nhập - Khách hàng"
            },
            new()
            {
                Id = DocumentDetailType.ImportSupplier,
                Name = "Nhập - Nhà cung cấp"
            },
            new()
            {
                Id = DocumentDetailType.ImportTransfer,
                Name = "Nhập - Chuyển kho"
            },
            new()
            {
                Id = DocumentDetailType.ImportStockCheck,
                Name = "Nhập - Bù trừ kiểm kho"
            },
            new()
            {
                Id = DocumentDetailType.ImportProduce,
                Name = "Nhập - sản xuất"
            },
            new()
            {
                Id = DocumentDetailType.ImportCodeChange,
                Name = "Nhập - chuyển mã"
            },
            new()
            {
                Id = DocumentDetailType.ImportOther,
                Name = "Nhập - Khác"
            },
            new()
            {
                Id = DocumentDetailType.ExportCustomer,
                Name = "Xuất - Khách hàng"
            },
            new()
            {
                Id = DocumentDetailType.ExportSupplier,
                Name = "Xuất - Nhà cung cấp"
            },
            new()
            {
                Id = DocumentDetailType.ExportTransfer,
                Name = "Xuất - Chuyển kho"
            },
            new()
            {
                Id = DocumentDetailType.ExportStockCheck,
                Name = "Xuất - Bù trừ kiểm kho"
            },
            new()
            {
                Id = DocumentDetailType.ExportProduce,
                Name = "Xuất - Sán xuất"
            },
            new()
            {
                Id = DocumentDetailType.ExportCodeChange,
                Name = "Xuất - chuyển mã"
            },
            new()
            {
                Id = DocumentDetailType.ExportMaintain,
                Name = "Xuất - Bảo hành"
            },
            new()
            {
                Id = DocumentDetailType.ExportCancel,
                Name = "Xuất - Hủy"
            },
            new()
            {
                Id = DocumentDetailType.ExportGift,
                Name = "Xuất - Quà tặng"
            },
            new()
            {
                Id = DocumentDetailType.ExportOther,
                Name = "Xuất - Khác"
            },
            new()
            {
                Id = DocumentDetailType.ExportVAT,
                Name = "Xuất VAT"
            },
            new()
            {
                Id = DocumentDetailType.ImportVAT,
                Name = "Nhập VAT"
            },
            new()
            {
                Id = DocumentDetailType.Order,
                Name = "Phiếu đặt hàng"
            },
            new()
            {
                Id = DocumentDetailType.DeliveryNote,
                Name = "Đơn vận chuyển"
            },
            new()
            {
                Id = DocumentDetailType.DebitNote,
                Name = "Báo nợ"
            },
            new()
            {
                Id = DocumentDetailType.CreditNote,
                Name = "Báo có"
            },
            new()
            {
                Id = DocumentDetailType.Receipt,
                Name = "Phiếu thu"
            },
            new()
            {
                Id = DocumentDetailType.PaymentVoucher,
                Name = "Phiếu chi"
            },
            new()
            {
                Id = DocumentDetailType.FundTransfer,
                Name = "Chuyển quỹ"
            },
            new()
            {
                Id = DocumentDetailType.ReturnProduct,
                Name = "Trả hàng"
            }
        };
    }
    public class DocumentDetailTypeData
    {
        public TicketTypes? TicketType { get; set; }
        public DocumentTypes? DocumentType { get; set; }
        public WarehousingBillType? WarehousingBillType { get; set; }
        public AudienceTypes? AudienceType { get; set; }
        public DocumentDetailType DocumentDetailType { get; set; }
        public bool IsWarehousingBillForm { get; set; } = false;
        public string Name { get; set; }

        public static readonly List<DocumentDetailTypeData> Datas = new()
        {
            new()
            {
                DocumentDetailType = DocumentDetailType.ImportCustomer,
                TicketType = TicketTypes.Import,
                DocumentType = DocumentTypes.InventoryImport,
                WarehousingBillType = Enums.WarehousingBillType.Import,
                AudienceType = AudienceTypes.Customer,
                IsWarehousingBillForm = true,
                Name = "Nhập - Khách hàng"
            },
            new()
            {
                DocumentDetailType = DocumentDetailType.ImportSupplier,
                TicketType = TicketTypes.Import,
                DocumentType = DocumentTypes.InventoryImport,
                WarehousingBillType = Enums.WarehousingBillType.Import,
                AudienceType = AudienceTypes.SupplierCN,
                IsWarehousingBillForm = true,
                Name = "Nhập - Nhà cung cấp"
            },
            new()
            {
                DocumentDetailType = DocumentDetailType.ImportTransfer,
                TicketType = TicketTypes.Import,
                DocumentType = DocumentTypes.InventoryImport,
                WarehousingBillType = Enums.WarehousingBillType.Import,
                Name = "Nhập - Chuyển kho"
            },
            new()
            {
                DocumentDetailType = DocumentDetailType.ImportStockCheck,
                TicketType = TicketTypes.Import,
                DocumentType = DocumentTypes.InventoryImport,
                WarehousingBillType = Enums.WarehousingBillType.Import,
                Name = "Nhập - Bù trừ kiểm kho"
            },
            new()
            {
                DocumentDetailType = DocumentDetailType.ImportProduce,
                TicketType = TicketTypes.Import,
                DocumentType = DocumentTypes.InventoryImport,
                WarehousingBillType = Enums.WarehousingBillType.Import,
                AudienceType = AudienceTypes.Other,
                IsWarehousingBillForm = true,
                Name = "Nhập - sản xuất"
            },
            new()
            {
                DocumentDetailType = DocumentDetailType.ImportCodeChange,
                TicketType = TicketTypes.Import,
                DocumentType = DocumentTypes.InventoryImport,
                WarehousingBillType = Enums.WarehousingBillType.Import,
                AudienceType = AudienceTypes.Other,
                IsWarehousingBillForm = true,
                Name = "Nhập - chuyển mã"
            },
            new()
            {
                DocumentDetailType = DocumentDetailType.ImportOther,
                TicketType = TicketTypes.Import,
                DocumentType = DocumentTypes.InventoryImport,
                WarehousingBillType = Enums.WarehousingBillType.Import,
                AudienceType = AudienceTypes.Other,
                IsWarehousingBillForm = true,
                Name = "Nhập - Khác"
            },
            new()
            {
                DocumentDetailType = DocumentDetailType.ExportCustomer,
                TicketType = TicketTypes.Export,
                DocumentType = DocumentTypes.InventoryExport,
                WarehousingBillType = Enums.WarehousingBillType.Export,
                AudienceType = AudienceTypes.Customer,
                IsWarehousingBillForm = true,
                Name = "Xuất - Khách hàng"
            },
            new()
            {
                DocumentDetailType = DocumentDetailType.ExportSupplier,
                TicketType = TicketTypes.Export,
                DocumentType = DocumentTypes.InventoryExport,
                WarehousingBillType = Enums.WarehousingBillType.Export,
                AudienceType = AudienceTypes.SupplierCN,
                IsWarehousingBillForm = true,
                Name = "Xuất - Nhà cung cấp"
            },
            new()
            {
                DocumentDetailType = DocumentDetailType.ExportTransfer,
                TicketType = TicketTypes.Export,
                DocumentType = DocumentTypes.InventoryExport,
                WarehousingBillType = Enums.WarehousingBillType.Export,
                Name = "Xuất - Chuyển kho"
            },
            new()
            {
                DocumentDetailType = DocumentDetailType.ExportStockCheck,
                TicketType = TicketTypes.Export,
                DocumentType = DocumentTypes.InventoryExport,
                WarehousingBillType = Enums.WarehousingBillType.Export,
                Name = "Xuất - Bù trừ kiểm kho"
            },
            new()
            {
                DocumentDetailType = DocumentDetailType.ExportProduce,
                TicketType = TicketTypes.Export,
                DocumentType = DocumentTypes.InventoryExport,
                WarehousingBillType = Enums.WarehousingBillType.Export,
                AudienceType = AudienceTypes.Other,
                IsWarehousingBillForm = true,
                Name = "Xuất - Sán xuất"
            },
            new()
            {
                DocumentDetailType = DocumentDetailType.ExportCodeChange,
                TicketType = TicketTypes.Export,
                DocumentType = DocumentTypes.InventoryExport,
                WarehousingBillType = Enums.WarehousingBillType.Export,
                AudienceType = AudienceTypes.Other,
                IsWarehousingBillForm = true,
                Name = "Xuất - chuyển mã"
            },
            new()
            {
                DocumentDetailType = DocumentDetailType.ExportMaintain,
                TicketType = TicketTypes.Export,
                DocumentType = DocumentTypes.InventoryExport,
                WarehousingBillType = Enums.WarehousingBillType.Export,
                AudienceType = AudienceTypes.Other,
                IsWarehousingBillForm = true,
                Name = "Xuất - Bảo hành"
            },
            new()
            {
                DocumentDetailType = DocumentDetailType.ExportCancel,
                TicketType = TicketTypes.Export,
                DocumentType = DocumentTypes.InventoryExport,
                WarehousingBillType = Enums.WarehousingBillType.Export,
                AudienceType = AudienceTypes.Other,
                IsWarehousingBillForm = true,
                Name = "Xuất - Hủy"
            },
            new()
            {
                DocumentDetailType = DocumentDetailType.ExportGift,
                TicketType = TicketTypes.Export,
                DocumentType = DocumentTypes.InventoryExport,
                WarehousingBillType = Enums.WarehousingBillType.Export,
                AudienceType = AudienceTypes.Other,
                IsWarehousingBillForm = true,
                Name = "Xuất - Quà tặng"
            },
            new()
            {
                DocumentDetailType = DocumentDetailType.ExportOther,
                TicketType = TicketTypes.Export,
                DocumentType = DocumentTypes.InventoryExport,
                WarehousingBillType = Enums.WarehousingBillType.Export,
                AudienceType = AudienceTypes.Other,
                IsWarehousingBillForm = true,
                Name = "Xuất - Khác"
            },
            new()
            {
                DocumentDetailType = DocumentDetailType.ExportVAT,
                Name = "Xuất VAT"
            },
            new()
            {
                DocumentDetailType = DocumentDetailType.ImportVAT,
                Name = "Nhập VAT"
            },
            new()
            {
                DocumentDetailType = DocumentDetailType.Order,
                Name = "Phiếu đặt hàng"
            },
            new()
            {
                DocumentDetailType = DocumentDetailType.DeliveryNote,
                Name = "Đơn vận chuyển"
            },
            new()
            {
                DocumentDetailType = DocumentDetailType.DebitNote,
                Name = "Báo nợ"
            },
            new()
            {
                DocumentDetailType = DocumentDetailType.CreditNote,
                Name = "Báo có"
            },
            new()
            {
                DocumentDetailType = DocumentDetailType.Receipt,
                Name = "Phiếu thu"
            },
            new()
            {
                DocumentDetailType = DocumentDetailType.PaymentVoucher,
                Name = "Phiếu chi"
            },
            new()
            {
                DocumentDetailType = DocumentDetailType.FundTransfer,
                Name = "Chuyển quỹ"
            },
            new()
            {
                DocumentDetailType = DocumentDetailType.ReturnProduct,
                Name = "Trả hàng"
            }
        };
    }
    public class EntityActionsData
    {
        public EntityActions Action { get; set; }
        public string Name { get; set; }

        public static readonly List<EntityActionsData> Datas = new()
        {
            new()
            {
                Action = EntityActions.Create,
                Name = "Tạo mới"
            },
            new()
            {
                Action = EntityActions.Update,
                Name = "Sửa"
            },
            new()
            {
                Action = EntityActions.Delete,
                Name = "Xóa"
            }
        };
    }
    public class WarehousingBillTypeData
    {
        public WarehousingBillType BillType { get; set; }
        public string Name { get; set; }

        public static readonly List<WarehousingBillTypeData> Datas = new()
        {
            new()
            {
                BillType = WarehousingBillType.Import,
                Name = "Nhập"
            },
            new()
            {
                BillType = WarehousingBillType.Export,
                Name = "Xuất"
            }
        };    
    }
    public class DocumentTypesData
    {
        public DocumentTypes DocumentTypes { get; set; }
        public string Name { get; set; }
        public static readonly List<DocumentTypesData> Datas = new()
        {
            new()
            {
                DocumentTypes = DocumentTypes.SupplierOrder,
                Name = "Đơn đặt hàng"
            },
            new()
            {
                DocumentTypes = DocumentTypes.InventoryImport,
                Name = "Phiếu Nhập kho"
            },
            new()
            {
                DocumentTypes = DocumentTypes.InventoryExport,
                Name = "Phiếu Xuất kho"
            },
            new()
            {
                DocumentTypes = DocumentTypes.ShippingNote,
                Name = "Đơn vận chuyển"
            },
            new()
            {
                DocumentTypes = DocumentTypes.Other,
                Name = "Chứng từ ngoài"
            },
            new()
            {
                DocumentTypes = DocumentTypes.DebitNote,
                Name = "Báo nợ"
            },
            new()
            {
                DocumentTypes = DocumentTypes.CreditNote,
                Name = "Báo có"
            },
            new()
            {
                DocumentTypes = DocumentTypes.Receipt,
                Name = "Phiếu thu"
            },
            new()
            {
                DocumentTypes = DocumentTypes.PaymentVoucher,
                Name = "Phiếu chi"
            },
            new()
            {
                DocumentTypes = DocumentTypes.Entry,
                Name = "Bút toán"
            },
            new()
            {
                DocumentTypes = DocumentTypes.FundTransfer,
                Name = "Chuyển quỹ"
            },
            new()
            {
                DocumentTypes = DocumentTypes.BillCustomer,
                Name = "Hóa đơn bán hàng"
            },
            new()
            {
                DocumentTypes = DocumentTypes.ReturnProduct,
                Name = "Hóa đơn trả hàng"
            },
            new()
            {
                DocumentTypes = DocumentTypes.ChangeCostPriceProduct,
                Name = "Chuyển giá vốn"
            }
        };
    }
}
