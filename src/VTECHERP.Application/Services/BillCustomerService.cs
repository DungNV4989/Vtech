using DocumentFormat.OpenXml.Vml.Office;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;
using VTECHERP.Constants;
using VTECHERP.DTOs.BillCustomers.Params;
using VTECHERP.DTOs.BillCustomers.Respons;
using VTECHERP.DTOs.WarehousingBills;
using VTECHERP.Entities;
using VTECHERP.Enums;
using VTECHERP.Enums.Bills;
using VTECHERP.Extensions;
using VTECHERP.ServiceInterfaces;
using BillCustomer = VTECHERP.Entities.BillCustomer;
using Customer = VTECHERP.Entities.Customer;
using PaymentReceipt = VTECHERP.Entities.PaymentReceipt;
using Entry = VTECHERP.Entities.Entry;
using Volo.Abp.Identity;
using VTECHERP.DTOs.Base;
using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.AspNetCore.Mvc;
using static VTECHERP.Constants.EntryConfig;
using VTECHERP.Services.Interface;
using VTECHERP.DTOs.BillCustomerVoucher.Dto;
using VTECHERP.DTOs.BillCustomerVoucher.Param;
using DocumentFormat.OpenXml.Wordprocessing;
using VTECHERP.DTOs.TransportInformation;
using DocumentFormat.OpenXml.Office2010.Excel;
using Volo.Abp.Timing;
using VTECHERP.DTOs.Customer;
using Vinpearl.Modelling.Library.Utility.Excel;
using Volo.Abp;
using Microsoft.AspNetCore.Http;
using VTECHERP.Domain.Shared.Helper.Excel.Model;
using ClosedXML.Excel;
using VTECHERP.DTOs.Suppliers.Respons;
using Volo.Abp.Data;
using Volo.Abp.MultiTenancy;

namespace VTECHERP.Services
{
    public class BillCustomerService : IBillCustomerService
    {
        private readonly IRepository<BillCustomer> _billCustomerRepository;
        private readonly IRepository<Products> _productRepository;
        private readonly IRepository<StoreProduct> _storeProductRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<BillCustomerProduct> _billCustomerProductRepository;
        private readonly IRepository<BillCustomerProductBonus> _billCustomerProductBonusRepository;
        private readonly IRepository<PriceTable> _priceTableRepository;
        private readonly IRepository<PriceTableProduct> _priceTableProductRepository;
        private readonly IRepository<PaymentReceipt> _paymentReceiptRepository;
        private readonly IRepository<Account> _accountRepository;
        private readonly IRepository<WarehousingBill> _wareHousingBillRepository;
        private readonly IWarehousingBillService _wareHousingBillService;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IEntryService _entryService;
        private readonly ITransportInformationService _transportInformationService;
        private readonly IRepository<Entry> _entryRepository;
        private readonly IRepository<EntryAccount> _entryAccountRepository;
        private readonly IRepository<BillCustomerLog> _billCustomerLogRepository;
        private readonly IDebtCustomerService _debtCustomerService;
        private readonly IRepository<CustomerReturn> _customerReturnRepository;
        private readonly IRepository<Stores> _storeRepository;
        private readonly IRepository<CustomerReturnProduct> _customerReturnProductRepository;
        private readonly IdentityUserManager _userManager;
        private readonly IRepository<TransportInformationLog> _transportInformationLogRepository;
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<TransportInformation> _transportInformationRepository;
        private readonly IPaymentReceiptService _paymentReceiptService;
        private readonly IRepository<Attachment> _attachmentRepository;
        private readonly ICustomerReturnService _customerReturnService;
        private readonly ICommonBillCustomerService _commonBillCustomerService;
        private readonly IBillCustomerVoucherService _billCustomerVoucherService;
        private readonly IRepository<TransporstBills> _transportBillRepository;
        private readonly IRepository<Voucher> _voucherRepository;
        private readonly IIdentityUserRepository _userRepository;
        private readonly IAttachmentService _attachmentService;
        private readonly IRepository<Debt> _debtRepository;
        private readonly IRepository<WarehousingBillProduct> _warehouseBillProductRepository;
        private readonly IDataFilter _dataFilter;

        readonly List<CustomerBillPayStatus> statusHitBillCustomer = new List<CustomerBillPayStatus>
                {
                    CustomerBillPayStatus.CustomerOrder,
                    CustomerBillPayStatus.WaitCall,
                    CustomerBillPayStatus.Confirm,
                    CustomerBillPayStatus.Taked
                };

        private readonly IClock _clock;
        public BillCustomerService(
            IRepository<BillCustomer> billCustomerRepository
            , IRepository<Products> productRepository
            , IRepository<StoreProduct> storeProductRepository
            , IRepository<Customer> customerRepository
            , IRepository<BillCustomerProduct> billCustomerProductRepository
            , IRepository<BillCustomerProductBonus> billCustomerProductBonusRepository
            , IRepository<PriceTable> priceTableRepository
            , IRepository<PriceTableProduct> priceTableProductRepository
            , IRepository<PaymentReceipt> paymentReceiptRepository
            , IRepository<Account> accountRepository
            , IWarehousingBillService wareHousingBillService
            , IUnitOfWorkManager unitOfWorkManager
            , IEntryService entryService
            , ITransportInformationService transportInformationService
            , IRepository<WarehousingBill> wareHousingBillRepository
            , IRepository<Entry> entryRepository
            , IRepository<BillCustomerLog> billCustomerLogRepository
            , IRepository<CustomerReturn> customerReturnRepository
            , IRepository<CustomerReturnProduct> customerReturnProductRepository
            , IDebtCustomerService debtCustomerService
            , IRepository<EntryAccount> entryAccountRepository
            , IdentityUserManager userManager
            , IRepository<Stores> storeRepository
            , IRepository<TransportInformationLog> transportInformationLogRepository
            , IRepository<TransportInformation> transportInformationRepository
            , IRepository<Employee> employeeRepository
            , IPaymentReceiptService paymentReceiptService
            , IRepository<Attachment> attachmentRepository
            , ICustomerReturnService customerReturnService
            , ICommonBillCustomerService commonBillCustomerService
            , IBillCustomerVoucherService billCustomerVoucherService
            , IRepository<Entities.TransporstBills> transportBillRepository
            , IClock clock
            , IRepository<Voucher> voucherRepository
            , IIdentityUserRepository userRepository
            , IAttachmentService attachmentService
            , IRepository<Debt> debtRepository
            , IDataFilter dataFilter
            , IRepository<WarehousingBillProduct> warehouseBillProductRepository
            )
        {
            _billCustomerRepository = billCustomerRepository;
            _productRepository = productRepository;
            _storeProductRepository = storeProductRepository;
            _customerRepository = customerRepository;
            _billCustomerProductRepository = billCustomerProductRepository;
            _billCustomerProductBonusRepository = billCustomerProductBonusRepository;
            _priceTableRepository = priceTableRepository;
            _priceTableProductRepository = priceTableProductRepository;
            _paymentReceiptRepository = paymentReceiptRepository;
            _accountRepository = accountRepository;
            _wareHousingBillService = wareHousingBillService;
            _unitOfWorkManager = unitOfWorkManager;
            _entryService = entryService;
            _transportInformationService = transportInformationService;
            _wareHousingBillRepository = wareHousingBillRepository;
            _entryRepository = entryRepository;
            _billCustomerLogRepository = billCustomerLogRepository;
            _debtCustomerService = debtCustomerService;
            _customerReturnRepository = customerReturnRepository;
            _customerReturnProductRepository = customerReturnProductRepository;
            _entryAccountRepository = entryAccountRepository;
            _userManager = userManager;
            _storeRepository = storeRepository;
            _transportInformationLogRepository = transportInformationLogRepository;
            _employeeRepository = employeeRepository;
            _transportInformationRepository = transportInformationRepository;
            _paymentReceiptService = paymentReceiptService;
            _attachmentRepository = attachmentRepository;
            _customerReturnService = customerReturnService;
            _commonBillCustomerService = commonBillCustomerService;
            _billCustomerVoucherService = billCustomerVoucherService;
            _transportBillRepository = transportBillRepository;
            _clock = clock;
            _voucherRepository = voucherRepository;
            _userRepository = userRepository;
            _attachmentService = attachmentService;
            _debtRepository = debtRepository;
            _dataFilter = dataFilter;
            _warehouseBillProductRepository = warehouseBillProductRepository;
        }

        public List<CustomerBillPayStatus> statusCreateEntry = new List<CustomerBillPayStatus>
                {
                    CustomerBillPayStatus.Checked,
                    CustomerBillPayStatus.Success
                };

        public async Task<(BillCustomerDto, string, bool)> CreateCustomerBill(BillCustomerCreateParam param)
        {
            var uow = _unitOfWorkManager.Current;

            var objDB = new BillCustomer()
            {
                Banking = param.Banking,
                Cash = param.Cash,
                COD = param.COD,
                Coupon = param.Coupon,
                CustomerBillPayStatus = param.CustomerBillPayStatus,
                DiscountUnit = param.DiscountUnit,
                DiscountValue = param.DiscountValue,
                CustomerId = param.CustomerId,
                EmployeeNote = param.EmployeeNote,
                StoreId = param.StoreId,
                PayNote = param.PayNote,
                TablePriceId = param.TablePriceId,
                TransportDate = param.TransportDate,
                TransportForm = param.TransportForm,
                VatUnit = param.VatUnit,
                VatValue = param.VatValue,
                AccountCode = param.AccountCode,
                AccountCodeBanking = param.AccountCodeBanking,
                NoteForProductBonus = param.NoteForProductBonus,
            };

            try
            {
                #region phần thêm khách hàng cho hóa đơn
                var paramCreateCustomer = new CustomerNewParam
                {
                    CustomerId = param.CustomerId,
                    ProvinceId = param.ProvinceId,
                    Address = param.Address,
                    CustomerName = param.CustomerName,
                    CustomerPhone = param.CustomerPhone,
                    CustomerType = param.CustomerType,
                    EmployeeCare = param.EmployeeCare,
                    EmployeeSell = param.EmployeeSell,
                    Note = param.Note
                };

                var resultCreateCustomer = await AddCustomerForBillCustomer(paramCreateCustomer);
                if (!resultCreateCustomer.Item3)
                {
                    return (null, resultCreateCustomer.Item2, resultCreateCustomer.Item3);
                }
                else
                {
                    objDB.CustomerId = (resultCreateCustomer.Item1 as CustomerRespon).Id;
                    objDB.CustomerType = (resultCreateCustomer.Item1 as CustomerRespon).CustomerType;
                }
                #endregion

                // Lưu thông tin hóa đơn
                await _billCustomerRepository.InsertAsync(objDB);

                var billCustomerProducts = new List<BillCustomerProduct>();
                var productBonus = new List<BillCustomerProductBonus>();
                #region Add sản phẩm vào hóa đơn
                var resultAddProduct = await _commonBillCustomerService.AddProductForBillCustomer(objDB.Id, param.BillCustomerProducts);
                billCustomerProducts = resultAddProduct.Item1;
                productBonus = resultAddProduct.Item2;

                // Kiểm tra số lượng yêu cầu với tồn của sản phẩm
                var statusCheckQuantity = new List<CustomerBillPayStatus> { CustomerBillPayStatus.Success
                    ,CustomerBillPayStatus.Taked
                    ,CustomerBillPayStatus.Checked
                    ,CustomerBillPayStatus.Confirm
                };
                if (statusCheckQuantity.Contains(objDB.CustomerBillPayStatus.GetValueOrDefault()))
                {
                    var productChecks = billCustomerProducts.Select(x => new BillCustomerProductCheckValid
                    {
                        ProductId = x.ProductId.GetValueOrDefault(),
                        Quantity = x.Quantity
                    })
                    .ToList();

                    var productBonusCheck = productBonus.Select(x => new BillCustomerProductCheckValid
                    {
                        ProductId = x.ProductId.GetValueOrDefault(),
                        Quantity = x.Quantity
                    })
                    .ToList();

                    productChecks.AddRange(productBonusCheck);
                    var responCheckQuantity = await CheckInventoryProduct(productChecks, objDB.StoreId);
                    if (!responCheckQuantity.isValid)
                        return (null, responCheckQuantity.message, responCheckQuantity.isValid);
                }

                await uow.SaveChangesAsync();
                #endregion

                #region tính toán tổng tiền, tiền triết khấu và voucher của hóa đơn
                // tổng tiền hóa đơn chưa trừ triết khấu, voucher
                decimal totalPrice = _commonBillCustomerService.CacularAmountsTotalBillCustomer(billCustomerProducts);
                // tổng tiền hóa đơn khi trừ đi tiền voucher
                decimal totalAfterVoucher = totalPrice;
                BillVoucher voucherDto = null;
                Promotion promotion = null;

                // Validate voucher if exist
                if (!string.IsNullOrEmpty(param.VoucherCode))
                {
                    var isValidVoucher = _billCustomerVoucherService.GetVoucher(new GetVoucherParam
                    {
                        BillAmount = totalPrice,
                        Code = param.VoucherCode,
                        StoreId = objDB.StoreId,
                        ProductIds = billCustomerProducts.Select(x => x.ProductId.GetValueOrDefault()).ToList()
                    });

                    if (!isValidVoucher.Item3)
                        return (null, isValidVoucher.Item2, isValidVoucher.Item3);

                    voucherDto = isValidVoucher.Item1;
                    promotion = _billCustomerVoucherService.GetPromotionById(voucherDto.PromotionId);
                    var productIds = billCustomerProducts.Select(x => x.ProductId.GetValueOrDefault()).ToList();

                    objDB.VoucherCode = voucherDto.Code;
                    objDB.VoucherId = voucherDto.Id;
                    objDB.VoucherDiscountUnit = voucherDto.DiscountUnit;
                    objDB.VoucherDiscountValue = voucherDto.DiscountValue.GetValueOrDefault();
                    objDB.VoucherApplyStoreIds = promotion.ApplyStoreIds;
                    objDB.VoucherBillMinValue = promotion.BillMinValue;
                    objDB.VoucherBillMaxValue = promotion.BillMaxValue;
                    objDB.VoucherApplyFor = promotion.ApplyFor;
                    objDB.VoucherApplyProductCategoryIds = promotion.ApplyProductCategoryIds;
                    objDB.VoucherApplyProductIds = promotion.ApplyProductIds;
                    objDB.VoucherNotApplyWithDiscount = promotion.NotApplyWithDiscount;
                    objDB.VoucherMaxDiscountValue = promotion.MaxDiscountValue;

                    List<Guid> productInVoucher = new List<Guid>();
                    if (voucherDto.ApplyFor == ApplyFor.Product)
                        productInVoucher = _billCustomerVoucherService.ProductsApplyVoucher(promotion, productIds);

                    if (voucherDto.ApplyFor == ApplyFor.ProductCategory)
                        productInVoucher = _billCustomerVoucherService.ProductsApplyVoucherByCategory(promotion, productIds);

                    var productInBill = billCustomerProducts.Where(x => productInVoucher.Contains(x.ProductId ?? Guid.Empty)).ToList();
                    var totalPriceProductInBill = productInBill.Sum(x => x.Price.GetValueOrDefault() * x.Quantity);

                    decimal precentVoucherOfEachProduct = 0;
                    decimal voucherCash = 0;

                    if (voucherDto.DiscountUnit == DiscountUnit.Cash)
                    {
                        precentVoucherOfEachProduct = (voucherDto.DiscountValue.GetValueOrDefault() / totalPriceProductInBill) * 100;

                        objDB.BillVoucherDiscountValue = voucherDto.DiscountValue.GetValueOrDefault();
                        totalAfterVoucher -= voucherDto.DiscountValue.GetValueOrDefault();
                    }

                    if (voucherDto.DiscountUnit == DiscountUnit.Precent)
                    {
                        precentVoucherOfEachProduct = voucherDto.DiscountValue.GetValueOrDefault();
                        voucherCash = (voucherDto.DiscountValue.GetValueOrDefault() / 100) * totalPriceProductInBill;

                        if (promotion.MaxDiscountValue.HasValue && voucherCash > promotion.MaxDiscountValue.Value)
                            voucherCash = promotion.MaxDiscountValue.Value;

                        objDB.BillVoucherDiscountValue = voucherCash;
                        totalAfterVoucher -= voucherCash;
                    }

                    foreach (var item in billCustomerProducts)
                    {
                        var voucherCashDiscount = (precentVoucherOfEachProduct / 100) * item.Price.GetValueOrDefault();
                        item.DiscountValueCash = voucherCashDiscount;
                    }

                    var voucher = await _voucherRepository.FindAsync(x => x.Code == objDB.VoucherCode);
                    voucher.Status = VoucherStatus.Used;
                }

                // Tổng tiền sau chiết khấu
                decimal totalPriceDiscount = totalAfterVoucher;
                if (voucherDto == null || !voucherDto.NotApplyWithDiscount)
                {
                    // Tính số tiền được chiết khấu
                    decimal amountDiscount = CacularAmountsDiscountBillCustomer(billCustomerProducts, totalPrice, param.DiscountUnit, param.DiscountValue);
                    totalPriceDiscount = totalAfterVoucher - amountDiscount;
                    objDB.DiscountCash = amountDiscount;
                }

                decimal discountPrecentEachProduct = _commonBillCustomerService.DiscountPrecentEachProduct(totalPrice, param.DiscountUnit, param.DiscountValue);

                foreach (var item in billCustomerProducts)
                {
                    var total = item.Price * item.Quantity;
                  
                    if (param.DiscountValue.HasValue && param.DiscountValue > 0)
                    {
                        var cashDiscount = (discountPrecentEachProduct / 100) * total;
                        item.DiscountValueCash = cashDiscount.GetValueOrDefault();
                        item.DiscountValue = cashDiscount;
                        item.DiscountUnit = MoneyModificationType.VND;
                    }
                    else if(item.DiscountValue.HasValue && item.DiscountValue > 0 && item.DiscountUnit == MoneyModificationType.VND)
                    {
                        item.DiscountValueCash = item.DiscountValue.GetValueOrDefault();
                    }
                    else if (item.DiscountValue.HasValue && item.DiscountValue > 0 && item.DiscountUnit == MoneyModificationType.Percent)
                    {
                        var cashDiscount = (item.DiscountValue / 100) * total;
                        item.DiscountValueCash = cashDiscount.GetValueOrDefault();
                    }
                }

                await _billCustomerProductRepository.UpdateManyAsync(billCustomerProducts);

                decimal totalPriceVat = 0;
                // Tính tổng tiền sau vat
                decimal? cashVat = 0;
                if (param.VatUnit.HasValue && param.VatValue.HasValue && param.VatValue > 0)
                {
                    if (param.VatUnit == MoneyModificationType.Percent)
                    {
                        cashVat = ((param.VatValue / 100) * totalPriceDiscount);
                        totalPriceVat = Convert.ToDecimal(totalPriceDiscount + cashVat);
                    }

                    if (param.VatUnit == MoneyModificationType.VND)
                    {
                        totalPriceVat = Convert.ToDecimal(totalPriceDiscount + param.VatValue);
                        cashVat = param.VatValue;
                    }
                }
                else totalPriceVat = totalPriceDiscount;

                objDB.AmountAfterDiscount = totalPriceDiscount;
                objDB.AmountCustomerPay = totalPriceVat;
                #endregion

                #region phần tạo bút toán và các phiếu
                var statusEntry = new List<CustomerBillPayStatus>
                {
                    CustomerBillPayStatus.Checked,
                    CustomerBillPayStatus.Success
                };

                var statusWareHouse = new List<CustomerBillPayStatus>
                {
                    //CustomerBillPayStatus.Confirm,
                    CustomerBillPayStatus.Checked,
                    CustomerBillPayStatus.Success
                };

                if (statusEntry.Contains(param.CustomerBillPayStatus.GetValueOrDefault()))
                {
                    // Tạo bút toán
                    await _entryService.AutoCreateEntryForCreatBillCustomer(objDB.Id, totalPriceDiscount);

                    // Tạo bút toán nếu vat > 0
                    if (param.VatUnit.HasValue && param.VatValue.HasValue && param.VatValue > 0)
                    {
                        await _entryService.AutoCreateEntryForCreatBillCustomerHasVAT(objDB.Id, cashVat.Value);
                    }
                }

                if (statusWareHouse.Contains(param.CustomerBillPayStatus.GetValueOrDefault()))
                {
                    // Tạo phiếu xuất kho - khách hàng
                    var listPro = MapToWarehousingBillProductsByBillCustomerProduct(billCustomerProducts);
                    await _commonBillCustomerService.CreateWarehousing(listPro, objDB, $"Kiểu xuất - khách hàng - hóa đơn {objDB.Code}", DocumentDetailType.ExportCustomer);

                    // Tạo phiếu xuất kho - quà tặng
                    if (productBonus.Any())
                    {
                        var listProBonus = MapToWarehousingBillProductsByBillCustomerProductBonus(productBonus);
                        await _commonBillCustomerService.CreateWarehousing(listProBonus, objDB, $"Kiểu xuất - quà tặng hóa đơn {objDB.Code}", DocumentDetailType.ExportGift);
                    }
                }

                // Tạo phiếu thu
                if (param.Cash.HasValue && param.Cash > 0)
                {
                    var paymentReceiptCash = new PaymentReceipt()
                    {
                        AccountingType = Enums.AccountingTypes.Auto,
                        AudienceType = Enums.AudienceTypes.Customer,
                        AudienceId = objDB.CustomerId,
                        ReciprocalAccountCode = EntryConfig.PaymentReceipt.ReciprocalAccountCustomer,
                        SourceId = objDB.Id,
                        Source = Enums.ActionSources.CreateBillCustomer,
                        StoreId = objDB.StoreId.GetValueOrDefault(),
                        TicketType = Enums.TicketTypes.Receipt,
                        TransactionDate = DateTime.Now,
                        DocumentType = Enums.DocumentTypes.BillCustomer,
                        DocumentCode = objDB.Code,
                        DocumentDetailType = Enums.DocumentDetailType.Receipt,
                        AmountVND = objDB.Cash,
                        AccountCode = objDB.AccountCode
                    };

                    await _paymentReceiptRepository.InsertAsync(paymentReceiptCash);
                    await uow.SaveChangesAsync();
                    await _entryService.AutoCreateEntryOnCreatePaymentReceipt(paymentReceiptCash.Id);
                }

                // Tạo báo có
                if (param.Banking.HasValue && param.Banking > 0)
                {
                    var paymentReceiptBanking = new PaymentReceipt()
                    {
                        AccountingType = Enums.AccountingTypes.Auto,
                        AudienceType = Enums.AudienceTypes.Customer,
                        AudienceId = objDB.CustomerId,
                        ReciprocalAccountCode = EntryConfig.PaymentReceipt.ReciprocalAccountCustomer,
                        SourceId = objDB.Id,
                        Source = Enums.ActionSources.CreateBillCustomer,
                        StoreId = objDB.StoreId.GetValueOrDefault(),
                        TicketType = Enums.TicketTypes.CreditNote,
                        TransactionDate = DateTime.Now,
                        DocumentType = Enums.DocumentTypes.BillCustomer,
                        DocumentCode = objDB.Code,
                        DocumentDetailType = Enums.DocumentDetailType.CreditNote,
                        AmountVND = objDB.Banking,
                        AccountCode = objDB.AccountCodeBanking
                    };

                    await _paymentReceiptRepository.InsertAsync(paymentReceiptBanking);
                    await uow.SaveChangesAsync();
                    await _entryService.AutoCreateEntryOnCreatePaymentReceipt(paymentReceiptBanking.Id);
                }
                #endregion

                // Tạo đơn vận chuyển
                await HandleTransportInformationForBill(objDB);

            }
            catch (Exception ex)
            {
                await uow.RollbackAsync();
                return (null, ex.Message, false);
            }

            var result = new BillCustomerDto
            {
                Id = objDB.Id,
                Banking = objDB.Banking,
                Cash = objDB.Cash,
                COD = objDB.COD,
                Coupon = objDB.Coupon,
                CustomerBillPayStatus = objDB.CustomerBillPayStatus,
                DiscountUnit = objDB.DiscountUnit,
                DiscountValue = objDB.DiscountValue,
                CustomerId = objDB.CustomerId,
                EmployeeNote = objDB.EmployeeNote,
                StoreId = objDB.StoreId,
                PayNote = objDB.PayNote,
                TablePriceId = objDB.TablePriceId,
                TransportDate = objDB.TransportDate,
                TransportForm = objDB.TransportForm,
                VatUnit = objDB.VatUnit,
                VatValue = objDB.VatValue,
            };

            return (result, "Tạo thành công", true);

        }

        public async Task<(BillCustomerDetail, string, bool)> GetDetail(Guid BillCustomerId)
        {
            var billCustomer = await _billCustomerRepository.FirstOrDefaultAsync(x => x.Id == BillCustomerId);
            if (billCustomer == null)
                return new(null, $"Không tìm thấy hóa đơn có id là {BillCustomerId}", false);

            var result = new BillCustomerDetail
            {
                BillCustomerId = billCustomer.Id,
                AccountCode = billCustomer.AccountCode,
                CustomerId = billCustomer.CustomerId,
                Cash = billCustomer.Cash,
                Banking = billCustomer.Banking,
                COD = billCustomer.COD,
                Coupon = billCustomer.Coupon,
                DiscountUnit = billCustomer.DiscountUnit,
                DiscountValue = billCustomer.DiscountValue,
                CustomerBillPayStatus = billCustomer.CustomerBillPayStatus,
                PayNote = billCustomer.PayNote,
                StoreId = billCustomer.StoreId,
                TablePriceId = billCustomer.TablePriceId,
                TransportDate = billCustomer.TransportDate,
                TransportForm = billCustomer.TransportForm,
                VatUnit = billCustomer.VatUnit,
                VatValue = billCustomer.VatValue,
                EmployeeNote = billCustomer.EmployeeNote,
                AccountCodeBanking = billCustomer.AccountCodeBanking,
                VoucherCode = billCustomer.VoucherCode,
                VoucherBillMaxValue = billCustomer.VoucherBillMaxValue,
                VoucherBillMinValue = billCustomer.VoucherBillMinValue,
                VoucherDiscountUnit = billCustomer.VoucherDiscountUnit,
                VoucherDiscountValue = billCustomer.VoucherDiscountValue,
                VoucherMaxDiscountValue = billCustomer.VoucherMaxDiscountValue,
                VoucherApplyFor = billCustomer.VoucherApplyFor,
                BillVoucherDiscountValue = billCustomer.BillVoucherDiscountValue,
                VoucherApplyProductCategoryIds = billCustomer.VoucherApplyProductCategoryIds,
                VoucherApplyProductIds = billCustomer.VoucherApplyProductIds,
                VoucherApplyStoreIds = billCustomer.VoucherApplyStoreIds,
                VoucherNotApplyWithDiscount = billCustomer.VoucherNotApplyWithDiscount
            };

            var customer = await _customerRepository.FirstOrDefaultAsync(x => x.Id == result.CustomerId);
            if (customer != null)
            {
                result.CustomerName = customer.Name;
                result.CustomerPhone = customer.PhoneNumber;
                result.CustomerType = customer.CustomerType;
                result.ProvinceId = customer.ProvinceId;
                result.Address = customer.Address;
                result.Note = customer.Note;
                result.EmployeeCare = customer.SupportEmployeeId;
                result.EmployeeSell = customer.HandlerEmployeeId;
                result.DebtLimit = customer.DebtLimit;
            }

            var billCustomerProductQueryable = await _billCustomerProductRepository.GetQueryableAsync();
            var productQueryable = await _productRepository.GetQueryableAsync();
            var storeProductQueryable = (await _storeProductRepository.GetQueryableAsync()).Where(x => x.StoreId == result.StoreId);

            var billProduct = (from billPro in billCustomerProductQueryable

                               join pro in productQueryable
                               on billPro.ProductId equals pro.Id

                               join storePro in storeProductQueryable
                               on billPro.ProductId equals storePro.ProductId
                               into storeProGr
                               from storePro in storeProGr.DefaultIfEmpty()

                               where billPro.BillCustomerId == result.BillCustomerId
                               select new BillCustomerProductItem
                               {
                                   Id = billPro.Id,
                                   ProductId = billPro.ProductId,
                                   ProductName = pro.Name,
                                   ParentId = billPro.ParentId,
                                   Price = billPro.Price,
                                   Quantity = billPro.Quantity,
                                   DiscountUnit = billPro.DiscountUnit,
                                   DiscountValue = billPro.DiscountValue,
                                   Inventory = storePro == null ? 0 : storePro.StockQuantity,
                                   CostPrice = billPro.CostPrice,
                               })
                               .GroupBy(x => x.Id)
                               .Select(x => x.FirstOrDefault())
                               .ToList();

            //var billProduct = await _billCustomerProductRepository.GetListAsync(x => x.BillCustomerId == result.BillCustomerId);

            result.BillCustomerProducts = billProduct.Where(x => x.ParentId == null).Select(x => new VTECHERP.DTOs.BillCustomers.Params.BillCustomerProductDto
            {
                Id = x.Id,
                ProductId = x.ProductId,
                DiscountUnit = x.DiscountUnit,
                DiscountValue = x.DiscountValue,
                Price = x.Price,
                Quantity = x.Quantity,
                ProductChildren = billProduct.Where(y => y.ParentId == x.Id).ToList(),
                ProductName = x.ProductName,
                Inventory = x.Inventory,
                CostPrice = x.CostPrice,
            })
            .ToList();

            var productBonusQueryable = await _billCustomerProductBonusRepository.GetQueryableAsync();

            var billProductIds = result.BillCustomerProducts.Select(x => x.Id).ToList();
            var productBonus = (from bonusPro in productBonusQueryable

                                join pro in productQueryable
                                on bonusPro.ProductId equals pro.Id
                                into proGr
                                from pro in proGr.DefaultIfEmpty()

                                join proStore in storeProductQueryable
                                on pro.Id equals proStore.ProductId
                                into proStoreGr
                                from proStore in proStoreGr.DefaultIfEmpty()

                                where billProductIds.Contains(bonusPro.BillCustomerProductId.Value)
                                && proStore.StoreId == result.StoreId
                                select new BillCustomerProductBonusDto
                                {
                                    IsDebt = bonusPro.IsDebt,
                                    ProductId = bonusPro.ProductId,
                                    Inventory = proStore.StockQuantity,
                                    Quantity = bonusPro.Quantity,
                                    BillCustomerProductId = bonusPro.BillCustomerProductId,
                                    ProductName = pro.Name,
                                    Price = customer.CustomerType == CustomerType.RetailCustomer ? pro.SalePrice ?? 0 : (customer.CustomerType == CustomerType.SPACustomer ? pro.SPAPrice ?? 0 : pro.WholeSalePrice ?? 0),
                                    CostPrice = bonusPro.CostPrice ?? 0,
                                }).ToList();


            //var productBonus = await _billCustomerProductBonusRepository
            //    .GetListAsync(x => billProductIds.Contains(x.BillCustomerProductId.Value));
            var attachments = (await _attachmentService.ListAttachmentByObjectIdAsync(result.BillCustomerProducts.Select(x => x.ProductId.Value).ToList()));
            foreach (var item in result.BillCustomerProducts)
            {
                item.ProductBonus = productBonus.Where(x => x.BillCustomerProductId == item.Id).ToList();
                item.Attachments = attachments.Where(x => x.ObjectId == item.ProductId).OrderBy(x => x.CreationTime).ToList() ?? new List<DTOs.Attachment.DetailAttachmentDto>();
            }

            return (result, "", true);
        }

        public async Task<(BillCustomerDto, string, bool)> UpdateBillCustomer(Guid BillCustomerId, BillCustomerCreateParam param)
        {
            var uow = _unitOfWorkManager.Current;
            try
            {
                var billCustomer = await _billCustomerRepository.FirstOrDefaultAsync(x => x.Id == BillCustomerId);
                if (billCustomer == null)
                    return (null, $"Không tìm thấy hóa đơn có id là {BillCustomerId}", false);

                if (!statusHitBillCustomer.Contains(billCustomer.CustomerBillPayStatus.Value))
                    return (null, $"Trạng thái hóa đơn không được phép sửa", false);

                var billCustomerOld = billCustomer.Clone();

                #region update customer
                var paramCreateCustomer = new CustomerNewParam
                {
                    CustomerId = param.CustomerId,
                    ProvinceId = param.ProvinceId,
                    Address = param.Address,
                    CustomerName = param.CustomerName,
                    CustomerPhone = param.CustomerPhone,
                    CustomerType = param.CustomerType,
                    EmployeeCare = param.EmployeeCare,
                    EmployeeSell = param.EmployeeSell,
                    Note = param.Note
                };

                var resultCreateCustomer = await AddCustomerForBillCustomer(paramCreateCustomer);
                if (!resultCreateCustomer.Item3)
                {
                    return (null, resultCreateCustomer.Item2, resultCreateCustomer.Item3);
                }
                else billCustomer.CustomerId = (resultCreateCustomer.Item1 as CustomerRespon).Id;
                #endregion

                #region Update info bill
                billCustomer.Banking = param.Banking;
                billCustomer.Cash = param.Cash;
                billCustomer.COD = param.COD;
                billCustomer.Coupon = param.Coupon;
                billCustomer.CustomerBillPayStatus = param.CustomerBillPayStatus;
                billCustomer.DiscountUnit = param.DiscountUnit;
                billCustomer.DiscountValue = param.DiscountValue;
                billCustomer.EmployeeCare = param.EmployeeCare;
                billCustomer.EmployeeNote = param.EmployeeNote;
                billCustomer.EmployeeSell = param.EmployeeSell;
                billCustomer.StoreId = param.StoreId;
                billCustomer.PayNote = param.PayNote;
                billCustomer.TablePriceId = param.TablePriceId;
                billCustomer.TransportDate = param.TransportDate;
                billCustomer.TransportForm = param.TransportForm;
                billCustomer.VatUnit = param.VatUnit;
                billCustomer.VatValue = param.VatValue;
                billCustomer.AccountCode = param.AccountCode;
                billCustomer.AccountCodeBanking = param.AccountCodeBanking;
                billCustomer.NoteForProductBonus = param.NoteForProductBonus;
                #endregion

                #region handle billCustomerProduct
                // Xóa các sản phẩm cũ của hóa đơn
                var billProducts = await _billCustomerProductRepository.GetListAsync(x => x.BillCustomerId == billCustomer.Id);
                var billProductId = billProducts.Select(x => x.ProductId).ToList();
                var billProductOrigin = billProducts.Select(x => new BillCustomerProductItem
                {
                    ProductId = x.ProductId,
                    Id = x.Id,
                    Quantity = x.Quantity,
                    ParentId = x.ParentId,
                    Price = x.Price,
                    DiscountUnit = x.DiscountUnit,
                    DiscountValue = x.DiscountValue,
                    CostPrice = x.CostPrice,
                }).ToList();

                var logsFromValueJson = new BillCustomerLogJson
                {
                    AccountCode = billCustomerOld.AccountCode,
                    AccountCodeBanking = billCustomerOld.AccountCodeBanking,
                    AmountAfterDiscount = billCustomerOld.AmountAfterDiscount,
                    AmountCustomerPay = billCustomerOld.AmountCustomerPay,
                    Banking = billCustomerOld.Banking,
                    BillCustomerStatus = billCustomerOld.CustomerBillPayStatus,
                    Cash = billCustomerOld.Cash,
                    COD = billCustomerOld.COD,
                    Coupon = billCustomerOld.Coupon,
                    CustomerId = billCustomerOld.CustomerId,
                    EmployeeCare = billCustomerOld.EmployeeCare,
                    EmployeeSuport = billCustomerOld.EmployeeSell,
                    NoteForProductBonus = billCustomerOld.NoteForProductBonus,
                    ReasonCancel = billCustomerOld.ReasonCancel,
                    TransportForm = billCustomerOld.TransportForm,
                    TransportDate = billCustomerOld.TransportDate,
                    Products = billProductOrigin,
                    VatUnit = billCustomerOld.VatUnit,
                    VatValue = billCustomerOld.VatValue,
                    DiscountUnit = billCustomerOld.DiscountUnit,
                    DiscountValue = billCustomerOld.DiscountValue
                };

                var billProductBonus = await _billCustomerProductBonusRepository.GetListAsync(x => billProductId.Contains(x.Id));
                await _billCustomerProductBonusRepository.DeleteManyAsync(billProductBonus);
                await _billCustomerProductRepository.DeleteManyAsync(billProducts);

                // Add sản phẩm cho hóa đơn
                var billCustomerProducts = new List<BillCustomerProduct>();
                var productBonus = new List<BillCustomerProductBonus>();
                var resultAddProduct = await _commonBillCustomerService.AddProductForBillCustomer(billCustomer.Id, param.BillCustomerProducts);
                billCustomerProducts = resultAddProduct.Item1;
                var billProductNew = billCustomerProducts.Select(x => new BillCustomerProductItem
                {
                    ProductId = x.ProductId,
                    Id = x.Id,
                    Quantity = x.Quantity,
                    ParentId = x.ParentId,
                    Price = x.Price,
                    DiscountUnit = x.DiscountUnit,
                    DiscountValue = x.DiscountValue,
                    CostPrice = x.CostPrice,
                }).ToList();

                productBonus = resultAddProduct.Item2;

                // Kiểm tra số lượng yêu cầu với tồn của sản phẩm
                var statusCheckQuantity = new List<CustomerBillPayStatus> { CustomerBillPayStatus.Success
                    ,CustomerBillPayStatus.Taked
                    ,CustomerBillPayStatus.Checked
                    ,CustomerBillPayStatus.Confirm
                };
                if (statusCheckQuantity.Contains(billCustomer.CustomerBillPayStatus.GetValueOrDefault()))
                {
                    var productChecks = billCustomerProducts.Select(x => new BillCustomerProductCheckValid
                    {
                        ProductId = x.ProductId.GetValueOrDefault(),
                        Quantity = x.Quantity
                    })
                .ToList();

                    var productBonusCheck = productBonus.Select(x => new BillCustomerProductCheckValid
                    {
                        ProductId = x.ProductId.GetValueOrDefault(),
                        Quantity = x.Quantity
                    })
                    .ToList();

                    productChecks.AddRange(productBonusCheck);
                    var responCheckQuantity = await CheckInventoryProduct(productChecks, billCustomer.StoreId);
                    if (!responCheckQuantity.isValid)
                        return (null, responCheckQuantity.message, responCheckQuantity.isValid);
                }

                await uow.SaveChangesAsync();
                #endregion

                decimal totalPrice = _commonBillCustomerService.CacularAmountsTotalBillCustomer(billCustomerProducts);
                decimal totalAfterVoucher = totalPrice;
                BillVoucher voucherDto = null;
                Promotion promotion = null;
                // Validate voucher if exist
                if (!string.IsNullOrEmpty(param.VoucherCode) && billCustomer.VoucherCode != param.VoucherCode.Trim())
                {
                    var isValidVoucher = _billCustomerVoucherService.GetVoucher(new GetVoucherParam
                    {
                        BillAmount = totalPrice,
                        Code = param.VoucherCode,
                        StoreId = billCustomer.StoreId,
                        ProductIds = billCustomerProducts.Select(x => x.ProductId.GetValueOrDefault()).ToList()
                    });

                    if (!isValidVoucher.Item3)
                        return (null, isValidVoucher.Item2, isValidVoucher.Item3);

                    voucherDto = isValidVoucher.Item1;
                    promotion = _billCustomerVoucherService.GetPromotionById(voucherDto.PromotionId);
                    var productIds = billCustomerProducts.Select(x => x.ProductId.GetValueOrDefault()).ToList();
                    List<Guid> productInVoucher = new List<Guid>();

                    billCustomer.VoucherCode = voucherDto.Code;
                    billCustomer.VoucherId = voucherDto.Id;
                    billCustomer.VoucherDiscountUnit = voucherDto.DiscountUnit;
                    billCustomer.VoucherDiscountValue = voucherDto.DiscountValue.GetValueOrDefault();
                    billCustomer.VoucherApplyStoreIds = promotion.ApplyStoreIds;
                    billCustomer.VoucherBillMinValue = promotion.BillMinValue;
                    billCustomer.VoucherBillMaxValue = promotion.BillMaxValue;
                    billCustomer.VoucherApplyFor = promotion.ApplyFor;
                    billCustomer.VoucherApplyProductCategoryIds = promotion.ApplyProductCategoryIds;
                    billCustomer.VoucherApplyProductIds = promotion.ApplyProductIds;
                    billCustomer.VoucherNotApplyWithDiscount = promotion.NotApplyWithDiscount;
                    billCustomer.VoucherMaxDiscountValue = promotion.MaxDiscountValue;

                    if (voucherDto.DiscountUnit == DiscountUnit.Cash)
                    {
                        billCustomer.BillVoucherDiscountValue = voucherDto.DiscountValue.GetValueOrDefault();
                        totalAfterVoucher -= voucherDto.DiscountValue.GetValueOrDefault();
                    }

                    if (voucherDto.DiscountUnit == DiscountUnit.Precent)
                    {
                        if (voucherDto.ApplyFor == ApplyFor.Product)
                            productInVoucher = _billCustomerVoucherService.ProductsApplyVoucher(promotion, productIds);

                        if (voucherDto.ApplyFor == ApplyFor.ProductCategory)
                            productInVoucher = _billCustomerVoucherService.ProductsApplyVoucherByCategory(promotion, productIds);

                        var productInBill = billCustomerProducts.Where(x => productInVoucher.Contains(x.ProductId ?? Guid.Empty)).ToList();
                        var totalPriceProductInBill = productInBill.Sum(x => x.Price * x.Quantity);
                        var voucherCash = (voucherDto.DiscountValue / 100) * totalPriceProductInBill;

                        if (promotion.MaxDiscountValue.HasValue && voucherCash > promotion.MaxDiscountValue.Value)
                            voucherCash = promotion.MaxDiscountValue.Value;

                        billCustomer.BillVoucherDiscountValue = voucherCash.GetValueOrDefault();
                        totalAfterVoucher -= voucherCash.GetValueOrDefault();
                    }

                    var voucher = _billCustomerVoucherService.GetVoucherByCode(billCustomer.VoucherCode);
                    voucher.Status = VoucherStatus.Used;
                }
                else if (!string.IsNullOrEmpty(param.VoucherCode))
                {
                    var productIds = billCustomerProducts.Select(x => x.ProductId.GetValueOrDefault()).ToList();
                    List<Guid> productInVoucher = new List<Guid>();

                    if (billCustomer.VoucherDiscountUnit == DiscountUnit.Cash)
                    {
                        billCustomer.BillVoucherDiscountValue = billCustomer.VoucherDiscountValue;
                        totalAfterVoucher -= billCustomer.VoucherDiscountValue;
                    }

                    if (voucherDto.DiscountUnit == DiscountUnit.Precent)
                    {
                        if (voucherDto.ApplyFor == ApplyFor.Product)
                            productInVoucher = _billCustomerVoucherService.ProductsApplyVoucher(billCustomer.VoucherApplyProductIds, productIds);

                        if (voucherDto.ApplyFor == ApplyFor.ProductCategory)
                            productInVoucher = _billCustomerVoucherService.ProductsApplyVoucherByCategory(billCustomer.VoucherApplyProductCategoryIds, productIds);

                        var productInBill = billCustomerProducts.Where(x => productInVoucher.Contains(x.ProductId ?? Guid.Empty)).ToList();
                        var totalPriceProductInBill = productInBill.Sum(x => x.Price * x.Quantity);
                        var voucherCash = (voucherDto.DiscountValue / 100) * totalPriceProductInBill;

                        if (billCustomer.VoucherMaxDiscountValue.HasValue && voucherCash > billCustomer.VoucherMaxDiscountValue.Value)
                            voucherCash = billCustomer.VoucherMaxDiscountValue.Value;

                        billCustomer.BillVoucherDiscountValue = voucherCash.GetValueOrDefault();
                        totalAfterVoucher -= voucherCash.GetValueOrDefault();
                    }
                }

                decimal totalPriceDiscount = totalAfterVoucher;
                if (voucherDto == null || !voucherDto.NotApplyWithDiscount)
                {
                    // Tính số tiền được triết khấu
                    decimal amountDiscount = CacularAmountsDiscountBillCustomer(billCustomerProducts, totalPrice, param.DiscountUnit, param.DiscountValue);
                    totalPriceDiscount = totalAfterVoucher - amountDiscount;
                    billCustomer.DiscountCash = amountDiscount;
                }

                decimal discountPrecentEachProduct = _commonBillCustomerService.DiscountPrecentEachProduct(totalPrice, param.DiscountUnit, param.DiscountValue);

                foreach (var item in billCustomerProducts)
                {
                    var total = item.Price * item.Quantity;

                    if (param.DiscountValue.HasValue && param.DiscountValue > 0)
                    {
                        var cashDiscount = (discountPrecentEachProduct / 100) * total;
                        item.DiscountValueCash = cashDiscount.GetValueOrDefault();
                        item.DiscountValue = cashDiscount;
                        item.DiscountUnit = MoneyModificationType.VND;
                    }
                    else if (item.DiscountValue.HasValue && item.DiscountValue > 0 && item.DiscountUnit == MoneyModificationType.VND)
                    {
                        item.DiscountValueCash = item.DiscountValue.GetValueOrDefault();
                    }
                    else if (item.DiscountValue.HasValue && item.DiscountValue > 0 && item.DiscountUnit == MoneyModificationType.Percent)
                    {
                        var cashDiscount = (item.DiscountValue / 100) * total;
                        item.DiscountValueCash = cashDiscount.GetValueOrDefault();
                    }
                }

                await _billCustomerProductRepository.UpdateManyAsync(billCustomerProducts);

                decimal totalPriceVat = 0;
                // Tính tổng tiền sau vat
                decimal? cashVat = 0;
                if (param.VatUnit.HasValue && param.VatValue.HasValue && param.VatValue > 0)
                {
                    if (param.VatUnit == MoneyModificationType.Percent)
                    {
                        cashVat = ((param.VatValue / 100) * totalPriceDiscount);
                        totalPriceVat = Convert.ToDecimal(totalPriceDiscount + cashVat);
                    }

                    if (param.VatUnit == MoneyModificationType.VND)
                    {
                        totalPriceVat = Convert.ToDecimal(totalPriceDiscount + param.VatValue);
                        cashVat = param.VatValue;
                    }
                }
                else totalPriceVat = totalPriceDiscount;

                billCustomer.AmountAfterDiscount = totalPriceDiscount;
                billCustomer.AmountCustomerPay = totalPriceVat;
                await uow.SaveChangesAsync();

                var logsToValueJson = new BillCustomerLogJson
                {
                    AccountCode = billCustomer.AccountCode,
                    AccountCodeBanking = billCustomer.AccountCodeBanking,
                    AmountAfterDiscount = billCustomer.AmountAfterDiscount,
                    AmountCustomerPay = billCustomer.AmountCustomerPay,
                    Banking = billCustomer.Banking,
                    BillCustomerStatus = billCustomer.CustomerBillPayStatus,
                    Cash = billCustomer.Cash,
                    COD = billCustomer.COD,
                    Coupon = billCustomer.Coupon,
                    CustomerId = billCustomer.CustomerId,
                    EmployeeCare = billCustomer.EmployeeCare,
                    EmployeeSuport = billCustomer.EmployeeSell,
                    NoteForProductBonus = billCustomer.NoteForProductBonus,
                    ReasonCancel = billCustomer.ReasonCancel,
                    TransportForm = billCustomer.TransportForm,
                    TransportDate = billCustomer.TransportDate,
                    Products = billProductNew,
                    VatUnit = billCustomer.VatUnit,
                    VatValue = billCustomer.VatValue,
                    DiscountUnit = billCustomer.DiscountUnit,
                    DiscountValue = billCustomer.DiscountValue
                };

                #region sử lý phần bút toán và các loại phiếu
                var statusNotEntry = new List<CustomerBillPayStatus>
                { CustomerBillPayStatus.CustomerOrder , CustomerBillPayStatus.WaitCall};

                var statusNotUpdateEntry = new List<CustomerBillPayStatus>
                { CustomerBillPayStatus.Delivery , CustomerBillPayStatus.Success};

                if (statusNotEntry.Contains(param.CustomerBillPayStatus.GetValueOrDefault()))
                {
                    //await _entryService.AutoDeleteEntryForBillCustomer(billCustomer.Id);
                    //await DeleteWarehouseByBillCustomer(billCustomer.Id);
                }
                else
                {
                    // Cập nhật lại phiếu thu
                    var paymentReceiptCash = await _paymentReceiptRepository.FindAsync(x => x.SourceId == billCustomer.Id && x.DocumentDetailType == DocumentDetailType.Receipt);
                    if (paymentReceiptCash != null)
                    {
                        paymentReceiptCash.AmountVND = param.Cash;
                        paymentReceiptCash.AccountCode = param.AccountCode;
                        paymentReceiptCash.StoreId = param.StoreId.GetValueOrDefault();
                        paymentReceiptCash.AudienceId = param.CustomerId.GetValueOrDefault();
                        await uow.SaveChangesAsync();

                        await _entryService.AutoUpdateEntryOnUpdatePaymentReceipt(paymentReceiptCash.Id);
                    }
                    else if (param.Cash > 0 && statusCreateEntry.Contains(param.CustomerBillPayStatus.GetValueOrDefault()))
                    {
                        var paymentReceiptCashNew = new PaymentReceipt()
                        {
                            AccountingType = Enums.AccountingTypes.Auto,
                            AudienceType = Enums.AudienceTypes.Customer,
                            AudienceId = billCustomer.CustomerId,
                            ReciprocalAccountCode = EntryConfig.PaymentReceipt.ReciprocalAccountCustomer,
                            SourceId = billCustomer.Id,
                            Source = Enums.ActionSources.CreateBillCustomer,
                            StoreId = param.StoreId.GetValueOrDefault(),
                            TicketType = Enums.TicketTypes.Receipt,
                            TransactionDate = DateTime.Now,
                            DocumentType = Enums.DocumentTypes.Receipt,
                            DocumentCode = billCustomer.Code,
                            DocumentDetailType = Enums.DocumentDetailType.Receipt,
                            AmountVND = billCustomer.Cash,
                            AccountCode = billCustomer.AccountCode
                        };

                        await _paymentReceiptRepository.InsertAsync(paymentReceiptCashNew);
                        await uow.SaveChangesAsync();
                        await _entryService.AutoCreateEntryOnCreatePaymentReceipt(paymentReceiptCashNew.Id);
                    }

                    // Cập nhật lại báo có
                    var paymentReceiptBanking = await _paymentReceiptRepository.FindAsync(x => x.SourceId == billCustomer.Id && x.DocumentDetailType == DocumentDetailType.CreditNote);
                    if (paymentReceiptBanking != null)
                    {
                        paymentReceiptBanking.AmountVND = param.Banking;
                        paymentReceiptBanking.AccountCode = param.AccountCodeBanking;
                        paymentReceiptBanking.StoreId = param.StoreId.GetValueOrDefault();
                        paymentReceiptBanking.AudienceId = param.CustomerId.GetValueOrDefault();
                        await uow.SaveChangesAsync();

                        await _entryService.AutoUpdateEntryOnUpdatePaymentReceipt(paymentReceiptBanking.Id);
                    }
                    else if (param.Banking > 0 && statusCreateEntry.Contains(param.CustomerBillPayStatus.GetValueOrDefault()))
                    {
                        var paymentReceiptBankingNew = new PaymentReceipt()
                        {
                            AccountingType = Enums.AccountingTypes.Auto,
                            AudienceType = Enums.AudienceTypes.Customer,
                            AudienceId = billCustomer.CustomerId,
                            ReciprocalAccountCode = EntryConfig.PaymentReceipt.ReciprocalAccountCustomer,
                            SourceId = billCustomer.Id,
                            Source = Enums.ActionSources.CreateBillCustomer,
                            StoreId = param.StoreId.GetValueOrDefault(),
                            TicketType = Enums.TicketTypes.Receipt,
                            TransactionDate = DateTime.Now,
                            DocumentType = Enums.DocumentTypes.CreditNote,
                            DocumentCode = billCustomer.Code,
                            DocumentDetailType = Enums.DocumentDetailType.CreditNote,
                            AmountVND = billCustomer.Banking,
                            AccountCode = billCustomer.AccountCodeBanking
                        };

                        await _paymentReceiptRepository.InsertAsync(paymentReceiptBankingNew);
                        await uow.SaveChangesAsync();
                        await _entryService.AutoCreateEntryOnCreatePaymentReceipt(paymentReceiptBankingNew.Id);
                    }

                    // Cập nhật lại phiếu xuất sản phẩm
                    var wareHouseCustomer = await _wareHousingBillRepository.FindAsync(x => x.SourceId == billCustomer.Id && x.DocumentDetailType == DocumentDetailType.ExportCustomer);
                    if (wareHouseCustomer != null)
                    {
                        var listPro = MapToWarehousingBillProductsByBillCustomerProduct(billCustomerProducts);
                        await UpdateWarehousing(listPro, billCustomer, wareHouseCustomer.Id, $"Kiểu xuất - khách hàng - hóa đơn {billCustomer.Code}");
                    }
                    else if (statusCreateEntry.Contains(param.CustomerBillPayStatus.GetValueOrDefault()))
                    {
                        var listPro = MapToWarehousingBillProductsByBillCustomerProduct(billCustomerProducts);
                        await _commonBillCustomerService.CreateWarehousing(listPro, billCustomer, $"Kiểu xuất - khách hàng - hóa đơn {billCustomer.Code}", DocumentDetailType.ExportCustomer);
                    }

                    // Cập nhật lại phiếu xuất quà tặng
                    var wareHouseGift = await _wareHousingBillRepository.FindAsync(x => x.SourceId == billCustomer.Id && x.DocumentDetailType == DocumentDetailType.ExportGift);
                    if (wareHouseGift != null)
                    {
                        if (productBonus.Any())
                        {
                            var listProBonus = MapToWarehousingBillProductsByBillCustomerProductBonus(productBonus);
                            await UpdateWarehousing(listProBonus, billCustomer, wareHouseCustomer.Id, $"Kiểu xuất - quà tặng - hóa đơn {billCustomer.Code}");
                        }

                        if (!productBonus.Any())
                            await _wareHousingBillService.DeleteBill(wareHouseGift.Id);
                    }
                    else if (productBonus.Any() && statusCreateEntry.Contains(param.CustomerBillPayStatus.GetValueOrDefault()))
                    {
                        var listProBonus = MapToWarehousingBillProductsByBillCustomerProductBonus(productBonus);
                        await _commonBillCustomerService.CreateWarehousing(listProBonus, billCustomer, $"Kiểu xuất - quà tặng - hóa đơn {billCustomer.Code}", DocumentDetailType.ExportGift);
                    }

                    // Cập nhật lại bút toán của hóa đơn
                    var isExistEntry = await _entryRepository.AnyAsync(x => x.DocumentId == BillCustomerId
                                                                            && x.ConfigCode == EntryConfig.BillCustomer.CodeEntry);
                    if (isExistEntry)
                        await _entryService.AutoUpdateEntryForBillCustomer(billCustomer.Id);
                    else await _entryService.AutoCreateEntryForCreatBillCustomer(billCustomer.Id, totalPriceDiscount);

                    // Cập nhật lại bút toán của vat
                    var isExistEntryVat = await _entryRepository.AnyAsync(x => x.DocumentId == BillCustomerId
                                                                            && x.ConfigCode == EntryConfig.BillCustomerHasVat.CodeEntry);
                    if (isExistEntryVat)
                    {
                        await _entryService.AutoUpdateEntryVatForBillCustomer(billCustomer.Id, cashVat.GetValueOrDefault());
                    }
                    else if (param.VatValue.HasValue && param.VatValue > 0 && statusCreateEntry.Contains(param.CustomerBillPayStatus.GetValueOrDefault()))
                    {
                        await _entryService.AutoCreateEntryForCreatBillCustomerHasVAT(billCustomer.Id, cashVat.Value);
                    }
                }

                // Tạo đơn vận chuyển
                await HandleTransportInformationForBill(billCustomer);
                #endregion

                await CreateLog(logsFromValueJson, logsToValueJson, billCustomer.Id, EntityActions.Update);

                await uow.SaveChangesAsync();

                return (null, "Cập nhật thành công", true);
            }
            catch (Exception ex)
            {
                await uow.RollbackAsync();
                return (null, ex.Message, false);
            }
        }

        private async Task UpdateWarehousing(List<WarehousingBillProductRequest> listPro, BillCustomer billCustomer, Guid WarehouseId, string note)
        {
            await _wareHousingBillService.UpdateBill(new DTOs.WarehousingBills.UpdateWarehousingBillRequest
            {
                BillType = WarehousingBillType.Export,
                AudienceType = AudienceTypes.Customer,
                AudienceId = billCustomer.CustomerId,
                StoreId = billCustomer.StoreId.GetValueOrDefault(),
                IsFromBillCustomer = true,
                DocumentDetailType = DocumentDetailType.ExportCustomer,
                Note = note,
                SourceId = billCustomer.Id,
                Products = listPro,
                Id = WarehouseId
            });
        }

        private List<WarehousingBillProductRequest> MapToWarehousingBillProductsByBillCustomerProduct(List<BillCustomerProduct> billCustomerProducts)
        {
            var listPro = new List<WarehousingBillProductRequest>();
            listPro = billCustomerProducts.Select(x => new WarehousingBillProductRequest
            {
                ProductId = x.ProductId.GetValueOrDefault(),
                Price = x.CostPrice.GetValueOrDefault(),
                Quantity = x.Quantity,
                DiscountType = MoneyModificationType.VND,
                DiscountAmount = 0
            })
            .ToList();

            return listPro;
        }

        private List<WarehousingBillProductRequest> MapToWarehousingBillProductsByBillCustomerProductBonus(List<BillCustomerProductBonus> productBonus)
        {
            var listProBonus = new List<WarehousingBillProductRequest>();
            listProBonus = productBonus.Select(x => new WarehousingBillProductRequest
            {
                ProductId = x.ProductId.GetValueOrDefault(),
                Price = x.CostPrice.GetValueOrDefault(),
                Quantity = x.Quantity,
                DiscountType = MoneyModificationType.VND,
                DiscountAmount = 0
            })
            .ToList();

            return listProBonus;
        }

        private async Task DeleteWarehouseByBillCustomer(Guid BillCustomerId)
        {
            var wareHouseCustomer = await _wareHousingBillRepository.GetListAsync(x => x.SourceId == BillCustomerId);
            if (wareHouseCustomer.Any())
            {
                foreach (var item in wareHouseCustomer)
                {
                    await _wareHousingBillService.DeleteBill(item.Id);
                }
            }
        }
        // End function con tạo phiếu xuất nhập kho

        private async Task CreateLog(BillCustomerLogJson oldValue, BillCustomerLogJson newValue, Guid BillCustomerId, EntityActions action)
        {
            var log = new Entities.BillCustomerLog()
            {
                Action = action,
                BillCustomerId = BillCustomerId,
                ToValue = JsonConvert.SerializeObject(newValue),
                FromValue = JsonConvert.SerializeObject(oldValue),
            };
            await _billCustomerLogRepository.InsertAsync(log);
        }

        public async Task<(CustomerRespon, string, bool)> AddCustomerForBillCustomer(CustomerNewParam param)
        {
            // Validate data khách hàng từ param
            if (string.IsNullOrEmpty(param.CustomerName) || string.IsNullOrEmpty(param.CustomerPhone))
                return (null, "Dữ liệu khách hàng thiếu", false);

            var dto = new CustomerRespon();
            string message;

            var customer = new Customer();
            customer.Name = param.CustomerName;
            customer.PhoneNumber = param.CustomerPhone;
            customer.ProvinceId = param.ProvinceId.GetValueOrDefault();
            customer.Address = param.Address;
            customer.CustomerType = param.CustomerType.GetValueOrDefault();
            customer.HandlerEmployeeId = param.EmployeeSell;
            customer.SupportEmployeeId = param.EmployeeSell;
            customer.Note = param.Note;

            // Kiểm tra xem phải tạo mới khách hàng không
            var existCustomer = await _customerRepository.FindAsync(p => p.Id == param.CustomerId);
            if (existCustomer == null)
            {
                // Tạo mới khách hàng
                if (await _customerRepository.AnyAsync(x => x.PhoneNumber == param.CustomerPhone))
                    return (null, "Trùng số điện thoại", false);

                if (customer.CustomerType == Enums.CustomerType.RetailCustomer)
                {
                    customer.DebtLimit = 0;
                }

                await _customerRepository.InsertAsync(customer);
                dto.Id = customer.Id;
                message = "Tạo mới khách hàng thành công";
            }
            else
            {
                if (await _customerRepository.AnyAsync(x => x.PhoneNumber == param.CustomerPhone && x.Id != param.CustomerId))
                    return (null, "Trùng số điện thoại", false);

                if (existCustomer.DebtGroup == DebtGroup.NoSale)
                    return (null, "Khách hàng thuộc loại công nợ không bán hàng", false);

                existCustomer.Name = customer.Name;
                existCustomer.PhoneNumber = customer.PhoneNumber;
                existCustomer.ProvinceId = customer.ProvinceId;
                existCustomer.Address = customer.Address;
                existCustomer.CustomerType = customer.CustomerType;
                existCustomer.Note = customer.Note;
                existCustomer.HandlerEmployeeId = customer.HandlerEmployeeId;
                existCustomer.SupportEmployeeId = customer.SupportEmployeeId;

                dto.Id = existCustomer.Id;
                message = "Cập nhật khách hàng thành công";
            }

            dto.Address = customer.Address;
            dto.CustomerType = customer.CustomerType;
            dto.HandlerEmployeeId = customer.HandlerEmployeeId;
            dto.Name = customer.Name;
            dto.Note = customer.Note;
            dto.PhoneNumber = customer.PhoneNumber;
            dto.ProvinceId = customer.ProvinceId;
            dto.SupportEmployeeId = customer.SupportEmployeeId;
            dto.DebtLimit = customer.DebtLimit;
            var totalDebtCustomer = await _debtCustomerService.TotalDebtCustomer(new DTOs.DebtCustomer.SearchDebtCustomerRequest
            {
                CustomerId = customer.Id,
                ToDate = DateTime.Now
            });
            dto.DebtTotal = totalDebtCustomer.Debt;

            return (dto, message, true);
        }

        public async Task<(BillCustomerDetailById, string, bool)> GetDetailById(Guid BillCustomerId)
        {
            var billCustomer = await _billCustomerRepository.FirstOrDefaultAsync(x => x.Id == BillCustomerId);
            if (billCustomer == null)
                return new(null, $"Không tìm thấy hóa đơn có id là {BillCustomerId}", false);

            var result = new BillCustomerDetailById
            {
                BillCustomerCode = billCustomer.Code,
                Id = billCustomer.Id,
                CreateTime = billCustomer.CreationTime,
                VoucherCode = billCustomer.VoucherCode,
                BillVoucherDiscountValue = billCustomer.BillVoucherDiscountValue
            };

            var customer = await _customerRepository.FirstOrDefaultAsync(x => x.Id == billCustomer.CustomerId);
            if (customer != null)
            {
                result.CustomerName = customer.Name;
                result.CustomerPhone = customer.PhoneNumber;
                result.CustomerAddress = customer.Address;
            }

            if (billCustomer.CreatorId.HasValue)
            {
                var user = await _userManager.FindByIdAsync(billCustomer.CreatorId.ToString());
                result.CreatorText = user == null ? "" : user.Name;
            }

            var store = await _storeRepository.FindAsync(x => x.Id == billCustomer.StoreId);
            result.StoreText = store == null ? "" : store.Name;

            result.AmountTotal = billCustomer.AmountCustomerPay;
            var customerDebt = billCustomer.AmountCustomerPay - (billCustomer.Cash ?? 0) - (billCustomer.Banking ?? 0);
            result.CustomerDebt = customerDebt;

            // Get history transportInformation log
            var transportInformation = await _transportInformationRepository.FirstOrDefaultAsync(x => x.SourceId == billCustomer.Id);
            if (transportInformation != null)
            {
                var transportLogQueryable = await _transportInformationLogRepository.GetQueryableAsync();
                var employeeQueryable = await _employeeRepository.GetQueryableAsync();

                var transportLogs = (from tranpsportLog in transportLogQueryable

                                     join employee in employeeQueryable
                                     on tranpsportLog.ShipperId equals employee.Id
                                     into employeeGr
                                     from employee in employeeGr.DefaultIfEmpty()

                                     where tranpsportLog.TransportInformationCode == transportInformation.Code

                                     select new TransportInformationLogDto
                                     {
                                         CreateTime = tranpsportLog.CreationTime,
                                         ShipperId = tranpsportLog.ShipperId,
                                         ShipperText = employee == null ? "" : employee.Name,
                                         ShipTime = tranpsportLog.ShipTime,
                                         TransportInformationCode = tranpsportLog.TransportInformationCode,
                                         Status = tranpsportLog.Status
                                     })
                                    .ToList();

                foreach (var item in transportLogs)
                {
                    if (item.Status == TransportStatus.New)
                        item.StatusText = "Mới";

                    if (item.Status == TransportStatus.WaitingDelivery)
                        item.StatusText = "Chờ giao hàng";

                    if (item.Status == TransportStatus.Delivering)
                        item.StatusText = "Đang giao";

                    if (item.Status == TransportStatus.Moved)
                        item.StatusText = "Đã chuyển kho";

                    if (item.Status == TransportStatus.Confirm)
                        item.StatusText = "Xác nhận chuyển kho";

                    if (item.Status == TransportStatus.Done)
                        item.StatusText = "Hoàn thành";

                    if (item.Status == TransportStatus.Cancel)
                        item.StatusText = "Đã hủy";
                }

                result.TransportInformationLogs = transportLogs;
            }

            var attachments = await _attachmentRepository.GetListAsync(x => x.ObjectType == AttachmentObjectType.BillCustomer && x.ObjectId == billCustomer.Id);
            result.Attachments = attachments.Select(x => new BillCustomerAttachment
            {
                Id = x.Id,
                FileName = x.Name,
                FileUrl = x.Url
            })
            .ToList();

            foreach (var item in result.Attachments)
            {
                if (!string.IsNullOrEmpty(item.FileName))
                {
                    var splitFileName = item.FileName.Split('.');
                    var extensionFile = splitFileName[splitFileName.Length - 1];

                    if (extensionFile.Trim().ToLower() == "pdf")
                        item.TypeFile = TypeFile.Pdf;

                    if (extensionFile.Trim().ToLower() == "xlsx" || extensionFile.Trim().ToLower() == "xls")
                        item.TypeFile = TypeFile.Excel;

                    if (extensionFile.Trim().ToLower() == "docx" || extensionFile.Trim().ToLower() == "doc")
                        item.TypeFile = TypeFile.Word;
                }
            }

            return new(result, "", true);
        }

        public async Task<(List<BillCustomerProductDetail>, string, bool, int)> GetBillProductByBillCustomerId(BillCustomerByIdParam param)
        {
            var result = new List<BillCustomerProductDetail>();
            var billProductQueryable = await _billCustomerProductRepository.GetQueryableAsync();
            var productQueryable = await _productRepository.GetQueryableAsync();

            var query = (from billProduct in billProductQueryable

                         join product in productQueryable
                         on billProduct.ProductId equals product.Id
                         orderby billProduct.Code descending

                         where billProduct.BillCustomerId == param.BillCustomerId

                         select new BillCustomerProductDetail
                         {
                             DiscountUnit = billProduct.DiscountUnit,
                             DiscountValue = billProduct.DiscountValue,
                             Price = billProduct.Price,
                             ProductName = product.Name,
                             Quantity = billProduct.Quantity,
                         });

            var count = query.Count();
            result = query.Skip((param.PageIndex - 1) * param.PageSize)
                          .Take(param.PageSize)
                          .ToList();

            foreach (var item in result)
            {
                item.Total = item.Price * item.Quantity;

                if (item.DiscountValue.HasValue && item.DiscountValue > 0 && item.DiscountUnit == MoneyModificationType.VND)
                {
                    item.Total -= item.DiscountValue;
                }

                if (item.DiscountValue.HasValue && item.DiscountValue > 0 && item.DiscountUnit == MoneyModificationType.Percent)
                {
                    var discountCash = (item.DiscountValue / 100) * item.Total;
                    item.Total -= discountCash;
                    item.DiscountValue = discountCash;
                    item.DiscountUnit = MoneyModificationType.VND;
                }
            }

            return (result, "", true, count);
        }

        public async Task<(List<BillCustomerEntries>, string, bool, int)> GetEntriesByBillCustomerId(BillCustomerByIdParam param)
        {
            var result = new List<BillCustomerEntries>();
            var entries = await _entryRepository.GetQueryableAsync();
            var accountEntry = await _entryAccountRepository.GetQueryableAsync();

            var query = (from entry in entries
                         join acc in accountEntry
                         on entry.Id equals acc.EntryId
                         where entry.DocumentId == param.BillCustomerId
                         select new BillCustomerEntries
                         {
                             Amount = entry.Amount,
                             Credit = acc.CreditAccountCode,
                             Debt = acc.DebtAccountCode,
                             EntryCode = entry.Code,
                             Note = entry.Note ?? "",
                             TransactionDate = entry.TransactionDate,
                             TicketType = entry.TicketType
                         });

            var count = query.Count();
            result = query.Skip((param.PageIndex - 1) * param.PageSize)
                          .Take(param.PageSize)
                          .ToList();

            return (result, "", true, count);
        }

        public async Task<PagingLogBillCustomerResponse> GetLogBillByIdAsync(LogBillCustomerRequest request)
        {
            var LogBillCustomerResponses = new List<LogBillCustomerResponse>();

            var billCustomers = new List<BillCustomer>();
            if (request.BillLogType == null || request.BillLogType == BillLogType.Sale)
                billCustomers = (await _billCustomerRepository.GetQueryableAsync())
                    .Where(x => x.CustomerId == request.CustomerId)
                    .WhereIf(request.From.HasValue, x => x.CreationTime.Date >= request.From.Value.Date)
                    .WhereIf(request.To.HasValue, x => x.CreationTime.Date <= request.To.Value.Date)
                    .WhereIf(!string.IsNullOrEmpty(request.BillCustomerCode), x => x.Code.Contains(request.BillCustomerCode.Trim()))
                    .ToList();

            var customerReturns = new List<CustomerReturn>();
            if (request.BillLogType == null || request.BillLogType == BillLogType.Return)
                customerReturns = (await _customerReturnRepository.GetQueryableAsync())
                    .Where(x => x.CustomerId == request.CustomerId)
                    .WhereIf(request.From.HasValue, x => x.CreationTime.Date >= request.From.Value.Date)
                    .WhereIf(request.To.HasValue, x => x.CreationTime.Date <= request.To.Value.Date)
                    .WhereIf(!string.IsNullOrEmpty(request.BillCustomerCode), x => x.Code.Contains(request.BillCustomerCode.Trim()))
                    .ToList();

            var bills = billCustomers.Select(x => new BillLogDto()
            {
                Id = x.Id,
                Code = x.Code,
                CustomerId = x.CustomerId,
                StoreId = x.StoreId,
                CreationTime = x.CreationTime,
                BillLogType = BillLogType.Sale,
            }).Union(customerReturns.Select(x => new BillLogDto()
            {
                Id = x.Id,
                Code = x.Code,
                CustomerId = x.CustomerId,
                StoreId = x.StoreId,
                CreationTime = x.CreationTime,
                BillLogType = BillLogType.Return,
            })).ToList();

            var stores = (await _storeRepository.GetQueryableAsync())
                .Where(x => bills.Select(x => x.StoreId).Any(storeId => storeId == x.Id)).ToList() ?? new List<Stores>();

            var customer = (await _customerRepository.GetQueryableAsync())
                .FirstOrDefault(x => x.Id == request.CustomerId) ?? new Customer();

            var billCustomerProducts = (await _billCustomerProductRepository.GetQueryableAsync())
                .Where(x => billCustomers.Select(x => x.Id).Any(id => id == (x.BillCustomerId ?? Guid.Empty))).ToList() ?? new List<BillCustomerProduct>();

            var customerReturnProducts = (await _customerReturnProductRepository.GetQueryableAsync())
                .Where(x => customerReturns.Select(x => x.Id).Any(id => id == (x.CustomerReturnId ?? Guid.Empty))).ToList() ?? new List<CustomerReturnProduct>();

            var products = (await _productRepository.GetQueryableAsync())
                .Where(x => billCustomerProducts.Select(x => x.ProductId).Any(productId => productId == x.Id)
                || customerReturnProducts.Select(x => x.ProductId).Any(productId => productId == x.Id))
                .WhereIf(!request.ProductName.IsNullOrWhiteSpace(), x => x.Name.ToLower().Contains(request.ProductName.ToLower()))
                .ToList() ?? new List<Products>();

            foreach (var bill in bills)
            {
                var store = stores.FirstOrDefault(x => x.Id == bill.StoreId) ?? new Stores();
                var bCProducts = billCustomerProducts.Where(x => x.BillCustomerId == bill.Id && products.Select(x => x.Id).Any(id => id == (x.ProductId ?? Guid.Empty)));
                var cRProducts = customerReturnProducts.Where(x => x.CustomerReturnId == bill.Id);
                if (bCProducts.Any())
                {
                    foreach (var bCProduct in bCProducts)
                    {
                        var product = products.FirstOrDefault(x => x.Id == bCProduct.ProductId);
                        var preDiscountTotal = (bCProduct.Price ?? 0) * bCProduct.Quantity;
                        var afterDiscountTotal = preDiscountTotal - (bCProduct.DiscountValue ?? 0);
                        if (bCProduct.DiscountUnit == MoneyModificationType.Percent)
                            afterDiscountTotal = preDiscountTotal - preDiscountTotal * (bCProduct.DiscountValue ?? 0) / 100;

                        LogBillCustomerResponses.Add(new LogBillCustomerResponse()
                        {
                            BillId = bill.Id,
                            BillCode = bill.Code,
                            BillLogType = bill.BillLogType,
                            CreationTime = bill.CreationTime,
                            StoreId = store.Id,
                            StoreCode = store.Code,
                            StoreName = store.Name,
                            CustomerId = customer.Id,
                            CustomerCode = customer.Code,
                            CustomerName = customer.Name,
                            CustomerPhone = customer.PhoneNumber,
                            ProductId = bCProduct.Id,
                            ProductCode = product?.Code,
                            ProductName = product?.Name,
                            Price = bCProduct.Price,
                            Quantity = bCProduct.Quantity,
                            DiscountValue = bCProduct.DiscountValue,
                            DiscountUnit = bCProduct.DiscountUnit,
                            PreDiscountTotal = preDiscountTotal,
                            AfterDiscountTotal = afterDiscountTotal,
                        });
                    }
                }
                if (cRProducts.Any())
                {
                    foreach (var cRProduct in cRProducts)
                    {
                        var product = products.FirstOrDefault(x => x.Id == cRProduct.ProductId);
                        var preDiscountTotal = (cRProduct.Price ?? 0) * (cRProduct.Quantity ?? 0);
                        var afterDiscountTotal = preDiscountTotal - (cRProduct.DiscountValue ?? 0);
                        if (cRProduct.DiscountUnit == MoneyModificationType.Percent)
                            afterDiscountTotal = preDiscountTotal - preDiscountTotal * (cRProduct.DiscountValue ?? 0) / 100;

                        LogBillCustomerResponses.Add(new LogBillCustomerResponse()
                        {
                            BillId = bill.Id,
                            BillCode = bill.Code,
                            BillLogType = bill.BillLogType,
                            CreationTime = bill.CreationTime,
                            StoreId = store.Id,
                            StoreCode = store.Code,
                            StoreName = store.Name,
                            CustomerId = customer.Id,
                            CustomerCode = customer.Code,
                            CustomerName = customer.Name,
                            CustomerPhone = customer.PhoneNumber,
                            ProductId = cRProduct.Id,
                            ProductCode = product?.Code,
                            ProductName = product?.Name,
                            Price = cRProduct.Price,
                            Quantity = cRProduct.Quantity,
                            DiscountValue = cRProduct.DiscountValue,
                            DiscountUnit = cRProduct.DiscountUnit,
                            PreDiscountTotal = preDiscountTotal,
                            AfterDiscountTotal = afterDiscountTotal,
                        });
                    }
                }
            }

            if (!LogBillCustomerResponses.Any())
                return new PagingLogBillCustomerResponse(0, new List<LogBillCustomerResponse>(), 0);

            var result = LogBillCustomerResponses
                .OrderBy(p => p.CreationTime)
                .Skip(request.Offset)
                .Take(request.PageSize)
                .ToList();

            return new PagingLogBillCustomerResponse(LogBillCustomerResponses.Count, result, result.Sum(x => x.AfterDiscountTotal));

        }

        public async Task<(string, bool)> DeleteBillCustomer(Guid BillCustomerId)
        {
            var billCustomer = await _billCustomerRepository.FirstOrDefaultAsync(x => x.Id == BillCustomerId);
            if (billCustomer == null)
                return ($"Không tìm thấy hóa đơn có id là {BillCustomerId}", false);

            if (!statusHitBillCustomer.Contains(billCustomer.CustomerBillPayStatus.Value))
            {
                return ($"Trạng thái hóa đơn không được phép xóa", false);
            }

            var billProducts = await _billCustomerProductRepository.GetListAsync(x => x.BillCustomerId == billCustomer.Id);
            var billProductId = billProducts.Select(x => x.ProductId).ToList();
            var billProductBonus = await _billCustomerProductBonusRepository.GetListAsync(x => billProductId.Contains(x.Id));

            // Sinh phiếu nhập
            //if (billProducts.Any())
            //{
            //    var listPro = new List<WarehousingBillProductRequest>();
            //    listPro = billProducts.Select(x => new WarehousingBillProductRequest
            //    {
            //        ProductId = x.ProductId.GetValueOrDefault(),
            //        Price = x.CostPrice.GetValueOrDefault(),
            //        Quantity = x.Quantity,
            //        DiscountType = x.DiscountUnit,
            //        DiscountAmount = x.DiscountValue
            //    })
            //    .ToList();

            //    await _wareHousingBillService.CreateBill(new DTOs.WarehousingBills.CreateWarehousingBillRequest
            //    {
            //        BillType = WarehousingBillType.Import,
            //        AudienceType = AudienceTypes.Customer,
            //        AudienceId = billCustomer.CustomerId,
            //        StoreId = billCustomer.StoreId.GetValueOrDefault(),
            //        IsFromBillCustomer = true,
            //        DocumentDetailType = DocumentDetailType.ImportCustomer,
            //        SourceId = billCustomer.Id,
            //        Products = listPro,
            //    }, true);
            //}

            await _billCustomerRepository.DeleteAsync(billCustomer);
            await _billCustomerProductBonusRepository.DeleteManyAsync(billProductBonus);
            await _billCustomerProductRepository.DeleteManyAsync(billProducts);

            return new("Xóa hóa đơn bán hàng thành công", true);
        }

        public async Task<Dictionary<object, object>> GetBillCustomerLog(Guid BillCustomerId)
        {
            var result = new Dictionary<object, object>();
            var logs = await _billCustomerLogRepository.GetListAsync(x => x.BillCustomerId == BillCustomerId);
            var lastLogs = logs.OrderByDescending(x => x.CreationTime).FirstOrDefault();
            var fromValue = JsonConvert.DeserializeObject<object>(lastLogs.FromValue);
            var toValue = JsonConvert.DeserializeObject<object>(lastLogs.ToValue);

            result.Add(fromValue, toValue);

            return result;
        }

        public string MapBillCustomerStatus(CustomerBillPayStatus? status)
        {
            if (status == null || !Enum.IsDefined(typeof(CustomerBillPayStatus), status))
                return "";

            return status switch
            {
                // null => "",
                CustomerBillPayStatus.Success => "Thành công",
                CustomerBillPayStatus.Confirm => "Lên đơn",
                CustomerBillPayStatus.Taked => "Đã nhặt hàng",
                CustomerBillPayStatus.CustomerOrder => "Khách đặt hàng",
                CustomerBillPayStatus.Checked => "Đã kiểm xong",
                CustomerBillPayStatus.Delivery => "Đang giao hàng",
                CustomerBillPayStatus.WaitCall => "Chờ gọi hàng",
                CustomerBillPayStatus.Cancel => "Đã hủy"
            };
        }

        public string MapBillCustomerTransportForm(TransportForm? transportForm)
        {
            if (!transportForm.HasValue) return "";

            if (transportForm == TransportForm.staff)
                return "Nhân viên giao";

            if (transportForm == TransportForm.None)
                return "Không vận chuyển";

            if (transportForm == TransportForm.Internal)
                return "Nội bộ";

            if (transportForm == TransportForm.Production)
                return "Qua hãng";

            return "";
        }

        public bool CheckRuleUpdateStatus(CustomerBillPayStatus? oldStatus, CustomerBillPayStatus? newStatus, TransportForm? transportForm)
        {
            var statusAbleUpdateBack = new[]
            {
                CustomerBillPayStatus.CustomerOrder,
                CustomerBillPayStatus.WaitCall,
                CustomerBillPayStatus.Confirm,
                CustomerBillPayStatus.Taked
            };

            if ((oldStatus == CustomerBillPayStatus.Checked || oldStatus == CustomerBillPayStatus.Success)
                && statusAbleUpdateBack.Contains(newStatus ?? CustomerBillPayStatus.CustomerOrder))
                return false;

            if (oldStatus == CustomerBillPayStatus.Delivery
                && (newStatus == CustomerBillPayStatus.Checked
                || statusAbleUpdateBack.Contains(newStatus ?? CustomerBillPayStatus.CustomerOrder)))
                return false;

            if (oldStatus == CustomerBillPayStatus.Success
                && (newStatus == CustomerBillPayStatus.Checked
                || newStatus == CustomerBillPayStatus.Delivery
                || statusAbleUpdateBack.Contains(newStatus ?? CustomerBillPayStatus.CustomerOrder)))
                return false;

            if (newStatus == CustomerBillPayStatus.Delivery)
                return false;

            if (oldStatus == CustomerBillPayStatus.Cancel)
                return false;

            return true;
        }

        private decimal CacularAmountsDiscountBillCustomer(List<BillCustomerProduct> billCustomerProducts, decimal totalPrice, MoneyModificationType? discountUnit, decimal? discountValue)
        {
            decimal result = 0;
            if (discountUnit.HasValue && discountValue.HasValue && discountValue > 0)
            {
                if (discountUnit == MoneyModificationType.Percent)
                    result = ((discountValue.GetValueOrDefault() / 100) * totalPrice);

                if (discountUnit == MoneyModificationType.VND)
                    result = discountValue.GetValueOrDefault();
            }
            else
            {
                foreach (var item in billCustomerProducts)
                {
                    if (item.DiscountUnit.HasValue && item.DiscountValue.HasValue && item.DiscountValue > 0)
                    {
                        if (item.DiscountUnit == MoneyModificationType.Percent)
                            result += Convert.ToDecimal((item.DiscountValue / 100) * (item.Price * item.Quantity));

                        if (item.DiscountUnit == MoneyModificationType.VND)
                            result += item.DiscountValue.GetValueOrDefault();
                    }
                }
            }

            return result;
        }

        public async Task HandleDocumentBillCustomer(Guid BillCustomerId, CustomerBillPayStatus? billStatus)
        {
            var billCustomer = await _billCustomerRepository.FindAsync(x => x.Id == BillCustomerId);
            var billCustomerProducts = await _billCustomerProductRepository.GetListAsync(x => x.BillCustomerId == billCustomer.Id);
            var billCustomerProductIds = billCustomerProducts.Select(x => x.Id).ToList();
            var productBonus = await _billCustomerProductBonusRepository.GetListAsync(x => billCustomerProductIds.Contains(x.BillCustomerProductId ?? Guid.Empty));

            decimal totalPrice = _commonBillCustomerService.CacularAmountsTotalBillCustomer(billCustomerProducts);
            decimal discountPrecentEachProduct = _commonBillCustomerService.DiscountPrecentEachProduct(totalPrice, billCustomer.DiscountUnit, billCustomer.DiscountValue);
            var totalPriceDiscount = billCustomer.AmountAfterDiscount;

            // Cập nhật lại phiếu xuất sản phẩm
            var wareHouseCustomer = await _wareHousingBillRepository.FindAsync(x => x.SourceId == billCustomer.Id && x.DocumentDetailType == DocumentDetailType.ExportCustomer);
            if (wareHouseCustomer != null)
            {
                var listPro = MapToWarehousingBillProductsByBillCustomerProduct(billCustomerProducts);
                await UpdateWarehousing(listPro, billCustomer, wareHouseCustomer.Id, $"Kiểu xuất - khách hàng - hóa đơn {billCustomer.Code}");
            }
            else if (statusCreateEntry.Contains(billStatus.GetValueOrDefault()))
            {
                var listPro = MapToWarehousingBillProductsByBillCustomerProduct(billCustomerProducts);
                await _commonBillCustomerService.CreateWarehousing(listPro, billCustomer, $"Kiểu xuất - khách hàng - hóa đơn {billCustomer.Code}", DocumentDetailType.ExportCustomer);
            }

            // Cập nhật lại phiếu xuất quà tặng
            var wareHouseGift = await _wareHousingBillRepository.FindAsync(x => x.SourceId == billCustomer.Id && x.DocumentDetailType == DocumentDetailType.ExportGift);
            if (wareHouseGift != null)
            {
                if (productBonus.Any())
                {
                    var listProBonus = MapToWarehousingBillProductsByBillCustomerProductBonus(productBonus);
                    await UpdateWarehousing(listProBonus, billCustomer, wareHouseCustomer.Id, $"Kiểu xuất - quà tặng - hóa đơn {billCustomer.Code}");
                }

                if (!productBonus.Any())
                    await _wareHousingBillService.DeleteBill(wareHouseGift.Id);
            }
            else if (productBonus.Any() && statusCreateEntry.Contains(billStatus.GetValueOrDefault()))
            {
                var listProBonus = MapToWarehousingBillProductsByBillCustomerProductBonus(productBonus);
                await _commonBillCustomerService.CreateWarehousing(listProBonus, billCustomer, $"Kiểu xuất - quà tặng - hóa đơn {billCustomer.Code}", DocumentDetailType.ExportGift);
            }

            // Cập nhật lại bút toán của hóa đơn
            var isExistEntry = await _entryRepository.AnyAsync(x => x.DocumentId == BillCustomerId
                                                                    && x.ConfigCode == EntryConfig.BillCustomer.CodeEntry);
            if (isExistEntry)
                await _entryService.AutoUpdateEntryForBillCustomer(billCustomer.Id);
            else await _entryService.AutoCreateEntryForCreatBillCustomer(billCustomer.Id, totalPriceDiscount.GetValueOrDefault());

            // Cập nhật lại bút toán của vat
            decimal? cashVat = 0;
            if (billCustomer.VatUnit.HasValue && billCustomer.VatValue.HasValue && billCustomer.VatValue > 0)
            {
                if (billCustomer.VatUnit == MoneyModificationType.Percent)
                    cashVat = ((billCustomer.VatValue / 100) * totalPriceDiscount);

                if (billCustomer.VatUnit == MoneyModificationType.VND)
                    cashVat = billCustomer.VatValue;
            }

            var isExistEntryVat = await _entryRepository.AnyAsync(x => x.DocumentId == BillCustomerId
                                                                    && x.ConfigCode == EntryConfig.BillCustomerHasVat.CodeEntry);
            if (isExistEntryVat)
                await _entryService.AutoUpdateEntryVatForBillCustomer(billCustomer.Id, cashVat.GetValueOrDefault());
            else if (billCustomer.VatValue.HasValue && billCustomer.VatValue > 0 && statusCreateEntry.Contains(billStatus.GetValueOrDefault()))
                await _entryService.AutoCreateEntryForCreatBillCustomerHasVAT(billCustomer.Id, cashVat.Value);
        }

        private async Task<Guid> GetTransportInformationBillCustomer(Guid BillCustomerId)
        {
            var transportQueryable = await _transportInformationRepository.GetQueryableAsync();
            var transportBillQueryable = await _transportBillRepository.GetQueryableAsync();

            var query = (from tran in transportQueryable
                         join tranBill in transportBillQueryable
                         on tran.Id equals tranBill.TransportInformationId

                         where tranBill.BillCustomerId == BillCustomerId
                         select tran)
                        .FirstOrDefault();

            return query == null ? Guid.Empty : query.Id;
        }

        private async Task UpdateStatusTransportInformation(Guid Id, TransportStatus status, DateTime? ShipTime)
        {
            var transportQueryable = await _transportInformationRepository.FirstOrDefaultAsync(x => x.Id == Id);

            if (transportQueryable != null)
            {
                transportQueryable.Status = status;
                transportQueryable.ShipTime = ShipTime;
                await _transportInformationRepository.UpdateAsync(transportQueryable);
            }
        }

        private async Task CancelTransportInformation(Guid Id)
        {
            var transportQueryable = await _transportInformationRepository.FirstOrDefaultAsync(x => x.Id == Id);

            if (transportQueryable != null)
            {
                transportQueryable.Status = TransportStatus.Cancel;
                transportQueryable.Note = "Hủy vì cập nhập đơn hàng thành không vận chuyển";
                await _transportInformationRepository.UpdateAsync(transportQueryable);
            }
        }

        public async Task HandleTransportInformationForBill(BillCustomer billCustomer)
        {
            if (billCustomer.TransportForm == TransportForm.Internal || billCustomer.TransportForm == TransportForm.Production)
            {
                var transportInfomationId = await GetTransportInformationBillCustomer(billCustomer.Id);
                if (transportInfomationId != Guid.Empty)
                {
                    await UpdateStatusTransportInformation(transportInfomationId, TransportStatus.WaitingDelivery, billCustomer.TransportDate);
                }
                else
                {
                    var statusTransport = billCustomer.CustomerBillPayStatus == CustomerBillPayStatus.Checked ? TransportStatus.WaitingDelivery : TransportStatus.New;
                    var transportInformation = await _transportInformationService.Create(new CreateTransportInformationDto
                    {
                        CustomerId = billCustomer.CustomerId,
                        FromStoreId = billCustomer.StoreId,
                        Status = statusTransport,
                        IsCOD = billCustomer.COD,
                        IsWarehouseTransfer = false,
                        TotalAmount = billCustomer.AmountCustomerPay,
                        TransportForm = TransportForm.staff,
                        SourceId = billCustomer.Id,
                        ActionSource = ActionSources.CreateBillCustomer,
                        ShipTime = billCustomer.TransportDate
                    });

                    await _transportBillRepository.InsertAsync(new TransporstBills()
                    {
                        Id = Guid.NewGuid(),
                        BillCustomerId = billCustomer.Id,
                        TransportInformationId = transportInformation.Id,
                        CreationTime = _clock.Now,
                    });
                }
            }
            else
            {
                var transportInfomationId = await GetTransportInformationBillCustomer(billCustomer.Id);
                if (transportInfomationId != Guid.Empty)
                    await CancelTransportInformation(transportInfomationId);
            }
        }

        public async Task<(bool isValid, string message)> CheckInventoryProduct(List<BillCustomerProductCheckValid> param, Guid? StoreId)
        {
            var isValid = true;
            var message = "";
            var productOrigin = await _productRepository.GetListAsync(x => param.Select(x => x.ProductId).Contains(x.Id));
            var productStore = await _storeProductRepository.GetListAsync(x => x.StoreId == StoreId && param.Select(x => x.ProductId).Contains(x.ProductId));

            foreach (var item in param)
            {
                var product = productStore.FirstOrDefault(x => x.ProductId == item.ProductId);
                if (product == null || product.StockQuantity < item.Quantity)
                {
                    var itemOrigin = productOrigin.Find(x => x.Id == item.ProductId);
                    isValid = false;
                    message = $"Sản phẩm {itemOrigin.Name} có tồn nhỏ hơn số lượng yêu cầu";
                    break;
                }
            }

            return (isValid: isValid, message: message);
        }

        public async Task<PagingResponse<HistoryBillResponse>> GetHistoryBillByCustomerId(HistoryBillRequest request)
        {
            var result = new List<HistoryBillResponse>();
            var billCustomers = new List<BillCustomer>();
            if (request.BillLogType == null || request.BillLogType == BillLogType.Sale)
                billCustomers = (await _billCustomerRepository.GetQueryableAsync())
                    .Where(x => x.CustomerId == request.CustomerId)
                    .WhereIf(!request.Code.IsNullOrWhiteSpace(), x => x.Code.ToLower().Contains(request.Code.ToLower()))
                    .WhereIf(request.From.HasValue, x => x.CreationTime.Date >= request.From.Value.Date)
                    .WhereIf(request.To.HasValue, x => x.CreationTime.Date <= request.To.Value.Date)
                    .ToList();

            var customerReturns = new List<CustomerReturn>();
            if (request.BillLogType == null || request.BillLogType == BillLogType.Return)
                customerReturns = (await _customerReturnRepository.GetQueryableAsync())
                    .Where(x => x.CustomerId == request.CustomerId)
                    .WhereIf(!request.Code.IsNullOrWhiteSpace(), x => x.Code.ToLower().Contains(request.Code.ToLower()))
                    .WhereIf(request.From.HasValue, x => x.CreationTime.Date >= request.From.Value.Date)
                    .WhereIf(request.To.HasValue, x => x.CreationTime.Date <= request.To.Value.Date)
                    .ToList();

            var stores = (await _storeRepository.GetListAsync()) ?? new List<Stores>();

            var customer = (await _customerRepository.GetQueryableAsync())
                .FirstOrDefault(x => x.Id == request.CustomerId) ?? new Customer();

            var billCustomerProducts = (await _billCustomerProductRepository.GetQueryableAsync())
                .Where(x => billCustomers.Select(x => x.Id).Any(id => id == (x.BillCustomerId ?? Guid.Empty))).ToList() ?? new List<BillCustomerProduct>();

            var customerReturnProducts = (await _customerReturnProductRepository.GetQueryableAsync())
                .Where(x => customerReturns.Select(x => x.Id).Any(id => id == (x.CustomerReturnId ?? Guid.Empty))).ToList() ?? new List<CustomerReturnProduct>();

            var data = billCustomers.Select(x => new HistoryBillResponse()
            {
                CreationTime = x.CreationTime,
                BillId = x.Id,
                BillCode = x.Code,
                BillLogType = BillLogType.Sale,
                StoreId = x.StoreId,
                StoreCode = stores.FirstOrDefault(s => s.Id == x.StoreId)?.Code,
                StoreName = stores.FirstOrDefault(s => s.Id == x.StoreId)?.Name,
                CustomerId = x.CustomerId,
                CustomerCode = customer.Code,
                CustomerName = customer.Name,
                CustomerPhone = customer.PhoneNumber,
                AmountProduct = billCustomerProducts.Where(bCP => bCP.BillCustomerId == x.Id).Count(),
                TotalAmountProduct = billCustomerProducts.Where(bCP => bCP.BillCustomerId == x.Id).Sum(bCP => bCP.Quantity),
                AmountCustomerPay = x.AmountCustomerPay,
                Note = x.PayNote,
            }).Union(customerReturns.Select(x => new HistoryBillResponse()
            {
                CreationTime = x.CreationTime,
                BillId = x.Id,
                BillCode = x.Code,
                BillLogType = BillLogType.Return,
                StoreId = x.StoreId,
                StoreCode = stores.FirstOrDefault(s => s.Id == x.StoreId)?.Code,
                StoreName = stores.FirstOrDefault(s => s.Id == x.StoreId)?.Name,
                CustomerId = x.CustomerId,
                CustomerCode = customer.Code,
                CustomerName = customer.Name,
                CustomerPhone = customer.PhoneNumber,
                AmountProduct = customerReturnProducts.Where(cRP => cRP.CustomerReturnId == x.Id).Count(),
                TotalAmountProduct = customerReturnProducts.Where(cRP => cRP.CustomerReturnId == x.Id).Sum(cRP => cRP.Quantity ?? 0),
                AmountCustomerPay = customerReturnProducts.Where(cRP => cRP.CustomerReturnId == x.Id).Sum(cRP => cRP.TotalPriceAfterDiscount),
                Note = x.PayNote,
            })).ToList();

            if (!data.Any())
                return new PagingResponse<HistoryBillResponse>(0, result);

            result = data
                .OrderByDescending(p => p.CreationTime)
                .Skip(request.Offset)
                .Take(request.PageSize)
                .ToList();


            return new PagingResponse<HistoryBillResponse>(data.Count, result);
        }

        public async Task<byte[]> ExportBillCustomer(BillCustomerGetListParam request)
        {
            try
            {
                var customers = await _customerRepository.GetQueryableAsync();
                var products = await _productRepository.GetQueryableAsync();
                var billCustomerProducts = await _billCustomerProductRepository.GetQueryableAsync();
                var billCustomers = await _billCustomerRepository.GetQueryableAsync();
                var query = from billCustomer in billCustomers
                            join billCustomerProduct in billCustomerProducts on billCustomer.Id equals billCustomerProduct.BillCustomerId
                            join customer in customers on billCustomer.CustomerId equals customer.Id
                            join product in products on billCustomerProduct.ProductId equals product.Id
                            orderby billCustomer.Id descending
                            select new
                            {
                                Code = billCustomer.Code,
                                StoreId = billCustomer.StoreId,
                                CustomerId = billCustomer.CustomerId,
                                CreationTime = billCustomer.CreationTime,
                                CustomerName = customer.Name,
                                EmployeeCare = billCustomer.EmployeeCare,
                                HandlerEmployeeName = customer.HandlerEmpName,
                                EmployeeSell = billCustomer.EmployeeSell,
                                Coupon = billCustomer.Coupon,
                                DateOfBirth = customer.DateOfBirth,
                                CustomerPhone = customer.PhoneNumber,
                                CustomerAddress = customer.Address,
                                BarCode = product.BarCode,
                                ProductCode = product.Code,
                                ParentProductCode = product.ParentCode,
                                ParentProductName = product.ParentName,
                                ProductName = product.Name,
                                ProductBonusCode = "",
                                CostPriceBonus = 0,
                                Unit = "",
                                IMEI = "",
                                SalePrice = product.SalePrice,
                                CostPrice = product.StockPrice,
                                Quantity = billCustomerProduct.Quantity,
                                VAT = billCustomer.VatValue,
                                AfterDiscount = billCustomer.AmountAfterDiscount,
                                DiscountValue = billCustomer.DiscountValue,
                                Cash = billCustomer.Cash,
                                AccountCodeBanking = billCustomer.AccountCodeBanking,
                                AmountCustomerPay = billCustomer.AmountCustomerPay,
                                Note = billCustomer.PayNote,
                                Profit = product.Profit,
                                VoucherCode = billCustomer.VoucherCode,
                                ProductCategoryName = "",
                                StillDebt = 0,
                                RefundBillCode = "",
                                TablePriceId = billCustomer.TablePriceId,
                                Carrier = "",
                                CarrierShippingCode = billCustomer.CarrierShippingCode,
                                Distance = 1,
                                CustomerBillPayStatus = billCustomer.CustomerBillPayStatus
                            };


                query = query.WhereIf(request.StoreIds != null && request.StoreIds.Count > 0, x => request.StoreIds.Contains(x.StoreId.HasValue ? x.StoreId.Value : new Guid()))
                    .WhereIf(!string.IsNullOrEmpty(request.BillCustomerCode), x => x.Code.Contains(request.BillCustomerCode))
                    .WhereIf(request.CreateTimeFrom != null, x => x.CreationTime.Date >= request.CreateTimeFrom.Value.Date)
                    .WhereIf(request.CreateTimeTo != null, x => x.CreationTime.Date <= request.CreateTimeTo.Value.Date)
                    .WhereIf(!string.IsNullOrEmpty(request.CustomerName), x => !string.IsNullOrEmpty(x.CustomerName) ? x.CustomerName.ToLower().Contains(request.CustomerName.ToLower()) : request.CustomerName == "")
                    .WhereIf(!string.IsNullOrEmpty(request.ProductName), x => x.ProductName.Contains(request.ProductName))
                    //.WhereIf(!string.IsNullOrEmpty(request.EmployeeCashier), x => x.)
                    //.WhereIf(!string.IsNullOrEmpty(request.CouponCode), x => x.CouponCode)
                    //.WhereIf(request.IMei)
                    //.WhereIf(request.ProductCategory)
                    //.WhereIf(!string.IsNullOrEmpty(request.EmployeeSell), x => x.EmployeeSell)
                    .WhereIf(!string.IsNullOrEmpty(request.Description), x => request.Description.Contains(x.Note))
                    //.WhereIf(request.EmployeeTech)
                    //.WhereIf(request.IsCheckData)
                    .WhereIf(request.CustomerBillPayStatus != null, x => x.CustomerBillPayStatus == request.CustomerBillPayStatus);

                var storeIds = query.Select(x => x.StoreId).ToList();
                var stores = (await _storeRepository.GetListAsync()).Where(p => storeIds.ToList().Contains(p.Id));
                var EmployeeCareIds = query.Select(x => x.EmployeeCare).ToList();
                var EmployeeCares = (await _userRepository.GetListAsync()).Where(p => EmployeeCareIds.ToList().Contains(p.Id));
                var EmployeeSellIds = query.Select(x => x.EmployeeSell).ToList();
                var EmployeeSells = (await _userRepository.GetListAsync()).Where(p => EmployeeSellIds.ToList().Contains(p.Id));

                var exportData = new List<ExportBillCustomerResponse>();
                foreach (var item in query)
                {
                    var storeName = stores.FirstOrDefault(x => x.Id == item.StoreId)?.Name;
                    var employeeCare = EmployeeCares.FirstOrDefault(x => x.Id == item.EmployeeCare)?.Name;
                    var employeeSell = EmployeeSells.FirstOrDefault(x => x.Id == item.EmployeeSell)?.Name;

                    exportData.Add(new ExportBillCustomerResponse()
                    {
                        Code = item.Code,
                        StoreName = storeName,
                        CreationTime = item.CreationTime.ToString("dd-MM-yyyy"),
                        CustomerName = item.CustomerName,
                        EmployeeCare = employeeCare,
                        HandlerEmployeeName = item.HandlerEmployeeName,
                        EmployeeSell = employeeSell,
                        DateOfBirth = item.DateOfBirth.HasValue ? item.DateOfBirth.Value.ToString("dd-MM-yyyy") : "",
                        CustomerPhone = item.CustomerPhone,
                        CustomerAddress = item.CustomerAddress,
                        BarCode = item.BarCode,
                        ProductCode = item.ProductCode,
                        ParentProductCode = item.ParentProductCode,
                        ParentProductName = item.ParentProductName,
                        ProductName = item.ProductName,
                        ProductBonusCode = "",
                        CostPriceBonus = 0,
                        Unit = "",
                        IMEI = "",
                        SalePrice = item.SalePrice.HasValue ? item.SalePrice.Value : 0,
                        CostPrice = item.CostPrice,
                        Quantity = item.Quantity,
                        VAT = item.VAT.HasValue ? item.VAT.Value : 0,
                        AfterDiscount = item.AfterDiscount.HasValue ? item.AfterDiscount.Value : 0,
                        DiscountValue = item.DiscountValue.HasValue ? item.DiscountValue.Value : 0,
                        Cash = item.Cash.HasValue ? item.Cash.Value : 0,
                        AccountCodeBanking = item.AccountCodeBanking,
                        AmountCustomerPay = item.AmountCustomerPay.HasValue ? item.AmountCustomerPay.Value : 0,
                        Note = item.Note,
                        Profit = item.Profit.HasValue ? item.Profit.Value : 0,
                        VoucherCode = item.VoucherCode,
                        ProductCategoryName = "",
                        StillDebt = 0,
                        RefundBillCode = "",
                        TablePriceId = item.TablePriceId.HasValue ? item.TablePriceId.ToString() : "",
                        //Carrier = "",
                        CarrierShippingCode = item.CarrierShippingCode,
                        Distance = 0
                    });
                }
                return ExcelHelper.ExportExcel(exportData);
            }
            catch (Exception ex)
            {
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }

        }

        public async Task<(string message, bool success, Guid? data, byte[]? fileRespon)> ImportBillCustomer(BillCustomerImportParam param)
        {
            var workbook = new CustomWorkBook();
            var listData = await MappingDataImportBillCustomerProduct(param.File, param.StoreId.GetValueOrDefault());

            var dataToInsert = listData.Where(x => x.Success)
                .Select(x => new DTOs.BillCustomers.Params.BillCustomerProductDto
                {
                    ProductId = x.ProductId,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    DiscountUnit = x.DiscountUnit,
                    DiscountValue = x.DiscountValue,
                    CostPrice = x.CostPrice,
                    Note = x.Note
                })
                .ToList();

            if (!dataToInsert.Any())
            {
                var sheet = RenderTemplateAfterImport(listData);
                workbook.Sheets.Add(sheet);

                var fileReturn = ExcelHelper.ExportExcel(workbook);
                return ("Danh sách sản phẩm không hợp lệ", false, null, fileReturn);
            }    
              

            var paramCreateBillCustomer = new BillCustomerCreateParam
            {
                DiscountValue = param.DiscountValue,
                DiscountUnit = param.DiscountUnit,
                AccountCode = param.AccountCode,
                AccountCodeBanking = param.AccountCodeBanking,
                Address = param.Address,
                Banking = param.Banking,
                BillCustomerProducts = dataToInsert,
                Cash = param.Cash,
                COD = param.COD,
                CustomerBillPayStatus = param.CustomerBillPayStatus,
                CustomerId = param.CustomerId,
                CustomerName = param.CustomerName,
                CustomerPhone = param.CustomerPhone,
                CustomerType = param.CustomerType,
                EmployeeCare = param.EmployeeCare,
                EmployeeNote = param.EmployeeNote,
                Note = param.Note,
                EmployeeSell = param.EmployeeSell,
                ProvinceId = param.ProvinceId,
                StoreId = param.StoreId,
                VatUnit = param.VatUnit,
                VatValue = param.VatValue,
                TransportDate = param.TransportDate,
                PayNote = param.PayNote,
                TransportForm = param.TransportForm,
            };

            var responCreateBill = await CreateCustomerBill(paramCreateBillCustomer);

            if (responCreateBill.Item3)
            {
                var sheet = RenderTemplateAfterImport(listData);
                workbook.Sheets.Add(sheet);

                var fileReturn = ExcelHelper.ExportExcel(workbook);
                return (responCreateBill.Item2, true, (responCreateBill.Item1 as BillCustomerDto).Id, fileReturn);
            }
            else return (responCreateBill.Item2, false, null, null);
        }

        private async Task<List<BillCustomerProductImportRespon>> MappingDataImportBillCustomerProduct(IFormFile file, Guid StoreId)
        {
            var result = new List<BillCustomerProductImportRespon>();

            using (var workbook = new XLWorkbook(file.OpenReadStream()))
            {
                var worksheet = workbook.Worksheet(1); // Chỉ định số trang tính trong tệp Excel (index bắt đầu từ 1)

                var firstRowUsed = worksheet.FirstRowUsed();
                var headers = firstRowUsed.CellsUsed()
                    .Select(c => c.Value.ToString().Trim())
                    .ToList();

                var products = await _productRepository.GetListAsync();
                var storeProduct = await _storeProductRepository.GetListAsync();
                Products objOrigin = null;

                foreach (var row in worksheet.RowsUsed().Skip(1)) // Bỏ qua hàng tiêu đề (hàng đầu tiên)
                {
                    var obj = new BillCustomerProductImportRespon();

                    for (int i = 0; i < headers.Count; i++)
                    {
                        var cellValue = row.Cell(i + 1).Value == null ? "" : row.Cell(i + 1).Value.ToString().Trim();

                        // Gán giá trị cho thuộc tính tương ứng trong đối tượng
                        if (headers[i] == "Sản phẩm")
                        {
                            obj.ColExcel1 = cellValue;
                            if (string.IsNullOrEmpty(cellValue))
                            {
                                obj.Success = false;
                                obj.Message += "Thông tin sản phẩm trống,";
                            }

                            objOrigin = products.FirstOrDefault(x => x.Name == cellValue || x.BarCode == cellValue || x.Code == cellValue);

                            if (objOrigin == null)
                            {
                                obj.Success = false;
                                obj.Message += "Không tìm thấy thông tin sản phẩm,";
                            }
                            else
                            {
                                obj.ProductId = objOrigin.Id;
                                obj.CostPrice = objOrigin.StockPrice;
                            }
                        }
                        else if (headers[i] == "Số lượng")
                        {
                            obj.ColExcel2 = cellValue;

                            if (obj.Success)
                            {
                                int quantityParse = 0;
                                if (!int.TryParse(cellValue, out quantityParse))
                                {
                                    obj.Success = false;
                                    obj.Message += "Dữ liệu số lượng không hợp lệ,";
                                }

                                var productStock = storeProduct.FirstOrDefault(x => x.ProductId == objOrigin.Id && x.StoreId == StoreId);
                                if (productStock == null)
                                {
                                    obj.Success = false;
                                    obj.Message += "Số lượng sản phẩm lớn hơn tồn kho,";
                                }
                                if (productStock != null && quantityParse > productStock.StockQuantity)
                                {
                                    obj.Success = false;
                                    obj.Message += "Số lượng sản phẩm lớn hơn tồn kho,";
                                }

                                obj.Quantity = quantityParse;
                            }
                        }
                        else if (headers[i] == "Giá")
                        {
                            obj.ColExcel3 = cellValue;
                            if (obj.Success)
                            {
                                decimal priceParse = 0;
                                if (!decimal.TryParse(cellValue, out priceParse) || priceParse < 0)
                                {
                                    obj.Success = false;
                                    obj.Message += "Dữ liệu số lượng không hợp lệ,";
                                }

                                obj.Price = priceParse;
                            }
                        }
                        else if (headers[i] == "Chiết khấu")
                        {
                            obj.ColExcel4 = cellValue;
                            string discountValueStr;
                            decimal discountValue = 0;
                            if (obj.Success && !string.IsNullOrEmpty(cellValue))
                            {
                                if (cellValue.Contains("%"))
                                {
                                    obj.DiscountUnit = MoneyModificationType.Percent;
                                    discountValueStr = cellValue.Replace("%", "");
                                }
                                else
                                {
                                    obj.DiscountUnit = MoneyModificationType.VND;
                                    discountValueStr = cellValue;
                                }

                                if (!decimal.TryParse(discountValueStr, out discountValue))
                                {
                                    obj.Success = false;
                                    obj.Message += "Dữ liệu triết khấu không hợp lệ,";
                                }
                                else obj.DiscountValue = discountValue;
                            }
                        }
                        else if (headers[i] == "Ghi chú")
                        {
                            obj.ColExcel5 = cellValue;
                            obj.Note = cellValue;
                        }
                    }

                    if (obj.Success)
                        obj.Message = "Sản phẩm đã được nhập thành công";

                    result.Add(obj);
                }

                return result;
            }
        }

        public async Task<byte[]> DownloadTemplateImport()
        {
            var workbook = new CustomWorkBook();
            var sheet = RenderTemplateImport();
            workbook.Sheets.Add(sheet);

            return ExcelHelper.ExportExcel(workbook);
        }

        private CustomSheet RenderTemplateImport()
        {
            var sheet = new CustomSheet("Sheet 1");

            var startRow = 1;

            var header = new CustomDataTable()
            {
                StartRowIndex = startRow,
                StartColumnIndex = 1,
                RowDirection = Directions.Horizontal,
                Rows = new List<DataRow>
                    {
                        new DataRow(
                            new HeaderCell("Sản phẩm"),
                            new HeaderCell("Số lượng"),
                            new HeaderCell("Giá"),
                            new HeaderCell("Chiết khấu"),
                            new HeaderCell("Ghi chú")
                            )
                    }
            };

            sheet.Tables.Add(header);

            return sheet;
        }

        private CustomSheet RenderTemplateAfterImport(List<BillCustomerProductImportRespon> list)
        {
            var sheet = new CustomSheet("Sheet 1");

            var startRow = 1;

            var header = new CustomDataTable()
            {
                StartRowIndex = startRow,
                StartColumnIndex = 1,
                RowDirection = Directions.Horizontal,
                Rows = new List<DataRow>
                    {
                        new DataRow(
                            new HeaderCell("Sản phẩm"),
                            new HeaderCell("Số lượng"),
                            new HeaderCell("Giá"),
                            new HeaderCell("Chiết khấu"),
                            new HeaderCell("Ghi chú"),
                            new HeaderCell("Trạng thái"),
                            new HeaderCell("Diễn giải")
                            )
                    }
            };
            sheet.Tables.Add(header);

            var indexSaleOrderColumn = startRow + 1;
            foreach (var item in list)
            {
                var row = new CustomDataTable()
                {
                    StartRowIndex = indexSaleOrderColumn++,
                    StartColumnIndex = 1,
                    RowDirection = Directions.Horizontal,
                    Rows = new List<DataRow>
                        {
                        new DataRow(
                            new Cell(item.ColExcel1),
                            new Cell(item.ColExcel2),
                            new Cell(item.ColExcel3),
                            new Cell(item.ColExcel4),
                            new Cell(item.ColExcel5),
                            new Cell(item.Success ? "Thành công" : "Thất bại"),
                            new Cell(item.Message)
                            )
                    }
                };

                sheet.Tables.Add(row);
            }

            return sheet;
        }

        public async Task DeleteBillCustomers(List<Guid> BillCustomerId)
        {
            var billCustomer = await _billCustomerRepository.GetListAsync(x => BillCustomerId.Contains(x.Id));
            await _billCustomerRepository.DeleteManyAsync(billCustomer);

            var billCustomerProducts = await _billCustomerProductRepository.GetListAsync(x => BillCustomerId.Contains(x.BillCustomerId ?? Guid.Empty));
            await _billCustomerProductRepository.DeleteManyAsync(billCustomerProducts);

            var listCustomerProductId = billCustomerProducts.Select(x => x.Id).ToList();
            var billProductBonus = await _billCustomerProductBonusRepository.GetListAsync(x => listCustomerProductId.Contains(x.BillCustomerProductId ?? Guid.Empty));
            await _billCustomerProductBonusRepository.DeleteManyAsync(billProductBonus);

            // Xóa bút toán của hóa đơn
            var entrieBillIds = await _entryRepository.GetListAsync(x => BillCustomerId.Contains(x.DocumentId ?? Guid.Empty)
            || BillCustomerId.Contains(x.SourceId ?? Guid.Empty));

            if (entrieBillIds.Any())
            {
                var lstEntriesBillId = entrieBillIds.Select(x => x.Id).ToList();
                var debtEntriesBill = await _debtRepository.GetListAsync(x => lstEntriesBillId.Contains(x.EntryId ?? Guid.Empty));
                await _debtRepository.DeleteManyAsync(debtEntriesBill);
            }

            await _entryRepository.DeleteManyAsync(entrieBillIds);

            // Xóa phiếu thu, báo có
            var paymentReceipts = await _paymentReceiptRepository.GetListAsync(x => BillCustomerId.Contains(x.SourceId ?? Guid.Empty));
            if (paymentReceipts.Any())
            {
                var paymentReceiptId = paymentReceipts.Select(x => x.Id).ToList();
                var entriesPayment = await _entryRepository.GetListAsync(x => paymentReceiptId.Contains(x.DocumentId ?? Guid.Empty)
                || paymentReceiptId.Contains(x.SourceId ?? Guid.Empty));

                var lstEntriesPaymentId = entriesPayment.Select(x => x.Id).ToList();
                var debtEntriesBill = await _debtRepository.GetListAsync(x => lstEntriesPaymentId.Contains(x.EntryId ?? Guid.Empty));
                await _debtRepository.DeleteManyAsync(debtEntriesBill);
                await _entryRepository.DeleteManyAsync(entriesPayment);
            }

            await _paymentReceiptRepository.DeleteManyAsync(paymentReceipts);

            // Xóa phiếu xuất nhập kho
            var wareHouse = await _wareHousingBillRepository.GetListAsync(x => BillCustomerId.Contains(x.SourceId ?? Guid.Empty));
            if (wareHouse.Any())
            {
                var wareHouseId = wareHouse.Select(x => x.Id).ToList();
                var entriesWareHouse = await _entryRepository.GetListAsync(x => wareHouseId.Contains(x.DocumentId ?? Guid.Empty)
                || wareHouseId.Contains(x.SourceId ?? Guid.Empty));

                var lstEntriesWareHouseId = entriesWareHouse.Select(x => x.Id).ToList();
                var debtEntriesBill = await _debtRepository.GetListAsync(x => lstEntriesWareHouseId.Contains(x.EntryId ?? Guid.Empty));

                var wareProducts = await _warehouseBillProductRepository.GetListAsync(x => wareHouseId.Contains(x.WarehousingBillId));

                await _debtRepository.DeleteManyAsync(debtEntriesBill);
                await _entryRepository.DeleteManyAsync(entriesWareHouse);
                await _warehouseBillProductRepository.DeleteManyAsync(wareProducts);
            }

            await _wareHousingBillRepository.DeleteManyAsync(wareHouse);
        }
    }
}
