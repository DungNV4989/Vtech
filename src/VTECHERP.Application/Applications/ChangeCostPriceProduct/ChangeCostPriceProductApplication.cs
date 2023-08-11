using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;
using VTECHERP.DTOs.ChangeCostPriceProduct.Param;
using VTECHERP.DTOs.HistoryChangeCostPriceProduct.Respon;
using VTECHERP.Entities;
using VTECHERP.Helper;

namespace VTECHERP.Applications.ChangeCostPriceProduct
{
    [Authorize]
    public class ChangeCostPriceProductApplication : ApplicationService
    {
        private readonly IRepository<Products> _productRepositry;
        private readonly IRepository<HistoryChangeCostPriceProduct> _historyChangeCostPriceRepository;
        private readonly IRepository<StoreProduct> _storeProductRepository;
        private readonly IRepository<Entry> _entryRepository;
        private readonly IRepository<EntryAccount> _entryAccountRepository;
        private readonly IIdentityUserRepository _userRepository;
        private readonly ICurrentUser _userManager;
        private readonly IRepository<Stores> _storeRepository;
        public ChangeCostPriceProductApplication(
            IRepository<Products> productRepositry
            , IRepository<HistoryChangeCostPriceProduct> historyChangeCostPriceRepository
            , IRepository<StoreProduct> storeProductRepository
            , IRepository<Entry> entryRepository
            , IRepository<EntryAccount> entryAccountRepository
            , IIdentityUserRepository userRepository
            , ICurrentUser userManager
            ,IRepository<Stores> storeRepository
            )
        {
            _productRepositry = productRepositry;
            _historyChangeCostPriceRepository = historyChangeCostPriceRepository;
            _storeProductRepository = storeProductRepository;
            _entryRepository = entryRepository;
            _entryAccountRepository = entryAccountRepository;
            _userRepository = userRepository;
            _userManager = userManager;
            _storeRepository = storeRepository;
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCostPriceProduct(ChangeCostPriceProductParam param)
        {
            var productOrigin = await _productRepositry.FirstOrDefaultAsync(x => x.Id == param.ProductId);
            if (productOrigin == null)
                return new GenericActionResult(400, false, $"Không tìm thấy sản phẩm có id là {param.ProductId}");

            var costPriceOld = productOrigin.StockPrice;
            productOrigin.StockPrice = param.PriceNew;

            var history = new HistoryChangeCostPriceProduct()
            {
                ProductId = param.ProductId,
                CostPriceOld = costPriceOld,
                CostPriceNew = param.PriceNew,
                Type = param.Type,
            };

            await _historyChangeCostPriceRepository.InsertAsync(history);

            if (param.Type == Enums.ChangeCostPriceProductType.Normal)
            {
                // Tính lợi nhuận giảm : (giá vốn cũ - giá vốn mới) * Tổng tồn của doanh nghiệp
                var inventory = await TotalInventoryProduct(productOrigin.Id);
                var profit = await CaculateProfitDecrease(costPriceOld, param.PriceNew, inventory);
                var storeId = (await _storeRepository.GetListAsync(x => x.IsMainStore == true)).Select(x => x.Id).FirstOrDefault();
                var entry = new Entry
                {
                    Note = "Gắn với cửa hàng chính",
                    SourceId = history.Id,
                    EntrySource = Enums.ActionSources.ChangeCostPriceProduct,
                    StoreId = storeId,
                    AudienceType = Enums.AudienceTypes.Other,
                    TransactionDate = DateTime.Now,
                    AccountingType = Enums.AccountingTypes.Auto,
                    Amount = Math.Abs(profit),
                    AudienceId = history.Id,
                    TicketType = Enums.TicketTypes.ChangeCostPriceProduct,
                    DocumentType = Enums.DocumentTypes.ChangeCostPriceProduct,
                    DocumentId = history.Id,
                };

                await _entryRepository.InsertAsync(entry);

                var entryAccount = new EntryAccount
                {
                    EntryId = entry.Id,
                    CreditAccountCode = "1561",
                    DocumentType = entry.DocumentType,
                    AmountVnd = Math.Abs(profit),
                };

                if (profit < 0)
                    entryAccount.DebtAccountCode = "6426";

                if (profit > 0)
                    entryAccount.DebtAccountCode = "2294";

                history.ProfitDecrease = profit;
                await _entryAccountRepository.InsertAsync(entryAccount);
            }

            await CurrentUnitOfWork.SaveChangesAsync();

            return new GenericActionResult(200, true, "Cập nhật giá thành công");
        }

        [HttpGet]
        public async Task<IActionResult> GetHistoryChangeCostPriceProduct(Guid ProductId)
        {
            var result = new List<HistoryChangeCostPriceProductRespon>();

            var histories = await _historyChangeCostPriceRepository.GetQueryableAsync();
            var users = await _userRepository.GetListAsync();

            result = (from history in histories

                      where history.ProductId == ProductId

                      select new HistoryChangeCostPriceProductRespon
                      {
                          CostPriceNew = history.CostPriceNew,
                          CreatorId = history.CreatorId,
                          CreatTime = history.CreationTime,
                          ProfitDecrease = history.ProfitDecrease,
                          Type= history.Type,
                      }
                      )
                      .OrderByDescending(x => x.CreatTime)
                    .ToList();

            foreach (var item in result)
            {
                if (item.Type == Enums.ChangeCostPriceProductType.Normal)
                    item.TypeText = "Cập nhật giá vốn";

                if (item.Type == Enums.ChangeCostPriceProductType.Quick)
                    item.TypeText = "Sửa giá vốn nhanh";

                if(item.CreatorId.HasValue)
                {
                    var user = await _userRepository.FindAsync(item.CreatorId.Value);
                    item.CreatorText = user == null ? "" : user.Name;
                }    
            }

            return new GenericActionResult(200, true, "", result);
        }

        [HttpGet]
        public async Task<IActionResult> GetInfoProduct(Guid ProductId)
        {
            var product = await _productRepositry.FirstOrDefaultAsync(x => x.Id == ProductId);
            if (product == null)
                return new GenericActionResult(400, false, $"Không tìm thấy sản phẩm có id là {ProductId}");

            // Tổng số lượng tồn
            var inventory = await TotalInventoryProduct(product.Id);
            var totalAmount = product.StockPrice * inventory;
            var costPriceOld = product.StockPrice;

            return new GenericActionResult(200, true, "", new
            {
                inventory = inventory,
                totalAmount = totalAmount,
                costPriceOld = costPriceOld
            });
        }

        private async Task<decimal> TotalInventoryProduct(Guid ProductId)
        {
            var storeProduct = await _storeProductRepository.GetListAsync(x => x.ProductId == ProductId);
            var inventory = storeProduct.Sum(x => x.StockQuantity);

            return inventory;
        }

        private async Task<decimal> CaculateProfitDecrease(decimal costPriceOld, decimal costPriceNew, decimal inventory)
        {
            var profit = (costPriceOld - costPriceNew) * inventory;

            return profit;
        }
    }
}
