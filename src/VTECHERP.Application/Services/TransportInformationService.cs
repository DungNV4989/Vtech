using DocumentFormat.OpenXml.Drawing.Charts;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.VariantTypes;
using System;
using System.Collections.Generic;
using System.Linq;
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
using VTECHERP.DTOs.Attachment;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.MasterDatas;
using VTECHERP.DTOs.TransportInformation;
using VTECHERP.Entities;
using VTECHERP.Enums;
using static VTECHERP.Permissions.VTECHERPPermissions.SupplierOrder;

namespace VTECHERP.Services
{
    public class TransportInformationService : ITransportInformationService
    {
        private readonly IObjectMapper _mapper;
        private readonly IRepository<TransportInformation> _transportInformationRepository;
        private readonly IClock _clock;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IIdentityUserRepository _userRepository;
        private readonly IRepository<Stores> _storesRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Employee> _shipperRepository;
        private readonly IRepository<BillCustomer> _billCustomerRepository;
        private readonly IRepository<TransportInformationLog> _transportInformationLogRepository;
        private readonly IRepository<TransporstBills> _transporstBillsRepository;
        private readonly IRepository<GroupTransportInformation> _groupTransportInformation;
        private readonly IRepository<UserStore> _userStoreRepository;
        private readonly IAttachmentService _attachmentService;
        private readonly ICurrentUser _currentUser;

        public TransportInformationService(
            IObjectMapper mapper,
            IRepository<TransportInformation> transportInformationRepository,
            IRepository<Stores> storesRepository,
            IRepository<Customer> customerRepository,
            IRepository<Employee> shipperRepository,
            IRepository<BillCustomer> billCustomerRepository,
            IRepository<TransportInformationLog> transportInformationLogRepository,
            ICurrentUser currentUser,
            IClock clock, IIdentityUserRepository userRepository, IUnitOfWorkManager unitOfWorkManager,
            IRepository<TransporstBills> transporstBillsRepository,
            IRepository<GroupTransportInformation> groupTransportInformation,
            IAttachmentService attachmentService,
            IRepository<UserStore> userStoreRepository
            )
        {
            _transportInformationRepository = transportInformationRepository;
            _clock = clock;
            _unitOfWorkManager = unitOfWorkManager;
            _mapper = mapper;
            _userRepository = userRepository;
            _storesRepository = storesRepository;
            _customerRepository = customerRepository;
            _shipperRepository = shipperRepository;
            _billCustomerRepository = billCustomerRepository;
            _transportInformationLogRepository = transportInformationLogRepository;
            _transporstBillsRepository = transporstBillsRepository;
            _groupTransportInformation = groupTransportInformation;
            _userStoreRepository = userStoreRepository;
            _attachmentService = attachmentService;
            _currentUser = currentUser;
        }
        public async Task<TransportInformationDTO> Create(CreateTransportInformationDto input)
        {
            var uow = _unitOfWorkManager.Current;
            var data = _mapper.Map<CreateTransportInformationDto, TransportInformation>(input);
            //data.Id = Guid.NewGuid();
            if (input.ToStoreId != null && input.ToStoreId.Count > 0)
            {
                data.ToStoreId = String.Join(",", input.ToStoreId);
            }
            else
            {
                data.ToStoreId = null;
            }
            if (input.attachment != null && input.attachment.Count > 0)
            {
                data.attachment = JsonSerializer.Serialize(input.attachment);
            }
            else
            {
                data.attachment = null;
            }
            await _transportInformationRepository.InsertAsync(data);
            // gắn các đơn bán hàng vào đơn vận chuyển
            if (input.ListBillId != null && input.ListBillId.Count > 0)
            {
                List<TransporstBills> listTranBillInsert = new List<TransporstBills>();
                List<GroupTransportInformation> listGroupTranInsert = new List<GroupTransportInformation>();
                foreach (var item in input.ListBillId)
                {
                    if (item != Guid.Empty)
                    {
                        //gắn đơn bán hàng vào đơn vận chuyển
                        TransporstBills tranbill = new TransporstBills()
                        {
                            TransportInformationId = data.Id,
                            BillCustomerId = item
                        };
                        listTranBillInsert.Add(tranbill);
                        //gắn đơn vận chuyển vào đơn vận chuyển
                        GroupTransportInformation groupTran = new GroupTransportInformation()
                        {
                            ParentTransportInformationId = data.Id,
                            ChildTransportInformationId = (await _transportInformationRepository.FindAsync(x => x.SourceId == item)).Id
                        };
                        listGroupTranInsert.Add(groupTran);
                    }
                }
                await _transporstBillsRepository.InsertManyAsync(listTranBillInsert);
                await _groupTransportInformation.InsertManyAsync(listGroupTranInsert);
            }
            await uow.SaveChangesAsync();
            return _mapper.Map<TransportInformation, TransportInformationDTO>(data);
        }

        public async Task<MasterDataDTO> GetBillInformationByCode(SearchTextRequest request)
        {

            var transportBill = await _transporstBillsRepository.GetQueryableAsync();
            var bills = await _billCustomerRepository.GetQueryableAsync();
            var customers = await _customerRepository.GetQueryableAsync();
            var data = from bill in bills
                       join tranbill in transportBill
                       on bill.Id equals tranbill.BillCustomerId
                       join cus in customers
                       on bill.CustomerId equals cus.Id
                       into tranGr
                       from Customer in tranGr.DefaultIfEmpty()
                       select new MasterDataDTO
                       {
                           Id = bill.Id,
                           Code = bill.Code,
                           Name = Customer.Name,
                           Phone = Customer.PhoneNumber
                       };
            var result = data.Where(p => p.Code.Contains(request.SearchText));

            if (result.Any())
            {
                return result.FirstOrDefault();
            }
            else
            {
                return null;
            }
        }

        public async Task Delete(Guid id)
        {
            var account = await _transportInformationRepository.FindAsync(p => p.Id == id);

            await _transportInformationRepository.DeleteAsync(account);
        }

        public async Task<string> Validate(CreateTransportInformationDto input)
        {
            var error = "";
            if (input.FromStoreId == null)
            {
                return error = error + "Cửa hàng";
            }
            if (input.TransportForm == Enums.Bills.TransportForm.Internal)
            {
                if (input.TransportId == null)
                {
                    return error = error + "Đơn vị vận chuyển";
                }
                if (string.IsNullOrEmpty(input.TransportName))
                {
                    return error = error + "Tên đơn vị vận chuyển";
                }
                if (string.IsNullOrEmpty(input.TransportPhoneNumber))
                {
                    return error = error + "Số điện thoại đơn vị vận chuyển";
                }
            }
            if (input.TransportForm == Enums.Bills.TransportForm.Production)
            {
                if (input.CarrierWay == null)
                {
                    return error = error + "Viettel Post hoặc GHTK là bắt buộc";
                }
            }
            return error;
        }

        public async Task<TransportInformationDTO> GetBillInformationById(Guid id)
        {
            var transportInformation = await _transportInformationRepository.FindAsync(p => p.Id == id);
            var transportBill = await _transporstBillsRepository.GetListAsync(p => p.TransportInformationId == id);
            List<BillDTO> listBill = new List<BillDTO>();
            if (transportBill != null && transportBill.Count > 0)
            {
                foreach (var item in transportBill)
                {
                    var bill = await _billCustomerRepository.GetAsync(x => x.Id == item.BillCustomerId);
                    BillDTO billDTO = new BillDTO();
                    if (bill != null)
                    {
                        billDTO.BillId = bill.Id;
                        billDTO.BillCode = bill.Code;
                        billDTO.CustomerName = (await _customerRepository.GetAsync(x => x.Id == bill.CustomerId))?.Name ?? "";
                    }
                    listBill.Add(billDTO);
                }
            }

            var result = _mapper.Map<TransportInformation, TransportInformationDTO>(transportInformation);
            result.Attachments = await _attachmentService.GetAttachmentByObjectIdAsync(id);
            result.ListBillDTO = listBill;
            return result;
        }

        public async Task<PagingResponse<TransportInformationResponse>> SearchTransportInformation(SearchTransportInformationRequest request)
        {
            var allUsers = await _userRepository.GetListAsync();
            var stores = await _storesRepository.GetListAsync();
            var customers = await _customerRepository.GetListAsync();
            var shippers = await _shipperRepository.GetListAsync();

            var userStores = (await _userStoreRepository.GetQueryableAsync()).Where(x => x.UserId == _currentUser.GetId()).AsEnumerable();
            if (userStores == null)
                return new PagingResponse<TransportInformationResponse>(0, new List<TransportInformationResponse>());

            var storeIds = userStores.Select(x => x.StoreId).ToList();

            if (request != null && request.SearchDateFrom != null)
            {
                request.SearchDateFrom = _clock.Normalize(request.SearchDateFrom.Value);
            }
            if (request != null && request.SearchDateTo != null)
            {
                request.SearchDateTo = _clock.Normalize(request.SearchDateTo.Value);
            }

            var transportInformations = (await _transportInformationRepository.GetQueryableAsync()).AsEnumerable()
                .Where(o => !o.IsDeleted && (storeIds.Any(id => id == (o.FromStoreId ?? Guid.NewGuid())) || (storeIds.Any(id => (o.ToStoreId == null ? false : o.ToStoreId.ToLower().Contains(id.ToString().ToLower())))) && (o.Status == TransportStatus.Moved || o.Status == TransportStatus.Confirm)));

            transportInformations = transportInformations.WhereIf(!string.IsNullOrEmpty(request.TransportInformationCode), x => x.Code.Contains(request.TransportInformationCode))
                .WhereIf(request.Status != null, x => x.Status == request.Status)
                .WhereIf(request.FromStoreId != null, x => x.FromStoreId == request.FromStoreId)
                .WhereIf(!string.IsNullOrEmpty(request.TransportName), x => x.TransportName.Contains(request.TransportName))
                .WhereIf(!string.IsNullOrEmpty(request.PhoneNumber), x => x.TransportPhoneNumber.Contains(request.PhoneNumber))
                .WhereIf(request.SearchDateFrom != null, x => x.CreationTime.Date >= request.SearchDateFrom.Value.Date)
                .WhereIf(request.SearchDateTo != null, x => x.CreationTime.Date <= request.SearchDateTo.Value.Date);

            if (!string.IsNullOrEmpty(request.CustomerName))
            {
                var customerIds = customers.Where(x => x.Name.ToLower().Contains(request.CustomerName.ToLower())).Select(x => x.Id).ToList();
                if (customerIds != null && customerIds.Count > 0)
                {
                    transportInformations = transportInformations.WhereIf(!string.IsNullOrEmpty(request.CustomerName), x => customerIds.Contains(x.CustomerId.Value));
                }
                else
                {
                    return new PagingResponse<TransportInformationResponse>();
                }
            }
            if (!string.IsNullOrEmpty(request.Shipper))
            {
                var shipperIds = shippers.Where(x => x.Name.ToLower().Contains(request.Shipper.ToLower())).Select(x => x.Id).ToList();
                if (shipperIds != null && shipperIds.Count > 0)
                {
                    transportInformations = transportInformations.WhereIf(!string.IsNullOrEmpty(request.Shipper), x => shipperIds.Contains(x.ShipperId.Value));
                }
                else
                {
                    return new PagingResponse<TransportInformationResponse>();
                }
            }
            var paged = transportInformations.OrderByDescending(p => p.Code).Skip(request.Offset).Take(request.PageSize).ToList();
            var attachments = (await _attachmentService.ListAttachmentByObjectIdAsync(paged.Select(x => x.Id).ToList())).OrderBy(x => x.CreationTime).ToList();
            var result = from p in paged
                         join str in stores on p.FromStoreId equals str.Id
                         into Stores
                         from store in Stores.DefaultIfEmpty()

                         join str2 in stores on p.ToStoreId?.ToLower() equals str2.Id.ToString().ToLower()
                         into Stores2
                         from store2 in Stores2.DefaultIfEmpty()

                         join cus in customers on p.CustomerId equals cus.Id
                         into customerGroups
                         from Customer in customerGroups.DefaultIfEmpty()
                         join shipper in shippers on p.ShipperId equals shipper.Id
                         into shipperGroups
                         from Shipper in shipperGroups.DefaultIfEmpty()
                         select new TransportInformationResponse
                         {
                             Id = p.Id,
                             Code = p.Code,
                             FromStoreName = store == null ? "" : store.Name,
                             ToStoreName = store2 == null ? "" : store2.Name,
                             CustomerName = Customer == null ? "" : Customer.Name,
                             CustomerPhoneNumber = Customer == null ? "" : Customer.PhoneNumber,
                             ShipperName = Shipper == null ? "" : Shipper.Name,
                             IsWarehouseTransfer = p.IsWarehouseTransfer,
                             Status = p.Status,
                             TransportName = p.TransportName,
                             TransportPhoneNumber = p.TransportPhoneNumber,
                             ShipperId = p.ShipperId,
                             ShipTime = p.ShipTime,
                             CreateTime = p.CreationTime,
                             Note = p.Note,
                             IsIransferWarehouse = p.IsWarehouseTransfer,
                             Attachments = attachments.Where(x=>x.ObjectId == p.Id).ToList() ?? new List<DetailAttachmentDto>(),
                         };
            //var result = _mapper.Map<List<TransportInformation>, List<TransportInformationDTO>>(paged);

            return new PagingResponse<TransportInformationResponse>(transportInformations.Count(), result);
        }

        public async Task<PagingResponse<TransportInformation3RDResponse>> SearchTransportInformationBy3RD(SearchTransportInformationBy3RDRequest request)
        {
            var allUsers = await _userRepository.GetListAsync();
            var stores = await _storesRepository.GetListAsync();
            var customers = await _customerRepository.GetListAsync();
            var shippers = await _shipperRepository.GetListAsync();

            var userStores = (await _userStoreRepository.GetQueryableAsync()).Where(x => x.UserId == _currentUser.GetId()).AsEnumerable();
            if (userStores == null)
                return new PagingResponse<TransportInformation3RDResponse>(0, new List<TransportInformation3RDResponse>());

            var storeIds = userStores.Select(x => x.StoreId).ToList();

            if (request != null && request.SearchDateFrom != null)
            {
                request.SearchDateFrom = _clock.Normalize(request.SearchDateFrom.Value);
            }
            if (request != null && request.SearchDateTo != null)
            {
                request.SearchDateTo = _clock.Normalize(request.SearchDateTo.Value);
            }

            var transportInformations = (await _transportInformationRepository.GetQueryableAsync()).AsEnumerable();

            transportInformations = transportInformations.WhereIf(!string.IsNullOrEmpty(request.TransportInformationCode), x => x.Code.Contains(request.TransportInformationCode))
                .Where(x=>!String.IsNullOrEmpty(x.CarrierShippingCode))
                .WhereIf(request.SearchDateFrom != null, x => x.CreationTime.Date >= request.SearchDateFrom.Value.Date)
                .WhereIf(request.SearchDateTo != null, x => x.CreationTime.Date <= request.SearchDateTo.Value.Date)
                .WhereIf(request.FromStoreId!=null,x=>request.FromStoreId.Contains(x.FromStoreId));
                
            if (!string.IsNullOrEmpty(request.CustomerName))
            {
                var customerIds = customers.Where(x => x.Name.ToLower().Contains(request.CustomerName.ToLower())).Select(x => x.Id).ToList();
                if (customerIds != null && customerIds.Count > 0)
                {
                    transportInformations = transportInformations.WhereIf(!string.IsNullOrEmpty(request.CustomerName), x => customerIds.Contains(x.CustomerId.Value));
                }
                else
                {
                    return new PagingResponse<TransportInformation3RDResponse>();
                }
            }
            var paged = transportInformations.OrderByDescending(p => p.Code).Skip(request.Offset).Take(request.PageSize).ToList();
            var attachments = (await _attachmentService.ListAttachmentByObjectIdAsync(paged.Select(x => x.Id).ToList())).OrderBy(x => x.CreationTime).ToList();
            var result = from p in paged
                         join str in stores on p.FromStoreId equals str.Id
                         into Stores
                         from store in Stores.DefaultIfEmpty()

                         join cus in customers on p.CustomerId equals cus.Id
                         into customerGroups
                         from Customer in customerGroups.DefaultIfEmpty()
                         select new TransportInformation3RDResponse
                         {
                             Id = p.Id,
                             Code = p.Code,
                             FromStoreName = store == null ? "" : store.Name,
                             CustomerName = Customer == null ? "" : Customer.Name,
                             Status = p.Status,
                             TransportName = p.CarrierWay,
                             TotalAmount = p.TotalAmount,
                             CreatetionTime = p.CreationTime,
                             CarrierShippingCode = p.CarrierShippingCode,
                         };
            return new PagingResponse<TransportInformation3RDResponse>(transportInformations.Count(), result);
        }

        public async Task<PagingResponse<TransportHistoryInformationResponse>> SearchHistoryTransportInformation(SearchHistoryTransportInformationRequest request)
        {
            var allUsers = await _userRepository.GetListAsync();
            var currentUserId = _currentUser.Id;
            var stores = await _storesRepository.GetListAsync();
            var customers = await _customerRepository.GetListAsync();
            var shippers = await _shipperRepository.GetListAsync();
            var userStores = (await _userStoreRepository.GetQueryableAsync()).Where(x => x.UserId == currentUserId).AsEnumerable();
            var storeIds = userStores.Select(x => x.StoreId).ToList();
            if (request != null && request.SearchDateFrom != null)
            {
                request.SearchDateFrom = _clock.Normalize(request.SearchDateFrom.Value);
            }
            if (request != null && request.SearchDateTo != null)
            {
                request.SearchDateTo = _clock.Normalize(request.SearchDateTo.Value);
            }

            var transportInformations = (await _transportInformationRepository.GetQueryableAsync())
                .Where(o => o.LastModificationTime.HasValue)
                .Where(delegate (TransportInformation o)
                {
                    if (storeIds.Contains(o.FromStoreId ?? Guid.NewGuid()))
                        return true;

                    if (!string.IsNullOrEmpty(o.ToStoreId))
                    {
                        var listToStore = Guid.TryParse(o.ToStoreId, out Guid toStoreIdParse) ? toStoreIdParse : Guid.Empty;
                        if (listToStore == Guid.Empty)
                            return false;

                        return storeIds.Contains(listToStore);
                    }

                    return false;
                }
                );

            var x = transportInformations.ToList();
            transportInformations = transportInformations.WhereIf(!string.IsNullOrEmpty(request.TransportInformationCode), x => x.Code.Contains(request.TransportInformationCode))
                .WhereIf(request.Status != null, x => x.Status == request.Status && !x.IsWarehouseTransfer)
                .WhereIf(request.SearchDateFrom != null, x => x.CreationTime.Date >= request.SearchDateFrom.Value.Date)
                .WhereIf(request.CustomerId != null, x => x.CustomerId == request.CustomerId)
                .WhereIf(request.SearchDateTo != null, x => x.CreationTime.Date <= request.SearchDateTo.Value.Date)
                .WhereIf(!request.TransportName.IsNullOrWhiteSpace(), x => (x.TransportName.IsNullOrWhiteSpace() ? false : x.TransportName.ToLower().Contains(request.TransportName.ToLower())))
                .WhereIf(request.StoreIds.Any(), x => request.StoreIds.Any(id => id == x.FromStoreId));


            if (!string.IsNullOrEmpty(request.Shipper))
            {
                var shipperIds = shippers.Where(x => x.Name.ToLower().Contains(request.Shipper.ToLower())).Select(x => x.Id).ToList();
                if (shipperIds != null && shipperIds.Count > 0)
                {
                    transportInformations = transportInformations.WhereIf(!string.IsNullOrEmpty(request.Shipper), x => shipperIds.Contains(x.ShipperId.Value));
                }
                else
                {
                    return new PagingResponse<TransportHistoryInformationResponse>();
                }
            }
            var paged = transportInformations.OrderByDescending(p => p.Code).Skip(request.Offset).Take(request.PageSize).ToList();

            var result = new List<TransportHistoryInformationResponse>();
            var attachments = (await _attachmentService.ListAttachmentByObjectIdAsync(paged.Select(x => x.Id).ToList())).OrderBy(x => x.CreationTime).ToList();   
            foreach(var item in paged)
            {
                var fromStore = stores.FirstOrDefault(x => x.Id == item.FromStoreId) ?? new Stores();
                var customer = customers.FirstOrDefault(x => x.Id == item.CustomerId) ?? new Customer();
                var toStore = new Stores();
                if (item.IsWarehouseTransfer && !item.ToStoreId.IsNullOrWhiteSpace())
                    toStore = stores.FirstOrDefault(x => x.Id.ToString().ToLower().Contains(item.ToStoreId.ToLower()));
                var shipper = shippers.FirstOrDefault(x => x.Id == item.ShipperId) ?? new Employee();
                result.Add(new TransportHistoryInformationResponse()
                {

                    Id = item.Id,
                    Code = item.Code,
                    ModifyTime = item.LastModificationTime,
                    FromStoreName = fromStore.Name,
                    CustomerName = customer.Name,
                    TransportName = item.TransportName,
                    ToStoreName = toStore.Name,
                    IsWarehouseTransfer = item.IsWarehouseTransfer,
                    TransferStatus = item.IsWarehouseTransfer ? item.Status : null ,
                    ShipperId = item.ShipperId,
                    ShipperName = shipper.Name,
                    Distance = item.Distance,
                    TransportStatus = item.IsWarehouseTransfer ? null : item.Status,
                    Note = item.Note,
                    Attachments = attachments.Where(x=>x.ObjectId == item.Id).ToList() ?? new List<DetailAttachmentDto>(),
                });
            }

            return new PagingResponse<TransportHistoryInformationResponse>(transportInformations.Count(), result);
        }

        public async Task<bool> UpdateInternal(Guid id, UpdateTransportInformationDto input)
        {
            try
            {
                var transportInformation = await _transportInformationRepository.FindAsync(p => p.Id == id);
                if (transportInformation == null)
                {
                    return false;
                }
                if (input.IsWarehouseTransfer == false)
                {
                    transportInformation.ToStoreId = null;
                }
                else
                {
                    if (input.ToStoreId != null && input.ToStoreId.Count > 0)
                    {
                        transportInformation.ToStoreId = String.Join(",", input.ToStoreId);
                    }
                    else { transportInformation.ToStoreId = null; }
                }

                transportInformation.CustomerId = input.CustomerId;
                transportInformation.IsWarehouseTransfer = input.IsWarehouseTransfer;
                transportInformation.TransportForm = input.TransportForm;
                transportInformation.Status = input.Status;
                transportInformation.TransportId = input.TransportId;
                transportInformation.TransportPhoneNumber = input.TransportPhoneNumber;
                transportInformation.TransportName = input.TransportName;
                transportInformation.CarrierWay = input.CarrierWay.GetValueOrDefault();
                if (input.attachment != null && input.attachment.Count > 0)
                {
                    transportInformation.attachment = JsonSerializer.Serialize(input.attachment);
                }
                transportInformation.Note = input.Note;
                // xóa hết các quan hệ bảng cũ
                List<TransporstBills> listTranBillDelete = new List<TransporstBills>();
                List<GroupTransportInformation> listGroupTranDelete = new List<GroupTransportInformation>();
                listTranBillDelete = await _transporstBillsRepository.GetListAsync(x => x.TransportInformationId == id);
                if (listTranBillDelete != null && listTranBillDelete.Count > 0)
                {
                    await _transporstBillsRepository.DeleteManyAsync(listTranBillDelete);
                }
                listGroupTranDelete = await _groupTransportInformation.GetListAsync(x => x.ParentTransportInformationId == id);
                if (listGroupTranDelete != null && listGroupTranDelete.Count > 0)
                {
                    await _groupTransportInformation.DeleteManyAsync(listGroupTranDelete);
                }
                // gắn các đơn bán hàng vào đơn vận chuyển
                if (input.ListBillId != null && input.ListBillId.Count > 0)
                {
                    List<TransporstBills> listTranBillInsert = new List<TransporstBills>();
                    List<GroupTransportInformation> listGroupTranInsert = new List<GroupTransportInformation>();
                    foreach (var item in input.ListBillId)
                    {
                        if (item != Guid.Empty)
                        {
                            //gắn đơn bán hàng vào đơn vận chuyển
                            TransporstBills tranbill = new TransporstBills()
                            {
                                TransportInformationId = id,
                                BillCustomerId = item
                            };
                            listTranBillInsert.Add(tranbill);
                            //gắn đơn vận chuyển vào đơn vận chuyển
                            GroupTransportInformation groupTran = new GroupTransportInformation()
                            {
                                ParentTransportInformationId = id,
                                ChildTransportInformationId = (await _transportInformationRepository.FindAsync(x => x.SourceId == item)).Id
                            };
                            listGroupTranInsert.Add(groupTran);
                        }
                    }
                    await _transporstBillsRepository.InsertManyAsync(listTranBillInsert);
                    await _groupTransportInformation.InsertManyAsync(listGroupTranInsert);
                }
                await _transportInformationRepository.UpdateAsync(transportInformation);

                return true;

            }
            catch (Exception)
            {
                return false;
            }

        }

        public async Task<bool> UpdateShipper(Guid id, MasterDataDTO input)
        {
            try
            {
                var transportInformation = await _transportInformationRepository.FindAsync(p => p.Id == id);
                if (transportInformation == null)
                {
                    return false;
                }
                transportInformation.ShipperId = input.Id;
                transportInformation.ShipTime = DateTime.Now;
                await _transportInformationRepository.UpdateAsync(transportInformation);

                await _transportInformationLogRepository.InsertAsync(new TransportInformationLog()
                {
                    CreatorId = _currentUser.Id,
                    ShipperId = transportInformation.ShipperId,
                    TransportInformationCode = transportInformation.Code,
                    ShipTime = transportInformation.ShipTime,
                    Status = transportInformation.Status,
                });
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateStatus(Guid id, TransportStatus input)
        {
            try
            {
                var transportInformation = await _transportInformationRepository.FindAsync(p => p.Id == id);
                if (transportInformation == null)
                {
                    return false;
                }

                var transporstBills = (await _transporstBillsRepository.GetQueryableAsync()).Where(x => x.TransportInformationId == id);
                var groupTransportInformations = (await _groupTransportInformation.GetQueryableAsync()).Where(x => x.ParentTransportInformationId == id);

                //2 trạng thái của đơn chuyển kho
                if (input == TransportStatus.Moved || input == TransportStatus.Confirm)
                {
                    //transportInformation không phải là đơn chuyển kho
                    if (transportInformation.IsWarehouseTransfer == false)
                        return false;
                }
                else
                {
                    //transportInformation là đơn chuyển kho
                    if (transportInformation.IsWarehouseTransfer == true)
                        return false;

                    await UpdateStatusChildrenTransportAsync(id, input);
                    await UpdateStatusBillAsync(id, input);
                }

                transportInformation.Status = input;
                if (input == TransportStatus.WaitingDelivery || input == TransportStatus.Cancel)
                {
                    transportInformation.ShipperId = null;
                    transportInformation.ShipTime = null;
                }

                await _transportInformationRepository.UpdateAsync(transportInformation);

                await _transportInformationLogRepository.InsertAsync(new TransportInformationLog()
                {
                    CreatorId = _currentUser.Id,
                    ShipperId = transportInformation.ShipperId,
                    TransportInformationCode = transportInformation.Code,
                    ShipTime = transportInformation.ShipTime,
                    Status = transportInformation.Status,
                });
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<bool> UpdateDistance(Guid id, decimal? input)
        {
            try
            {
                var transportInformation = await _transportInformationRepository.FindAsync(p => p.Id == id);
                if (transportInformation == null)
                {
                    return false;
                }
                transportInformation.Distance = input;
                await _transportInformationRepository.UpdateAsync(transportInformation);
                return true;

            }
            catch (Exception)
            {
                return false;
            }
        }

        public async Task<List<MasterDataDTO>> SearchCustomer(SearchTextRequest request)
        {
            var list = new List<MasterDataDTO>();
            list = (await _customerRepository
                .GetQueryableAsync())
                .Where(p =>
                    request.SearchText == null || p.Name.Contains(request.SearchText) || p.Code.Contains(request.SearchText)
                )
                .Select(p => new MasterDataDTO { Code = p.Code, Name = p.Name, Id = p.Id })
                .OrderBy(p => p.Name)
                .ToList();

            return list;
        }

        public async Task<List<MasterDataDTO>> SearchBillInformation(SearchTextRequest request)
        {
            var list = new List<MasterDataDTO>();
            var billCustomers = await _billCustomerRepository.GetQueryableAsync();
            var customers = await _customerRepository.GetQueryableAsync();
            var data = from bill in billCustomers
                       join cus in customers
                       on bill.CustomerId equals cus.Id
                       into tranGr
                       from Customer in tranGr.DefaultIfEmpty()
                       select new MasterDataDTO
                       {
                           Id = bill.Id,
                           Code = bill.Code,
                           Name = Customer.Name,
                           Phone = Customer.PhoneNumber
                       };
            list = data.Where(p =>
                    request.SearchText == null || p.Name.Contains(request.SearchText) || p.Code.Contains(request.SearchText)
                )
                .Select(p => new MasterDataDTO { Code = p.Code, Name = p.Name, Id = p.Id })
                .OrderBy(p => p.Name)
                .ToList();

            return list;
        }

        public async Task<List<MasterDataDTO>> SearchShipper()
        {
            var list = new List<MasterDataDTO>();

            list = (await _shipperRepository
                .GetQueryableAsync())
                .Select(p => new MasterDataDTO { Code = p.Code, Name = p.Name, Id = p.Id })
                .OrderBy(p => p.Name)
                .ToList();

            return list;
        }

        public async Task<byte[]> ExportTransportInformationAsync(SearchTransportInformationRequest request)
        {
            try
            {
                if (request != null && request.SearchDateFrom != null)
                {
                    request.SearchDateFrom = _clock.Normalize(request.SearchDateFrom.GetValueOrDefault());
                }
                if (request != null && request.SearchDateTo != null)
                {
                    request.SearchDateTo = _clock.Normalize(request.SearchDateTo.GetValueOrDefault());
                }
                var result = await SearchTransportInformation(request);
                var data = result.Data.ToList();
                var file = ExcelHelper.ExportExcel(data);
                return file;
            }
            catch (Exception ex)
            {
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }
        }

        /// <summary>
        /// Cập nhật trạng thái các đơn vận chuyển con
        /// </summary>
        /// <param name="transportId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        private async Task UpdateStatusChildrenTransportAsync(Guid transportId, TransportStatus status)
        {
            var groupTransportInformations = (await _groupTransportInformation.GetQueryableAsync()).Where(x => x.ParentTransportInformationId == transportId);
            var transportInformations = (await _transportInformationRepository.GetQueryableAsync()).Where(x => groupTransportInformations.Any(gTI => gTI.ChildTransportInformationId == x.Id));
            if (transportInformations.Any())
            {
                //Update Trạng thái các đơn vận chuyển con
                transportInformations.ToList().ForEach(x =>
                {
                    x.Status = status;
                });
                await _transportInformationRepository.UpdateManyAsync(transportInformations);
            }
        }

        /// <summary>
        /// Cập nhật trạng thái các hóa đơn bán hàng khi cập nhật trạng thái đơn vận chuyển
        /// </summary>
        /// <param name="transportId"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        private async Task UpdateStatusBillAsync(Guid transportId, TransportStatus status)
        {

            var transportInformation = (await _transportInformationRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == transportId);
            if (transportInformation == null)
                return;
            if (transportInformation.IsWarehouseTransfer == false && (status == TransportStatus.Done || status == TransportStatus.Delivering))
            {
                var transporstBills = (await _transporstBillsRepository.GetQueryableAsync()).Where(x => x.TransportInformationId == transportId);
                var bills = (await _billCustomerRepository.GetQueryableAsync()).Where(x => transporstBills.Any(tB => tB.BillCustomerId == x.Id));
                if (bills.Any())
                {
                    //Update Trạng thái các hóa đơn bán hàng
                    bills.ToList().ForEach(x =>
                    {
                        x.CustomerBillPayStatus = status == TransportStatus.Done ? Enums.Bills.CustomerBillPayStatus.Success : Enums.Bills.CustomerBillPayStatus.Delivery;
                    });
                    await _billCustomerRepository.UpdateManyAsync(bills);
                }
            }
        }
    }
}
