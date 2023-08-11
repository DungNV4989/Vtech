using DocumentFormat.OpenXml.VariantTypes;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Application.Services;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.Users;
using VTECHERP.Constants;
using VTECHERP.Debts;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.OrderTransports;
using VTECHERP.DTOs.OrderTransports.Params;
using VTECHERP.Entities;
using VTECHERP.Enums;
using VTECHERP.Helper;
using VTECHERP.Services;

namespace VTECHERP
{
    [Route("api/app/order-transport/[action]")]
    public class OrderTransportApplication : ApplicationService
    {
        private readonly IOrderTransportService _orderTransportService;
        private readonly IRepository<OrderTransport> _orderTransportRepository;
        private readonly IRepository<Entry> _entryRepository;
        private readonly IRepository<EntryAccount> _entryAccountRepository;
        private readonly IRepository<OrderTransportSale> _orderTransportSaleRepository;
        private readonly IRepository<Debt> _debtRepository;
        private readonly IRepository<EntryLog> _entryLogRepository;
        private readonly IDebtAppService _debtAppService;
        private readonly IAttachmentService _attachmentService;

        private readonly IRepository<SaleOrders> _saleOrderRepository;
        private readonly IRepository<IdentityUser> _userRepository;
        public OrderTransportApplication(IOrderTransportService orderTransportService
            , IRepository<OrderTransport> orderTransportRepository
            , IRepository<OrderTransportSale> orderTransportSaleRepository
            , IRepository<Entry> entryRepository
            , IRepository<EntryAccount> entryAccountRepository
            , IDebtAppService debtAppService
            , IRepository<Debt> debtRepository
            , IAttachmentService attachmentService
            , IRepository<SaleOrders> saleOrderRepository
            , IRepository<IdentityUser> userRepository
            )
        {
            _orderTransportService = orderTransportService;
            _orderTransportRepository = orderTransportRepository;
            _orderTransportSaleRepository = orderTransportSaleRepository;
            _entryAccountRepository = entryAccountRepository;
            _debtAppService = debtAppService;
            _entryRepository = entryRepository;
            _debtRepository = debtRepository;
            _attachmentService = attachmentService;
            _saleOrderRepository = saleOrderRepository;
            _userRepository = userRepository;
        }

        [HttpPost]
        public async Task<IActionResult> Create(List<Guid> OrderSalesId)
        {
            var orderTransportId = await _orderTransportService.Create(OrderSalesId);
            return new OkObjectResult(new
            {
                message = "Tạo thành công",
                data = orderTransportId
            });
        }

        [HttpGet]
        public async Task<PagingResponse<OrderTransportItemList>> GetList(GetListOrderTransportParm param)
        {
            try
            {
                var result = await _orderTransportService.GetList(param);

                var count = result.Count();
                var respon = result.OrderByDescending(x => x.CreateTime).Skip((param.PageIndex - 1) * param.PageSize).Take(param.PageSize).ToList();
                var attachments = await _attachmentService.ListAttachmentByObjectIdAsync(respon.Select(x => x.Id).ToList());
                foreach(var item in respon)
                {
                    item.Attachments = attachments.Where(x=>x.ObjectId == item.Id).ToList() ?? new List<DTOs.Attachment.DetailAttachmentDto>();
                }
                return new PagingResponse<OrderTransportItemList>(count, respon);
            }
            catch (Exception ex)
            {
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, ex.StackTrace, ex.InnerException);
            }
        }

        [HttpPut]
        public async Task<IActionResult> Update(Guid OrderTransportId, UpdateOrderTransportParam param)
        {
            if (param == null)
            {
                return new GenericActionResult(400, true, $"Dữ liệu không hợp lệ");
            }
            var orderTransport = await _orderTransportRepository.FindAsync(x => x.Id == OrderTransportId);

            if (orderTransport == null)
                return new GenericActionResult(400, true, $"Không tìm thấy đơn vận chuyển có id là {OrderTransportId}");

            orderTransport.DateArrive = param.DateArrive;
            orderTransport.DateTransport = param.DateTransport;
            orderTransport.TransportCode = param.TransportCode;
            orderTransport.Status = param.Status.GetValueOrDefault();
            orderTransport.TransporterId = param.Transporter;

            var storeId = Guid.Empty;
            var saleOrders = (await _saleOrderRepository.GetQueryableAsync()).Where(x => param.SaleOrdersId.Any(id => id == x.Id));
            if (saleOrders.Any())
            {
                storeId = saleOrders.FirstOrDefault().StoreId;
                if(saleOrders.Select(x=>x.StoreId).Distinct().Count() > 1)
                {
                    var currentUser = (await _userRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == CurrentUser.GetId());
                    if (currentUser != null)
                        storeId = currentUser.GetProperty("MainStoreId",Guid.Empty);
                }    
            }

            // Tạo bút toán
            if (param.TotalPrice > 0 && orderTransport.TotalPrice <= 0)
            {
                var entry = new Entry()
                {
                    SourceId = OrderTransportId,
                    DocumentId = OrderTransportId,
                    Amount = param.TotalPrice.GetValueOrDefault(),
                    AudienceId = param.Transporter,
                    AudienceType = AudienceTypes.SupplierVN,
                    AccountingType = AccountingTypes.Auto,
                    DocumentType = DocumentTypes.ShippingNote,
                    StoreId = storeId,
                    TransactionDate = DateTime.UtcNow,
                    TicketType = TicketTypes.Other,
                    Currency = Currencies.VND,
                    IsActive = true,
                    Note = "",
                    SourceCode = param.TransportCode,
                    DocumentCode = param.TransportCode
                };

                await _entryRepository.InsertAsync(entry);

                var entryAccount = new EntryAccount()
                {
                    AmountVnd = param.TotalPrice,
                    CreditAccountCode = "331.2",
                    DebtAccountCode = "64112",
                    EntryId = entry.Id,
                    DocumentType = DocumentTypes.Other,
                    IsActive= true,
                    Note = ""
                };

                await _entryAccountRepository.InsertAsync(entryAccount);
                await AutoInsertDebt(entry, false);
            }

            // Cập nhật bút toán
            if (param.TotalPrice > 0 && orderTransport.TotalPrice > 0)
            {
                var entry = await _entryRepository.FindAsync(x => x.SourceId == OrderTransportId);
                if(entry != null)
                {
                    entry.Amount = param.TotalPrice.GetValueOrDefault();
                    entry.AudienceId = param.Transporter;

                    var entryAccount = await _entryAccountRepository.FindAsync(x => x.EntryId == entry.Id);
                    if (entryAccount != null)
                    {
                        entryAccount.AmountVnd = param.TotalPrice;
                    }

                    await AutoUpdateDebt(entry, false);
                }    
            }

            // Xóa bút toán
            if(param.TotalPrice <= 0 || !param.TotalPrice.HasValue)
            {
                var entries = await _entryRepository.GetListAsync(x => x.SourceId == OrderTransportId);
                if (entries != null)
                {
                    await AutoDeleteDebt(entries.Select(x => x.Id).ToArray());
                    await _entryRepository.DeleteManyAsync(entries);
                }
            }    

            orderTransport.TotalPrice = param.TotalPrice.GetValueOrDefault();

            var saleTransport = await _orderTransportSaleRepository.GetListAsync(x => x.OrderTransportId == OrderTransportId);
            await _orderTransportSaleRepository.DeleteManyAsync(saleTransport);

            var orderTransportSales = new List<OrderTransportSale>();

            foreach (var item in param.SaleOrdersId)
            {
                var orderTransportSale = new OrderTransportSale();
                orderTransportSale.OrderTransportId = OrderTransportId;
                orderTransportSale.OrderSaleId = item;

                orderTransportSales.Add(orderTransportSale);
            }

            await _orderTransportSaleRepository.InsertManyAsync(orderTransportSales);

            await CurrentUnitOfWork.SaveChangesAsync();
            return new GenericActionResult(200, true, $"Cập nhật thành công", orderTransport);
        }

        [HttpGet]
        public async Task<IActionResult> GetDetail(Guid OrderTransportId)
        {
            var orderTransport = (await _orderTransportService.GetOrderTransportFull()).FirstOrDefault(x => x.Id == OrderTransportId);

            var orderTransportSale = await _orderTransportSaleRepository.GetListAsync(x => x.OrderTransportId == orderTransport.Id);

            orderTransport.SaleOrders = orderTransport.SaleOrders.Where(x => !orderTransportSale.Select(y => y.Id).Contains(x.Id)).ToList();

            if (orderTransport == null)
                return new GenericActionResult(400, true, $"Không tìm thấy đơn vận chuyển có id là {OrderTransportId}");

            orderTransport.Attachments = await _attachmentService.GetAttachmentByObjectIdAsync(OrderTransportId);

            return new GenericActionResult(200, true, $"", orderTransport);
        }

        [HttpGet]
        public async Task<IActionResult> Export(GetListOrderTransportParm param)
        {
            var file = await _orderTransportService.ExportAsync(param);
            return new FileContentResult(file, ContentTypes.SPREADSHEET)
            {
                FileDownloadName = $"danhsachdonvanchuyen_{DateTime.Now:dd_MM_yyyy_hh_mm_ss}.xlsx"
            };
        }

        private async Task AutoInsertDebt(Entry entry, bool isDebt, Currencies currency = Currencies.VND)
        {
            if (entry.AudienceType == AudienceTypes.Other)
            {
                return;
            }
            var billProductDebt = new Debt
            {
                TransactionId = entry.SourceId ?? entry.Id,
                SupplierId = (entry.AudienceType == AudienceTypes.SupplierCN || entry.AudienceType == AudienceTypes.SupplierVN) ? entry.AudienceId : null,
                CustomerId = (entry.AudienceType == AudienceTypes.Customer) ? entry.AudienceId : null,
                EmployeeId = (entry.AudienceType == AudienceTypes.Employee) ? entry.AudienceId : null,
                IsAutoGenerated = true,
                IsActive = true,
                Debts = isDebt ? entry.Amount : 0,
                Credits = isDebt ? 0 : entry.Amount,
                TransactionDate = entry.TransactionDate,
                TicketType = entry.TicketType,
                AudienceType = entry.AudienceType,
                EntryId = entry.Id,
                DocumentType = entry.DocumentType,
                DocumentId = entry.DocumentId,
                Currency = currency
            };

            await _debtRepository.InsertAsync(billProductDebt);
        }

        /// <summary>
        /// Cập nhật công nợ khi tạo bút toán
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="isDebt"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        private async Task AutoUpdateDebt(Entry entry, bool isDebt)
        {
            try
            {
                var debt = await _debtRepository.GetAsync(p => p.EntryId == entry.Id);
                if (debt != null)
                {
                    debt.TransactionDate = entry.TransactionDate;
                    debt.Debts = isDebt ? entry.Amount : 0;
                    debt.Credits = isDebt ? 0 : entry.Amount;
                    if (entry.AudienceType == AudienceTypes.Other)
                    {
                        await _debtRepository.DeleteAsync(debt);
                        await _debtAppService.StatisticalMonthDebt(debt.TransactionDate);
                        return;
                    }

                    debt.SupplierId = (entry.AudienceType == AudienceTypes.SupplierCN || entry.AudienceType == AudienceTypes.SupplierVN) ? entry.AudienceId : null;
                    debt.CustomerId = (entry.AudienceType == AudienceTypes.Customer) ? entry.AudienceId : null;
                    debt.EmployeeId = (entry.AudienceType == AudienceTypes.Employee) ? entry.AudienceId : null;

                    await _debtRepository.UpdateAsync(debt);
                    await _debtAppService.StatisticalMonthDebt(debt.TransactionDate);
                }
            }
            catch (Exception ex)
            {
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }
        }

        private async Task AutoDeleteDebt(params Guid[] entryIds)
        {
            var lstdebt = await _debtRepository.GetListAsync(p => entryIds.Contains(p.EntryId ?? Guid.Empty));
            if (lstdebt.Any())
            {
                await _debtRepository.DeleteManyAsync(lstdebt);
                await _debtAppService.StatisticalMonthDebt(lstdebt.FirstOrDefault().TransactionDate);
            }
        }
    }
}
