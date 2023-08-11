using DocumentFormat.OpenXml.Spreadsheet;
using DocumentFormat.OpenXml.Vml.Office;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text.Json;
using System.Threading.Tasks;
using Vinpearl.Modelling.Library.Utility.Excel;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Timing;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VTECHERP.Constants;
using VTECHERP.Debts;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.Entries;
using VTECHERP.Entities;
using VTECHERP.Enums;
using VTECHERP.Extensions;
using VTECHERP.Models;
using static VTECHERP.Constants.EntryConfig;
using static VTECHERP.Constants.EntryConfig.PaymentReceipt;
using BillCustomer = VTECHERP.Entities.BillCustomer;
using Entry = VTECHERP.Entities.Entry;
using PaymentReceipt = VTECHERP.Entities.PaymentReceipt;

namespace VTECHERP.Services
{
    public class EntryService : IEntryService
    {
        private readonly IRepository<Entry> _entryRepository;
        private readonly IRepository<EntryAccount> _entryAccountRepository;
        private readonly IRepository<Account> _accountRepository;
        private readonly IRepository<SaleOrders> _saleOrderRepository;
        private readonly IRepository<SaleOrderLines> _saleOrderLineRepository;
        private readonly IRepository<Suppliers> _supplierRepository;
        private readonly IRepository<EntryLog> _entryLogRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IIdentityUserRepository _userRepository;
        private readonly IRepository<Debt> _debtRepository;
        private readonly IRepository<WarehousingBill> _warehousingBillRepository;
        private readonly IRepository<WarehousingBillProduct> _warehousingBillProductRepository;
        private readonly IClock _clock;
        private readonly ICommonService _commonService;
        private readonly IRepository<PaymentReceipt> _paymentReceiptRepository;
        private readonly IDebtAppService _debtAppService;
        private readonly IRepository<UserStore> _userStoreRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IObjectMapper _mapper;
        private readonly IRepository<SupplierOrderReport> _supplierOrderReportRepository;
        private readonly IRepository<BillCustomer> _billCustomerRepository;
        private readonly IRepository<CustomerReturn> _customerReturnRepository;
        private readonly IAttachmentService _attachmentService;
        private readonly IRepository<DayConfiguration> _dayConfigurationRepository;

        private readonly IStoreService _storeService;

        public EntryService(
            IRepository<Entry> entryRepository,
            IRepository<EntryAccount> entryAccountRepository,
            IRepository<Account> accountRepository,
            IRepository<SaleOrders> saleOrderRepository,
            IRepository<SaleOrderLines> saleOrderLineRepository,
            IRepository<Suppliers> supplierRepository,
            IRepository<EntryLog> entryLogRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IIdentityUserRepository userRepository,
            IRepository<Debt> debtRepository,
            IRepository<WarehousingBill> warehousingBillRepository,
            IRepository<WarehousingBillProduct> warehousingBillProductRepository,
            ICommonService commonService,
            IRepository<PaymentReceipt> paymentReceiptRepository,
            IClock clock,
            IDebtAppService debtAppService,
            IRepository<UserStore> userStoreRepository,
            ICurrentUser currentUser,
            IObjectMapper mapper,
            IRepository<SupplierOrderReport> supplierOrderReportRepository,
            IStoreService storeService,
            IRepository<BillCustomer> billCustomerRepository,
            IRepository<CustomerReturn> customerReturnRepository,
            IAttachmentService attachmentService
            , IRepository<DayConfiguration> dayConfigurationRepository
            )
        {
            _entryRepository = entryRepository;
            _entryAccountRepository = entryAccountRepository;
            _accountRepository = accountRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _saleOrderRepository = saleOrderRepository;
            _saleOrderLineRepository = saleOrderLineRepository;
            _supplierRepository = supplierRepository;
            _entryLogRepository = entryLogRepository;
            _userRepository = userRepository;
            _debtRepository = debtRepository;
            _warehousingBillRepository = warehousingBillRepository;
            _warehousingBillProductRepository = warehousingBillProductRepository;
            _clock = clock;
            _commonService = commonService;
            _paymentReceiptRepository = paymentReceiptRepository;
            _debtAppService = debtAppService;
            _userStoreRepository = userStoreRepository;
            _currentUser = currentUser;
            _mapper = mapper;

            _storeService = storeService;
            _supplierOrderReportRepository = supplierOrderReportRepository;
            _billCustomerRepository = billCustomerRepository;
            _customerReturnRepository = customerReturnRepository;
            _attachmentService = attachmentService;
            _dayConfigurationRepository = dayConfigurationRepository;
        }

        public async Task<PagingResponse<EntryDTO>> SearchEntry(SearchEntryRequest request)
        {
            try
            {
                var userStores = await _userStoreRepository.GetListAsync(p => p.UserId == _currentUser.Id);
                if (!userStores.Any())
                {
                    throw new BusinessException(ErrorMessages.UserStore.StoreNotDefine);
                }
                var userStoreIds = userStores.Select(p => p.StoreId).ToList();
                // get all queryable ref
                var entries = (await _entryRepository.GetQueryableAsync()).Where(o => !o.IsDeleted);
                var entryAccounts = (await _entryAccountRepository.GetQueryableAsync()).Where(o => !o.IsDeleted);
                var accounts = (await _accountRepository.GetQueryableAsync()).Where(o => !o.IsDeleted);
                var suppliers = (await _supplierRepository.GetQueryableAsync()).Where(o => !o.IsDeleted);
                var allUsers = await _userRepository.GetListAsync();

                // find entry with account code
                var entryWithAccount = new List<Guid>();
                if (!request.AccountCode.IsNullOrWhiteSpace())
                {
                    entryWithAccount = entryAccounts.Where(p => p.CreditAccountCode == request.AccountCode || p.DebtAccountCode == request.AccountCode).Select(p => p.EntryId).ToList();
                }

                // find entry with audience id/name
                var entryWithAudience = new List<Guid>();

                if (request.SearchDateFrom.HasValue)
                {
                    request.SearchDateFrom = _clock.Normalize(request.SearchDateFrom.Value);
                }
                if (request.SearchDateTo.HasValue)
                {
                    request.SearchDateTo = _clock.Normalize(request.SearchDateTo.Value).AddHours(23).AddMinutes(59).AddSeconds(59);
                }

                // find creator Ids
                var creatorIds = new List<Guid>();
                if (!request.Creator.IsNullOrWhiteSpace())
                {
                    creatorIds = allUsers
                        .Where(p =>
                        (
                            !p.Name.IsNullOrEmpty() && p.Name.Contains(request.Creator))
                            || (!p.UserName.IsNullOrEmpty() && p.UserName.Contains(request.Creator))
                            || (!p.Email.IsNullOrEmpty() && p.Email.Contains(request.Creator))
                        )
                        .Select(p => p.Id)
                        .ToList();
                }

                // build query
                var query =
                    from entry in entries
                    where
                        (
                            (request.EntryCode == null || entry.Code.Contains(request.EntryCode))
                            && (request.StoreIds == null || request.StoreIds.Count == 0 || request.StoreIds.Contains(entry.StoreId))
                            && (request.SearchDateType == null
                                || (
                                    request.SearchDateType == SearchDateType.TransactionDate
                                    && (request.SearchDateFrom == null || entry.TransactionDate >= request.SearchDateFrom)
                                    && (request.SearchDateTo == null || entry.TransactionDate <= request.SearchDateTo)
                                )
                                || (
                                    request.SearchDateType == SearchDateType.CreationDate
                                    && (request.SearchDateFrom == null || entry.CreationTime >= request.SearchDateFrom)
                                    && (request.SearchDateTo == null || entry.CreationTime <= request.SearchDateTo)
                                )
                            )
                            && (request.TicketType == null || entry.TicketType == request.TicketType)
                            && (request.DocumentCode.IsNullOrWhiteSpace() || (entry.DocumentCode != null && entry.DocumentCode.Contains(request.DocumentCode)))
                            && (request.AccountingType == null || entry.AccountingType == request.AccountingType)
                            && (
                                request.IsDocument == null
                                || (request.IsDocument == false && (entry.DocumentCode == null || entry.DocumentCode == String.Empty))
                                || (request.IsDocument == true && entry.DocumentCode != null && entry.DocumentCode != String.Empty)
                            )
                            && (request.AudienceType == null || entry.AudienceType == request.AudienceType)
                            && (request.AudienceId == null || entry.AudienceId == request.AudienceId)
                            && (request.AccountCode == null || request.AccountCode == "" || entryWithAccount.Contains(entry.Id))
                            && (request.Amount == null || entry.Amount == request.Amount)
                            && (request.DocumentDetailType == null || request.DocumentDetailType.Count == 0 || (entry.DocumentDetailType != null && request.DocumentDetailType.Contains(entry.DocumentDetailType.Value)))
                            && (request.Note.IsNullOrWhiteSpace() || (entry.Note != null && entry.Note.Contains(request.Note)))
                            && (request.Creator.IsNullOrWhiteSpace() || creatorIds.Contains(entry.CreatorId ?? Guid.Empty))
                            && (userStoreIds.Contains(entry.StoreId))
                        )
                    select new EntryDTO()
                    {
                        Id = entry.Id,
                        StoreId = entry.StoreId,
                        AccountingType = entry.AccountingType,
                        Amount = entry.Amount,
                        AudienceType = entry.AudienceType,
                        AudienceId = entry.AudienceId,
                        Code = entry.Code,
                        CreatorId = entry.CreatorId,
                        CreationTime = entry.CreationTime,
                        Currency = entry.Currency,
                        DocumentCode = entry.DocumentCode,
                        DocumentType = entry.DocumentType,
                        TicketType = entry.TicketType,
                        EntrySource = entry.EntrySource,
                        IsActive = entry.IsActive,
                        Note = entry.Note,
                        TransactionDate = _clock.Normalize(entry.TransactionDate),
                        //Attachments = entry.Attachments,
                        SourceId = entry.SourceId,
                        LastModifierId = entry.LastModifierId,
                        LastModificationTime = entry.LastModificationTime,
                        SourceCode = entry.SourceCode,
                    };
                var paged = query.OrderByDescending(p => p.Code).Skip(request.Offset).Take(request.PageSize).ToList();
                var entryIds = paged.Select(p => p.Id).ToList();
                var entryAccountDatas = entryAccounts.Where(p => entryIds.Contains(p.EntryId)).Select(p => new
                {
                    p.DebtAccountCode,
                    p.CreditAccountCode,
                    p.EntryId,
                    p.AmountCny,
                    p.AmountVnd
                }).ToList();
                // other info
                var audienceRequests = paged.Where(p => p.AudienceId != null).Select(p =>
                {
                    (AudienceTypes, Guid?) item = (p.AudienceType, p.AudienceId);
                    return item;
                }).Distinct().ToArray();
                var audiences = await _commonService.GetAudiences(audienceRequests);
                var attachments = await _attachmentService.ListAttachmentByObjectIdAsync(paged.Select(x => x.Id.Value).ToList());
                foreach (var item in paged)
                {
                    item.Accounts = entryAccountDatas
                        .Where(p => p.EntryId == item.Id)
                        .Select(p => new EntryAccountValue
                        {
                            DebtAccountCode = p.DebtAccountCode.IsNullOrEmpty() ? null : p.DebtAccountCode,
                            CreditAccountCode = p.CreditAccountCode.IsNullOrEmpty() ? null : p.CreditAccountCode,
                            AmountVnd = p.AmountVnd == 0 ? null : p.AmountVnd,
                            AmountCny = p.AmountCny == 0 ? null : p.AmountCny
                        })
                        .ToList();
                    item.Amount = item.Accounts.Sum(p => p.AmountVnd ?? 0);
                    var audience = audiences.FirstOrDefault(p => p.Id == (item.AudienceId ?? Guid.Empty));
                    if (audience != null)
                    {
                        item.AudienceCode = audience?.Code;
                        item.AudienceName = audience?.Name;
                        item.AudiencePhone = audience?.Phone;
                    }

                    if (item.CreatorId != null)
                    {
                        var creator = allUsers.FirstOrDefault(p => p.Id == item.CreatorId);
                        item.CreatorName = creator?.Name;
                    }

                    if (item.LastModifierId != null)
                    {
                        var modifier = allUsers.FirstOrDefault(p => p.Id == item.LastModifierId);
                        item.LastModifierName = modifier?.Name;
                    }

                    item.IsEditable = item.AccountingType == AccountingTypes.Manual;
                    item.IsDeletable = item.AccountingType == AccountingTypes.Manual;

                    item.DocumentTypeName = _commonService.GetDocumentTypeName(item.DocumentType);

                    item.Attachments = attachments.Where(x=>x.ObjectId == item.Id).ToList() ?? new List<DTOs.Attachment.DetailAttachmentDto>();
                }

                return new PagingResponse<EntryDTO>(query.Count(), paged);
            }
            catch (Exception ex)
            {
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, ex.StackTrace, ex.InnerException);
            }
        }

        public async Task AutoCreateEntryByCreateSupplierOrder(Guid orderId)
        {
            var uow = _unitOfWorkManager.Current;

            var saleOrder = await _saleOrderRepository.GetAsync(p => p.Id == orderId);
            var saleOrderLines = await _saleOrderLineRepository.GetListAsync(p => p.OrderId == orderId);
            if (saleOrder != null)
            {
                var supplier = await _supplierRepository.GetAsync(p => p.Id == saleOrder.SupplierId);
                var saleAmount = saleOrderLines.Sum(p => p.RequestQuantity * p.RequestPrice);
                var vnSaleAmount = saleAmount;
                if (supplier.Origin == SupplierOrigin.CN)
                {
                    // nếu ncc trung quốc, add thêm bút toán tiền trung
                    var cnEntry = new Entry
                    {
                        EntrySource = ActionSources.OrderCreate,
                        SourceId = saleOrder.Id,
                        SourceCode = saleOrder.Code,
                        StoreId = saleOrder.StoreId,
                        AudienceType = AudienceTypes.SupplierCN,
                        AudienceId = supplier.Id,
                        IsActive = true,
                        Amount = saleAmount ?? 0,
                        TransactionDate = _clock.Normalize(saleOrder.OrderDate),
                        Currency = Currencies.CNY,
                        AccountingType = AccountingTypes.Auto,
                        TicketType = TicketTypes.Import,
                        DocumentCode = saleOrder.Code,
                        DocumentId = saleOrder.Id,
                        DocumentType = DocumentTypes.SupplierOrder,
                        ConfigCode = EntryConfig.OrderSupplier.CreateSupplierCN.Code
                    };
                    await _entryRepository.InsertAsync(cnEntry);
                    await uow.SaveChangesAsync();
                    // lưu vào TK có
                    var cnEntryAccount = new EntryAccount
                    {
                        EntryId = cnEntry.Id,
                        AmountCny = saleAmount ?? 0,
                        DebtAccountCode = null,
                        CreditAccountCode = EntryConfig.OrderSupplier.CreateSupplierCN.Credit
                    };
                    await _entryAccountRepository.InsertAsync(cnEntryAccount);
                    await uow.SaveChangesAsync();

                    // tính lại tổng giá VN theo tỷ giá trong đơn hàng
                    vnSaleAmount = saleAmount * (saleOrder.Rate ?? 1);

                    await AutoInsertDebt(cnEntry, false, Currencies.CNY);
                }

                var vnEntry = new Entry
                {
                    EntrySource = ActionSources.OrderCreate,
                    SourceId = saleOrder.Id,
                    SourceCode = saleOrder.Code,
                    StoreId = saleOrder.StoreId,
                    AudienceType = supplier.Origin == SupplierOrigin.CN ? AudienceTypes.SupplierCN : AudienceTypes.SupplierVN,
                    AudienceId = supplier.Id,
                    IsActive = true,
                    Amount = vnSaleAmount ?? 0,
                    TransactionDate = _clock.Normalize(saleOrder.OrderDate),
                    Currency = Currencies.VND,
                    AccountingType = AccountingTypes.Auto,
                    TicketType = TicketTypes.Import,
                    DocumentCode = saleOrder.Code,
                    DocumentId = saleOrder.Id,
                    DocumentType = DocumentTypes.SupplierOrder,
                    ConfigCode = supplier.Origin == SupplierOrigin.CN ? EntryConfig.OrderSupplier.CreateSupplierCN.Code : EntryConfig.OrderSupplier.CreateSupplierVN.Code
                };
                await _entryRepository.InsertAsync(vnEntry);
                await uow.SaveChangesAsync();

                // Lưu vào TK nợ và TK có
                var vnEntryAccount = new EntryAccount
                {
                    EntryId = vnEntry.Id,
                    AmountVnd = vnSaleAmount ?? 0,
                    DebtAccountCode = supplier.Origin == SupplierOrigin.CN ? EntryConfig.OrderSupplier.CreateSupplierCN.Debt : EntryConfig.OrderSupplier.CreateSupplierVN.Debt,
                    CreditAccountCode = supplier.Origin == SupplierOrigin.CN ? null : EntryConfig.OrderSupplier.CreateSupplierVN.Credit
                };
                await _entryAccountRepository.InsertAsync(vnEntryAccount);
                if (supplier.Origin != SupplierOrigin.CN)
                {
                    await AutoInsertDebt(vnEntry, false, Currencies.VND);
                }
                await uow.SaveChangesAsync();
            }
        }

        public async Task AutoUpdateEntryByUpdateSupplierOrder(Guid orderId)
        {
            var uow = _unitOfWorkManager.Current;

            var saleOrder = await _saleOrderRepository.GetAsync(p => p.Id == orderId);

            if (saleOrder != null)
            {
                var saleOrderLines = await _saleOrderLineRepository.GetListAsync(p => p.OrderId == orderId);
                var supplier = await _supplierRepository.GetAsync(p => p.Id == saleOrder.SupplierId);
                var saleAmount = saleOrderLines.Sum(p => p.RequestQuantity * p.RequestPrice);
                var vnSaleAmount = saleAmount;

                var entries = await _entryRepository.GetListAsync(p => p.SourceId == saleOrder.Id);
                var entryIds = entries.Select(p => p.Id).ToList();

                var entryAccounts = await _entryAccountRepository.GetListAsync(p => entryIds.Contains(p.EntryId));
                if (supplier.Origin == SupplierOrigin.CN)
                {
                    vnSaleAmount = (saleAmount * saleOrder.Rate) ?? 0;
                }
                List<EntryAccount> newEntryAccounts = new List<EntryAccount>();
                foreach (var entry in entries)
                {
                    var oldAccounts = await _entryAccountRepository.GetListAsync(p => entryIds.Contains(p.EntryId));
                    var oldEntry = entry.Clone();
                    entry.TransactionDate = saleOrder.OrderDate;
                    var entryAccount = entryAccounts.FirstOrDefault(p => p.EntryId == entry.Id);
                    // bút toán NCC TQ - tiền TQ
                    if (entry.ConfigCode == EntryConfig.OrderSupplier.CreateSupplierCN.Code && entry.Currency == Currencies.CNY)
                    {
                        entry.Amount = saleAmount ?? 0;
                        entryAccount.AmountCny = saleAmount ?? 0;
                        AutoUpdateDebt(entry, false);
                    }
                    // bút toán NCC TQ - tiền VN / bút toán NCC VN
                    else
                    {
                        entry.Amount = vnSaleAmount ?? 0;
                        entryAccount.AmountVnd = vnSaleAmount ?? 0;
                    }
                    newEntryAccounts.Add(entryAccount);
                    await _entryRepository.UpdateAsync(entry);
                    await _entryAccountRepository.UpdateAsync(entryAccount);
                    await CreateEntryLog(oldEntry, oldAccounts, entry, newEntryAccounts, false);
                }
            }

            await uow.SaveChangesAsync();
        }

        public async Task AutoCreateEntryByConfirmSupplierOrder(Guid orderId, Guid warehousingBillId, decimal? saleAmount, bool confirm = false)
        {
            var uow = _unitOfWorkManager.Current;
            var saleOrder = await _saleOrderRepository.GetAsync(p => p.Id == orderId);
            // var saleOrderLines = await _saleOrderLineRepository.GetListAsync(p => p.OrderId == orderId);
            var bill = await _warehousingBillRepository.GetAsync(p => p.Id == warehousingBillId);

            if (saleOrder != null)
            {
                // var saleAmount = saleOrderLines.Sum(p => p.RequestQuantity * p.RequestPrice);
                var vnSaleAmount = (saleAmount ?? 0);

                var vnEntry = new Entry
                {
                    EntrySource = ActionSources.OrderConfirmation,
                    SourceId = saleOrder.Id,
                    SourceCode = saleOrder.Code,
                    StoreId = saleOrder.StoreId,
                    AudienceType = bill.AudienceType,
                    AudienceId = saleOrder.SupplierId,
                    IsActive = true,
                    Amount = vnSaleAmount,
                    TransactionDate = DateTime.UtcNow,
                    Currency = Currencies.VND,
                    AccountingType = AccountingTypes.Auto,
                    TicketType = TicketTypes.Import,
                    DocumentCode = bill.Code,
                    DocumentType = confirm == false ? DocumentTypes.SupplierOrder : DocumentTypes.InventoryImport,
                    DocumentDetailType = bill.DocumentDetailType,
                    ConfigCode = EntryConfig.OrderSupplier.ConfirmSupplier.Code
                };
                await _entryRepository.InsertAsync(vnEntry);
                await uow.SaveChangesAsync();
                // Lưu vào TK nợ và TK có
                var vnEntryAccount = new EntryAccount
                {
                    EntryId = vnEntry.Id,
                    AmountVnd = vnSaleAmount,
                    DebtAccountCode = EntryConfig.OrderSupplier.ConfirmSupplier.Debt,
                    CreditAccountCode = EntryConfig.OrderSupplier.ConfirmSupplier.Credit
                };
                await _entryAccountRepository.InsertAsync(vnEntryAccount);
                await uow.SaveChangesAsync();
            }
        }

        public async Task AutoCreateEntryByConfirmOrderSurplus(Guid orderId, Guid warehousingBillId, decimal? saleAmount)
        {
            var uow = _unitOfWorkManager.Current;
            var saleOrder = await _saleOrderRepository.GetAsync(p => p.Id == orderId);
            // var saleOrderLines = await _saleOrderLineRepository.GetListAsync(p => p.OrderId == orderId);
            var bill = await _warehousingBillRepository.GetAsync(p => p.Id == warehousingBillId);

            if (saleOrder != null)
            {
                var vnSaleAmount = (saleAmount ?? 0);

                if (vnSaleAmount > 0)
                {
                    var surplusEntry = new Entry
                    {
                        EntrySource = ActionSources.OrderConfirmation,
                        SourceId = saleOrder.Id,
                        SourceCode = saleOrder.Code,
                        StoreId = saleOrder.StoreId,
                        AudienceType = bill.AudienceType,
                        AudienceId = saleOrder.SupplierId,
                        IsActive = true,
                        Amount = vnSaleAmount,
                        TransactionDate = DateTime.UtcNow,
                        Currency = Currencies.VND,
                        AccountingType = AccountingTypes.Auto,
                        TicketType = TicketTypes.Import,
                        DocumentCode = bill.Code,
                        DocumentType = DocumentTypes.InventoryImport,
                        DocumentDetailType = bill.DocumentDetailType,
                        ConfigCode = EntryConfig.OrderSupplier.ConfirmSupplierSurplus.Code
                    };
                    await _entryRepository.InsertAsync(surplusEntry);
                    await uow.SaveChangesAsync();
                    // Lưu vào TK nợ và TK có
                    var surplusEntryAccount = new EntryAccount
                    {
                        EntryId = surplusEntry.Id,
                        AmountVnd = vnSaleAmount,
                        DebtAccountCode = EntryConfig.OrderSupplier.ConfirmSupplierSurplus.Debt,
                        CreditAccountCode = EntryConfig.OrderSupplier.ConfirmSupplierSurplus.Credit
                    };
                    await _entryAccountRepository.InsertAsync(surplusEntryAccount);
                    await uow.SaveChangesAsync();
                }
            }
        }

        public async Task AutoCreateEntryByCompleteOrder(Guid orderId)
        {
            var uow = _unitOfWorkManager.Current;
            var saleOrder = await _saleOrderRepository.GetAsync(p => p.Id == orderId);
            var saleOrderLines = await _saleOrderLineRepository.GetListAsync(p => p.OrderId == orderId);

            if (saleOrder != null)
            {
                // tạo bút toán nếu hoàn thành thiếu
                decimal totalShortAmount = 0;
                foreach (var orderLine in saleOrderLines)
                {
                    if (orderLine.ImportQuantity < orderLine.RequestQuantity)
                    {
                        var surplusQuantity = orderLine.RequestQuantity - orderLine.ImportQuantity;
                        var shortAmount = (surplusQuantity * orderLine.RequestPrice) ?? 0;
                        shortAmount *= (saleOrder.Rate ?? 1);
                        totalShortAmount += shortAmount;
                    }
                }

                if (totalShortAmount > 0)
                {
                    var shortEntry = new Entry
                    {
                        EntrySource = ActionSources.OrderCompleted,
                        SourceId = saleOrder.Id,
                        SourceCode = saleOrder.Code,
                        StoreId = saleOrder.StoreId,
                        AudienceType = AudienceTypes.SupplierCN,
                        AudienceId = saleOrder.SupplierId,
                        IsActive = true,
                        Amount = totalShortAmount,
                        TransactionDate = DateTime.UtcNow,
                        Currency = Currencies.VND,
                        AccountingType = AccountingTypes.Auto,
                        TicketType = TicketTypes.Import,
                        DocumentCode = saleOrder.Code,
                        DocumentType = DocumentTypes.SupplierOrder,
                        ConfigCode = EntryConfig.OrderSupplier.CompleteSupplierLack.Code
                    };
                    await _entryRepository.InsertAsync(shortEntry);
                    await uow.SaveChangesAsync();
                    // Lưu vào TK nợ và TK có
                    var surplusEntryAccount = new EntryAccount
                    {
                        EntryId = shortEntry.Id,
                        AmountVnd = totalShortAmount,
                        DebtAccountCode = EntryConfig.OrderSupplier.CompleteSupplierLack.Debt,
                        CreditAccountCode = EntryConfig.OrderSupplier.CompleteSupplierLack.Credit
                    };
                    await _entryAccountRepository.InsertAsync(surplusEntryAccount);
                    await uow.SaveChangesAsync();
                }
            }
        }

        public async Task AutoCreateEntryByCreateWarehousingBill(Guid billId, bool autoInsertDebt = true)
        {
            var uow = _unitOfWorkManager.Current;
            var bill = await _warehousingBillRepository.GetAsync(p => p.Id == billId);

            if (bill.IsFromWarehouseTransfer == true)
            {
                await Task.CompletedTask;
                return;
            }

            var billProducts = await _warehousingBillProductRepository.GetListAsync(p => p.WarehousingBillId == billId);
            ActionSources entrySource = ActionSources.WarehousingBillCreate;

            var audience = await _commonService.GetAudience(bill.AudienceType, bill.AudienceId);

            if (bill.IsFromOrderConfirmation == true)
            {
                entrySource = ActionSources.OrderConfirmation;
            }

            switch (bill.BillType)
            {
                case WarehousingBillType.Import:
                    if (bill.AudienceType == AudienceTypes.SupplierVN || bill.AudienceType == AudienceTypes.SupplierCN)
                    {
                        // bút toán tổng tiền SP sau chiết khấu
                        var billProductEntry = new Entry
                        {
                            EntrySource = entrySource,
                            SourceId = bill.Id,
                            SourceCode = bill.Code,
                            StoreId = bill.StoreId,
                            AudienceType = bill.AudienceType,
                            AudienceId = bill.AudienceId,
                            IsActive = true,
                            Amount = bill.TotalPriceBeforeTax,
                            TransactionDate = _clock.Normalize(bill.CreationTime),
                            Currency = Currencies.VND,
                            AccountingType = AccountingTypes.Auto,
                            TicketType = TicketTypes.Import,
                            DocumentCode = bill.Code,
                            DocumentId = bill.Id,
                            DocumentType = DocumentTypes.InventoryImport,
                            ConfigCode = EntryConfig.WarehousingImport.Supplier.Product.Code
                        };
                        await _entryRepository.InsertAsync(billProductEntry);
                        await uow.SaveChangesAsync();

                        var billProductEntryAccount = new EntryAccount
                        {
                            EntryId = billProductEntry.Id,
                            AmountVnd = billProductEntry.Amount,
                            DebtAccountCode = EntryConfig.WarehousingImport.Supplier.Product.Debt,
                            CreditAccountCode = EntryConfig.WarehousingImport.Supplier.Product.Credit
                        };
                        await _entryAccountRepository.InsertAsync(billProductEntryAccount);
                        if (bill.IsFromOrderConfirmation != true)
                        {
                            await AutoInsertDebt(billProductEntry, false, Currencies.VND);
                        }

                        // bút toán VAT
                        if (bill.VATType != null
                            && bill.VATAmount.HasValue
                            && bill.VATAmount.Value > 0)
                        {
                            var vatEntry = new Entry
                            {
                                EntrySource = entrySource,
                                SourceId = bill.Id,
                                SourceCode = bill.Code,
                                StoreId = bill.StoreId,
                                AudienceType = bill.AudienceType,
                                AudienceId = bill.AudienceId,
                                IsActive = true,
                                Amount = (bill.VATType == MoneyModificationType.Percent ? bill.TotalPriceBeforeTax * bill.VATAmount / 100 : bill.VATAmount) ?? 0,
                                TransactionDate = _clock.Normalize(bill.VATHaveValueDate.Value),
                                Currency = Currencies.VND,
                                AccountingType = AccountingTypes.Auto,
                                TicketType = TicketTypes.Import,
                                DocumentCode = bill.Code,
                                DocumentId = bill.Id,
                                DocumentType = DocumentTypes.InventoryImport,
                                ConfigCode = EntryConfig.WarehousingImport.Supplier.VAT.Code
                            };
                            await _entryRepository.InsertAsync(vatEntry);
                            await uow.SaveChangesAsync();
                            var vatEntryAccount = new EntryAccount
                            {
                                EntryId = vatEntry.Id,
                                AmountVnd = vatEntry.Amount,
                                DebtAccountCode = EntryConfig.WarehousingImport.Supplier.VAT.Debt,
                                CreditAccountCode = EntryConfig.WarehousingImport.Supplier.VAT.Credit
                            };
                            await _entryAccountRepository.InsertAsync(vatEntryAccount);

                            await AutoInsertDebt(vatEntry, false, Currencies.VND);
                        }

                        await uow.SaveChangesAsync();
                    }
                    else if (bill.AudienceType == AudienceTypes.Customer)
                    {
                        // bút toán tổng tiền SP sau chiết khấu
                        var documentType = DocumentTypes.InventoryImport;
                        var documentId = bill.Id;
                        var documentCode = bill.Code;
                        var note = "";
                        if (bill.IsFromCustomerReturn.GetValueOrDefault())
                        {
                            documentType = DocumentTypes.ReturnProduct;
                            documentId = bill.SourceId.GetValueOrDefault();
                            var billReturn = await _customerReturnRepository.FindAsync(x => x.Id == documentId);
                            documentCode = billReturn == null ? documentCode : billReturn.Code;
                            note = bill.Note;
                        }

                        var billProductEntry = new Entry
                        {
                            EntrySource = entrySource,
                            SourceId = bill.Id,
                            SourceCode = bill.Code,
                            StoreId = bill.StoreId,
                            AudienceType = bill.AudienceType,
                            AudienceId = bill.AudienceId,
                            IsActive = true,
                            Amount = bill.TotalPriceBeforeTax,
                            TransactionDate = _clock.Normalize(bill.CreationTime),
                            Currency = Currencies.VND,
                            AccountingType = AccountingTypes.Auto,
                            TicketType = TicketTypes.Import,
                            DocumentCode = documentCode,
                            DocumentId = documentId,
                            DocumentType = documentType,
                            ConfigCode = EntryConfig.WarehousingImport.Customer.Product.Code,
                            Note = note
                        };
                        await _entryRepository.InsertAsync(billProductEntry);
                        await uow.SaveChangesAsync();

                        var creditAccount = EntryConfig.WarehousingImport.Customer.Product.Credit;
                        if ((bill.IsFromBillCustomer.HasValue && bill.IsFromBillCustomer.Value) || (bill.IsFromCustomerReturn.HasValue && bill.IsFromCustomerReturn.Value))
                        {
                            creditAccount = "632";
                        }

                        var billProductEntryAccount = new EntryAccount
                        {
                            EntryId = billProductEntry.Id,
                            AmountVnd = billProductEntry.Amount,
                            DebtAccountCode = EntryConfig.WarehousingImport.Customer.Product.Debt,
                            CreditAccountCode = creditAccount
                        };
                        await _entryAccountRepository.InsertAsync(billProductEntryAccount);

                        if (bill.IsFromOrderConfirmation != true && (!bill.IsFromBillCustomer.HasValue || !bill.IsFromBillCustomer.Value) && autoInsertDebt)
                        {
                            await AutoInsertDebt(billProductEntry, false, Currencies.VND);
                        }

                        // bút toán VAT
                        if (bill.VATType != null
                            && bill.VATAmount.HasValue
                            && bill.VATAmount.Value > 0)
                        {
                            var vatEntry = new Entry
                            {
                                EntrySource = entrySource,
                                SourceId = bill.Id,
                                SourceCode = bill.Code,
                                StoreId = bill.StoreId,
                                AudienceType = bill.AudienceType,
                                AudienceId = bill.AudienceId,
                                IsActive = true,
                                Amount = (bill.VATType == MoneyModificationType.Percent ? bill.TotalPriceBeforeTax * bill.VATAmount / 100 : bill.VATAmount) ?? 0,
                                TransactionDate = _clock.Normalize(bill.VATHaveValueDate.Value),
                                Currency = Currencies.VND,
                                AccountingType = AccountingTypes.Auto,
                                TicketType = TicketTypes.Import,
                                DocumentCode = bill.Code,
                                DocumentId = bill.Id,
                                DocumentType = DocumentTypes.InventoryImport,
                                ConfigCode = EntryConfig.WarehousingImport.Customer.VAT.Code
                            };
                            await _entryRepository.InsertAsync(vatEntry);
                            await uow.SaveChangesAsync();
                            var vatEntryAccount = new EntryAccount
                            {
                                EntryId = vatEntry.Id,
                                AmountVnd = vatEntry.Amount,
                                DebtAccountCode = EntryConfig.WarehousingImport.Customer.VAT.Debt,
                                CreditAccountCode = EntryConfig.WarehousingImport.Customer.VAT.Credit
                            };
                            await _entryAccountRepository.InsertAsync(vatEntryAccount);

                            await AutoInsertDebt(vatEntry, false, Currencies.VND);
                        }
                        await uow.SaveChangesAsync();
                    }
                    else if (bill.AudienceType == AudienceTypes.Other)
                    {
                        // bút toán tổng tiền SP sau chiết khấu
                        var billProductEntry = new Entry
                        {
                            EntrySource = entrySource,
                            SourceId = bill.Id,
                            SourceCode = bill.Code,
                            StoreId = bill.StoreId,
                            AudienceType = bill.AudienceType,
                            AudienceId = bill.AudienceId,
                            IsActive = true,
                            Amount = bill.TotalPriceBeforeTax,
                            TransactionDate = _clock.Normalize(bill.CreationTime),
                            Currency = Currencies.VND,
                            AccountingType = AccountingTypes.Auto,
                            TicketType = TicketTypes.Import,
                            DocumentCode = bill.Code,
                            DocumentId = bill.Id,
                            DocumentType = DocumentTypes.InventoryImport,
                            ConfigCode = EntryConfig.WarehousingImport.Other.Code
                        };
                        await _entryRepository.InsertAsync(billProductEntry);
                        await uow.SaveChangesAsync();

                        var billProductEntryAccount = new EntryAccount
                        {
                            EntryId = billProductEntry.Id,
                            AmountVnd = billProductEntry.Amount,
                            DebtAccountCode = EntryConfig.WarehousingImport.Other.Debt,
                            CreditAccountCode = EntryConfig.WarehousingImport.Other.Credit
                        };
                        await _entryAccountRepository.InsertAsync(billProductEntryAccount);

                        if (bill.IsFromOrderConfirmation != true)
                        {
                            await AutoInsertDebt(billProductEntry, false, Currencies.VND);
                        }

                        await uow.SaveChangesAsync();
                    }
                    break;

                case WarehousingBillType.Export:
                    if (bill.AudienceType == AudienceTypes.SupplierCN)
                    {
                        // bút toán tổng tiền SP sau chiết khấu
                        var billProductEntry = new Entry
                        {
                            EntrySource = entrySource,
                            SourceId = bill.Id,
                            SourceCode = bill.Code,
                            StoreId = bill.StoreId,
                            AudienceType = bill.AudienceType,
                            AudienceId = bill.AudienceId,
                            IsActive = true,
                            Amount = bill.TotalPriceBeforeTax,
                            TransactionDate = _clock.Normalize(bill.CreationTime),
                            Currency = Currencies.VND,
                            AccountingType = AccountingTypes.Auto,
                            TicketType = TicketTypes.Export,
                            DocumentCode = bill.Code,
                            DocumentId = bill.Id,
                            DocumentType = DocumentTypes.InventoryExport,
                            ConfigCode = EntryConfig.WarehousingExport.SupplierCN.Product.Code
                        };
                        await _entryRepository.InsertAsync(billProductEntry);
                        await uow.SaveChangesAsync();

                        var billProductEntryAccount = new EntryAccount
                        {
                            EntryId = billProductEntry.Id,
                            AmountVnd = billProductEntry.Amount,
                            DebtAccountCode = EntryConfig.WarehousingExport.SupplierCN.Product.Debt,
                            CreditAccountCode = EntryConfig.WarehousingExport.SupplierCN.Product.Credit
                        };
                        await _entryAccountRepository.InsertAsync(billProductEntryAccount);

                        // MinhNH comment code: Không tính công nợ khi xuất trả hàng nhà cung cấp Trung Quốc
                        //if (bill.IsFromOrderConfirmation != true)
                        //{
                        //    await AutoInsertDebt(billProductEntry, true, Currencies.VND);
                        //}

                        await uow.SaveChangesAsync();
                    }
                    else if (bill.AudienceType == AudienceTypes.SupplierVN)
                    {
                        // bút toán tổng tiền SP sau chiết khấu
                        var billProductEntry = new Entry
                        {
                            EntrySource = entrySource,
                            SourceId = bill.Id,
                            SourceCode = bill.Code,
                            StoreId = bill.StoreId,
                            AudienceType = bill.AudienceType,
                            AudienceId = bill.AudienceId,
                            IsActive = true,
                            Amount = bill.TotalPriceBeforeTax,
                            TransactionDate = _clock.Normalize(bill.CreationTime),
                            Currency = Currencies.VND,
                            AccountingType = AccountingTypes.Auto,
                            TicketType = TicketTypes.Export,
                            DocumentCode = bill.Code,
                            DocumentId = bill.Id,
                            DocumentType = DocumentTypes.InventoryExport,
                            ConfigCode = EntryConfig.WarehousingExport.SupplierVN.Product.Code
                        };
                        await _entryRepository.InsertAsync(billProductEntry);
                        await uow.SaveChangesAsync();

                        var billProductEntryAccount = new EntryAccount
                        {
                            EntryId = billProductEntry.Id,
                            AmountVnd = billProductEntry.Amount,
                            DebtAccountCode = EntryConfig.WarehousingExport.SupplierVN.Product.Debt,
                            CreditAccountCode = EntryConfig.WarehousingExport.SupplierVN.Product.Credit
                        };
                        await _entryAccountRepository.InsertAsync(billProductEntryAccount);

                        if (bill.IsFromOrderConfirmation != true)
                        {
                            await AutoInsertDebt(billProductEntry, true, Currencies.VND);
                        }
                        // bút toán VAT
                        if (bill.VATType != null
                            && bill.VATAmount.HasValue
                            && bill.VATAmount.Value > 0)
                        {
                            var vatEntry = new Entry
                            {
                                EntrySource = entrySource,
                                SourceId = bill.Id,
                                SourceCode = bill.Code,
                                StoreId = bill.StoreId,
                                AudienceType = bill.AudienceType,
                                AudienceId = bill.AudienceId,
                                IsActive = true,
                                Amount = (bill.VATType == MoneyModificationType.Percent ? bill.TotalPriceBeforeTax * bill.VATAmount / 100 : bill.VATAmount) ?? 0,
                                TransactionDate = _clock.Normalize(bill.VATHaveValueDate.Value),
                                Currency = Currencies.VND,
                                AccountingType = AccountingTypes.Auto,
                                TicketType = TicketTypes.Export,
                                DocumentCode = bill.Code,
                                DocumentId = bill.Id,
                                DocumentType = DocumentTypes.InventoryExport,
                                ConfigCode = EntryConfig.WarehousingExport.SupplierVN.VAT.Code
                            };
                            await _entryRepository.InsertAsync(vatEntry);
                            await uow.SaveChangesAsync();
                            var vatEntryAccount = new EntryAccount
                            {
                                EntryId = vatEntry.Id,
                                AmountVnd = vatEntry.Amount,
                                DebtAccountCode = EntryConfig.WarehousingExport.SupplierVN.VAT.Debt,
                                CreditAccountCode = EntryConfig.WarehousingExport.SupplierVN.VAT.Credit
                            };
                            await _entryAccountRepository.InsertAsync(vatEntryAccount);
                            await AutoInsertDebt(vatEntry, true, Currencies.VND);
                        }

                        await uow.SaveChangesAsync();
                    }
                    else if (bill.AudienceType == AudienceTypes.Other)
                    {
                        // bút toán tổng giá vốn các sản phẩm
                        var billProductEntry = new Entry
                        {
                            EntrySource = entrySource,
                            SourceId = bill.Id,
                            SourceCode = bill.Code,
                            StoreId = bill.StoreId,
                            AudienceType = bill.AudienceType,
                            AudienceId = bill.AudienceId,
                            IsActive = true,
                            Amount = bill.TotalStockPrice,
                            TransactionDate = _clock.Normalize(bill.CreationTime),
                            Currency = Currencies.VND,
                            AccountingType = AccountingTypes.Auto,
                            TicketType = TicketTypes.Export,
                            DocumentCode = bill.Code,
                            DocumentType = DocumentTypes.InventoryExport,
                            ConfigCode = EntryConfig.WarehousingExport.Other.Code
                        };
                        await _entryRepository.InsertAsync(billProductEntry);
                        await uow.SaveChangesAsync();

                        var billProductEntryAccount = new EntryAccount
                        {
                            EntryId = billProductEntry.Id,
                            AmountVnd = billProductEntry.Amount,
                            DebtAccountCode = EntryConfig.WarehousingExport.Other.Debt,
                            CreditAccountCode = EntryConfig.WarehousingExport.Other.Credit
                        };
                        await _entryAccountRepository.InsertAsync(billProductEntryAccount);

                        if (bill.IsFromOrderConfirmation != true)
                        {
                            await AutoInsertDebt(billProductEntry, true, Currencies.VND);
                        }

                        await uow.SaveChangesAsync();
                    }
                    else if (bill.AudienceType == AudienceTypes.Customer)
                    {
                        // bút toán tổng tiền SP sau chiết khấu
                        var documentType = DocumentTypes.InventoryImport;
                        var documentId = bill.Id;
                        var documentCode = bill.Code;
                        var note = "";

                        if (bill.IsFromCustomerReturn.GetValueOrDefault())
                        {
                            documentType = DocumentTypes.ReturnProduct;
                            documentId = bill.SourceId.GetValueOrDefault();
                            var billReturn = await _customerReturnRepository.FindAsync(x => x.BillCustomerId == documentId);
                            documentCode = billReturn == null ? documentCode : billReturn.Code;
                            note = bill.Note;
                        }
                        else if (bill.IsFromBillCustomer.GetValueOrDefault())
                        {
                            documentType = DocumentTypes.BillCustomer;
                            documentId = bill.SourceId.GetValueOrDefault();
                            var billCustomer = await _billCustomerRepository.FindAsync(x => x.Id == documentId);
                            documentCode = billCustomer == null ? documentCode : billCustomer.Code;
                            note = bill.Note;
                        }

                        var billProductEntry = new Entry
                        {
                            EntrySource = entrySource,
                            SourceId = bill.Id,
                            SourceCode = bill.Code,
                            StoreId = bill.StoreId,
                            AudienceType = bill.AudienceType,
                            AudienceId = bill.AudienceId,
                            IsActive = true,
                            Amount = bill.TotalStockPrice,
                            TransactionDate = _clock.Normalize(bill.CreationTime),
                            Currency = Currencies.VND,
                            AccountingType = AccountingTypes.Auto,
                            TicketType = TicketTypes.Export,
                            DocumentCode = documentCode,
                            DocumentType = documentType,
                            ConfigCode = EntryConfig.WarehousingExport.Other.Code,
                            DocumentId = documentId,
                            Note = note
                        };
                        await _entryRepository.InsertAsync(billProductEntry);
                        await uow.SaveChangesAsync();

                        var billProductEntryAccount = new EntryAccount
                        {
                            EntryId = billProductEntry.Id,
                            AmountVnd = billProductEntry.Amount,
                            DebtAccountCode = EntryConfig.WarehousingExport.Other.Debt,
                            CreditAccountCode = EntryConfig.WarehousingExport.Other.Credit
                        };
                        await _entryAccountRepository.InsertAsync(billProductEntryAccount);

                        await uow.SaveChangesAsync();
                    }
                    break;
            }
        }

        public async Task AutoUpdateEntryByUpdateWarehousingBill(Guid billId)
        {
            var uow = _unitOfWorkManager.Current;
            var bill = await _warehousingBillRepository.GetAsync(p => p.Id == billId);

            if (bill.IsFromWarehouseTransfer == true)
            {
                await Task.CompletedTask;
                return;
            }

            var billEntries = await _entryRepository
                .GetListAsync(p =>
                    p.EntrySource == ActionSources.WarehousingBillCreate
                    && p.SourceId == bill.Id
                    && !p.IsDeleted);
            var entryIds = billEntries.Select(p => p.Id).ToList();
            var billEntryAccounts = await _entryAccountRepository
                .GetListAsync(p => entryIds.Contains(p.EntryId) && !p.IsDeleted);

            ActionSources entrySource = ActionSources.WarehousingBillCreate;
            var audience = await _commonService.GetAudience(bill.AudienceType, bill.AudienceId);
            switch (bill.BillType)
            {
                case WarehousingBillType.Import:
                    if (bill.AudienceType == AudienceTypes.SupplierVN || bill.AudienceType == AudienceTypes.SupplierCN)
                    {
                        // cập nhật bút toán tổng tiền sp
                        var productEntry = billEntries.FirstOrDefault(p => p.ConfigCode == EntryConfig.WarehousingImport.Supplier.Product.Code);

                        if (productEntry != null)
                        {
                            var oldProductEntry = productEntry.Clone();

                            productEntry.StoreId = bill.StoreId;
                            productEntry.Amount = bill.TotalPriceBeforeTax;
                            productEntry.AudienceId = bill.AudienceId;

                            await _entryRepository.UpdateAsync(productEntry);
                            // update account
                            var productEntryAccounts = billEntryAccounts.Where(p => p.EntryId == productEntry.Id).ToList();
                            var oldProductEntryAccounts = productEntryAccounts.Clone();
                            productEntryAccounts.ForEach(p => p.AmountVnd = productEntry.Amount);

                            await _entryAccountRepository.UpdateManyAsync(productEntryAccounts);
                            await CreateEntryLog(oldProductEntry, oldProductEntryAccounts, productEntry, productEntryAccounts, false);
                            await AutoUpdateDebt(productEntry, false);
                        }

                        // Nếu cập nhật số tiền VAT null -> xóa bút toán hiện có
                        // Nếu cập nhật VAT > 0 -> cập nhật hoặc tạo mới bút toán VAT
                        var vatEntry = billEntries.FirstOrDefault(p => p.ConfigCode == EntryConfig.WarehousingImport.Supplier.VAT.Code);
                        if (bill.VATAmount != null && bill.VATAmount > 0)
                        {
                            if (vatEntry != null && bill.VATAmount != vatEntry.Amount)
                            {
                                var oldVatEntry = vatEntry.Clone();
                                vatEntry.TransactionDate = _clock.Normalize(bill.VATHaveValueDate.Value);
                                vatEntry.Amount = (bill.VATType == MoneyModificationType.Percent ? bill.TotalPriceBeforeTax * bill.VATAmount / 100 : bill.VATAmount) ?? 0;
                                vatEntry.AudienceId = bill.AudienceId;

                                await _entryRepository.UpdateAsync(vatEntry);
                                // update account
                                var vatEntryAccounts = billEntryAccounts.Where(p => p.EntryId == vatEntry.Id).ToList();
                                var oldVatEntryAccounts = vatEntryAccounts.Clone();
                                vatEntryAccounts.ForEach(p => p.AmountVnd = vatEntry.Amount);
                                await _entryAccountRepository.UpdateManyAsync(vatEntryAccounts);

                                await AutoUpdateDebt(vatEntry, false);
                                await CreateEntryLog(oldVatEntry, oldVatEntryAccounts, vatEntry, vatEntryAccounts, false);
                            }
                            else
                            {
                                var vatPayEntry = new Entry
                                {
                                    EntrySource = entrySource,
                                    SourceId = bill.Id,
                                    SourceCode = bill.Code,
                                    StoreId = bill.StoreId,
                                    AudienceType = bill.AudienceType,
                                    AudienceId = bill.AudienceId,
                                    IsActive = true,
                                    Amount = (bill.VATType == MoneyModificationType.Percent ? bill.TotalPriceBeforeTax * bill.VATAmount / 100 : bill.VATAmount) ?? 0,
                                    TransactionDate = _clock.Normalize(bill.VATHaveValueDate.Value),
                                    Currency = Currencies.VND,
                                    AccountingType = AccountingTypes.Auto,
                                    TicketType = TicketTypes.Receipt,
                                    DocumentCode = bill.Code,
                                    DocumentId = bill.Id,
                                    DocumentType = DocumentTypes.InventoryExport,
                                    ConfigCode = EntryConfig.WarehousingImport.Supplier.VAT.Code
                                };
                                await _entryRepository.InsertAsync(vatPayEntry);
                                await uow.SaveChangesAsync();
                                var vatPayEntryAccount = new EntryAccount
                                {
                                    EntryId = vatPayEntry.Id,
                                    AmountVnd = vatPayEntry.Amount,
                                    DebtAccountCode = EntryConfig.WarehousingImport.Supplier.VAT.Debt,
                                    CreditAccountCode = EntryConfig.WarehousingImport.Supplier.VAT.Credit
                                };
                                await _entryAccountRepository.InsertAsync(vatPayEntryAccount);

                                await AutoInsertDebt(vatPayEntry, false);
                            }
                        }
                        else
                        {
                            if (vatEntry != null)
                            {
                                await _entryRepository.DeleteAsync(vatEntry);
                                // update account
                                var vatEntryAccounts = billEntryAccounts.Where(p => p.EntryId == vatEntry.Id).ToList();
                                await _entryAccountRepository.DeleteManyAsync(vatEntryAccounts);
                                await CreateEntryLog(vatEntry, null, null, null, true);
                                await AutoDeleteDebt(vatEntry.Id);
                            }
                        }
                    }
                    else if (bill.AudienceType == AudienceTypes.Customer)
                    {
                        // cập nhật bút toán tổng tiền sp
                        var productEntry = billEntries.FirstOrDefault(p => p.ConfigCode == EntryConfig.WarehousingImport.Customer.Product.Code);
                        if (productEntry != null)
                        {
                            var oldProductEntry = productEntry.Clone();

                            productEntry.StoreId = bill.StoreId;
                            productEntry.Amount = bill.TotalPriceBeforeTax;
                            productEntry.AudienceId = bill.AudienceId;

                            await _entryRepository.UpdateAsync(productEntry);
                            // update account
                            var productEntryAccounts = billEntryAccounts.Where(p => p.EntryId == productEntry.Id).ToList();
                            var oldProductEntryAccounts = productEntryAccounts.Clone();
                            productEntryAccounts.ForEach(p => p.AmountVnd = productEntry.Amount);

                            await _entryAccountRepository.UpdateManyAsync(productEntryAccounts);
                            await CreateEntryLog(oldProductEntry, oldProductEntryAccounts, productEntry, productEntryAccounts, false);
                            await AutoUpdateDebt(productEntry, false);
                        }

                        // Nếu cập nhật số tiền VAT null -> xóa bút toán hiện có
                        // Nếu cập nhật VAT > 0 -> cập nhật hoặc tạo mới bút toán VAT
                        var vatEntry = billEntries.FirstOrDefault(p => p.ConfigCode == EntryConfig.WarehousingImport.Customer.VAT.Code);
                        if (bill.VATAmount != null && bill.VATAmount > 0)
                        {
                            if (vatEntry != null && bill.VATAmount != vatEntry.Amount)
                            {
                                var oldVatEntry = vatEntry.Clone();

                                vatEntry.TransactionDate = _clock.Normalize(bill.VATHaveValueDate.Value);
                                vatEntry.Amount = (bill.VATType == MoneyModificationType.Percent ? bill.TotalPriceBeforeTax * bill.VATAmount / 100 : bill.VATAmount) ?? 0;
                                vatEntry.AudienceId = bill.AudienceId;

                                await _entryRepository.UpdateAsync(vatEntry);
                                // update account
                                var vatEntryAccounts = billEntryAccounts.Where(p => p.EntryId == vatEntry.Id).ToList();
                                var oldVatEntryAccounts = vatEntryAccounts.Clone();
                                vatEntryAccounts.ForEach(p => p.AmountVnd = vatEntry.Amount);
                                await _entryAccountRepository.UpdateManyAsync(vatEntryAccounts);
                                await CreateEntryLog(oldVatEntry, oldVatEntryAccounts, vatEntry, vatEntryAccounts, false);
                                await AutoUpdateDebt(vatEntry, false);
                            }
                            else
                            {
                                var vatPayEntry = new Entry
                                {
                                    EntrySource = entrySource,
                                    SourceId = bill.Id,
                                    SourceCode = bill.Code,
                                    StoreId = bill.StoreId,
                                    AudienceType = bill.AudienceType,
                                    AudienceId = bill.AudienceId,
                                    IsActive = true,
                                    Amount = (bill.VATType == MoneyModificationType.Percent ? bill.TotalPriceBeforeTax * bill.VATAmount / 100 : bill.VATAmount) ?? 0,
                                    TransactionDate = _clock.Normalize(bill.VATHaveValueDate.Value),
                                    Currency = Currencies.VND,
                                    AccountingType = AccountingTypes.Auto,
                                    TicketType = TicketTypes.Receipt,
                                    DocumentCode = bill.Code,
                                    DocumentType = DocumentTypes.InventoryExport,
                                    ConfigCode = EntryConfig.WarehousingImport.Customer.VAT.Code
                                };
                                await _entryRepository.InsertAsync(vatPayEntry);
                                await uow.SaveChangesAsync();
                                var vatPayEntryAccount = new EntryAccount
                                {
                                    EntryId = vatPayEntry.Id,
                                    AmountVnd = vatPayEntry.Amount,
                                    DebtAccountCode = EntryConfig.WarehousingImport.Customer.VAT.Debt,
                                    CreditAccountCode = EntryConfig.WarehousingImport.Customer.VAT.Credit
                                };
                                await _entryAccountRepository.InsertAsync(vatPayEntryAccount);

                                await AutoInsertDebt(vatPayEntry, false);
                            }
                        }
                        else
                        {
                            if (vatEntry != null)
                            {
                                await _entryRepository.DeleteAsync(vatEntry);
                                var vatEntryAccounts = billEntryAccounts.Where(p => p.EntryId == vatEntry.Id).ToList();
                                await _entryAccountRepository.DeleteManyAsync(vatEntryAccounts);
                                await CreateEntryLog(vatEntry, null, null, null, true);
                                await AutoDeleteDebt(vatEntry.Id);
                            }
                        }
                    }
                    else if (bill.AudienceType == AudienceTypes.Other)
                    {
                        // cập nhật bút toán tổng tiền sp
                        var productEntry = billEntries.FirstOrDefault(p => p.ConfigCode == EntryConfig.WarehousingImport.Other.Code);
                        if (productEntry != null)
                        {
                            var oldProductEntry = productEntry.Clone();

                            productEntry.StoreId = bill.StoreId;
                            productEntry.Amount = bill.TotalPriceBeforeTax;
                            productEntry.AudienceId = bill.AudienceId;

                            await _entryRepository.UpdateAsync(productEntry);
                            // update account
                            var productEntryAccounts = billEntryAccounts.Where(p => p.EntryId == productEntry.Id).ToList();
                            var oldProductEntryAccounts = productEntryAccounts.Clone();
                            productEntryAccounts.ForEach(p => p.AmountVnd = productEntry.Amount);

                            await _entryAccountRepository.UpdateManyAsync(productEntryAccounts);
                            await CreateEntryLog(oldProductEntry, oldProductEntryAccounts, productEntry, productEntryAccounts, false);
                            await AutoUpdateDebt(productEntry, false);
                        }
                    }
                    break;

                case WarehousingBillType.Export:
                    if (bill.AudienceType == AudienceTypes.SupplierCN)
                    {
                        // cập nhật bút toán tổng tiền sp
                        var productEntry = billEntries.FirstOrDefault(p => p.ConfigCode == EntryConfig.WarehousingExport.SupplierCN.Product.Code);
                        if (productEntry != null)
                        {
                            var oldProductEntry = productEntry.Clone();

                            productEntry.StoreId = bill.StoreId;
                            productEntry.Amount = bill.TotalPriceBeforeTax;
                            productEntry.AudienceId = bill.AudienceId;

                            await _entryRepository.UpdateAsync(productEntry);
                            // update account
                            var productEntryAccounts = billEntryAccounts.Where(p => p.EntryId == productEntry.Id).ToList();
                            var oldProductEntryAccounts = productEntryAccounts.Clone();
                            productEntryAccounts.ForEach(p => p.AmountVnd = productEntry.Amount);

                            await _entryAccountRepository.UpdateManyAsync(productEntryAccounts);
                            await CreateEntryLog(oldProductEntry, oldProductEntryAccounts, productEntry, productEntryAccounts, false);
                            await AutoUpdateDebt(productEntry, true);
                        }
                    }
                    else if (bill.AudienceType == AudienceTypes.SupplierVN)
                    {
                        // cập nhật bút toán tổng tiền sp
                        var productEntry = billEntries.FirstOrDefault(p => p.ConfigCode == EntryConfig.WarehousingExport.SupplierVN.Product.Code);
                        if (productEntry != null)
                        {
                            var oldProductEntry = productEntry.Clone();

                            productEntry.StoreId = bill.StoreId;
                            productEntry.Amount = bill.TotalPriceBeforeTax;
                            productEntry.AudienceId = bill.AudienceId;

                            await _entryRepository.UpdateAsync(productEntry);
                            // update account
                            var productEntryAccounts = billEntryAccounts.Where(p => p.EntryId == productEntry.Id).ToList();
                            var oldProductEntryAccounts = productEntryAccounts.Clone();
                            productEntryAccounts.ForEach(p => p.AmountVnd = productEntry.Amount);

                            await _entryAccountRepository.UpdateManyAsync(productEntryAccounts);
                            await CreateEntryLog(oldProductEntry, oldProductEntryAccounts, productEntry, productEntryAccounts, false);
                            await AutoUpdateDebt(productEntry, true);
                        }

                        // Nếu cập nhật số tiền VAT null -> xóa bút toán hiện có
                        // Nếu cập nhật VAT > 0 -> cập nhật hoặc tạo mới bút toán VAT
                        var vatEntry = billEntries.FirstOrDefault(p => p.ConfigCode == EntryConfig.WarehousingExport.SupplierVN.VAT.Code);
                        if (bill.VATAmount != null && bill.VATAmount > 0)
                        {
                            if (vatEntry != null && bill.VATAmount != vatEntry.Amount)
                            {
                                var oldVatEntry = vatEntry.Clone();

                                vatEntry.TransactionDate = _clock.Normalize(bill.VATHaveValueDate.Value);
                                vatEntry.Amount = (bill.VATType == MoneyModificationType.Percent ? bill.TotalPriceBeforeTax * bill.VATAmount / 100 : bill.VATAmount) ?? 0;
                                vatEntry.AudienceId = bill.AudienceId;

                                await _entryRepository.UpdateAsync(vatEntry);
                                // update account
                                var vatEntryAccounts = billEntryAccounts.Where(p => p.EntryId == vatEntry.Id).ToList();
                                var oldVatEntryAccounts = vatEntryAccounts.Clone();
                                vatEntryAccounts.ForEach(p => p.AmountVnd = vatEntry.Amount);
                                await _entryAccountRepository.UpdateManyAsync(vatEntryAccounts);
                                await CreateEntryLog(oldVatEntry, oldVatEntryAccounts, vatEntry, vatEntryAccounts, false);
                                await AutoUpdateDebt(vatEntry, true);
                            }
                            else
                            {
                                var vatPayEntry = new Entry
                                {
                                    EntrySource = entrySource,
                                    SourceId = bill.Id,
                                    SourceCode = bill.Code,
                                    StoreId = bill.StoreId,
                                    AudienceType = bill.AudienceType,
                                    AudienceId = bill.AudienceId,
                                    IsActive = true,
                                    Amount = (bill.VATType == MoneyModificationType.Percent ? bill.TotalPriceBeforeTax * bill.VATAmount / 100 : bill.VATAmount) ?? 0,
                                    TransactionDate = _clock.Normalize(bill.VATHaveValueDate.Value),
                                    Currency = Currencies.VND,
                                    AccountingType = AccountingTypes.Auto,
                                    TicketType = TicketTypes.Receipt,
                                    DocumentCode = bill.Code,
                                    DocumentType = DocumentTypes.InventoryExport,
                                    ConfigCode = EntryConfig.WarehousingExport.SupplierVN.VAT.Code
                                };
                                await _entryRepository.InsertAsync(vatPayEntry);
                                await uow.SaveChangesAsync();
                                var vatPayEntryAccount = new EntryAccount
                                {
                                    EntryId = vatPayEntry.Id,
                                    AmountVnd = vatPayEntry.Amount,
                                    DebtAccountCode = EntryConfig.WarehousingExport.SupplierVN.VAT.Debt,
                                    CreditAccountCode = EntryConfig.WarehousingExport.SupplierVN.VAT.Credit
                                };
                                await _entryAccountRepository.InsertAsync(vatPayEntryAccount);

                                await AutoInsertDebt(vatPayEntry, true);
                            }
                        }
                        else
                        {
                            if (vatEntry != null)
                            {
                                await _entryRepository.DeleteAsync(vatEntry);
                                // update account
                                var vatEntryAccounts = billEntryAccounts.Where(p => p.EntryId == vatEntry.Id).ToList();
                                await _entryAccountRepository.DeleteManyAsync(vatEntryAccounts);
                                await CreateEntryLog(vatEntry, null, null, null, true);
                                await AutoDeleteDebt(vatEntry.Id);
                            }
                        }
                    }
                    else if (bill.AudienceType == AudienceTypes.Other)
                    {
                        // cập nhật bút toán tổng giá vốn sp
                        var productEntry = billEntries.FirstOrDefault(p => p.ConfigCode == EntryConfig.WarehousingExport.Other.Code);
                        if (productEntry != null)
                        {
                            var oldProductEntry = productEntry.Clone();
                            productEntry.StoreId = bill.StoreId;
                            productEntry.Amount = bill.TotalStockPrice;
                            productEntry.AudienceId = bill.AudienceId;
                            await _entryRepository.UpdateAsync(productEntry);
                            // update account
                            var productEntryAccounts = billEntryAccounts.Where(p => p.EntryId == productEntry.Id).ToList();
                            var oldProductEntryAccounts = productEntryAccounts.Clone();
                            productEntryAccounts.ForEach(p => p.AmountVnd = productEntry.Amount);

                            await _entryAccountRepository.UpdateManyAsync(productEntryAccounts);
                            await CreateEntryLog(oldProductEntry, oldProductEntryAccounts, productEntry, productEntryAccounts, false);
                            await AutoUpdateDebt(productEntry, true);
                        }
                    }
                    break;
            }

            await uow.SaveChangesAsync();
        }

        public async Task AutoDeleteEntryByDeleteWarehousingBill(Guid billId)
        {
            var bill = await _warehousingBillRepository.GetAsync(p => p.Id == billId);
            if (bill.IsFromWarehouseTransfer == true)
            {
                return;
            }
            var entries = await _entryRepository
                .GetListAsync(p =>
                    p.EntrySource == ActionSources.WarehousingBillCreate
                    && p.SourceId == bill.Id);
            var entryIds = entries.Select(p => p.Id).ToList();
            var entryAccounts = await _entryAccountRepository
                .GetListAsync(p => entryIds.Contains(p.EntryId));

            if (entries.Any())
            {
                await _entryRepository.DeleteManyAsync(entries);
                await AutoDeleteDebt(entryIds.ToArray());
                foreach (var entry in entries)
                {
                    await CreateEntryLog(entry, null, null, null, true);
                }
            }

            if (entryAccounts.Any())
            {
                await _entryAccountRepository.DeleteManyAsync(entryAccounts);
            }
        }

        public async Task AutoCreateEntryOnCreatePaymentReceipt(Guid receiptId)
        {
            var uow = _unitOfWorkManager.Current;
            var receipt = await _paymentReceiptRepository.FindAsync(p => p.Id == receiptId);
            var audience = await _commonService.GetAudience(receipt.AudienceType, receipt.AudienceId);
            var configCode = string.Empty;
            var debtAccount = string.Empty;
            var creditAccount = string.Empty;
            var isDebt = false;
            switch (receipt.TicketType)
            {
                case TicketTypes.Receipt:
                    configCode = EntryConfig.PaymentReceipt.Receipt.Code;
                    creditAccount = receipt.ReciprocalAccountCode;
                    debtAccount = receipt.AccountCode;
                    isDebt = false;
                    break;

                case TicketTypes.CreditNote:
                    configCode = EntryConfig.PaymentReceipt.Credit.Code;
                    creditAccount = receipt.ReciprocalAccountCode;
                    debtAccount = receipt.AccountCode;
                    isDebt = false;
                    break;

                case TicketTypes.PaymentVoucher:
                    configCode = EntryConfig.PaymentReceipt.PaymentVoucher.Code;
                    debtAccount = receipt.ReciprocalAccountCode;
                    creditAccount = receipt.AccountCode;
                    isDebt = true;
                    break;

                case TicketTypes.DebitNote:
                    configCode = EntryConfig.PaymentReceipt.Debit.Code;
                    debtAccount = receipt.ReciprocalAccountCode;
                    creditAccount = receipt.AccountCode;
                    isDebt = true;
                    break;

                case TicketTypes.FundTransfer:
                    configCode = EntryConfig.PaymentReceipt.Base;
                    debtAccount = receipt.ReciprocalAccountCode;
                    creditAccount = receipt.AccountCode;
                    isDebt = true;
                    break;
            };
            if (receipt.AudienceType == AudienceTypes.SupplierCN)
            {
                var cnDebtAccount = debtAccount;
                var cnCreditAccount = creditAccount;
                var vnDebtAccount = debtAccount;
                var vnCreditAccount = creditAccount;
                switch (receipt.TicketType)
                {
                    case TicketTypes.DebitNote:
                        cnCreditAccount = "";
                        vnDebtAccount = "";
                        break;

                    case TicketTypes.CreditNote:
                        cnDebtAccount = "";
                        vnCreditAccount = "";
                        break;

                    case TicketTypes.Receipt:
                        cnDebtAccount = "";
                        vnCreditAccount = "";
                        break;

                    case TicketTypes.PaymentVoucher:
                        cnCreditAccount = "";
                        vnDebtAccount = "";
                        break;
                };
                var cnEntry = new Entry
                {
                    TicketType = receipt.TicketType,
                    AudienceType = receipt.AudienceType,
                    AudienceId = receipt.AudienceId,
                    Amount = receipt.AmountCNY ?? 0,
                    Currency = Currencies.CNY,
                    AccountingType = AccountingTypes.Auto,
                    ConfigCode = configCode,
                    DocumentType = receipt.DocumentType ?? DocumentTypes.Other,
                    DocumentCode = !receipt.DocumentCode.IsNullOrEmpty() ? receipt.DocumentCode : receipt.Code,
                    DocumentId = receipt.Id,
                    TransactionDate = receipt.TransactionDate,
                    DocumentDetailType = receipt.DocumentDetailType,
                    EntrySource = ActionSources.CreatePaymentReceipt,
                    SourceId = receipt.Id,
                    Note = receipt.Note,
                    StoreId = receipt.StoreId
                };

                await _entryRepository.InsertAsync(cnEntry);
                await uow.SaveChangesAsync();

                var cnEntryAccount = new EntryAccount
                {
                    DebtAccountCode = cnDebtAccount,
                    CreditAccountCode = cnCreditAccount,
                    AmountCny = receipt.AmountCNY,
                    EntryId = cnEntry.Id,
                };

                await _entryAccountRepository.InsertAsync(cnEntryAccount);
                await uow.SaveChangesAsync();

                await AutoInsertDebt(cnEntry, isDebt, Currencies.CNY);

                var vnEntry = new Entry
                {
                    TicketType = receipt.TicketType,
                    AudienceType = receipt.AudienceType,
                    AudienceId = receipt.AudienceId,
                    Amount = receipt.AmountVND ?? 0,
                    Currency = Currencies.VND,
                    AccountingType = AccountingTypes.Auto,
                    ConfigCode = configCode,
                    DocumentType = receipt.DocumentType ?? DocumentTypes.Other,
                    DocumentCode = !receipt.DocumentCode.IsNullOrEmpty() ? receipt.DocumentCode : receipt.Code,
                    DocumentId = receipt.Id,
                    TransactionDate = receipt.TransactionDate,
                    DocumentDetailType = receipt.DocumentDetailType,
                    EntrySource = ActionSources.CreatePaymentReceipt,
                    SourceId = receipt.Id,
                    Note = receipt.Note,
                    StoreId = receipt.StoreId
                };

                await _entryRepository.InsertAsync(vnEntry);
                await uow.SaveChangesAsync();

                var vnEntryAccount = new EntryAccount
                {
                    DebtAccountCode = vnDebtAccount,
                    CreditAccountCode = vnCreditAccount,
                    AmountVnd = receipt.AmountVND,
                    EntryId = vnEntry.Id,
                };

                await _entryAccountRepository.InsertAsync(vnEntryAccount);
                await uow.SaveChangesAsync();
            }
            else
            {
                var documentId = receipt.Id;
                var documentCode = !receipt.DocumentCode.IsNullOrEmpty() ? receipt.DocumentCode : receipt.Code;
                var documentType = receipt.DocumentType ?? DocumentTypes.Other;

                if (receipt.Source == ActionSources.CreateBillCustomer)
                {
                    documentId = receipt.SourceId.GetValueOrDefault();
                }

                var entry = new Entry
                {
                    StoreId = receipt.StoreId,
                    TicketType = receipt.TicketType,
                    AudienceType = receipt.AudienceType,
                    AudienceId = receipt.AudienceId,
                    Amount = receipt.AmountVND ?? 0,
                    Currency = Currencies.VND,
                    AccountingType = AccountingTypes.Auto,
                    ConfigCode = configCode,
                    DocumentType = documentType,
                    DocumentCode = documentCode,
                    DocumentId = documentId,
                    TransactionDate = receipt.TransactionDate,
                    DocumentDetailType = receipt.DocumentDetailType,
                    EntrySource = ActionSources.CreatePaymentReceipt,
                    SourceId = receipt.Id,
                    Note = receipt.Note,
                };

                await _entryRepository.InsertAsync(entry);
                await uow.SaveChangesAsync();

                var entryAccount = new EntryAccount
                {
                    DebtAccountCode = debtAccount,
                    CreditAccountCode = creditAccount,
                    AmountCny = receipt.AmountCNY,
                    AmountVnd = receipt.AmountVND,
                    EntryId = entry.Id,
                };

                await _entryAccountRepository.InsertAsync(entryAccount);
                await AutoInsertDebt(entry, isDebt);
                await uow.SaveChangesAsync();
            }
        }

        public async Task AutoUpdateEntryOnUpdatePaymentReceipt(Guid receiptId)
        {
            var uow = _unitOfWorkManager.Current;
            var receipt = await _paymentReceiptRepository.FindAsync(p => p.Id == receiptId);
            var entries = await _entryRepository.GetListAsync(p => p.SourceId == receipt.Id);
            var entryIds = entries.Select(p => p.Id).ToList();
            var entryAccounts = await _entryAccountRepository.GetListAsync(p => entryIds.Contains(p.EntryId));
            var audience = await _commonService.GetAudience(receipt.AudienceType, receipt.AudienceId);
            var configCode = string.Empty;
            var debtAccount = string.Empty;
            var creditAccount = string.Empty;
            var isDebt = false;
            switch (receipt.TicketType)
            {
                case TicketTypes.DebitNote:
                    configCode = EntryConfig.PaymentReceipt.Debit.Code;
                    debtAccount = receipt.AccountCode;
                    creditAccount = receipt.ReciprocalAccountCode;
                    isDebt = true;
                    break;

                case TicketTypes.CreditNote:
                    configCode = EntryConfig.PaymentReceipt.Credit.Code;
                    creditAccount = receipt.AccountCode;
                    debtAccount = receipt.ReciprocalAccountCode;
                    isDebt = false;
                    break;

                case TicketTypes.Receipt:
                    creditAccount = receipt.ReciprocalAccountCode;
                    debtAccount = receipt.AccountCode;
                    configCode = EntryConfig.PaymentReceipt.Receipt.Code;
                    isDebt = false;
                    break;

                case TicketTypes.PaymentVoucher:
                    configCode = EntryConfig.PaymentReceipt.PaymentVoucher.Code;
                    debtAccount = receipt.AccountCode;
                    creditAccount = receipt.ReciprocalAccountCode;
                    isDebt = true;
                    break;
            };
            if (receipt.AudienceType == AudienceTypes.SupplierCN)
            {
                var cnDebtAccount = debtAccount;
                var cnCreditAccount = creditAccount;
                var vnDebtAccount = debtAccount;
                var vnCreditAccount = creditAccount;
                switch (receipt.TicketType)
                {
                    case TicketTypes.DebitNote:
                        cnCreditAccount = "";
                        vnDebtAccount = "";
                        break;

                    case TicketTypes.CreditNote:
                        cnDebtAccount = "";
                        vnCreditAccount = "";
                        break;

                    case TicketTypes.Receipt:
                        cnDebtAccount = "";
                        vnCreditAccount = "";
                        break;

                    case TicketTypes.PaymentVoucher:
                        cnCreditAccount = "";
                        vnDebtAccount = "";
                        break;
                };

                var cnEntry = entries.FirstOrDefault(p => p.Currency == Currencies.CNY);
                var oldCnEntry = cnEntry.Clone();
                if (cnEntry != null)
                {
                    cnEntry.Amount = receipt.AmountCNY ?? 0;
                    cnEntry.TransactionDate = receipt.TransactionDate;
                    cnEntry.AudienceId = receipt.AudienceId;

                    await _entryRepository.UpdateAsync(cnEntry);
                }

                var cnEntryAccount = entryAccounts.FirstOrDefault(p => p.EntryId == cnEntry.Id);
                var oldCnEntryAccount = new List<EntryAccount> { cnEntryAccount.Clone() };
                if (cnEntryAccount != null)
                {
                    cnEntryAccount.AmountVnd = receipt.AmountCNY;
                    cnEntryAccount.DebtAccountCode = debtAccount;
                    cnEntryAccount.CreditAccountCode = creditAccount;

                    await _entryAccountRepository.UpdateAsync(cnEntryAccount);
                }

                await CreateEntryLog(oldCnEntry, oldCnEntryAccount, cnEntry, new List<EntryAccount> { cnEntryAccount }, false);
                await AutoUpdateDebt(cnEntry, isDebt);

                var vnEntry = entries.FirstOrDefault(p => p.Currency == Currencies.VND);
                var oldVnEntry = vnEntry.Clone();
                if (vnEntry != null)
                {
                    vnEntry.Amount = receipt.AmountCNY ?? 0;
                    vnEntry.TransactionDate = receipt.TransactionDate;
                    vnEntry.AudienceId = receipt.AudienceId;

                    await _entryRepository.UpdateAsync(vnEntry);
                }

                var vnEntryAccount = entryAccounts.FirstOrDefault(p => p.EntryId == vnEntry.Id);
                var oldVnEntryAccount = new List<EntryAccount> { vnEntryAccount.Clone() };
                if (vnEntryAccount != null)
                {
                    vnEntryAccount.AmountVnd = receipt.AmountVND;
                    vnEntryAccount.DebtAccountCode = debtAccount;
                    vnEntryAccount.CreditAccountCode = creditAccount;

                    await _entryAccountRepository.UpdateAsync(vnEntryAccount);
                }
                await CreateEntryLog(oldVnEntry, oldVnEntryAccount, vnEntry, new List<EntryAccount> { vnEntryAccount }, false);
            }
            else
            {
                var entry = entries.FirstOrDefault();
                var oldEntry = entry.Clone();
                if (entry != null)
                {
                    entry.Amount = receipt.AmountVND ?? 0;
                    entry.TransactionDate = receipt.TransactionDate;
                    entry.AudienceId = receipt.AudienceId;

                    await _entryRepository.UpdateAsync(entry);
                }

                var entryAccount = entryAccounts.FirstOrDefault();
                var oldVnEntryAccount = new List<EntryAccount> { entryAccount.Clone() };
                if (entryAccount != null)
                {
                    entryAccount.AmountVnd = receipt.AmountVND;
                    entryAccount.DebtAccountCode = debtAccount;
                    entryAccount.CreditAccountCode = creditAccount;

                    await _entryAccountRepository.UpdateAsync(entryAccount);
                }
                await CreateEntryLog(oldEntry, oldVnEntryAccount, entry, new List<EntryAccount> { entryAccount }, false);
                await AutoUpdateDebt(entry, isDebt);
            }

            await uow.SaveChangesAsync();
        }

        public async Task AutoDeleteEntryOnDeletePaymentReceipt(Guid receiptId)
        {
            var uow = _unitOfWorkManager.Current;
            var receipt = await _paymentReceiptRepository.GetAsync(p => p.Id == receiptId);
            var entries = await _entryRepository.GetListAsync(p => p.EntrySource == ActionSources.CreatePaymentReceipt && p.SourceId == receipt.Id);
            var entryIds = entries.Select(p => p.Id).ToList();
            var entryAccounts = await _entryAccountRepository.GetListAsync(p => entryIds.Contains(p.EntryId));

            await _entryRepository.DeleteManyAsync(entries);
            await _entryAccountRepository.DeleteManyAsync(entryAccounts);
            await AutoDeleteDebt(entryIds.ToArray());
            foreach (var entry in entries)
            {
                await CreateEntryLog(entry, null, null, null, true);
            }
            await uow.SaveChangesAsync();
        }

        public async Task AutoDeleteEntryOnDeleteSaleOrder(Guid saleOrderId)
        {
            var uow = _unitOfWorkManager.Current;
            var entries = await _entryRepository.GetListAsync(p => p.EntrySource == ActionSources.OrderCreate && p.SourceId == saleOrderId);
            var entryIds = entries.Select(p => p.Id).ToList();
            var entryAccounts = await _entryAccountRepository.GetListAsync(p => entryIds.Contains(p.EntryId));

            foreach (var entry in entries)
            {
                await CreateEntryLog(entry, null, null, null, true);
            }

            await _entryRepository.DeleteManyAsync(entries);
            await _entryAccountRepository.DeleteManyAsync(entryAccounts);
            await AutoDeleteDebt(entryIds.ToArray());

            await uow.SaveChangesAsync();
        }

        public async Task<EntryResponse> GetEntryById(Guid id)
        {
            var userStores = await _userStoreRepository.GetListAsync(p => p.UserId == _currentUser.Id);
            if (!userStores.Any())
            {
                throw new BusinessException(ErrorMessages.UserStore.StoreNotDefine);
            }
            var userStoreIds = userStores.Select(p => p.StoreId).ToList();
            var entry = await _entryRepository.GetAsync(x => x.Id == id);
            if (!userStoreIds.Contains(entry.StoreId))
            {
                throw new BusinessException(ErrorMessages.UserStore.StoreNotDefine);
            }
            var entryDto = _mapper.Map<Entry, EntryDTO>(entry);
            var entryAcount = (await _entryAccountRepository.GetListAsync(x => x.EntryId == id)).Select(x =>
                _mapper.Map<EntryAccount, EntryAccountDto>(x)
            ).ToList();
            var audience = await _commonService.GetAudience(entry.AudienceType, entry.AudienceId);
            entryDto.AudienceName = audience?.Name;
            entryDto.AudienceCode = audience?.Code;
            entryDto.AudiencePhone = audience?.Phone;
            entryDto.DocumentTypeName = _commonService.GetDocumentTypeName(entryDto.DocumentType);
            entryDto.Attachments = await _attachmentService.GetAttachmentByObjectIdAsync(id);
            return new EntryResponse()
            {
                Entry = entryDto,
                EntryAccount = entryAcount
            };
        }

        public async Task Delete(Guid id)
        {
            var entry = await _entryRepository.GetAsync(x => x.Id == id);

            if (entry.EntrySource.Equals(ActionSources.ManualCreateEntry))
            {
                await DeletePaymentReceipt(entry);
                await AutoDeleteDebt(entry.Id);
                //Manual
                await _entryRepository.DeleteAsync(entry);

                await CreateEntryLog(entry, null, null, null, true);
            }
            else
            {
                //Check xóa chứng từ => xóa
                throw new ApplicationException(ErrorMessages.Common.ModifyAutoGeneratedData);
            }
        }

        public async Task<Guid> ManualCreateEntry(EntryCreateRequest request, UploadFileResult uploadFile)
        {
            using (var uow = _unitOfWorkManager.Begin(
               requiresNew: true, isTransactional: false
           ))
            {
                try
                {
                    var userStore = (await _userStoreRepository.GetQueryableAsync()).FirstOrDefault(p => p.UserId == _currentUser.Id);
                    var now = _clock.Now;
                    if (userStore == null)
                    {
                        throw new BusinessException();
                    }
                    await ValidateCreate(request);
                    var isDebt = false;
                    var entry = await _entryRepository.InsertAsync(new Entry()
                    {
                        StoreId = userStore.StoreId,
                        TransactionDate = _clock.Normalize(request.TransactionDate),
                        TicketType = request.TicketType,
                        AudienceId = request.AudienceId,
                        AudienceType = request.AudienceType,
                        Note = request.Note,
                        EntrySource = ActionSources.ManualCreateEntry,
                        DocumentType = DocumentTypes.Other,
                        DocumentCode = request.DocumentCode,
                        Attachments = JsonSerializer.Serialize(uploadFile),
                        AccountingType = AccountingTypes.Manual,
                    });

                    if (entry.AudienceType == AudienceTypes.SupplierCN)
                    {
                        entry.Currency = Currencies.CNY;
                        entry.Amount = request.EntryAccounts.Sum(p => p.AmountCny ?? 0);
                    }
                    else
                    {
                        entry.Currency = Currencies.VND;
                        entry.Amount = request.EntryAccounts.Sum(p => p.AmountVnd ?? 0);
                    }
                    var entryAccounts = request.EntryAccounts.Select(x => new EntryAccount()
                    {
                        EntryId = entry.Id,
                        AmountVnd = x.AmountVnd,
                        AmountCny = x.AmountCny,
                        CreditAccountCode = x.CreditAccountCode,
                        DebtAccountCode = x.DebtAccountCode,
                        DocumentType = DocumentTypes.Other,
                        Note = x.Note,
                        CreationTime = DateTime.UtcNow
                    }).ToList();

                    await _entryAccountRepository.InsertManyAsync(entryAccounts);

                    switch (entry.TicketType)
                    {
                        case TicketTypes.Import:
                        case TicketTypes.Receipt:
                        case TicketTypes.CreditNote:
                            isDebt = false;
                            break;

                        case TicketTypes.Export:
                        case TicketTypes.PaymentVoucher:
                        case TicketTypes.DebitNote:
                            isDebt = true;
                            break;
                    }
                    await AutoInsertDebt(entry, isDebt, entry.Currency);
                    await CreatePaymentReceipt(entry, entryAccounts);
                    await uow.SaveChangesAsync();
                    await uow.CompleteAsync();
                    return entry.Id;
                }
                catch (Exception)
                {
                    await uow.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task ManualUpdateEntry(EntryUpdateRequest request, UploadFileResult uploadFile)
        {
            using (var uow = _unitOfWorkManager.Begin(
               requiresNew: true, isTransactional: true
           ))
            {
                try
                {
                    var userStore = (await _userStoreRepository.GetQueryableAsync()).FirstOrDefault(p => p.UserId == _currentUser.Id);
                    if (userStore == null)
                    {
                        throw new BusinessException();
                    }
                    await ValidateUpdate(request);
                    var isDebt = false;
                    var entry = await _entryRepository.GetAsync(x => x.Id == request.Id);
                    var oldEntry = entry.Clone();
                    entry.TransactionDate = _clock.Normalize(request.TransactionDate);
                   //entry.StoreId = userStore.Id;
                    entry.TicketType = request.TicketType;
                    entry.DocumentType = DocumentTypes.Other;
                    entry.DocumentCode = request.DocumentCode;
                    entry.AudienceId = request.AudienceId;
                    entry.AudienceType = request.AudienceType;
                    entry.Note = request.Note;

                    switch (entry.TicketType)
                    {
                        case TicketTypes.Import:
                        case TicketTypes.Receipt:
                        case TicketTypes.CreditNote:
                            isDebt = false;
                            break;

                        case TicketTypes.Export:
                        case TicketTypes.PaymentVoucher:
                        case TicketTypes.DebitNote:
                            isDebt = true;
                            break;
                    }

                    if (entry.AudienceType == AudienceTypes.SupplierCN)
                    {
                        entry.Currency = Currencies.CNY;
                        entry.Amount = request.EntryAccounts.Sum(p => p.AmountCny ?? 0);
                    }
                    else
                    {
                        entry.Currency = Currencies.VND;
                        entry.Amount = request.EntryAccounts.Sum(p => p.AmountVnd ?? 0);
                    }

                    if (uploadFile != null && uploadFile.Files.Count > 0)
                        entry.Attachments = JsonSerializer.Serialize(uploadFile);

                    await _entryRepository.UpdateAsync(entry);
                    var oldAccounts = await _entryAccountRepository.GetListAsync(x => x.EntryId == request.Id);

                    var entryAccounts = request.EntryAccounts.Select(x => new EntryAccount()
                    {
                        EntryId = entry.Id,
                        AmountVnd = x.AmountVnd,
                        AmountCny = x.AmountCny,
                        DocumentType = DocumentTypes.Other,
                        CreditAccountCode = x.CreditAccountCode,
                        DebtAccountCode = x.DebtAccountCode,
                        Note = x.Note,
                        CreationTime = DateTime.UtcNow
                    }).ToList();

                    await _entryAccountRepository.InsertManyAsync(entryAccounts);
                    if (oldAccounts.Any())
                    {
                        await _entryAccountRepository.DeleteManyAsync(oldAccounts);
                    }
                    await UpdatePaymentReceiptOnUpdateEntry(oldEntry, entry, entryAccounts);
                    await CreateEntryLog(oldEntry, oldAccounts, entry, entryAccounts, false);
                    await AutoUpdateDebt(entry, isDebt);

                    await uow.CompleteAsync();
                }
                catch (Exception ex)
                {
                    await uow.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task ManualEditNoteEntry(EditNoteEntryRequest request)
        {
            using (var uow = _unitOfWorkManager.Begin(
               requiresNew: true, isTransactional: true
           ))
            {
                try
                {
                    var entry = await _entryRepository.GetAsync(x => x.Id == request.Id);

                    var compare = entry.CompareObjectValues(request);
                    if (compare.Item1.Count > 0)
                    {
                        _ = _entryLogRepository.InsertAsync(new EntryLog()
                        {
                            Action = EntityActions.Update,
                            EntryId = request.Id,
                            ToValue = JsonSerializer.Serialize(new { Note = entry.Note }),
                            FromValue = JsonSerializer.Serialize(new { Note = request.Note }),
                        });
                    }

                    entry.Note = request.Note;

                    await _entryRepository.UpdateAsync(entry);
                    await uow.CompleteAsync();
                }
                catch (Exception)
                {
                    await uow.RollbackAsync();
                    throw;
                }
            }
        }

        public async Task<PagingResponse<EntryDetailResponse>> EntryDetailAsync(SearchEntryDetailRequest request)
        {
            var result = new List<EntryDetailResponse>();
            if (request.StoreIds.Any() ||
                request.TicketType.HasValue ||
                !request.ParentCode.IsNullOrWhiteSpace() ||
                !request.Code.IsNullOrWhiteSpace() ||
                !request.DocumentCode.IsNullOrWhiteSpace() ||
                !request.AccountCode.IsNullOrWhiteSpace() ||
                request.Start.HasValue ||
                request.End.HasValue ||
                request.AudienceType.HasValue ||
                !request.Audience.IsNullOrWhiteSpace())
                request.ParentId = null;

            var entrys = (await _entryRepository.GetQueryableAsync())
            .WhereIf(request.ParentId.HasValue, x => x.Id == request.ParentId)
            .WhereIf(request.StoreIds.Any(), x => request.StoreIds.Any(sId => sId == x.StoreId))
            .WhereIf(request.TicketType.HasValue, x => x.TicketType == request.TicketType)
            .WhereIf(!request.ParentCode.IsNullOrWhiteSpace(), x => x.Code.ToLower().Contains(request.ParentCode.ToLower()))
            .WhereIf(!request.DocumentCode.IsNullOrWhiteSpace(), x => x.DocumentCode.ToLower().Contains(request.DocumentCode.ToLower()))
            .WhereIf(request.Start.HasValue, x => x.TransactionDate.Date >= request.Start.Value.Date)
            .WhereIf(request.End.HasValue, x => x.TransactionDate.Date <= request.End.Value.Date)
            .WhereIf(request.AudienceType.HasValue, x => x.AudienceType == request.AudienceType)
            .ToList();

            if (entrys.Any())
            {
                var entryAccounts = (await _entryAccountRepository.GetQueryableAsync())
                    .Where(x => entrys.Select(x => x.Id).Any(id => id == x.EntryId))
                    .WhereIf(!request.Code.IsNullOrWhiteSpace(), x => x.Code.ToLower().Contains(request.Code.ToLower()));

                if (entryAccounts == null)
                    return new PagingResponse<EntryDetailResponse>(0, result);

                var audienceRequests = entrys.Where(p => p.AudienceId != null).Select(p =>
                {
                    (AudienceTypes, Guid?) item = (p.AudienceType, p.AudienceId);
                    return item;
                }).Distinct().ToArray();

                var audiences = await _commonService.GetAudiences(audienceRequests);

                foreach (var item in entryAccounts)
                {
                    var entryDetailResponse = new EntryDetailResponse();
                    var entry = entrys.FirstOrDefault(x => x.Id == item.EntryId);
                    var audience = audiences.FirstOrDefault(x => x.Id == entry.AudienceId);

                    entryDetailResponse.ParentId = entry.Id;
                    entryDetailResponse.ParentCode = entry.Code;
                    entryDetailResponse.Id = item.Id;
                    entryDetailResponse.Code = item.Code;
                    entryDetailResponse.Date = entry.TransactionDate;

                    entryDetailResponse.AudienceType = entry.AudienceType;
                    if (audience != null)
                    {
                        entryDetailResponse.AudienceName = audience.Name;
                        entryDetailResponse.AudienceCode = audience.Code;
                        entryDetailResponse.AudiencePhone = audience.Phone;
                    }

                    entryDetailResponse.DebtAccountCode = item.DebtAccountCode ?? "";
                    entryDetailResponse.CreditAccountCode = item.CreditAccountCode ?? "";
                    entryDetailResponse.AmountVnd = item.AmountVnd;
                    entryDetailResponse.AmountCny = item.AmountCny;

                    entryDetailResponse.DocumentId = entry.DocumentId;
                    entryDetailResponse.DocumentCode = entry.DocumentCode;
                    entryDetailResponse.DocumentType = entry.DocumentType;
                    entryDetailResponse.DocumentTypeName = _commonService.GetDocumentTypeName(entry.DocumentType);
                    entryDetailResponse.DocumentDetailType = entry.DocumentDetailType;
                    entryDetailResponse.Note = item.Note;

                    result.Add(entryDetailResponse);
                }

                var resultPage = result
                    .WhereIf(!request.Audience.IsNullOrWhiteSpace(), x =>
                    x.AudienceCode.ToLower().Contains(request.Audience.ToLower()) ||
                    x.AudienceName.ToLower().Contains(request.Audience.ToLower()) ||
                    x.AudiencePhone.ToLower().Contains(request.Audience.ToLower()))
                    .WhereIf(!request.AccountCode.IsNullOrWhiteSpace(), x =>
                    x.DebtAccountCode.ToLower().Contains(request.AccountCode.ToLower()) ||
                    x.CreditAccountCode.ToLower().Contains(request.AccountCode.ToLower()));

                return new PagingResponse<EntryDetailResponse>(resultPage.Count(), resultPage
                    .OrderByDescending(x => x.ParentCode)
                    .Skip(request.Offset)
                    .Take(request.PageSize)
                    .ToList());
            }

            return new PagingResponse<EntryDetailResponse>(0, result);
        }

        public async Task<byte[]> ExportEntryAsync(SearchEntryRequest request)
        {
            request.PageIndex = 1;
            request.PageSize = int.MaxValue;
            var data = (await SearchEntry(request)).Data;

            var exportData = new List<ExportEntryResponse>();

            var stores = await _storeService.GetByIdsAsync(data.Select(x => x.StoreId).DistinctBy(x => x).Cast<Guid>().ToList());
            var users = (await _userRepository.GetListAsync()).Where(x => data.Select(d => d.CreatorId).Any(createId => createId == x.Id));

            foreach (var item in data)
            {
                var store = stores.FirstOrDefault(x => x.Id == item.StoreId) ?? new DTOs.Stores.StoreDto();

                var vnd = item.Accounts.Where(x => x.AmountVnd != null).Select(x => x.AmountVnd).ToList();
                var cyn = item.Accounts.Where(x => x.AmountCny != null).Select(x => x.AmountCny).ToList();
                vnd.AddRange(cyn);
                string money = "";
                string debt = "";
                string credit = "";
                var culture = new CultureInfo("en-US");
                culture.NumberFormat.NumberDecimalSeparator = ",";
                culture.NumberFormat.NumberGroupSeparator = ".";
                foreach (var account in item.Accounts)
                {
                    if (account.AmountVnd != null)
                        money += account.AmountVnd.Value.ToString("N", culture) + Environment.NewLine;
                    if (account.AmountCny != null)
                        money += account.AmountCny.Value.ToString("N", culture) + Environment.NewLine;
                    debt += account.DebtAccountCode != null ? account.DebtAccountCode + Environment.NewLine : Environment.NewLine;
                    credit += account.CreditAccountCode != null ? account.CreditAccountCode + Environment.NewLine : Environment.NewLine;
                }
                money = (money.Length >= Environment.NewLine.Length) ? money.Substring(0, money.Length - Environment.NewLine.Length).Replace(",000", "") : money;
                debt = (debt.Length >= Environment.NewLine.Length) ? debt.Substring(0, debt.Length - Environment.NewLine.Length) : debt;
                credit = (credit.Length >= Environment.NewLine.Length) ? credit.Substring(0, credit.Length - Environment.NewLine.Length) : credit;

                var creator = users.FirstOrDefault(x => x.Id == item.CreatorId);
                exportData.Add(new ExportEntryResponse()
                {
                    Id = item.Code,
                    TransactionDate = item.TransactionDate.ToString("dd/MM/yyyy"),
                    Enterprise = null,
                    StoreName = store.Name,
                    TicketName = SetTicketName(item.TicketType),
                    AudienceCode = item.AudienceCode,
                    AudienceName = item.AudienceName,
                    Document = SetDocument(item.DocumentType),
                    DocumentCode = item.DocumentCode,
                    Money = money,
                    Debt = debt,
                    Credit = credit,
                    Note = item.Note,
                    Creator = creator != null ? creator.Name : null,
                    CreateTime = _clock.Normalize(item.CreationTime).ToString("dd/MM/yyyy"),
                });
            }
            return ExcelHelper.ExportExcel(exportData);
        }

        /// <summary>
        /// Tự động tạo công nợ khi tạo bút toán
        /// </summary>
        /// <param name="entry"></param>
        /// <param name="isDebt"></param>
        /// <param name="currency"></param>
        /// <returns></returns>
        private async Task AutoInsertDebt(Entry entry, bool isDebt, Currencies currency = Currencies.VND)
        {
            if (entry.AudienceType == AudienceTypes.Other)
            {
                return;
            }
            var billProductDebt = new Debt
            {
                TransactionId = entry.SourceId ?? entry.Id,
                AudienceType = entry.AudienceType,
                AudienceId = entry.AudienceId,
                SupplierId = (entry.AudienceType == AudienceTypes.SupplierCN || entry.AudienceType == AudienceTypes.SupplierVN) ? entry.AudienceId : null,
                CustomerId = (entry.AudienceType == AudienceTypes.Customer) ? entry.AudienceId : null,
                EmployeeId = (entry.AudienceType == AudienceTypes.Employee) ? entry.AudienceId : null,
                IsAutoGenerated = true,
                IsActive = true,
                Debts = isDebt ? entry.Amount : 0,
                Credits = isDebt ? 0 : entry.Amount,
                TransactionDate = entry.TransactionDate,
                TicketType = entry.TicketType,
                EntryId = entry.Id,
                DocumentType = entry.DocumentType,
                DocumentId = entry.DocumentId,
                Currency = currency,
                StoreId = entry.StoreId
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
            var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                var debt = await _debtRepository.FirstOrDefaultAsync(p => p.EntryId == entry.Id);
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
                await uow.CompleteAsync();
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

        /// <summary>
        /// Tạo log từ bút toán + ds tài khoản cũ, bút toán + ds tài khoản mới
        /// </summary>
        /// <param name="oldEntry"></param>
        /// <param name="oldAccounts"></param>
        /// <param name="newEntry"></param>
        /// <param name="newAccounts"></param>
        /// <param name="isDelete"></param>
        /// <returns></returns>
        private async Task CreateEntryLog(Entry oldEntry, List<EntryAccount> oldAccounts, Entry newEntry, List<EntryAccount> newAccounts, bool isDelete)
        {
            if (isDelete)
            {
                await _entryLogRepository.InsertAsync(new EntryLog()
                {
                    Action = EntityActions.Delete,
                    EntryId = oldEntry.Id,
                    ToValue = null,
                    FromValue = null,
                });
            }
            else
            {
                var compare = oldEntry.CompareObjectValues(newEntry);
                var listAccountOld = new List<EntryAccount>();
                var listAccountNew = new List<EntryAccount>();

                // Compare 2 list account
                // Với obj trên cùng dòng ở 2 list, add vào list compare nếu compare khác nhau
                // nếu tổng số lượng ở 2 list khác nhau, Add nốt vào list compare các dòng còn lại ở bên list nhiều hơn
                var accountCount = oldAccounts.Count > newAccounts.Count ? oldAccounts.Count : newAccounts.Count;
                for (int i = 0; i < accountCount; i++)
                {
                    if (i < oldAccounts.Count && i < newAccounts.Count)
                    {
                        var accCompare = oldAccounts[i].CompareObjectValues(newAccounts[i]);
                        if (accCompare.Item1.Any())
                        {
                            listAccountOld.Add(oldAccounts[i]);
                            listAccountNew.Add(newAccounts[i]);
                        }
                    }
                    else if (i < oldAccounts.Count)
                    {
                        listAccountOld.Add(oldAccounts[i]);
                    }
                    else if (i < newAccounts.Count)
                    {
                        listAccountNew.Add(newAccounts[i]);
                    }
                }

                if (listAccountOld.Any() && listAccountNew.Any())
                {
                    compare.Item1["Accounts"] = listAccountOld.Select(p => new
                    {
                        p.DebtAccountCode,
                        p.CreditAccountCode,
                        p.AmountVnd,
                        p.AmountCny,
                        p.Note
                    }).ToList();
                    compare.Item2["Accounts"] = listAccountNew.Select(p => new
                    {
                        p.DebtAccountCode,
                        p.CreditAccountCode,
                        p.AmountVnd,
                        p.AmountCny,
                        p.Note
                    }).ToList();
                }

                if (compare.Item1.Count > 0)
                {
                    await _entryLogRepository.InsertAsync(new EntryLog()
                    {
                        Action = EntityActions.Update,
                        EntryId = newEntry.Id,
                        ToValue = JsonSerializer.Serialize(compare.Item2),
                        FromValue = JsonSerializer.Serialize(compare.Item1),
                    });
                }
            }
        }

        private async Task ValidateCreate(EntryCreateRequest request)
        {
        }

        private async Task ValidateUpdate(EntryUpdateRequest request)
        {
            var entry = await _entryRepository.GetAsync(p => p.Id == request.Id);
            var now = _clock.Now;
            // cập nhật bút toán trong vòng x ngày được cấu hình trong DayConfuguration
            var userTenantId = _currentUser.TenantId;
            var dayConfiguration = await _dayConfigurationRepository.GetQueryableAsync();
            if (dayConfiguration == null && dayConfiguration.Count() > 0)
            {
                var day = dayConfiguration.FirstOrDefault(x => x.TenantId == userTenantId);
                if (day != null)
                {
                    if ((now - entry.CreationTime).Days > day.DayNumbers)
                    {
                        throw new BusinessException(ErrorMessages.Common.ModifyDeadline, "Bút toán đã quá hạn điều chỉnh");
                    }
                    else if ((now - entry.CreationTime).Days > 60)
                    {
                        throw new BusinessException(ErrorMessages.Common.ModifyDeadline, "Bút toán đã quá hạn điều chỉnh");
                    }
                }
            }
            else if ((now - entry.CreationTime).Days > 60)
            {
                throw new BusinessException(ErrorMessages.Common.ModifyDeadline, "Bút toán đã quá hạn điều chỉnh");
            }

            if (entry.AccountingType == AccountingTypes.Auto)
            {
                throw new BusinessException(ErrorMessages.Common.ModifyAutoGeneratedData, "Vui lòng cập nhật chứng từ gốc để cập nhật bút toán này");
            }
        }

        private async Task CreatePaymentReceipt(Entry entry, List<EntryAccount> entryAccounts)
        {
            var uow = _unitOfWorkManager.Current;
            if (IsTicketTypeCreateReceipt(entry.TicketType))
            {
                foreach (var account in entryAccounts)
                {
                    var accountCode = string.Empty;
                    var reciprocalAccount = string.Empty;

                    if (entry.TicketType == TicketTypes.CreditNote || entry.TicketType == TicketTypes.Receipt)
                    {
                        accountCode = account.DebtAccountCode;
                        reciprocalAccount = account.CreditAccountCode;
                    }
                    if (entry.TicketType == TicketTypes.DebitNote || entry.TicketType == TicketTypes.PaymentVoucher)
                    {
                        accountCode = account.CreditAccountCode;
                        reciprocalAccount = account.DebtAccountCode;
                    }
                    var receipt = new PaymentReceipt
                    {
                        StoreId = entry.StoreId,
                        AccountCode = accountCode,
                        ReciprocalAccountCode = reciprocalAccount,
                        AudienceType = entry.AudienceType,
                        AudienceId = entry.AudienceId,
                        AmountVND = account.AmountVnd,
                        AmountCNY = account.AmountCny,
                        TransactionDate = entry.TransactionDate,
                        Source = ActionSources.ManualCreateEntry,
                        SourceId = entry.Id,
                        DocumentCode = entry.Code,
                        DocumentType = DocumentTypes.Entry,
                        DocumentDetailType = entry.DocumentDetailType,
                        TicketType = entry.TicketType,
                        IsFromWarehousingBill = false,
                        IsFromManualEntry = true,
                        AccountingType = AccountingTypes.Auto
                    };

                    await _paymentReceiptRepository.InsertAsync(receipt);
                }
                await uow.SaveChangesAsync();
                if
                    (
                        (entry.AudienceType == AudienceTypes.SupplierVN || entry.AudienceType == AudienceTypes.SupplierCN)
                        && entry.AudienceId.HasValue
                        && (entry.TicketType == TicketTypes.PaymentVoucher || entry.TicketType == TicketTypes.DebitNote)
                    )
                {
                    await _commonService.TriggerCalculateSupplierOrderReport(entry.AudienceId.Value, entry.TransactionDate.Date);
                }

                await uow.SaveChangesAsync();
            }
        }

        private async Task UpdatePaymentReceiptOnUpdateEntry(Entry oldEntry, Entry newEntry, List<EntryAccount> entryAccounts)
        {
            var uow = _unitOfWorkManager.Current;
            if (IsTicketTypeCreateReceipt(oldEntry.TicketType) && IsTicketTypeCreateReceipt(newEntry.TicketType))
            {
                var receipts = await _paymentReceiptRepository.GetListAsync(p => p.IsFromManualEntry && p.SourceId == oldEntry.Id);
                var updateReceipts = new List<PaymentReceipt>();
                var insertReceipts = new List<PaymentReceipt>();
                var deleteReceipts = new List<PaymentReceipt>();
                foreach (var account in entryAccounts)
                {
                    var oldAccountCode = string.Empty;
                    var oldReciprocalAccount = string.Empty;

                    if (oldEntry.TicketType == TicketTypes.CreditNote || oldEntry.TicketType == TicketTypes.Receipt)
                    {
                        oldAccountCode = account.DebtAccountCode;
                        oldReciprocalAccount = account.CreditAccountCode;
                    }
                    if (oldEntry.TicketType == TicketTypes.DebitNote || oldEntry.TicketType == TicketTypes.PaymentVoucher)
                    {
                        oldAccountCode = account.CreditAccountCode;
                        oldReciprocalAccount = account.DebtAccountCode;
                    }

                    var newAccountCode = string.Empty;
                    var newReciprocalAccount = string.Empty;

                    if (newEntry.TicketType == TicketTypes.CreditNote || newEntry.TicketType == TicketTypes.Receipt)
                    {
                        newAccountCode = account.DebtAccountCode;
                        newReciprocalAccount = account.CreditAccountCode;
                    }
                    if (newEntry.TicketType == TicketTypes.DebitNote || newEntry.TicketType == TicketTypes.PaymentVoucher)
                    {
                        newAccountCode = account.CreditAccountCode;
                        newReciprocalAccount = account.DebtAccountCode;
                    }
                    var existReceipt = receipts.FirstOrDefault(p => p.AccountCode == oldAccountCode);
                    if (existReceipt != null)
                    {
                        existReceipt.AccountCode = newAccountCode;
                        existReceipt.ReciprocalAccountCode = newReciprocalAccount;
                        existReceipt.AudienceType = newEntry.AudienceType;
                        existReceipt.AudienceId = newEntry.AudienceId;
                        existReceipt.AmountVND = account.AmountVnd;
                        existReceipt.AmountCNY = account.AmountCny;
                        existReceipt.TransactionDate = newEntry.TransactionDate;
                        existReceipt.DocumentDetailType = newEntry.DocumentDetailType;
                        existReceipt.TicketType = newEntry.TicketType;

                        updateReceipts.Add(existReceipt);
                    }
                    else
                    {
                        var receipt = new PaymentReceipt
                        {
                            StoreId = newEntry.StoreId,
                            AccountCode = newAccountCode,
                            ReciprocalAccountCode = newReciprocalAccount,
                            AudienceType = newEntry.AudienceType,
                            AudienceId = newEntry.AudienceId,
                            AmountVND = account.AmountVnd,
                            AmountCNY = account.AmountCny,
                            TransactionDate = newEntry.TransactionDate,
                            Source = ActionSources.ManualCreateEntry,
                            SourceId = newEntry.Id,
                            DocumentCode = newEntry.Code,
                            DocumentType = DocumentTypes.Entry,
                            DocumentDetailType = newEntry.DocumentDetailType,
                            TicketType = newEntry.TicketType,
                            IsFromWarehousingBill = false,
                            IsFromManualEntry = true,
                            AccountingType = AccountingTypes.Auto
                        };

                        insertReceipts.Add(receipt);
                    }
                }
                deleteReceipts = receipts.Where(p => !updateReceipts.Any(r => r.Id == p.Id)).ToList();
                if (updateReceipts.Any())
                {
                    await _paymentReceiptRepository.UpdateManyAsync(updateReceipts);
                }
                if (insertReceipts.Any())
                {
                    await _paymentReceiptRepository.InsertManyAsync(insertReceipts);
                }
                if (deleteReceipts.Any())
                {
                    await _paymentReceiptRepository.DeleteManyAsync(deleteReceipts);
                }
                await uow.SaveChangesAsync();
                if
                (
                    (newEntry.AudienceType == AudienceTypes.SupplierVN || newEntry.AudienceType == AudienceTypes.SupplierCN)
                    && newEntry.AudienceId.HasValue
                    && (newEntry.TicketType == TicketTypes.PaymentVoucher || newEntry.TicketType == TicketTypes.DebitNote)
                )
                {
                    await _commonService.TriggerCalculateSupplierOrderReport(newEntry.AudienceId.Value, newEntry.TransactionDate.Date);
                }
                if (
                    (oldEntry.AudienceType == AudienceTypes.SupplierVN || oldEntry.AudienceType == AudienceTypes.SupplierCN)
                    && oldEntry.AudienceId.HasValue
                    && (oldEntry.TicketType == TicketTypes.PaymentVoucher || oldEntry.TicketType == TicketTypes.DebitNote)
                    && oldEntry.TransactionDate != newEntry.TransactionDate
                )
                {
                    await _commonService.TriggerCalculateSupplierOrderReport(oldEntry.AudienceId.Value, oldEntry.TransactionDate.Date);
                }
            }
            else if (IsTicketTypeCreateReceipt(oldEntry.TicketType) && !IsTicketTypeCreateReceipt(newEntry.TicketType))
            {
                await DeletePaymentReceipt(oldEntry);
            }
            else if (!IsTicketTypeCreateReceipt(oldEntry.TicketType) && IsTicketTypeCreateReceipt(newEntry.TicketType))
            {
                await CreatePaymentReceipt(newEntry, entryAccounts);
            }
            await uow.SaveChangesAsync();
        }

        private async Task DeletePaymentReceipt(Entry entry)
        {
            var uow = _unitOfWorkManager.Current;
            var receipts = await _paymentReceiptRepository.GetListAsync(p => p.IsFromManualEntry && p.SourceId == entry.Id);
            await _paymentReceiptRepository.DeleteManyAsync(receipts);
            await uow.SaveChangesAsync();
            if
            (
                (entry.AudienceType == AudienceTypes.SupplierVN || entry.AudienceType == AudienceTypes.SupplierCN)
                && entry.AudienceId.HasValue
                && (entry.TicketType == TicketTypes.PaymentVoucher || entry.TicketType == TicketTypes.DebitNote)
            )
            {
                await _commonService.TriggerCalculateSupplierOrderReport(entry.AudienceId.Value, entry.TransactionDate.Date);
            }
        }

        private bool IsTicketTypeCreateReceipt(TicketTypes type)
        {
            if (type == TicketTypes.DebitNote ||
                type == TicketTypes.CreditNote ||
                type == TicketTypes.Receipt ||
                type == TicketTypes.PaymentVoucher)
            {
                return true;
            }

            return false;
        }

        private string SetTicketName(TicketTypes type)
        {
            switch (type)
            {
                case TicketTypes.Import:
                    return "Phiếu nhập";

                case TicketTypes.Export:
                    return "Phiếu xuất";

                case TicketTypes.DebitNote:
                    return "Báo nợ";

                case TicketTypes.CreditNote:
                    return "Báo có";

                case TicketTypes.Receipt:
                    return "Phiếu thu";

                case TicketTypes.PaymentVoucher:
                    return "Phiếu chi";

                case TicketTypes.ClosingEntry:
                    return "Kết chuyển";

                case TicketTypes.Other:
                    return "Khác";

                case TicketTypes.Sales:
                    return "Bán hàng";

                case TicketTypes.Return:
                    return "Trả hàng";

                default: return null;
            }
        }

        private string SetDocument(DocumentTypes type)
        {
            switch (type)
            {
                case DocumentTypes.SupplierOrder:
                    return "Phiếu đặt hàng";

                case DocumentTypes.InventoryImport:
                    return "Phiếu nhập kho";

                case DocumentTypes.InventoryExport:
                    return "Phiếu xuất kho";

                case DocumentTypes.ShippingNote:
                    return "Phiếu vận chuyển";

                case DocumentTypes.Other:
                    return "Chứng từ ngoài";

                case DocumentTypes.DebitNote:
                    return "Báo nợ";

                case DocumentTypes.CreditNote:
                    return "Báo có";

                case DocumentTypes.Receipt:
                    return "Phiếu thu";

                case DocumentTypes.PaymentVoucher:
                    return "Phiếu chi";

                case DocumentTypes.Entry:
                    return "Bút toán";

                default: return null;
            }
        }

        public async Task AutoCreateEntryForCreatBillCustomer(Guid BillCustomerId, decimal amountAfterDiscount)
        {
            var uow = _unitOfWorkManager.Current;
            var billCustomer = await _billCustomerRepository.FirstOrDefaultAsync(x => x.Id == BillCustomerId);
            var billReturn = await _customerReturnRepository.FirstOrDefaultAsync(x => x.BillCustomerId == BillCustomerId);

            var docType = DocumentTypes.BillCustomer;
            var docCode = billCustomer.Code;
            var sourceId = billCustomer.Id;
            var ticketType = TicketTypes.Sales;
            var entrySource = ActionSources.CreateBillCustomer;
            if (billReturn != null)
            {
                docType = DocumentTypes.ReturnProduct;
                docCode = billReturn.Code;
                sourceId = billReturn.Id;
                ticketType = TicketTypes.Sales;
                entrySource = ActionSources.ReturnProduct;
            }
            var entry = new Entry
            {
                AccountingType = AccountingTypes.Auto,
                Amount = amountAfterDiscount,
                AudienceType = AudienceTypes.Customer,
                AudienceId = billCustomer.CustomerId,
                Currency = Currencies.VND,
                DocumentCode = docCode,
                DocumentType = docType,
                DocumentId = sourceId,
                EntrySource = entrySource,
                SourceCode = docCode,
                SourceId = sourceId,
                StoreId = billCustomer.StoreId.GetValueOrDefault(),
                TransactionDate = DateTime.UtcNow,
                TicketType = ticketType,
                ConfigCode = EntryConfig.BillCustomer.CodeEntry
            };

            await _entryRepository.InsertAsync(entry);
            await uow.SaveChangesAsync();

            var entryAccount = new EntryAccount
            {
                EntryId = entry.Id,
                DebtAccountCode = EntryConfig.BillCustomer.Debt,
                CreditAccountCode = EntryConfig.BillCustomer.Credit,
                AmountVnd = amountAfterDiscount,
            };
            await _entryAccountRepository.InsertAsync(entryAccount);
            await uow.SaveChangesAsync();

            //Sinh công nợ
            await AutoInsertDebt(entry, true);
        }

        public async Task AutoCreateEntryForCreatBillCustomerHasVAT(Guid BillCustomerId, decimal Amount)
        {
            var uow = _unitOfWorkManager.Current;
            var billCustomer = await _billCustomerRepository.FirstOrDefaultAsync(x => x.Id == BillCustomerId);
            var entry = new Entry
            {
                AccountingType = AccountingTypes.Auto,
                Amount = Amount,
                AudienceType = AudienceTypes.Customer,
                AudienceId = billCustomer.CustomerId,
                Currency = Currencies.VND,
                DocumentCode = billCustomer.Code,
                DocumentType = DocumentTypes.BillCustomer,
                DocumentId = BillCustomerId,
                EntrySource = ActionSources.CreateBillCustomer,
                SourceCode = billCustomer.Code,
                SourceId = billCustomer.Id,
                StoreId = billCustomer.StoreId.GetValueOrDefault(),
                TransactionDate = DateTime.UtcNow,
                TicketType = TicketTypes.Export,
                ConfigCode = EntryConfig.BillCustomerHasVat.CodeEntry
            };

            await _entryRepository.InsertAsync(entry);
            await uow.SaveChangesAsync();

            var entryAccount = new EntryAccount
            {
                EntryId = entry.Id,
                DebtAccountCode = EntryConfig.BillCustomerHasVat.Debt,
                CreditAccountCode = EntryConfig.BillCustomerHasVat.Credit,
                AmountVnd = Amount,
            };
            await _entryAccountRepository.InsertAsync(entryAccount);
            await uow.SaveChangesAsync();

            //Sinh công nợ
            await AutoInsertDebt(entry, true);
        }

        public async Task AutoDeleteEntryForBillCustomer(Guid BillCustomerId)
        {
            var entries = await _entryRepository.GetListAsync(x => x.DocumentId == BillCustomerId);

            if (entries.Any())
            {
                foreach (var item in entries)
                {
                    var accountEntry = await _entryAccountRepository.GetListAsync(x => x.EntryId == item.Id);
                    await _entryAccountRepository.DeleteManyAsync(accountEntry);
                    var entryIds = new Guid[] { item.Id };
                    await AutoDeleteDebt(entryIds);
                    await _entryRepository.DeleteAsync(item);

                    await CreateEntryLog(item, null, null, null, true);
                }
            }
        }

        public async Task AutoDeleteEntryForReturnProduct(Guid billId)
        {
            var entries = await _entryRepository.GetListAsync(x => x.DocumentId == billId);

            if (entries.Any())
            {
                foreach (var item in entries)
                {
                    var accountEntry = await _entryAccountRepository.GetListAsync(x => x.EntryId == item.Id);
                    await _entryAccountRepository.DeleteManyAsync(accountEntry);
                    var entryIds = new Guid[] { item.Id };
                    await AutoDeleteDebt(entryIds);
                    await _entryRepository.DeleteAsync(item);

                    await CreateEntryLog(item, null, null, null, true);
                }
            }
        }

        public async Task AutoUpdateEntryForBillCustomer(Guid BillCustomerId)
        {
            var billCustomer = await _billCustomerRepository.FindAsync(x => x.Id == BillCustomerId);

            var entryBillCustomer = await _entryRepository.FindAsync(x => x.DocumentId == BillCustomerId
            && x.ConfigCode == EntryConfig.BillCustomer.CodeEntry);

            if (entryBillCustomer != null)
            {
                var oldProductEntry = entryBillCustomer.Clone();

                entryBillCustomer.StoreId = billCustomer.StoreId.GetValueOrDefault();
                entryBillCustomer.Amount = billCustomer.AmountCustomerPay.GetValueOrDefault();
                entryBillCustomer.AudienceId = billCustomer.CustomerId;

                await _entryRepository.UpdateAsync(entryBillCustomer);
                // update account
                var entryAccounts = await _entryAccountRepository.GetListAsync(p => p.EntryId == entryBillCustomer.Id);
                var oldProductEntryAccounts = entryAccounts.Clone();
                entryAccounts.ForEach(p => p.AmountVnd = entryBillCustomer.Amount);

                await _entryAccountRepository.UpdateManyAsync(entryAccounts);
                await CreateEntryLog(oldProductEntry, oldProductEntryAccounts, entryBillCustomer, entryAccounts, false);
                await AutoUpdateDebt(entryBillCustomer, false);
            }
        }

        public async Task AutoUpdateEntryVatForBillCustomer(Guid BillCustomerId, decimal amount)
        {
            var billCustomer = await _billCustomerRepository.FindAsync(x => x.Id == BillCustomerId);

            var entryBillCustomer = await _entryRepository.FindAsync(x => x.DocumentId == BillCustomerId
            && x.ConfigCode == EntryConfig.BillCustomerHasVat.CodeEntry);

            if (entryBillCustomer != null)
            {
                var oldProductEntry = entryBillCustomer.Clone();

                entryBillCustomer.StoreId = billCustomer.StoreId.GetValueOrDefault();
                entryBillCustomer.Amount = amount;
                entryBillCustomer.AudienceId = billCustomer.CustomerId;

                await _entryRepository.UpdateAsync(entryBillCustomer);
                // update account
                var entryAccounts = await _entryAccountRepository.GetListAsync(p => p.EntryId == entryBillCustomer.Id);
                var oldProductEntryAccounts = entryAccounts.Clone();
                entryAccounts.ForEach(p => p.AmountVnd = entryBillCustomer.Amount);

                await _entryAccountRepository.UpdateManyAsync(entryAccounts);
                await CreateEntryLog(oldProductEntry, oldProductEntryAccounts, entryBillCustomer, entryAccounts, false);
                await AutoUpdateDebt(entryBillCustomer, false);
            }
        }

        public async Task AutoCreateEntryForReturnProduct(Guid CustomerReturnId, decimal amount)
        {
            var uow = _unitOfWorkManager.Current;
            var bill = await _customerReturnRepository.FirstOrDefaultAsync(x => x.Id == CustomerReturnId);
            var entry = new Entry
            {
                AccountingType = AccountingTypes.Auto,
                Amount = amount,
                AudienceType = AudienceTypes.Customer,
                AudienceId = bill.CustomerId,
                Currency = Currencies.VND,
                DocumentCode = bill.Code,
                DocumentType = DocumentTypes.ReturnProduct,
                DocumentId = CustomerReturnId,
                EntrySource = ActionSources.ReturnProduct,
                SourceCode = bill.Code,
                SourceId = bill.Id,
                StoreId = bill.StoreId.GetValueOrDefault(),
                TransactionDate = DateTime.UtcNow,
                TicketType = TicketTypes.Return,
                ConfigCode = EntryConfig.ReturnProduct.Base,
                Note = bill.PayNote
            };
            await _entryRepository.InsertAsync(entry);

            var entryAccount = new EntryAccount
            {
                EntryId = entry.Id,
                DebtAccountCode = EntryConfig.ReturnProduct.Return.Debt,
                CreditAccountCode = EntryConfig.ReturnProduct.Return.Credit,
                AmountVnd = amount,
            };
            await _entryAccountRepository.InsertAsync(entryAccount);
            await uow.SaveChangesAsync();

            //Sinh công nợ
            await AutoInsertDebt(entry, false);
        }
    }
}