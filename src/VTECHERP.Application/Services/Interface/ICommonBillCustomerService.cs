using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.BillCustomers.Params;
using VTECHERP.DTOs.BillCustomers.Respons;
using VTECHERP.DTOs.WarehousingBills;
using VTECHERP.Entities;
using VTECHERP.Enums;

namespace VTECHERP.Services.Interface
{
    public interface ICommonBillCustomerService : IScopedDependency
    {
        Task CreateWarehousing(List<WarehousingBillProductRequest> listPro, BillCustomer billCustomer, string note, DocumentDetailType documentDetailType, bool calculateStockPrice = true, bool isFromReturnProduct = false);
        public Task<(List<BillCustomerProduct>, List<BillCustomerProductBonus>)> AddProductForBillCustomer(Guid BillCustomerId, List<DTOs.BillCustomers.Params.BillCustomerProductDto> BillCustomerProducts);
        public decimal CacularAmountsTotalBillCustomer(List<BillCustomerProduct> billCustomerProducts);
        public decimal CacularAmountsAfterDiscountBillCustomer(List<BillCustomerProduct> billCustomerProducts, decimal totalPrice, MoneyModificationType? discountUnit, decimal? discountValue);
        public decimal DiscountPrecentEachProduct(decimal totalPrice, MoneyModificationType? discountUnit, decimal? discountValue);
    }
}
