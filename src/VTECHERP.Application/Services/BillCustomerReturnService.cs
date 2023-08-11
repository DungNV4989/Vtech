using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using VTECHERP.DTOs.BillCustomers.Respons;
using VTECHERP.DTOs.WarehousingBills;
using VTECHERP.Entities;
using VTECHERP.Enums.Bills;
using VTECHERP.Enums;
using VTECHERP.ServiceInterfaces;
using Volo.Abp.Uow;
using Volo.Abp.Domain.Repositories;
using VTECHERP.Services.Interface;

namespace VTECHERP.Services
{
    public class BillCustomerReturnService : IBillCustomerReturnService
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<BillCustomer> _billCustomerRepository;

        private readonly IRepository<CustomerReturn> _customerReturnRepository;
        private readonly IRepository<CustomerReturnProduct> _customerReturnProductRepository;
        private readonly IRepository<Products> _productsRepository;
        private readonly IEntryService _entryService;
        private readonly ICommonBillCustomerService _commonBillCustomerService;
        public BillCustomerReturnService(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<BillCustomer> billCustomerRepository,
            IRepository<CustomerReturn> customerReturnRepository,
            IRepository<CustomerReturnProduct> customerReturnProductRepository,
            IRepository<Products> productsRepository,
            IEntryService entryService,
            ICommonBillCustomerService commonBillCustomerService
            )
        {
            _unitOfWorkManager = unitOfWorkManager;
            _billCustomerRepository = billCustomerRepository;
            _customerReturnRepository = customerReturnRepository;
            _customerReturnProductRepository = customerReturnProductRepository;
            _productsRepository = productsRepository;
            _entryService = entryService;
            _commonBillCustomerService = commonBillCustomerService;
        }

        public async Task<(BillCustomerDto, string, bool)> AutoCreateCustomerBillForReturnProduct(Guid id)
        {
            var uow = _unitOfWorkManager.Current;

            var returnProductBill = await _customerReturnRepository.GetAsync(x => x.Id == id);
            var returnProductBillProducts = await _customerReturnProductRepository.GetListAsync(x => x.CustomerReturnId == returnProductBill.Id);
            var storeProducts = await _productsRepository.GetListAsync(x => returnProductBillProducts.Select(y => y.ProductId).Contains(x.Id));
            var objDB = new BillCustomer()
            {
                Banking = returnProductBill.Banking,
                Cash = returnProductBill.Cash,
                COD = false,
                CustomerBillPayStatus = CustomerBillPayStatus.Success,
                CustomerId = returnProductBill.CustomerId,
                StoreId = returnProductBill.StoreId,
                PayNote = returnProductBill.PayNote,
                AccountCode = returnProductBill.AccountCode,
                AccountCodeBanking = returnProductBill.AccountCodeBanking,
                EmployeeNote = returnProductBill.PayNote
            };
            var listBillProds = new List<DTOs.BillCustomers.Params.BillCustomerProductDto>();
            returnProductBillProducts.ForEach(x =>
            {
                var prd = new DTOs.BillCustomers.Params.BillCustomerProductDto()
                {
                    ProductId = x.ProductId,
                    DiscountUnit = x.DiscountUnit,
                    DiscountValue = x.DiscountValue,
                    Price = x.Price,
                    Quantity = x.Quantity.Value,
                    ProductName = x.Name,
                };
                listBillProds.Add(prd);
            });

            try
            {
                await _billCustomerRepository.InsertAsync(objDB);
                returnProductBill.BillCustomerId = objDB.Id;
                returnProductBill.BillCustomerCode = objDB.Code;
                await _customerReturnRepository.UpdateAsync(returnProductBill);
                var billCustomerProducts = new List<BillCustomerProduct>();
                var productBonus = new List<BillCustomerProductBonus>();
                var resultAddProduct = await _commonBillCustomerService.AddProductForBillCustomer(objDB.Id, listBillProds);
                billCustomerProducts = resultAddProduct.Item1;
                productBonus = resultAddProduct.Item2;

                await uow.SaveChangesAsync();

                decimal totalPrice = _commonBillCustomerService.CacularAmountsTotalBillCustomer(billCustomerProducts);
                decimal totalPriceDiscount = _commonBillCustomerService.CacularAmountsAfterDiscountBillCustomer(billCustomerProducts, totalPrice, returnProductBill.DiscountUnit, returnProductBill.DiscountValue);
                decimal discountPrecentEachProduct = _commonBillCustomerService.DiscountPrecentEachProduct(totalPrice, returnProductBill.DiscountUnit, returnProductBill.DiscountValue);

                decimal totalPriceVat = 0;
                //Tính tổng tiền sau vat
                decimal? cashVat = 0;
                totalPriceVat = totalPriceDiscount;

                objDB.AmountAfterDiscount = totalPriceDiscount;
                objDB.AmountCustomerPay = totalPriceVat;


                // Tạo bút toán
                await _entryService.AutoCreateEntryForCreatBillCustomer(objDB.Id, totalPriceDiscount);

                var billCustomerProductsExport = new List<WarehousingBillProductRequest>();
                var productIds = returnProductBillProducts.Select(p => p.ProductId).ToList();
                var products = await _productsRepository.GetListAsync(p => productIds.Contains(p.Id));
                returnProductBillProducts.ForEach(x =>
                {
                    decimal price = 0;
                    //var storeProd = storeProducts.FirstOrDefault(y => y.ProductId == x.ProductId);
                    // lấy giá vốn
                    var prod = products.FirstOrDefault(y => y.Id == x.ProductId);
                    if (prod != null)
                        price = prod.StockPrice;
                    var prd = new WarehousingBillProductRequest()
                    {
                        ProductId = x.ProductId.Value,
                        Price = price,
                        Quantity = x.Quantity.Value,
                        DiscountType = x.DiscountUnit,
                        DiscountAmount = x.DiscountValue.Value,
                    };
                    billCustomerProductsExport.Add(prd);
                });
                // Tạo phiếu xuất kho - khách hàng
                await _commonBillCustomerService.CreateWarehousing(billCustomerProductsExport, objDB, "Kiểu xuất - khách hàng", DocumentDetailType.ExportCustomer, true, true);

                await uow.SaveChangesAsync();
            }
            catch (Exception ex) 
            {
                await uow.RollbackAsync();
                throw;
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

        public Task<(BillCustomerDto, string, bool)> AutoDeleteCustomerBillForReturnProduct(Guid id)
        {
            throw new NotImplementedException();
        }
    }
}
