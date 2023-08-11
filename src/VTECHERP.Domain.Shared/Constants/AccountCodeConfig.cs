namespace VTECHERP.Constants
{
    /// <summary>
    /// Cấu hình mã bút toán, tài khoản nợ/có để lưu bút toán với hành động tương ứng
    /// </summary>
    public class EntryConfig
    {
        /// <summary>
        /// Đơn Hàng nhà cung cấp
        /// </summary>
        public class OrderSupplier
        {
            public const string Base = "order_supplier";
            public class CreateSupplierCN
            {
                public const string Code = $"{Base}_create_cn";
                public const string Debt = "151";
                public const string Credit = "331.1";
            }

            public class CreateSupplierVN
            {
                public const string Code = $"{Base}_create_vn";
                public const string Debt = "151";
                public const string Credit = "";
            }

            public class ConfirmSupplier
            {
                public const string Code = $"{Base}_confirm_total";
                public const string Debt = "1561";
                public const string Credit = "151";
            }

            public class ConfirmSupplierSurplus
            {
                public const string Code = $"{Base}_confirm_surplus";
                public const string Debt = "711";
                public const string Credit = "632";
            }

            public class CompleteSupplierLack
            {
                public const string Code = $"{Base}_complete_lack";
                public const string Debt = "2294";
                public const string Credit = "151";
            }

            public class DeliverySupplier
            {
                public const string Code = $"{Base}_delivery";
                public const string Debt = "64112";
                public const string Credit = "331.2";
            }
        }
        
        /// <summary>
        /// Phiếu nhập kho
        /// </summary>
        public class WarehousingImport
        {
            public const string Base = "warehousing_import";
            /// <summary>
            /// Loại Đối tượng NCC
            /// </summary>
            public class Supplier
            {
                public const string Base = "supplier";
                /// <summary>
                /// Bút toán tổng tiền sản phẩm
                /// </summary>
                public class Product
                {
                    public const string Code = $"{WarehousingImport.Base}_{Base}_product";
                    public const string Debt = "1561";
                    public const string Credit = "331.2";
                }
                /// <summary>
                /// Bút toán thanh toán tiền mặt
                /// </summary>
                public class CashPay
                {
                    public const string Code = $"{WarehousingImport.Base}_{Base}_cash";
                    public const string Debt = "331.2";
                }

                /// <summary>
                /// Bút toán thanh toán chuyển khoản
                /// </summary>
                public class BankPay
                {
                    public const string Code = $"{WarehousingImport.Base}_{Base}_bank";
                    public const string Debt = "331.2";
                }

                /// <summary>
                /// Bút toán thuế VAT
                /// </summary>
                public class VAT
                {
                    public const string Code = $"{WarehousingImport.Base}_{Base}_vat";
                    public const string Debt = "1331";
                    public const string Credit = "331.2";
                }
            }

            /// <summary>
            /// Loại đối tượng Khách hàng
            /// </summary>
            public class Customer
            {
                public const string Base = "customer";
                /// <summary>
                /// Bút toán tổng tiền sản phẩm
                /// </summary>
                public class Product
                {
                    public const string Code = $"{WarehousingImport.Base}_{Base}_product";
                    public const string Debt = "1561";
                    public const string Credit = "131";
                }

                /// <summary>
                /// Bút toán thanh toán tiền mặt
                /// </summary>
                public class CashPay
                {
                    public const string Code = $"{WarehousingImport.Base}_{Base}_cash";
                    public const string Debt = "131";
                }

                /// <summary>
                /// Bút toán thanh toán chuyển khoản
                /// </summary>
                public class BankPay
                {
                    public const string Code = $"{WarehousingImport.Base}_{Base}_bank";
                    public const string Debt = "131";
                }

                /// <summary>
                /// Bút toán thuế VAT
                /// </summary>
                public class VAT
                {
                    public const string Code = $"{WarehousingImport.Base}_{Base}_vat";
                    public const string Debt = "33311";
                    public const string Credit = "131";
                }
            }

            /// <summary>
            /// Loại đối tượng Khác
            /// </summary>
            public class Other
            {
                public const string Code = $"{WarehousingImport.Base}_other";
                public const string Debt = "1561";
                public const string Credit = "632";
            }
        }

        /// <summary>
        /// Phiếu xuất kho
        /// </summary>
        public class WarehousingExport
        {
            public const string Base = "warehousing_export";
            /// <summary>
            /// Loại đối tượng NCC TQ
            /// </summary>
            public class SupplierCN
            {
                public const string Base = "supplier_cn";
                /// <summary>
                /// Bút toán tổng tiền sản phẩm
                /// </summary>
                public class Product
                {
                    public const string Code = $"{WarehousingExport.Base}_{Base}_product";
                    public const string Debt = "33883";
                    public const string Credit = "1561";
                }
            }

            /// <summary>
            /// Loại Đối tượng NCC VN
            /// </summary>
            public class SupplierVN
            {
                public const string Base = "supplier_vn";
                /// <summary>
                /// Bút toán tổng tiền sản phẩm
                /// </summary>
                public class Product
                {
                    public const string Code = $"{WarehousingExport.Base}_{Base}_product";
                    public const string Debt = "331.2";
                    public const string Credit = "1561";
                }
                /// <summary>
                /// Bút toán thanh toán tiền mặt
                /// </summary>
                public class CashPay
                {
                    public const string Code = $"{WarehousingExport.Base}_{Base}_cash";
                    public const string Credit = "331.2";
                }

                /// <summary>
                /// Bút toán thanh toán chuyển khoản
                /// </summary>
                public class BankPay
                {
                    public const string Code = $"{WarehousingExport.Base}_{Base}_bank";
                    public const string Credit = "331.2";
                }

                /// <summary>
                /// Bút toán thuế VAT
                /// </summary>
                public class VAT
                {
                    public const string Code = $"{WarehousingExport.Base}_{Base}_vat";
                    public const string Debt = "331.2";
                    public const string Credit = "1331";
                }
            }

            /// <summary>
            /// Loại đối tượng Khác
            /// </summary>
            public class Other
            {
                public const string Code = $"{WarehousingExport.Base}_other";
                public const string Debt = "632";
                public const string Credit = "1561";
            }
        }

        public class PaymentReceipt
        {
            public const string Base = "payment_receipt";

            public const string ReciprocalAccountSupplierCN = "331.1";
            public const string ReciprocalAccountSupplierVN = "331.2";
            public const string ReciprocalAccountCustomer = "131";
            /// <summary>
            /// Báo nợ
            /// </summary>
            public class Debit
            {
                public const string Code = $"{Base}_debit";

            }

            /// <summary>
            /// Báo có
            /// </summary>
            public class Credit
            {
                public const string Code = $"{Base}_credit";
            }

            /// <summary>
            /// Phiếu thu
            /// </summary>
            public class Receipt
            {
                public const string Code = $"{Base}_receipt";
            }

            /// <summary>
            /// Phiếu chi
            /// </summary>
            public class PaymentVoucher
            {
                public const string Code = $"{Base}_payment";
            }
        }

        /// <summary>
        /// Hóa đơn bán hàng
        /// </summary>
        public class BillCustomer
        {
            public const string CodeEntry = "entry_bill_customer";
            public const string Debt = "131";
            public const string Credit = "5111";
        }

        public class BillCustomerHasVat
        {
            public const string CodeEntry = "entry_bill_customer_vat";
            public const string Debt = "131";
            public const string Credit = "33311";
        }

        public class ReturnProduct
        {
            public const string Base = "return_product";

            public class Return
            {
                public const string Code = $"{Base}_return";
                public const string Debt = "5213";
                public const string Credit = "131";
            }

            public class Import
            {
                public const string Code = $"{Base}_import";
                public const string Debt = "1561";
                public const string Credit = "632";
            }

            /// <summary>
            /// Báo nợ
            /// </summary>
            public class Debit
            {
                public const string Code = $"{Base}_debit";
                public const string Debt = "131";
            }

            /// <summary>
            /// Phiếu chi
            /// </summary>
            public class PaymentVoucher
            {
                public const string Code = $"{Base}_payment";
                public const string Debt = "131";
            }
        }
        public class ExchangeProduct
        {
            public const string Base = "exchange_product";

            public class Sell
            {
                public const string Code = $"{Base}_sell";
                public const string Debt = "131";
                public const string Credit = "5111";
            }

            public class Export
            {
                public const string Code = $"{Base}_export";
                public const string Debt = "632";
                public const string Credit = "1561";
            }
        }
    }
}
