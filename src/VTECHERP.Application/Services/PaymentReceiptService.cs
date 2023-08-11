using System;
using System.Collections.Generic;
using System.Linq;
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
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.PaymentReceipt;
using VTECHERP.Entities;
using VTECHERP.Enums;
using VTECHERP.Extensions;

namespace VTECHERP.Services
{
    public class PaymentReceiptService : IPaymentReceiptService
    {
        private readonly IRepository<PaymentReceipt> _paymentReceiptRepository;
        private readonly IRepository<WarehousingBill> _warehousingBillRepository;
        private readonly IEntryService _entryService;
        private readonly IObjectMapper _objectMapper;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IClock _clock;
        private readonly IRepository<Account> _accountRepository;
        private readonly ICommonService _commonService;
        private readonly IIdentityUserRepository _userRepository;
        private readonly IRepository<UserStore> _userStoreRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IRepository<Suppliers> _supplierRepository;
        private readonly IRepository<CustomerReturn> _customerReturnRepository;
        private readonly IAttachmentService _attachmentService;

        public PaymentReceiptService(
            IRepository<PaymentReceipt> paymentReceiptRepository,
            IEntryService entryService,
            IObjectMapper objectMapper,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<WarehousingBill> warehousingBillRepository,
            IClock clock,
            IRepository<Account> accountRepository,
            ICommonService commonService,
            IIdentityUserRepository userRepository,
            IRepository<UserStore> userStoreRepository,
            ICurrentUser currentUser,
            IRepository<Suppliers> supplierRepository,
            IRepository<CustomerReturn> customerReturnRepository,
            IAttachmentService attachmentService)
        {
            _paymentReceiptRepository = paymentReceiptRepository;
            _entryService = entryService;
            _objectMapper = objectMapper;
            _unitOfWorkManager = unitOfWorkManager;
            _warehousingBillRepository = warehousingBillRepository;
            _clock = clock;
            _accountRepository = accountRepository;
            _commonService = commonService;
            _userRepository = userRepository;
            _userStoreRepository = userStoreRepository;
            _currentUser = currentUser;
            _supplierRepository = supplierRepository;
            _customerReturnRepository = customerReturnRepository;
            _attachmentService = attachmentService;
        }

        public async Task<Guid> CreatePaymentReceipt(CreatePaymentReceiptRequest request)
        {
            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                var userStore = (await _userStoreRepository.GetQueryableAsync()).FirstOrDefault(p => p.UserId == _currentUser.Id);
                var account = (await _accountRepository.GetQueryableAsync()).FirstOrDefault(p => p.Id.ToString() == request.AccountCode);
                if (userStore == null)
                {
                    throw new BusinessException();
                }
                var receipt = _objectMapper.Map<CreatePaymentReceiptRequest, PaymentReceipt>(request);
                if (account != null)
                {
                    receipt.AccountCode = account.Code;
                }
                receipt.Source = ActionSources.CreatePaymentReceipt;
                receipt.DocumentCode = receipt.Code;
                receipt.StoreId = userStore.StoreId;
                if (receipt.AudienceType == AudienceTypes.SupplierCN)
                {
                    receipt.ReciprocalAccountCode = EntryConfig.PaymentReceipt.ReciprocalAccountSupplierCN;
                }
                else if (receipt.AudienceType == AudienceTypes.SupplierVN)
                {
                    receipt.ReciprocalAccountCode = EntryConfig.PaymentReceipt.ReciprocalAccountSupplierVN;
                }
                else if (receipt.AudienceType == AudienceTypes.Customer)
                {
                    receipt.ReciprocalAccountCode = EntryConfig.PaymentReceipt.ReciprocalAccountCustomer;
                }
                switch (receipt.TicketType)
                {
                    case TicketTypes.DebitNote:
                        receipt.DocumentType = DocumentTypes.DebitNote;
                        receipt.DocumentDetailType = DocumentDetailType.DebitNote;

                        break;
                    case TicketTypes.CreditNote:
                        receipt.DocumentType = DocumentTypes.CreditNote;
                        receipt.DocumentDetailType = DocumentDetailType.CreditNote;
                        break;
                    case TicketTypes.Receipt:
                        receipt.DocumentType = DocumentTypes.Receipt;
                        receipt.DocumentDetailType = DocumentDetailType.Receipt;
                        break;
                    case TicketTypes.PaymentVoucher:
                        receipt.DocumentType = DocumentTypes.PaymentVoucher;
                        receipt.DocumentDetailType = DocumentDetailType.PaymentVoucher;
                        break;
                    case TicketTypes.FundTransfer:
                        receipt.DocumentType = DocumentTypes.FundTransfer;
                        receipt.DocumentDetailType = DocumentDetailType.FundTransfer;
                        break;
                }
                receipt.AccountingType = AccountingTypes.Manual;
                await _paymentReceiptRepository.InsertAsync(receipt);
                await uow.SaveChangesAsync();
                await _entryService.AutoCreateEntryOnCreatePaymentReceipt(receipt.Id);
                await uow.SaveChangesAsync();
                if
                (
                    (receipt.AudienceType == AudienceTypes.SupplierVN || receipt.AudienceType == AudienceTypes.SupplierCN)
                    && receipt.AudienceId.HasValue
                    && (receipt.TicketType == TicketTypes.PaymentVoucher || receipt.TicketType == TicketTypes.DebitNote)
                )
                {
                    await _commonService.TriggerCalculateSupplierOrderReport(receipt.AudienceId.Value, receipt.TransactionDate.Date);
                }
                await uow.CompleteAsync();
                return receipt.Id;
            }
            catch
            {
                await uow.RollbackAsync();
                throw new BusinessException();
            }
        }

        public async Task UpdatePaymentReceipt(UpdatePaymentReceiptRequest request)
        {
            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                var userStore = (await _userStoreRepository.GetQueryableAsync()).FirstOrDefault(p => p.UserId == _currentUser.Id);
                if (userStore == null)
                {
                    throw new BusinessException();
                }
                var receipt = await _paymentReceiptRepository.GetAsync(p => p.Id == request.Id);
                var oldReceipt = receipt.Clone();
                if (receipt.AccountingType == AccountingTypes.Auto)
                {
                    throw new BusinessException(ErrorMessages.Common.ModifyAutoGeneratedData);
                }
                if (receipt.StoreId != userStore.StoreId)
                {
                    throw new BusinessException();
                }
                if (receipt.AudienceType == AudienceTypes.SupplierCN)
                {
                    receipt.ReciprocalAccountCode = EntryConfig.PaymentReceipt.ReciprocalAccountSupplierCN;
                }
                else if (receipt.AudienceType == AudienceTypes.SupplierVN)
                {
                    receipt.ReciprocalAccountCode = EntryConfig.PaymentReceipt.ReciprocalAccountSupplierVN;
                }
                else if (receipt.AudienceType == AudienceTypes.Customer)
                {
                    receipt.ReciprocalAccountCode = EntryConfig.PaymentReceipt.ReciprocalAccountCustomer;
                }

                receipt.TransactionDate = request.TransactionDate;
                receipt.AccountCode = request.AccountCode;
                receipt.ReciprocalAccountCode = string.IsNullOrEmpty(request.ReciprocalAccountCode) ? receipt.ReciprocalAccountCode : request.ReciprocalAccountCode;
                receipt.AmountCNY = request.AmountCNY;
                receipt.AmountVND = request.AmountVND;
                receipt.Note = request.Note;
                receipt.TicketType = request.TicketType;
                receipt.AudienceType = request.AudienceType;
                receipt.AudienceId = request.AudienceId;

                await _paymentReceiptRepository.UpdateAsync(receipt);
                await uow.SaveChangesAsync();

                await _entryService.AutoUpdateEntryOnUpdatePaymentReceipt(receipt.Id);
                await uow.SaveChangesAsync();
                if
                (
                    (
                        (oldReceipt.AudienceType == AudienceTypes.SupplierVN || oldReceipt.AudienceType == AudienceTypes.SupplierCN)
                        && oldReceipt.AudienceId.HasValue
                        && (oldReceipt.TicketType == TicketTypes.PaymentVoucher || oldReceipt.TicketType == TicketTypes.DebitNote)
                    )
                    ||
                    (
                        (receipt.AudienceType == AudienceTypes.SupplierVN || receipt.AudienceType == AudienceTypes.SupplierCN)
                        && receipt.AudienceId.HasValue
                        && (receipt.TicketType == TicketTypes.PaymentVoucher || receipt.TicketType == TicketTypes.DebitNote)
                    )
                )
                {
                    var olderDate = oldReceipt.TransactionDate.Date < receipt.TransactionDate.Date ? oldReceipt.TransactionDate.Date : receipt.TransactionDate.Date;
                    await _commonService.TriggerCalculateSupplierOrderReport(oldReceipt.AudienceId.Value, oldReceipt.TransactionDate.Date);
                }

                await uow.CompleteAsync();
            }
            catch
            {
                await uow.RollbackAsync();
                throw;
            }
        }

        public async Task<PagingResponse<PaymentReceiptDTO>> SearchPaymentReceipt(SearchPaymentReceiptRequest request)
        {
            var userStores = await _userStoreRepository.GetListAsync(p => p.UserId == _currentUser.Id);
            if (!userStores.Any())
            {
                throw new BusinessException(ErrorMessages.UserStore.StoreNotDefine);
            }
            var userStoreIds = userStores.Select(p => p.StoreId).ToList();
            var paymentReceipts = await _paymentReceiptRepository.GetQueryableAsync();
            var accounts = (await _accountRepository.GetQueryableAsync()).Where(o => !o.IsDeleted).ToList();
            var allUsers = await _userRepository.GetListAsync();
            // find payment receipt with audience id/name
            var entryWithAudience = new List<Guid>();

            if (request.SearchDateFrom.HasValue)
            {
                request.SearchDateFrom = _clock.Normalize(request.SearchDateFrom.Value);
            }
            if (request.SearchDateTo.HasValue)
            {
                request.SearchDateTo = _clock.Normalize(request.SearchDateTo.Value).AddDays(1);
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
            Guid? audienceId = Guid.Empty;
            if (!request.AudienceName.IsNullOrWhiteSpace())
            {
                var audienceSearch = (await _supplierRepository.GetQueryableAsync()).Where(x => x.Code.Contains(request.AudienceName) || x.Name.Contains(request.AudienceName)).First();
                if (audienceSearch != null)
                    audienceId = audienceSearch.Id;
            }
            var query =
                    from receipt in paymentReceipts
                    where
                        (
                            (request.StoreIds == null || request.StoreIds.Count == 0 || request.StoreIds.Contains(receipt.StoreId))
                            && (
                                (request.SearchDateFrom == null || receipt.TransactionDate >= request.SearchDateFrom)
                                && (request.SearchDateTo == null || receipt.TransactionDate <= request.SearchDateTo)
                            )
                            && (request.TicketType == null || request.TicketType.Count == 0 || request.TicketType.Contains(receipt.TicketType))
                            && (
                                request.IsDocument == null
                                || (request.IsDocument == false && (receipt.DocumentCode == null || receipt.DocumentCode == String.Empty))
                                || (request.IsDocument == true && receipt.DocumentCode != null && receipt.DocumentCode != String.Empty)
                            )
                            && (request.DocumentCode.IsNullOrWhiteSpace() || receipt.DocumentCode.Contains(request.DocumentCode))
                            && (request.AccountingType == null || receipt.AccountingType == request.AccountingType)
                            && (request.AudienceType == null || receipt.AudienceType == request.AudienceType)
                            && (request.AudienceId == null || receipt.AudienceId == request.AudienceId)
                            && (request.AccountCode.IsNullOrWhiteSpace() || receipt.AccountCode.Contains(request.AccountCode) || receipt.ReciprocalAccountCode.Contains(request.AccountCode))

                            && (request.DocumentDetailType == null || receipt.DocumentDetailType == request.DocumentDetailType)
                            && (request.Note.IsNullOrWhiteSpace() || (receipt.Note != null && receipt.Note.Contains(request.Note)))
                            && (request.Creator.IsNullOrWhiteSpace() || creatorIds.Contains(receipt.CreatorId ?? Guid.Empty))
                            && (request.PaymentReceiptCode.IsNullOrWhiteSpace() || receipt.Code.Contains(request.PaymentReceiptCode))
                            && (request.AudienceName.IsNullOrWhiteSpace() || audienceId == receipt.AudienceId)
                            && userStoreIds.Contains(receipt.StoreId)
                        )
                    select receipt;
            var paged = query.OrderByDescending(p => p.Code).Skip(request.Offset).Take(request.PageSize).ToList();
            var pagedDto = _objectMapper.Map<List<PaymentReceipt>, List<PaymentReceiptDTO>>(paged);
            var audienceRequests = pagedDto.Where(p => p.AudienceId != null).Select(p =>
            {
                (AudienceTypes, Guid?) item = (p.AudienceType, p.AudienceId);
                return item;
            }).Distinct().ToArray();
            var audiences = await _commonService.GetAudiences(audienceRequests);
            var attachmets = (await _attachmentService.ListAttachmentByObjectIdAsync(pagedDto.Select(x => x.Id.Value).ToList())).OrderBy(x => x.CreationTime).ToList();
            foreach (var item in pagedDto)
            {
                if (item.AudienceId != null)
                {
                    var audience = audiences.FirstOrDefault(p => p.Id == item.AudienceId);
                    if (audience != null)
                    {
                        item.AudienceCode = audience.Code;
                        item.AudienceName = audience.Name;
                        item.AudiencePhone = audience.Phone;
                    }
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

                var account = accounts.FirstOrDefault(x => x.Code == item.AccountCode);
                if (account != null)
                    item.AccountName = account.Name;

                var reciprocalAccount = accounts.FirstOrDefault(x => x.Code == item.ReciprocalAccountCode);
                if (reciprocalAccount != null)
                    item.ReciprocalAccountName = reciprocalAccount.Name;

                item.Attachments = attachmets.Where(x => x.ObjectId == item.Id).ToList() ?? new List<DTOs.Attachment.DetailAttachmentDto>();
            }

            return new PagingResponse<PaymentReceiptDTO>(query.Count(), pagedDto);
        }

        public async Task<List<PaymentReceiptDTO>> SearchPaymentReceiptFilter(SearchPaymentReceiptRequest request)
        {
            var userStores = await _userStoreRepository.GetListAsync(p => p.UserId == _currentUser.Id);
            if (!userStores.Any())
            {
                throw new BusinessException(ErrorMessages.UserStore.StoreNotDefine);
            }
            var userStoreIds = userStores.Select(p => p.StoreId).ToList();
            var paymentReceipts = await _paymentReceiptRepository.GetQueryableAsync();
            var accounts = (await _accountRepository.GetQueryableAsync()).Where(o => !o.IsDeleted).ToList();
            var allUsers = await _userRepository.GetListAsync();
            // find payment receipt with audience id/name
            var entryWithAudience = new List<Guid>();

            if (request.SearchDateFrom.HasValue)
            {
                request.SearchDateFrom = _clock.Normalize(request.SearchDateFrom.Value);
            }
            if (request.SearchDateTo.HasValue)
            {
                request.SearchDateTo = _clock.Normalize(request.SearchDateTo.Value).AddDays(1);
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
            Guid? audienceId = Guid.Empty;
            if (!request.AudienceName.IsNullOrWhiteSpace())
            {
                var audienceSearch = (await _supplierRepository.GetQueryableAsync()).Where(x => x.Code.Contains(request.AudienceName) || x.Name.Contains(request.AudienceName)).First();
                if (audienceSearch != null)
                    audienceId = audienceSearch.Id;
            }
            var query =
                    from receipt in paymentReceipts
                    where
                        (
                            (request.StoreIds == null || request.StoreIds.Count == 0 || request.StoreIds.Contains(receipt.StoreId))
                            && (
                                (request.SearchDateFrom == null || receipt.TransactionDate >= request.SearchDateFrom)
                                && (request.SearchDateTo == null || receipt.TransactionDate <= request.SearchDateTo)
                            )
                            && (request.TicketType == null || request.TicketType.Count == 0 || request.TicketType.Contains(receipt.TicketType))
                            && (
                                request.IsDocument == null
                                || (request.IsDocument == false && (receipt.DocumentCode == null || receipt.DocumentCode == String.Empty))
                                || (request.IsDocument == true && receipt.DocumentCode != null && receipt.DocumentCode != String.Empty)
                            )
                            && (request.DocumentCode.IsNullOrWhiteSpace() || receipt.DocumentCode.Contains(request.DocumentCode))
                            && (request.AccountingType == null || receipt.AccountingType == request.AccountingType)
                            && (request.AudienceType == null || receipt.AudienceType == request.AudienceType)
                            && (request.AudienceId == null || receipt.AudienceId == request.AudienceId)
                            && (request.AccountCode.IsNullOrWhiteSpace() || receipt.AccountCode.Contains(request.AccountCode) || receipt.ReciprocalAccountCode.Contains(request.AccountCode))

                            && (request.DocumentDetailType == null || receipt.DocumentDetailType == request.DocumentDetailType)
                            && (request.Note.IsNullOrWhiteSpace() || (receipt.Note != null && receipt.Note.Contains(request.Note)))
                            && (request.Creator.IsNullOrWhiteSpace() || creatorIds.Contains(receipt.CreatorId ?? Guid.Empty))
                            && (request.PaymentReceiptCode.IsNullOrWhiteSpace() || receipt.Code.Contains(request.PaymentReceiptCode))
                            && (request.AudienceName.IsNullOrWhiteSpace() || audienceId == receipt.AudienceId)
                            && userStoreIds.Contains(receipt.StoreId)
                        )
                    select receipt;
            var data = query.ToList();
            var pagedDto = _objectMapper.Map<List<PaymentReceipt>, List<PaymentReceiptDTO>>(data);
            var audienceRequests = pagedDto.Where(p => p.AudienceId != null).Select(p =>
            {
                (AudienceTypes, Guid?) item = (p.AudienceType, p.AudienceId);
                return item;
            }).Distinct().ToArray();
            var audiences = await _commonService.GetAudiences(audienceRequests);
            foreach (var item in pagedDto)
            {
                if (item.AudienceId != null)
                {
                    var audience = audiences.FirstOrDefault(p => p.Id == item.AudienceId);
                    if (audience != null)
                    {
                        item.AudienceCode = audience.Code;
                        item.AudienceName = audience.Name;
                        item.AudiencePhone = audience.Phone;
                    }
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

                var account = accounts.FirstOrDefault(x => x.Code == item.AccountCode);
                if (account != null)
                    item.AccountName = account.Name;

                var reciprocalAccount = accounts.FirstOrDefault(x => x.Code == item.ReciprocalAccountCode);
                if (reciprocalAccount != null)
                    item.ReciprocalAccountName = reciprocalAccount.Name;
            }

            return pagedDto;
        }

        public async Task<PaymentReceiptDTO> GetPaymentReceipt(Guid id)
        {
            var userStores = await _userStoreRepository.GetListAsync(p => p.UserId == _currentUser.Id);
            if (!userStores.Any())
            {
                throw new BusinessException(ErrorMessages.UserStore.StoreNotDefine);
            }
            var userStoreIds = userStores.Select(p => p.StoreId).ToList();
            var paymentReceipt = await _paymentReceiptRepository.GetAsync(p => p.Id == id);
            if (!userStoreIds.Contains(paymentReceipt.StoreId))
            {
                throw new BusinessException(ErrorMessages.UserStore.StoreNotDefine);
            }
            var receiptDto = _objectMapper.Map<PaymentReceipt, PaymentReceiptDTO>(paymentReceipt);

            if (receiptDto.AudienceId != null)
            {
                var audience = await _commonService.GetAudience(receiptDto.AudienceType, receiptDto.AudienceId);
                if (audience != null)
                {
                    receiptDto.AudienceCode = audience.Code;
                    receiptDto.AudienceName = audience.Name;
                    receiptDto.AudiencePhone = audience.Phone;
                }
            }

            receiptDto.IsEditable = receiptDto.AccountingType == AccountingTypes.Manual;

            receiptDto.Attachments = await _attachmentService.GetAttachmentByObjectIdAsync(id);

            return receiptDto;
        }

        public async Task DeletePaymentReceipt(Guid id)
        {
            using var uow = _unitOfWorkManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                var userStore = (await _userStoreRepository.GetQueryableAsync()).FirstOrDefault(p => p.UserId == _currentUser.Id);
                if (userStore == null)
                {
                    throw new BusinessException();
                }

                var receipt = await _paymentReceiptRepository.GetAsync(p => p.Id == id);
                if (receipt.StoreId != userStore.StoreId)
                {
                    throw new BusinessException("400", "Người tạo và người thực hiện xóa không cùng cửa hàng");
                }

                await _entryService.AutoDeleteEntryOnDeletePaymentReceipt(receipt.Id);
                await _paymentReceiptRepository.DeleteAsync(receipt);
                await uow.SaveChangesAsync();
                if
                    (
                        (receipt.AudienceType == AudienceTypes.SupplierVN || receipt.AudienceType == AudienceTypes.SupplierCN)
                        && receipt.AudienceId.HasValue
                        && (receipt.TicketType == TicketTypes.PaymentVoucher || receipt.TicketType == TicketTypes.DebitNote)
                    )
                {
                    await _commonService.TriggerCalculateSupplierOrderReport(receipt.AudienceId.Value, receipt.TransactionDate.Date);
                }

                await uow.CompleteAsync();
            }
            catch
            {
                await uow.RollbackAsync();
                throw;
            }
        }
        // tự động tạo phiếu thu/chi
        public async Task AutoCreatePaymentReceiptOnWarehousingBill(Guid billId)
        {
            var uow = _unitOfWorkManager.Current;
            var bill = await _warehousingBillRepository.GetAsync(p => p.Id == billId);

            var paymentReceiptByBillIds = await _paymentReceiptRepository.GetListAsync(p => p.IsFromWarehousingBill && p.SourceId == bill.Id);

            if (bill.BillType == WarehousingBillType.Import)
            {
                var currentCashPaymentReceipt = paymentReceiptByBillIds.FirstOrDefault(p => p.TicketType == TicketTypes.PaymentVoucher);
                if (bill.CashPaymentAccountCode != null && bill.CashPaymentAmount != null && bill.CashPaymentAmount > 0)
                {
                    if (currentCashPaymentReceipt != null)
                    {
                        currentCashPaymentReceipt.AmountVND = bill.CashPaymentAmount ?? 0;
                        currentCashPaymentReceipt.TransactionDate = bill.CashPaymentHaveValueDate.Value;
                        currentCashPaymentReceipt.AudienceId = bill.AudienceId;

                        await _paymentReceiptRepository.UpdateAsync(currentCashPaymentReceipt);
                        await _entryService.AutoUpdateEntryOnUpdatePaymentReceipt(currentCashPaymentReceipt.Id);
                    }
                    else
                    {
                        var reciprocalAccount = string.Empty;
                        if (bill.AudienceType == AudienceTypes.SupplierVN)
                        {
                            reciprocalAccount = EntryConfig.WarehousingImport.Supplier.CashPay.Debt;
                        }
                        else if (bill.AudienceType == AudienceTypes.Customer)
                        {
                            reciprocalAccount = EntryConfig.WarehousingImport.Customer.CashPay.Debt;
                        }
                        var cashPaymentReceipt = new PaymentReceipt
                        {
                            StoreId = bill.StoreId,
                            AccountCode = bill.CashPaymentAccountCode,
                            ReciprocalAccountCode = reciprocalAccount,
                            AudienceType = bill.AudienceType,
                            AudienceId = bill.AudienceId,
                            AmountVND = bill.CashPaymentAmount ?? 0,
                            TransactionDate = bill.CashPaymentHaveValueDate.Value,
                            Source = ActionSources.WarehousingBillCreate,
                            SourceId = bill.Id,
                            DocumentCode = bill.Code,
                            DocumentType = DocumentTypes.InventoryImport,
                            DocumentDetailType = bill.DocumentDetailType,
                            TicketType = TicketTypes.PaymentVoucher,
                            IsFromWarehousingBill = true,
                            AccountingType = AccountingTypes.Auto
                        };

                        await _paymentReceiptRepository.InsertAsync(cashPaymentReceipt);
                        await uow.SaveChangesAsync();
                        await _entryService.AutoCreateEntryOnCreatePaymentReceipt(cashPaymentReceipt.Id);
                    }
                }
                else
                {
                    if (currentCashPaymentReceipt != null)
                    {
                        await _paymentReceiptRepository.DeleteAsync(currentCashPaymentReceipt);
                        await _entryService.AutoDeleteEntryOnDeletePaymentReceipt(currentCashPaymentReceipt.Id);
                    }
                }

                var currentBankPaymentReceipt = paymentReceiptByBillIds.FirstOrDefault(p => p.TicketType == TicketTypes.DebitNote);
                if (bill.BankPaymentAccountCode != null && bill.BankPaymentAmount != null && bill.BankPaymentAmount > 0)
                {
                    if (currentBankPaymentReceipt != null)
                    {
                        currentBankPaymentReceipt.AmountVND = bill.BankPaymentAmount ?? 0;
                        currentBankPaymentReceipt.TransactionDate = bill.BankPaymentHaveValueDate.Value;
                        currentBankPaymentReceipt.AudienceId = bill.AudienceId;

                        await _paymentReceiptRepository.UpdateAsync(currentBankPaymentReceipt);
                        await _entryService.AutoUpdateEntryOnUpdatePaymentReceipt(currentBankPaymentReceipt.Id);
                    }
                    else
                    {
                        var reciprocalAccount = string.Empty;
                        if (bill.AudienceType == AudienceTypes.SupplierVN)
                        {
                            reciprocalAccount = EntryConfig.WarehousingImport.Supplier.BankPay.Debt;
                        }
                        else if (bill.AudienceType == AudienceTypes.Customer)
                        {
                            reciprocalAccount = EntryConfig.WarehousingImport.Customer.BankPay.Debt;
                        }

                        var bankPaymentReceipt = new PaymentReceipt
                        {
                            StoreId = bill.StoreId,
                            AccountCode = bill.BankPaymentAccountCode,
                            ReciprocalAccountCode = reciprocalAccount,
                            AudienceType = bill.AudienceType,
                            AudienceId = bill.AudienceId,
                            AmountVND = bill.BankPaymentAmount ?? 0,
                            TransactionDate = bill.BankPaymentHaveValueDate.Value,
                            Source = ActionSources.WarehousingBillCreate,
                            SourceId = bill.Id,
                            DocumentCode = bill.Code,
                            DocumentType = DocumentTypes.InventoryImport,
                            DocumentDetailType = bill.DocumentDetailType,
                            TicketType = TicketTypes.DebitNote,
                            IsFromWarehousingBill = true,
                            AccountingType = AccountingTypes.Auto
                        };

                        await _paymentReceiptRepository.InsertAsync(bankPaymentReceipt);
                        await uow.SaveChangesAsync();
                        await _entryService.AutoCreateEntryOnCreatePaymentReceipt(bankPaymentReceipt.Id);
                    }
                }
                else
                {
                    if (currentBankPaymentReceipt != null)
                    {
                        await _paymentReceiptRepository.DeleteAsync(currentBankPaymentReceipt);
                        await _entryService.AutoDeleteEntryOnDeletePaymentReceipt(currentBankPaymentReceipt.Id);
                    }
                }
            }
            else
            {
                var currentCashPaymentReceipt = paymentReceiptByBillIds.FirstOrDefault(p => p.TicketType == TicketTypes.Receipt);
                if (bill.CashPaymentAccountCode != null && bill.CashPaymentAmount != null && bill.CashPaymentAmount > 0)
                {
                    if (currentCashPaymentReceipt != null)
                    {
                        currentCashPaymentReceipt.AmountVND = bill.CashPaymentAmount ?? 0;
                        currentCashPaymentReceipt.TransactionDate = bill.CashPaymentHaveValueDate.Value;
                        currentCashPaymentReceipt.AudienceId = bill.AudienceId;

                        await _paymentReceiptRepository.UpdateAsync(currentCashPaymentReceipt);
                        await _entryService.AutoUpdateEntryOnUpdatePaymentReceipt(currentCashPaymentReceipt.Id);
                    }
                    else
                    {
                        var reciprocalAccount = string.Empty;
                        if (bill.AudienceType == AudienceTypes.SupplierVN)
                        {
                            reciprocalAccount = EntryConfig.WarehousingExport.SupplierVN.CashPay.Credit;
                        }
                        var cashPaymentReceipt = new PaymentReceipt
                        {
                            StoreId = bill.StoreId,
                            AccountCode = bill.CashPaymentAccountCode,
                            ReciprocalAccountCode = reciprocalAccount,
                            AudienceType = bill.AudienceType,
                            AudienceId = bill.AudienceId,
                            AmountVND = bill.CashPaymentAmount ?? 0,
                            TransactionDate = bill.CashPaymentHaveValueDate.Value,
                            Source = ActionSources.WarehousingBillCreate,
                            SourceId = bill.Id,
                            DocumentCode = bill.Code,
                            DocumentType = DocumentTypes.InventoryExport,
                            DocumentDetailType = bill.DocumentDetailType,
                            TicketType = TicketTypes.Receipt,
                            IsFromWarehousingBill = true,
                            AccountingType = AccountingTypes.Auto
                        };

                        await _paymentReceiptRepository.InsertAsync(cashPaymentReceipt);
                        await uow.SaveChangesAsync();
                        await _entryService.AutoCreateEntryOnCreatePaymentReceipt(cashPaymentReceipt.Id);
                    }
                }
                else
                {
                    if (currentCashPaymentReceipt != null)
                    {
                        await _paymentReceiptRepository.DeleteAsync(currentCashPaymentReceipt);
                        await _entryService.AutoDeleteEntryOnDeletePaymentReceipt(currentCashPaymentReceipt.Id);
                    }
                }

                var currentBankPaymentReceipt = paymentReceiptByBillIds.FirstOrDefault(p => p.TicketType == TicketTypes.CreditNote);
                if (bill.BankPaymentAccountCode != null && bill.BankPaymentAmount != null && bill.BankPaymentAmount > 0)
                {
                    if (currentBankPaymentReceipt != null)
                    {
                        currentBankPaymentReceipt.AmountVND = bill.BankPaymentAmount ?? 0;
                        currentBankPaymentReceipt.TransactionDate = bill.BankPaymentHaveValueDate.Value;
                        currentBankPaymentReceipt.AudienceId = bill.AudienceId;

                        await _paymentReceiptRepository.UpdateAsync(currentBankPaymentReceipt);
                        await _entryService.AutoUpdateEntryOnUpdatePaymentReceipt(currentBankPaymentReceipt.Id);
                    }
                    else
                    {
                        var reciprocalAccount = string.Empty;
                        if (bill.AudienceType == AudienceTypes.SupplierVN)
                        {
                            reciprocalAccount = EntryConfig.WarehousingExport.SupplierVN.BankPay.Credit;
                        }
                        var bankPaymentReceipt = new PaymentReceipt
                        {
                            StoreId = bill.StoreId,
                            AccountCode = bill.BankPaymentAccountCode,
                            ReciprocalAccountCode = reciprocalAccount,
                            AudienceType = bill.AudienceType,
                            AudienceId = bill.AudienceId,
                            AmountVND = bill.BankPaymentAmount ?? 0,
                            TransactionDate = bill.BankPaymentHaveValueDate.Value,
                            Source = ActionSources.WarehousingBillCreate,
                            SourceId = bill.Id,
                            DocumentCode = bill.Code,
                            DocumentType = DocumentTypes.InventoryExport,
                            DocumentDetailType = bill.DocumentDetailType,
                            TicketType = TicketTypes.CreditNote,
                            IsFromWarehousingBill = true,
                            AccountingType = AccountingTypes.Auto
                        };

                        await _paymentReceiptRepository.InsertAsync(bankPaymentReceipt);
                        await uow.SaveChangesAsync();
                        await _entryService.AutoCreateEntryOnCreatePaymentReceipt(bankPaymentReceipt.Id);
                    }
                }
                else
                {
                    if (currentBankPaymentReceipt != null)
                    {
                        await _paymentReceiptRepository.DeleteAsync(currentBankPaymentReceipt);
                        await _entryService.AutoDeleteEntryOnDeletePaymentReceipt(currentBankPaymentReceipt.Id);
                    }
                }
            }
        }

        public async Task AutoDeletePaymentReceiptOnWarehousingBill(Guid billId)
        {
            var uow = _unitOfWorkManager.Current;
            var paymentReceiptByBillIds = await _paymentReceiptRepository.GetListAsync(p => p.IsFromWarehousingBill && p.SourceId == billId);
            foreach (var receipt in paymentReceiptByBillIds)
            {
                await _entryService.AutoDeleteEntryOnDeletePaymentReceipt(receipt.Id);
            }
            await _paymentReceiptRepository.DeleteManyAsync(paymentReceiptByBillIds);

            await uow.SaveChangesAsync();
        }

        public async Task AutoCreatePaymentReceiptForReturnProduct(Guid id)
        {
            var uow = _unitOfWorkManager.Current;
            var bill = await _customerReturnRepository.GetAsync(x => x.Id == id);

            List<PaymentReceipt> lst = new List<PaymentReceipt>();

            if (bill != null)
            {
                PaymentReceipt newPaymentReceiptPayment = null;
                if (bill.Cash != null && bill.Cash != 0)
                {
                    newPaymentReceiptPayment = new PaymentReceipt()
                    {
                        StoreId = bill.StoreId.Value,
                        AccountCode = bill.AccountCode,
                        ReciprocalAccountCode = EntryConfig.ReturnProduct.Return.Credit,
                        AudienceType = AudienceTypes.Customer,
                        AudienceId = bill.CustomerId,
                        AmountVND = bill.Cash ?? 0,
                        TransactionDate = bill.CreationTime,
                        Source = ActionSources.ReturnProduct,
                        SourceId = bill.Id,
                        DocumentCode = bill.Code,
                        DocumentType = DocumentTypes.ReturnProduct,
                        DocumentDetailType = DocumentDetailType.ReturnProduct,
                        TicketType = TicketTypes.PaymentVoucher,
                        IsFromWarehousingBill = false,
                        AccountingType = AccountingTypes.Auto,
                        Note = bill.PayNote
                    };
                    lst.Add(newPaymentReceiptPayment);
                }

                PaymentReceipt newPaymentReceiptDebitNote = null;
                if (bill.Banking != null && bill.Banking != 0)
                {
                    newPaymentReceiptDebitNote = new PaymentReceipt()
                    {
                        StoreId = bill.StoreId.Value,
                        AccountCode = bill.AccountCodeBanking,
                        ReciprocalAccountCode = EntryConfig.ReturnProduct.Return.Credit,
                        AudienceType = AudienceTypes.Customer,
                        AudienceId = bill.CustomerId,
                        AmountVND = bill.Banking ?? 0,
                        TransactionDate = bill.CreationTime,
                        Source = ActionSources.ReturnProduct,
                        SourceId = bill.Id,
                        DocumentCode = bill.Code,
                        DocumentType = DocumentTypes.ReturnProduct,
                        DocumentDetailType = DocumentDetailType.ReturnProduct,
                        TicketType = TicketTypes.DebitNote,
                        IsFromWarehousingBill = false,
                        AccountingType = AccountingTypes.Auto,
                        Note = bill.PayNote
                    };

                    lst.Add(newPaymentReceiptDebitNote);
                }

                if (lst.Count > 0)
                {
                    await _paymentReceiptRepository.InsertManyAsync(lst);
                    await uow.SaveChangesAsync();
                }
                if ((bill.Cash != null && bill.Cash != 0) || newPaymentReceiptPayment != null)
                    await _entryService.AutoCreateEntryOnCreatePaymentReceipt(newPaymentReceiptPayment.Id);
                if ((bill.Banking != null && bill.Banking != 0) || newPaymentReceiptDebitNote != null)
                    await _entryService.AutoCreateEntryOnCreatePaymentReceipt(newPaymentReceiptDebitNote.Id);

            }

        }

        public async Task AutoDeletePaymentReceiptForReturnProduct(Guid id)
        {
            var listPayment = await _paymentReceiptRepository.GetListAsync(x => x.SourceId == id);

            listPayment.ForEach(x =>
            {
                _entryService.AutoDeleteEntryOnDeletePaymentReceipt(x.Id);
            });

        }

        public async Task<bool> ChangeLiquidity(Guid id)
        {
            var paymentReceipt = (await _paymentReceiptRepository.FirstOrDefaultAsync(x => x.Id == id));
            if (paymentReceipt == null)
                return false;

            paymentReceipt.IsLiquidity = !paymentReceipt.IsLiquidity;

            await _paymentReceiptRepository.UpdateAsync(paymentReceipt);

            return true;
        }

        public async Task<byte[]> ExportPaymentReceipt(SearchPaymentReceiptRequest request)
        {
            var data = await SearchPaymentReceiptFilter(request);
            var exportData = new List<ExportPaymentReceiptResponse>();
            foreach (var item in data)
            {
                exportData.Add(new ExportPaymentReceiptResponse
                {
                    Code = item.Code,
                    TransactionDate = item.TransactionDate.ToString("dd-MM-yyyy"),
                    AccountCode = item.AccountCode,
                    AccountName = item.AccountName,
                    ReciprocalAccountCode = item.ReciprocalAccountCode,
                    ReciprocalAccountName = item.ReciprocalAccountName,
                    TicketType = item.TicketTypeName,
                    Audience = item.AudienceTypeName + " - " + item.AudienceCode + " - " + item.AudienceName + " - " + item.AudiencePhone,
                    DocumentTypeName = item.DocumentTypeName,
                    DocumentType = item.DocumentCode,
                    RecieveAmount = (item.TicketType == TicketTypes.Receipt || item.TicketType == TicketTypes.CreditNote) ? (item.AmountVND.HasValue ? item.AmountVND.Value : 0 ) : 0,
                    PaymentAmount = (item.TicketType == TicketTypes.PaymentVoucher || item.TicketType == TicketTypes.DebitNote) ? item.AmountVND.Value : 0,
                    Note = item.Note,
                });
            }
            return ExcelHelper.ExportExcel(exportData);
        }
    }
}
