using DocumentFormat.OpenXml.Spreadsheet;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vinpearl.Modelling.Library.Utility.Excel;
using Volo.Abp;
using Volo.Abp.Data;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Timing;
using Volo.Abp.Users;
using VTECHERP.Constants;
using VTECHERP.Datas;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.BillCustomers.Respons;
using VTECHERP.DTOs.WarehousingBillLogs;
using VTECHERP.DTOs.WarehousingBills;
using VTECHERP.Entities;
using static Volo.Abp.Identity.Settings.IdentitySettingNames;

namespace VTECHERP.Services
{
    internal class WarehousingBillLogsService : IWarehousingBillLogsService
    {
        private readonly IDataFilter _dataFilter;
        private readonly IRepository<WarehousingBillLogs> _warehousingBillLogsRepository;
        private readonly IRepository<WarehousingBill> _warehousingBillRepository;
        private readonly IObjectMapper _mapper;
        private readonly IIdentityUserRepository _userRepository;
        private readonly IClock _clock;
        private readonly ICurrentUser _currentUser;
        private readonly IRepository<Stores> _storeRepository;
        private readonly IRepository<UserStore> _userStoreRepository;

        public WarehousingBillLogsService(
            IDataFilter dataFilter,
            IRepository<WarehousingBillLogs> warehousingBillLogsRepository,
            IRepository<WarehousingBill> warehousingBillRepository,
            IObjectMapper mapper,
            IIdentityUserRepository userRepository,
            IClock clock,
            ICurrentUser currentUser,
            IRepository<Stores> storeRepository,
            IRepository<UserStore> userStoreRepository
            )
        {
            _dataFilter = dataFilter;
            _warehousingBillLogsRepository = warehousingBillLogsRepository;
            _warehousingBillRepository = warehousingBillRepository;
            _mapper = mapper;
            _userRepository = userRepository;
            _clock = clock;
            _currentUser = currentUser;
            _storeRepository = storeRepository;
            _userStoreRepository = userStoreRepository;
        }

        public async Task<byte[]> ExportWarehousingBillLog(SearchWarehousingBillLogsRequest request)
        {
            var allUsers = await _userRepository.GetListAsync();
            var currentUserId = _currentUser.Id;
            var creatorIdSearch = new Guid();
            if (request.FromDate != null)
            {
                request.FromDate = _clock.Normalize(request.FromDate.Value);
            }
            if (request.ToDate != null)
            {
                request.ToDate = _clock.Normalize(request.ToDate.Value);
            }

            if (request.FromDateBill != null)
            {
                request.FromDateBill = _clock.Normalize(request.FromDateBill.Value);
            }
            if (request.ToDateBill != null)
            {
                request.ToDateBill = _clock.Normalize(request.ToDateBill.Value);
            }
            if (!request.Creator.IsNullOrEmpty())
            {
                var user = allUsers.FirstOrDefault(p => p.UserName.ToLower().Contains(request.Creator.ToLower()));
                if (user != null)
                    creatorIdSearch = user.Id;
            }

            using (_dataFilter.Disable<ISoftDelete>())
            {
                var query =
                    from billLogs in _warehousingBillLogsRepository.GetQueryableAsync().Result.AsQueryable()
                    join bill in _warehousingBillRepository.GetQueryableAsync().Result.AsQueryable() on billLogs.WarehousingBillId equals bill.Id
                    join store in _storeRepository.GetQueryableAsync().Result.AsQueryable() on bill.StoreId equals store.Id
                    join userstore in _userStoreRepository.GetQueryableAsync().Result.Where(x => x.UserId == currentUserId).AsQueryable() on store.Id equals userstore.StoreId
                    orderby billLogs.CreationTime descending
                    where
                        (request.StoreIds == null || request.StoreIds.Count == 0 || request.StoreIds.Contains(bill.StoreId))
                        && (request.BillCode.IsNullOrWhiteSpace() || bill.Code.Contains(request.BillCode))
                        && (request.Action == null || billLogs.Action == request.Action)
                        && (request.BillType == null || bill.BillType == request.BillType)
                        && (request.DocumentDetailType == null || request.DocumentDetailType.Count == 0 || (request.DocumentDetailType.Contains(bill.DocumentDetailType)))
                        && (request.FromDate == null || bill.CreationTime >= request.FromDate)
                        && (request.ToDate == null || bill.CreationTime <= request.ToDate)
                        && (request.FromDateBill == null || billLogs.CreationTime >= request.FromDateBill)
                        && (request.ToDateBill == null || billLogs.CreationTime <= request.ToDateBill)
                        && (request.Creator.IsNullOrEmpty() || billLogs.CreatorId == creatorIdSearch)
                    select billLogs;
                var lstWarehousingBillLog = query.ToList();
                var result = _mapper.Map<List<WarehousingBillLogs>, List<WarehousingBillLogsDTO>>(lstWarehousingBillLog);

                var listBillId = query.Select(x => x.WarehousingBillId).ToList();
                var listBillDTO = _mapper.Map<List<WarehousingBill>, List<WarehousingBillDto>>(_warehousingBillRepository.GetListAsync(p => listBillId.Contains(p.Id)).Result.ToList());

                foreach (var item in result)
                {
                    var billDTO = listBillDTO.Where(x => x.Id == item.WarehousingBillId).First();
                    billDTO.DocumentDetailTypeName = DocumentDetailTypeData.Datas.FirstOrDefault(x => x.DocumentDetailType == billDTO.DocumentDetailType).Name;
                    billDTO.BillTypeName = WarehousingBillTypeData.Datas.FirstOrDefault(x => x.BillType == billDTO.BillType).Name;
                    item.WarehousingBillDto = billDTO;
                    item.ActionName = EntityActionsData.Datas.FirstOrDefault(x => x.Action == item.Action).Name;
                    item.Creator = allUsers.FirstOrDefault(p => p.Id == item.CreatorId).UserName;
                }
                var exportData = new List<ExportWarehousingBillLogResponse>();
                foreach (var item in result)
                {
                    if (string.IsNullOrEmpty(item.FromValue))
                    {
                        continue;
                    }
                    else
                    {
                        var warehousingBillDto = JsonConvert.DeserializeObject<WarehousingBillProductDto>(item.FromValue);
                        exportData.Add(new ExportWarehousingBillLogResponse
                        {
                            Code = item.WarehousingBillDto.Code,
                            ProductCode = warehousingBillDto.ProductCode,
                            ActionName = item.ActionName,
                            BillTypeName = item.WarehousingBillDto.BillTypeName,
                            DocumentDetailTypeName = item.WarehousingBillDto.DocumentDetailTypeName,
                            ProductName = warehousingBillDto.ProductName,
                            CurrentStockQuantity = warehousingBillDto.CurrentStockQuantity,
                            TotalPriceBeforeTax = item.WarehousingBillDto.TotalPriceBeforeTax,
                            UserAction = item.ActionName,
                            CreationTime = item.CreationTime.Value.ToString("dd-MM-yyyy")
                        });
                    }
                }
                return ExcelHelper.ExportExcel(exportData);
            }
        }

        public async Task<WarehousingBillLogsDTO> GetById(Guid id)
        {
            var entity = await _warehousingBillLogsRepository.GetAsync(p => p.Id == id);
            if (entity == null)
            {
                throw new BusinessException(ErrorMessages.WarehousingBillLogs.NotExist);
            }
            var objectDTO = _mapper.Map<WarehousingBillLogs, WarehousingBillLogsDTO>(entity);
            var billDTO = _mapper.Map<WarehousingBill, WarehousingBillDto>(await _warehousingBillRepository.GetAsync(p => p.Id == objectDTO.WarehousingBillId));
            objectDTO.WarehousingBillDto = billDTO;
            return objectDTO;
        }

        public async Task<PagingResponse<WarehousingBillLogsDTO>> SearchBillLogs(SearchWarehousingBillLogsRequest request)
        {
            var allUsers = await _userRepository.GetListAsync();
            var currentUserId = _currentUser.Id;
            var creatorIdSearch = new Guid();
            if (request.FromDate != null)
            {
                request.FromDate = _clock.Normalize(request.FromDate.Value);
            }
            if (request.ToDate != null)
            {
                request.ToDate = _clock.Normalize(request.ToDate.Value);
            }

            if (request.FromDateBill != null)
            {
                request.FromDateBill = _clock.Normalize(request.FromDateBill.Value);
            }
            if (request.ToDateBill != null)
            {
                request.ToDateBill = _clock.Normalize(request.ToDateBill.Value);
            }
            if (!request.Creator.IsNullOrEmpty())
            {
                var user = allUsers.FirstOrDefault(p => p.UserName.ToLower().Contains(request.Creator.ToLower()));
                if (user != null) 
                    creatorIdSearch = user.Id;
            }

            using (_dataFilter.Disable<ISoftDelete>())
            {
                var query =
                    from billLogs in _warehousingBillLogsRepository.GetQueryableAsync().Result.AsQueryable()
                    join bill in _warehousingBillRepository.GetQueryableAsync().Result.AsQueryable() on billLogs.WarehousingBillId equals bill.Id
                    join store in _storeRepository.GetQueryableAsync().Result.AsQueryable() on bill.StoreId equals store.Id
                    join userstore in _userStoreRepository.GetQueryableAsync().Result.Where(x => x.UserId == currentUserId).AsQueryable() on store.Id equals userstore.StoreId
                    orderby billLogs.CreationTime descending
                    where
                        (request.StoreIds == null || request.StoreIds.Count == 0 || request.StoreIds.Contains(bill.StoreId))
                        && (request.BillCode.IsNullOrWhiteSpace() || bill.Code.Contains(request.BillCode))
                        && (request.Action == null || billLogs.Action == request.Action)
                        && (request.BillType == null || bill.BillType == request.BillType)
                        && (request.DocumentDetailType == null || request.DocumentDetailType.Count == 0 || (request.DocumentDetailType.Contains(bill.DocumentDetailType)))
                        && (request.FromDate == null || bill.CreationTime >= request.FromDate)
                        && (request.ToDate == null || bill.CreationTime <= request.ToDate)
                        && (request.FromDateBill == null || billLogs.CreationTime >= request.FromDateBill)
                        && (request.ToDateBill == null || billLogs.CreationTime <= request.ToDateBill)
                        && (request.Creator.IsNullOrEmpty() || billLogs.CreatorId == creatorIdSearch)
                    select billLogs;
                var paged = query
                .Skip(request.Offset)
                .Take(request.PageSize)
                .ToList();
                var pagedDto = _mapper.Map<List<WarehousingBillLogs>, List<WarehousingBillLogsDTO>>(paged);

                var listBillId = paged.Select(x => x.WarehousingBillId).ToList();
                var listBillDTO = _mapper.Map<List<WarehousingBill>, List<WarehousingBillDto>>(_warehousingBillRepository.GetListAsync(p => listBillId.Contains(p.Id)).Result.ToList());

                foreach (var item in pagedDto)
                {
                    var billDTO = listBillDTO.Where(x => x.Id == item.WarehousingBillId).First();
                    billDTO.DocumentDetailTypeName = DocumentDetailTypeData.Datas.FirstOrDefault(x => x.DocumentDetailType == billDTO.DocumentDetailType).Name;
                    billDTO.BillTypeName = WarehousingBillTypeData.Datas.FirstOrDefault(x => x.BillType == billDTO.BillType).Name;
                    item.WarehousingBillDto = billDTO;
                    item.ActionName = EntityActionsData.Datas.FirstOrDefault(x => x.Action == item.Action).Name;
                    item.Creator = allUsers.FirstOrDefault(p => p.Id == item.CreatorId).UserName;
                }

                return new PagingResponse<WarehousingBillLogsDTO>(query.Count(), pagedDto);
            }
        }
    }
}
