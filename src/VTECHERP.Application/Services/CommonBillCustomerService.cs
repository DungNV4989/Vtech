using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VTECHERP.Constants;
using VTECHERP.DTOs.BillCustomers.Params;
using VTECHERP.DTOs.BillCustomers.Respons;
using VTECHERP.DTOs.WarehousingBills;
using VTECHERP.Entities;
using VTECHERP.Enums;
using VTECHERP.Enums.Bills;
using VTECHERP.Services.Interface;

namespace VTECHERP.Services
{
    public class CommonBillCustomerService : ICommonBillCustomerService
    {
        private readonly IRepository<BillCustomerProduct> _billCustomerProductRepository;
        private readonly IRepository<BillCustomerProductBonus> _billCustomerProductBonusRepository;
        private readonly IRepository<Products> _productRepository;
        private readonly IWarehousingBillService _warehousingBillService;
        public CommonBillCustomerService(
            IRepository<BillCustomerProduct> billCustomerProductRepository,
            IRepository<BillCustomerProductBonus> billCustomerProductBonusRepository,
            IWarehousingBillService warehousingBillService,
            IRepository<Products> productRepository
            ) 
        {
            _billCustomerProductRepository = billCustomerProductRepository;
            _billCustomerProductBonusRepository = billCustomerProductBonusRepository;
            _warehousingBillService = warehousingBillService;
            _productRepository = productRepository;
        }
        public async Task CreateWarehousing(List<WarehousingBillProductRequest> listPro, BillCustomer billCustomer, string note, DocumentDetailType documentDetailType, bool calculateStockPrice = true, bool isFromReturnProduct = false)
        {
            await _warehousingBillService.CreateBill(new DTOs.WarehousingBills.CreateWarehousingBillRequest
            {
                BillType = WarehousingBillType.Export,
                AudienceType = AudienceTypes.Customer,
                AudienceId = billCustomer.CustomerId,
                StoreId = billCustomer.StoreId.GetValueOrDefault(),
                IsFromBillCustomer = true,
                IsFromCustomerReturn = isFromReturnProduct,
                DocumentDetailType = documentDetailType,
                Note = note,
                SourceId = billCustomer.Id,
                Products = listPro,
            }, calculateStockPrice);
        }
        public async Task<(List<BillCustomerProduct>, List<BillCustomerProductBonus>)> AddProductForBillCustomer(Guid BillCustomerId, List<DTOs.BillCustomers.Params.BillCustomerProductDto> BillCustomerProducts)
        {
            var billCustomerProducts = new List<BillCustomerProduct>();
            var productBonus = new List<BillCustomerProductBonus>();
            var products = await _productRepository.GetListAsync();

            foreach (var item in BillCustomerProducts)
            {
                var billCustomerProduct = new BillCustomerProduct()
                {
                    Id = Guid.NewGuid(),
                    BillCustomerId = BillCustomerId,
                    DiscountUnit = item.DiscountUnit,
                    DiscountValue = item.DiscountValue,
                    Price = item.Price,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    CostPrice = item.CostPrice,
                    Note= item.Note,
                };

                var productBillCustomerProduct = products.Find(x => x.Id == billCustomerProduct.ProductId);
                if (productBillCustomerProduct != null)
                    billCustomerProduct.ProductCategoryId = productBillCustomerProduct.CategoryId;

                billCustomerProducts.Add(billCustomerProduct);

                if (item.ProductBonus.Any())
                {
                    foreach (var proBonus in item.ProductBonus)
                    {
                        var productBonusItem = new BillCustomerProductBonus()
                        {
                            Id = Guid.NewGuid(),
                            BillCustomerProductId = billCustomerProduct.Id,
                            Quantity = proBonus.Quantity,
                            IsDebt = proBonus.IsDebt,
                            ProductId = proBonus.ProductId,
                        };

                        productBonus.Add(productBonusItem);
                    }
                }

                if (item.ProductChildren.Any())
                {
                    foreach (var proChild in item.ProductChildren)
                    {
                        var billCustomerProductChild = new BillCustomerProduct()
                        {
                            Id = Guid.NewGuid(),
                            BillCustomerId = BillCustomerId,
                            DiscountUnit = proChild.DiscountUnit,
                            DiscountValue = proChild.DiscountValue,
                            Price = proChild.Price,
                            ProductId = proChild.ProductId,
                            Quantity = proChild.Quantity,
                            ParentId = billCustomerProduct.Id,
                            CostPrice = proChild.CostPrice,
                            Note= proChild.Note,
                        };

                        billCustomerProducts.Add(billCustomerProductChild);
                    }
                }
            }

            await _billCustomerProductRepository.InsertManyAsync(billCustomerProducts);
            await _billCustomerProductBonusRepository.InsertManyAsync(productBonus);

            return (billCustomerProducts, productBonus);
        }

        public decimal CacularAmountsTotalBillCustomer(List<BillCustomerProduct> billCustomerProducts)
        {
            decimal totalPrice = 0;
            totalPrice = Convert.ToDecimal(billCustomerProducts.Sum(x => x.Quantity * x.Price));

            return totalPrice;
        }

        public decimal CacularAmountsAfterDiscountBillCustomer(List<BillCustomerProduct> billCustomerProducts, decimal totalPrice, MoneyModificationType? discountUnit, decimal? discountValue)
        {
            decimal totalPriceDiscount = 0;
            if (discountUnit.HasValue && discountValue.HasValue && discountValue > 0)
            {
                if (discountUnit == MoneyModificationType.Percent)
                {
                    var cashDiscount = ((discountValue / 100) * totalPrice);
                    totalPriceDiscount = Convert.ToDecimal(totalPrice - cashDiscount);
                }

                if (discountUnit == MoneyModificationType.VND)
                {
                    totalPriceDiscount = Convert.ToDecimal(totalPrice - discountValue);
                }
            }
            else
            {
                totalPriceDiscount = totalPrice;
                foreach (var item in billCustomerProducts)
                {
                    if (item.DiscountUnit.HasValue && item.DiscountValue.HasValue && item.DiscountValue > 0)
                    {
                        if (item.DiscountUnit == MoneyModificationType.Percent)
                        {
                            var cashDiscount = ((item.DiscountValue / 100) * (item.Price * item.Quantity));
                            totalPriceDiscount -= Convert.ToDecimal(cashDiscount);
                        }

                        if (item.DiscountUnit == MoneyModificationType.VND)
                        {
                            totalPriceDiscount -= Convert.ToDecimal(item.DiscountValue);
                        }
                    }
                }
            }

            return totalPriceDiscount;
        }

        public decimal DiscountPrecentEachProduct(decimal totalPrice, MoneyModificationType? discountUnit, decimal? discountValue)
        {
            decimal discountPrecentEachProduct = 0;
            if (discountUnit.HasValue && discountValue.HasValue && discountValue > 0)
            {
                if (discountUnit == MoneyModificationType.Percent)
                {
                    discountPrecentEachProduct = discountValue.GetValueOrDefault();
                }

                if (discountUnit == MoneyModificationType.VND)
                {
                    discountPrecentEachProduct = Convert.ToDecimal((discountValue / totalPrice) * 100);
                }
            }

            return discountPrecentEachProduct;
        }
    }
}
