using AutoMapper;
using ClosedXML.Excel;
using DocumentFormat.OpenXml.Office2010.Excel;
using DocumentFormat.OpenXml.Vml.Office;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Dynamic.Core;
using System.Text;
using System.Threading.Tasks;
using Vinpearl.Modelling.Library.Utility.Excel;
using Volo.Abp;
using Volo.Abp.Domain.Entities;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Timing;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VTECHERP.Constants;
using VTECHERP.Domain.Shared.Helper.Excel.Model;
using VTECHERP.DTOs.Attachment;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.BillCustomers.Params;
using VTECHERP.DTOs.BillCustomers.Respons;
using VTECHERP.DTOs.CustomerReturnImport.Params;
using VTECHERP.DTOs.CustomerReturnImport.Respon;
using VTECHERP.DTOs.ExchangeReturn;
using VTECHERP.DTOs.WarehousingBills;
using VTECHERP.Entities;
using VTECHERP.Enums;
using VTECHERP.Helper;
using VTECHERP.ServiceInterfaces;

namespace VTECHERP.Services
{
    public class CustomerReturnService : ICustomerReturnService
    {
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<CustomerReturn> _customerReturnRepository;
        private readonly IRepository<CustomerReturnProduct> _customerReturnProductRepository;
        private readonly IRepository<Products> _productsRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<UserStore> _userStoreRepository;
        private readonly ICurrentUser _currentUser;
        private readonly IEntryService _entryService;
        private readonly IPaymentReceiptService _paymentReceiptService;
        private readonly IWarehousingBillService _warehousingBillService;
        private readonly IObjectMapper _mapper;
        private readonly IClock _clock;
        private readonly IBillCustomerReturnService _billCustomerReturnService;
        private readonly IIdentityUserRepository _userRepository;
        private readonly IRepository<Stores> _storeRepository;
        private readonly IAttachmentService _attachmentService;
        private readonly IRepository<StoreProduct> _storeProductRepository;
        public CustomerReturnService(
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<CustomerReturn> customerReturnRepository,
            IRepository<CustomerReturnProduct> customerReturnProductRepository,
            IRepository<Products> productsRepository,
            IRepository<Customer> customerRepository,
            IRepository<UserStore> userStoreRepository,
            IRepository<Stores> storeRepository,
            ICurrentUser currentUser,
            IEntryService entryService,
            IPaymentReceiptService paymentReceiptService,
            IWarehousingBillService warehousingBillService,
            IObjectMapper mapper,
            IClock clock,
            IBillCustomerReturnService billCustomerReturnService,
            IIdentityUserRepository userRepository,
            IAttachmentService attachmentService,
            IRepository<StoreProduct> storeProductRepository
            )
        {
            _unitOfWorkManager = unitOfWorkManager;
            _customerReturnRepository = customerReturnRepository;
            _customerReturnProductRepository = customerReturnProductRepository;
            _productsRepository = productsRepository;
            _customerRepository = customerRepository;
            _userStoreRepository = userStoreRepository;
            _currentUser = currentUser;
            _entryService = entryService;
            _paymentReceiptService = paymentReceiptService;
            _warehousingBillService = warehousingBillService;
            _mapper = mapper;
            _clock = clock;
            _billCustomerReturnService = billCustomerReturnService;
            _userRepository = userRepository;
            _storeRepository = storeRepository;
            _attachmentService = attachmentService;
            _storeProductRepository = storeProductRepository;
        }

        public async Task<(string, bool)> ConfirmAsync(Guid customerReturnId)
        {
            var uow = _unitOfWorkManager.Current;
            try
            {
                var entity = await _customerReturnRepository.GetAsync(x => x.Id == customerReturnId);
                var products = await _customerReturnProductRepository.GetListAsync(x => x.CustomerReturnId == customerReturnId);
                if (entity == null)
                {
                    return ("Không tồn tại bản ghi!", false);
                }
                else if (entity.isConfirmed == 1)
                    return ("Không thể xác nhận bản ghi đã xác nhận", false);

                var total = 0m;
                if (entity.DiscountValue.HasValue && entity.DiscountValue > 0)
                {
                    var totalProductPrice = products.Sum(x => x.TotalPrice);
                    total = (totalProductPrice - (entity.DiscountUnit == Enums.MoneyModificationType.VND ? entity.DiscountValue : (totalProductPrice * entity.DiscountValue / 100))).Value;
                }
                else
                {
                    total = products.Sum(x => x.TotalPriceAfterDiscount).Value;
                }
                await _entryService.AutoCreateEntryForReturnProduct(entity.Id, total);

                if (entity.IsExchange == Enums.ExchangeEnum.Exchange)
                {
                    await _billCustomerReturnService.AutoCreateCustomerBillForReturnProduct(entity.Id);
                }
                else
                {
                    await _paymentReceiptService.AutoCreatePaymentReceiptForReturnProduct(entity.Id);
                }

                await _warehousingBillService.AutoCreateWearhousingBillForReturnProduct(entity.Id);

                entity.isConfirmed = 1;
                entity.DateConfirm = DateTime.Now;
                await _customerReturnRepository.UpdateAsync(entity);
                await uow.SaveChangesAsync();
                return ("Tạo thành công", true);
            }catch (Exception ex)
            {
                await uow.RollbackAsync();
                return (ex.Message, false);
            }
        }

        public async Task<(CustomerReturnDTO, string, bool)> Create(CreateCustomerReturnRequest request)
        {
            var uow = _unitOfWorkManager.Current;
            var entity = _mapper.Map<CreateCustomerReturnRequest, CustomerReturn>(request);
            var products = _mapper.Map<List<CustomerReturnProductDTO>, List<CustomerReturnProduct>> (request.Products);
            entity.Id = Guid.NewGuid();
            var productList = await _productsRepository.GetListAsync(x => request.Products.Select(y => y.ProductId).Contains(x.Id));
            var customer = await _customerRepository.FindAsync(x => request.CustomerId == x.Id);
            // calculate totalAmount 
            var totalProductPrice = request.Products.Sum(x => (x.Price * x.Quantity));
            decimal? totalDiscount = 0m;
            decimal precentDiscountEachProduct = 0;

            if (entity.DiscountValue.HasValue && entity.DiscountValue > 0)
            {
                totalDiscount = entity.DiscountUnit == Enums.MoneyModificationType.VND
                    ? entity.DiscountValue
                    : (totalProductPrice * entity.DiscountValue / 100).Value;

                precentDiscountEachProduct = entity.DiscountUnit == Enums.MoneyModificationType.Percent
                    ? entity.DiscountValue.GetValueOrDefault()
                    : (entity.DiscountValue / totalProductPrice).Value * 100;
            }
            entity.TotalAmount = totalProductPrice 
                - (entity.DiscountUnit == Enums.MoneyModificationType.VND 
                    ? entity.DiscountValue 
                    : (totalProductPrice * entity.DiscountValue / 100))
                - entity.Cash 
                - entity.Banking;
            entity.isConfirmed = 0;
            entity.CreatorName = _currentUser.Name;
            entity.CustomerName = customer.Name;
            
            products.ForEach(x => {
                var totalProPrice = x.Price * x.Quantity;
                x.TotalPrice = totalProPrice;
                x.TotalPriceAfterDiscount = totalProPrice;
                x.CustomerReturnId = entity.Id;
                x.Name = productList.FirstOrDefault(y => y.Id == x.ProductId).Name;
                if (x.DiscountUnit.HasValue && x.DiscountValue.HasValue && x.DiscountValue.Value > 0)
                    x.TotalPriceAfterDiscount = totalProPrice - (x.DiscountUnit == Enums.MoneyModificationType.VND ? x.DiscountValue : (totalProPrice * (x.DiscountValue / 100)));
                else
                {
                    var discountCash = totalProPrice * (precentDiscountEachProduct / 100);
                    x.TotalPriceAfterDiscount = totalProPrice - discountCash;
                }
            });
            if (totalDiscount == 0)
            {
                totalDiscount = products.Sum(x => x.TotalPrice - x.TotalPriceAfterDiscount);
            }
            entity.TotalDiscount = totalDiscount;
            await _customerReturnRepository.InsertAsync(entity);
            await _customerReturnProductRepository.InsertManyAsync(products);
            await uow.SaveChangesAsync();

            return (_mapper.Map<CustomerReturn, CustomerReturnDTO>(entity), "Tạo thành công", true);
        }

        public async Task<(bool, string, bool)> Delete(Guid id)
        {
            var entity = await _customerReturnRepository.GetAsync(x => x.Id == id);
            var products = await _customerReturnProductRepository.GetListAsync(x => x.CustomerReturnId == id);
            if (entity == null)
            {
                return (false, "Không tồn tại bản ghi!", false);
            }else if (entity.isConfirmed == 1)
                return (false, "Không thể xóa bản ghi đã xác nhận", false);
            await _customerReturnProductRepository.DeleteManyAsync(products);
            await _customerReturnRepository.DeleteAsync(entity);
            return (true, "Xóa thành công", true);
        }

       
        public async Task<byte[]> ExportCustomerReturn(SearchCustomerReturnRequest request)
        {
            try
            {
                var customers = await _customerRepository.GetQueryableAsync();
                var products = await _productsRepository.GetQueryableAsync();
                var customerReturnProducts = await _customerReturnProductRepository.GetQueryableAsync();
                var customerReturns = await _customerReturnRepository.GetQueryableAsync();
                var query = from customerReturn in customerReturns
                            join customerReturnProduct in customerReturnProducts on customerReturn.Id equals customerReturnProduct.CustomerReturnId
                            join customer in customers on customerReturn.CustomerId equals customer.Id
                            join product in products on customerReturnProduct.ProductId equals product.Id
                            orderby customerReturn.Id descending
                            select new
                            {
                                Code = customerReturn.Code,
                                StoreId = customerReturn.StoreId,
                                CustomerId = customerReturn.CustomerId,
                                CreationTime = customerReturn.CreationTime,
                                CustomerName = customer.Name,
                                HandlerEmployeeName = customer.HandlerEmpName,
                                EmployeeSell = customerReturn.EmployeeSell,
                                DateOfBirth = customer.DateOfBirth,
                                CustomerPhone = customer.PhoneNumber,
                                CustomerAddress = customer.Address,
                                ParentProductCode = product.ParentCode,
                                ParentProductName = product.ParentName,
                                ProductCode = product.Code,
                                ProductName = product.Name,
                                Unit = "",
                                IMEI = "",
                                ReturnPrice = customerReturnProduct.Price,
                                CostPrice = product.StockPrice,
                                Quantity = customerReturnProduct.Quantity,
                                DiscountValue = customerReturn.DiscountValue,
                                ReturnMoney = customerReturnProduct.TotalPrice,
                                ReturnAmount = customerReturnProduct.TotalPriceAfterDiscount,
                                Cash = customerReturn.Cash,
                                Banking = customerReturn.Banking,
                                Note = customerReturn.PayNote,
                                Exchange = "",
                                Carrier = "",
                                CarrierShippingCode = ""
                            };


                query = query.WhereIf(request.StoreIds != null && request.StoreIds.Count > 0, x => request.StoreIds.Contains(x.StoreId.HasValue ? x.StoreId.Value : new Guid()))
                    .WhereIf(!string.IsNullOrEmpty(request.BillCustomerCode), x => x.Code.Contains(request.BillCustomerCode))
                    //.WhereIf(request.CreateTimeFrom != null, x => x.CreationTime.Date >= request.CreateTimeFrom.Value.Date)
                    //.WhereIf(request.CreateTimeTo != null, x => x.CreationTime.Date <= request.CreateTimeTo.Value.Date)
                    .WhereIf(!string.IsNullOrEmpty(request.CustomerName), x => !string.IsNullOrEmpty(x.CustomerName) ? x.CustomerName.ToLower().Contains(request.CustomerName.ToLower()) : request.CustomerName == "")
                    .WhereIf(!string.IsNullOrEmpty(request.ProductName), x => x.ProductName.Contains(request.ProductName));
                    //.WhereIf(!string.IsNullOrEmpty(request.EmployeeCashier), x => x.)
                    //.WhereIf(!string.IsNullOrEmpty(request.CouponCode), x => x.CouponCode)
                    //.WhereIf(request.IMei)
                    //.WhereIf(request.ProductCategory)
                    //.WhereIf(!string.IsNullOrEmpty(request.EmployeeSell), x => x.EmployeeSell)
                    //.WhereIf(!string.IsNullOrEmpty(request.Description), x => request.Description.Contains(x.Note))
                    //.WhereIf(request.EmployeeTech)
                    //.WhereIf(request.IsCheckData)
                    //.WhereIf(request.CustomerBillPayStatus != null, x => x.CustomerBillPayStatus == request.CustomerBillPayStatus);

                var storeIds = query.Select(x => x.StoreId).ToList();
                var stores = (await _storeRepository.GetListAsync()).Where(p => storeIds.ToList().Contains(p.Id));
                var EmployeeSellIds = query.Select(x => x.EmployeeSell).ToList();
                var EmployeeSells = (await _userRepository.GetListAsync()).Where(p => EmployeeSellIds.ToList().Contains(p.Id));

                var exportData = new List<ExportCustomerReturnResponse>();
                foreach (var item in query)
                {
                    var storeName = stores.FirstOrDefault(x => x.Id == item.StoreId)?.Name;
                    var employeeSell = EmployeeSells.FirstOrDefault(x => x.Id == item.EmployeeSell)?.Name;

                    exportData.Add(new ExportCustomerReturnResponse()
                    {
                        Code = item.Code,
                        StoreName = storeName,
                        CreationTime = item.CreationTime.ToString("dd-MM-yyyy"),
                        CustomerName = item.CustomerName,
                        HandlerEmployeeName = item.HandlerEmployeeName,
                        EmployeeSell = employeeSell,
                        CustomerPhone = item.CustomerPhone,
                        CustomerAddress = item.CustomerAddress,
                        ParentProductCode = item.ParentProductCode,
                        ParentProductName = item.ParentProductName,
                        ProductCode = item.ProductCode,
                        ProductName = item.ProductName,
                        Unit = "",
                        IMEI = "",
                        ReturnPrice = item.ReturnPrice.HasValue ? item.ReturnPrice.Value : 0,
                        CostPrice = item.CostPrice,
                        Quantity = item.Quantity.HasValue ? item.Quantity.Value : 0,
                        DiscountValue = item.DiscountValue.HasValue ? item.DiscountValue.Value : 0,
                        ReturnMoney = item.ReturnMoney.HasValue ? item.ReturnMoney.Value : 0,
                        ReturnAmount = item.ReturnAmount.HasValue ? item.ReturnAmount.Value : 0,
                        Cash = item.Cash.HasValue ? item.Cash.Value : 0,
                        Banking = item.Banking.HasValue ? item.Banking.Value : 0,
                        Note = item.Note,
                        Exchange = "",
                        Carrier = "",
                        CarrierShippingCode = item.CarrierShippingCode,
                    });
                }
                return ExcelHelper.ExportExcel(exportData);
            }
            catch (Exception ex)
            {
                throw new BusinessException(ErrorMessages.Unexpected, ex.Message, null, ex.InnerException);
            }
        }

        public async Task<(CustomerReturnDTO, string, bool)> Get(Guid id)
        {
            var entity = await _customerReturnRepository.GetAsync(x => x.Id == id);
            var customer = await _customerRepository.GetAsync(x => x.Id == entity.CustomerId);
            var products = await _customerReturnProductRepository.GetListAsync(x => x.CustomerReturnId == id);
            
            var dto = _mapper.Map<CustomerReturn, CustomerReturnDTO>(entity);
            dto.Attachments = await _attachmentService.GetAttachmentByObjectIdAsync(id);
            var productDTO = _mapper.Map<List<CustomerReturnProduct>, List<CustomerReturnProductDTO>>(products);
            dto.Products = productDTO;

            if (customer != null)
            {
                dto.PhoneNumber = customer.PhoneNumber;
            }
            var totalProductPrice = dto.Products.Sum(y => (decimal)(y.Price.Value * y.Quantity.Value));
            dto.TotalAmount = totalProductPrice - (dto.DiscountUnit == Enums.MoneyModificationType.VND ? dto.DiscountValue : (totalProductPrice * dto.DiscountValue / 100));
            var attachmentProducts = (await _attachmentService.ListAttachmentByObjectIdAsync(products.Select(x => x.ProductId.Value).ToList())).OrderBy(x=>x.CreationTime).ToList();
            foreach(var item in dto.Products)
            {
                item.Attachments = attachmentProducts.Where(x=>x.ObjectId == item.ProductId.Value).ToList() ?? new List<DetailAttachmentDto>();
            }
            return (dto, "Lấy thành công", true);
        }

        public async Task<(PagingResponse<CustomerReturnDTO>, string, bool)> Search(SearchCustomerReturnRequest searchRequest)
        {
            if (searchRequest.FromDate != null)
            {
                searchRequest.FromDate = _clock.Normalize(searchRequest.FromDate.Value);
            }
            if (searchRequest.ToDate != null)
            {
                searchRequest.ToDate = _clock.Normalize(searchRequest.ToDate.Value);
            }

            List<CustomerReturn> customerReturns = new List<CustomerReturn>();
            List<UserStore> users = new List<UserStore>();

            var productQuery = _customerReturnProductRepository.GetQueryableAsync().Result;
            
            var query =
                from bill in _customerReturnRepository.GetQueryableAsync().Result
                where
                    !bill.IsDeleted
                    && (searchRequest.StoreIds == null || searchRequest.StoreIds.Count == 0 || searchRequest.StoreIds.Contains(bill.StoreId.Value))
                    && (searchRequest.BillCode.IsNullOrWhiteSpace() || bill.Code.Contains(searchRequest.BillCode))
                    && (searchRequest.ProductName.IsNullOrWhiteSpace() || 
                        _customerReturnProductRepository.GetQueryableAsync().Result.Any(x => x.CustomerReturnId == bill.Id && x.Name.Contains(searchRequest.ProductName)))
                    && (searchRequest.ProductName.IsNullOrWhiteSpace() || productQuery.Any(x => x.CustomerReturnId == bill.Id && x.Name.Contains(searchRequest.ProductName)))
                    && (searchRequest.FromDate == null || bill.CreationTime >= searchRequest.FromDate)
                    && (searchRequest.ToDate == null || bill.CreationTime <= searchRequest.ToDate)
                    && (searchRequest.CustomerName.IsNullOrWhiteSpace() || bill.CustomerName.Contains(EF.Functions.Collate(searchRequest.CustomerName, "SQL_Latin1_General_CP1_CI_AI")))
                    && (searchRequest.EmployeeName.IsNullOrWhiteSpace() || bill.EmployeeSellName.Contains(EF.Functions.Collate(searchRequest.EmployeeName, "SQL_Latin1_General_CP1_CI_AI")))
                    && (searchRequest.CreatorName.IsNullOrWhiteSpace() || bill.CreatorName.Contains(EF.Functions.Collate(searchRequest.CreatorName, "SQL_Latin1_General_CP1_CI_AI")))
                    && (searchRequest.IsConfirmed == null || bill.isConfirmed == searchRequest.IsConfirmed)
                    && (_userStoreRepository.GetQueryableAsync().Result.Any(x => x.StoreId == bill.StoreId && x.UserId == _currentUser.Id))
                select bill;

            var paged = query
                .OrderByDescending(p => p.Code)
                .Skip(searchRequest.Offset)
                .Take(searchRequest.PageSize)
                .ToList();

            var pagedDTO = _mapper.Map<List<CustomerReturn>, List<CustomerReturnDTO>>(paged);
            List<Guid> listBillId = paged.Select(x => x.Id).ToList();
            List<Guid> listCustomerId = paged.Select(x => x.CustomerId.Value).ToList();

            var listProd = await _customerReturnProductRepository.GetListAsync(x => listBillId.Contains(x.CustomerReturnId.Value));
            var customers = await _customerRepository.GetListAsync(x => listCustomerId.Contains(x.Id));
            var attachments = (await _attachmentService.ListAttachmentByObjectIdAsync(pagedDTO.Select(x => x.Id).ToList())).OrderBy(x=>x.CreationTime).ToList();
            pagedDTO.ForEach(x => 
            {
                x.Products = _mapper.Map<List<CustomerReturnProduct>, List<CustomerReturnProductDTO>>(listProd.Where(y => y.CustomerReturnId == x.Id).ToList());
                x.TotalAmount = x.Products.Sum(y => y.TotalPriceAfterDiscount);
                x.Attachments = attachments.Where(a => a.ObjectId == x.Id).ToList() ?? new List<DetailAttachmentDto>(); 
                var customer = customers.FirstOrDefault(c => c.Id == x.CustomerId);
                if (customer != null)
                 x.PhoneNumber = customer.PhoneNumber;
            });

            return (new PagingResponse<CustomerReturnDTO>(query.Count(), pagedDTO), "Lấy thành công", true);
        }

        public async Task<(CustomerReturnDTO, string, bool)> Update(CreateCustomerReturnRequest request)
        {
            var uow = _unitOfWorkManager.Current;
            try
            {
                var entity = await _customerReturnRepository.GetAsync(x => x.Id == request.Id);
                if (entity == null)
                {
                    return (null, "Không tồn tại bản ghi!", false);
                }
                else if (entity.isConfirmed == 1)
                    return (null, "Không thể cập nhật bản ghi đã xác nhận", false);

                var code = entity.Code;

                var productBefores = await _customerReturnProductRepository.GetListAsync(x => x.CustomerReturnId == request.Id);
                await _customerReturnProductRepository.DeleteManyAsync(productBefores);
                _mapper.Map<CreateCustomerReturnRequest, CustomerReturn>(request, entity);
                entity.Code = code;

                var productList = await _productsRepository.GetListAsync(x => request.Products.Select(y => y.ProductId).Contains(x.Id));
                var customer = await _customerRepository.GetAsync(x => request.CustomerId == x.Id);
                // calculate totalAmount 
                var totalProductPrice = request.Products.Sum(x => (x.Price * x.Quantity));
                decimal? totalDiscount = 0m;
                decimal precentDiscountEachProduct = 0;

                if (entity.DiscountValue.HasValue && entity.DiscountValue > 0)
                {
                    totalDiscount = entity.DiscountUnit == Enums.MoneyModificationType.VND
                        ? entity.DiscountValue
                        : (totalProductPrice * entity.DiscountValue / 100).Value;

                    precentDiscountEachProduct = entity.DiscountUnit == Enums.MoneyModificationType.Percent
                    ? entity.DiscountValue.GetValueOrDefault()
                    : (entity.DiscountValue / totalProductPrice).Value * 100;
                }
                entity.TotalAmount = totalProductPrice
                    - (entity.DiscountUnit == Enums.MoneyModificationType.VND
                        ? entity.DiscountValue
                        : (totalProductPrice * entity.DiscountValue / 100))
                    - entity.Cash
                    - entity.Banking;
                entity.isConfirmed = 0;
                entity.CreatorName = _currentUser.Name;
                entity.CustomerName = customer.Name;
                var products = _mapper.Map<List<CustomerReturnProductDTO>, List<CustomerReturnProduct>>(request.Products);
                products.ForEach(x => {
                    var totalProPrice = x.Price * x.Quantity;
                    x.TotalPrice = totalProPrice;
                    x.TotalPriceAfterDiscount = totalProPrice;
                    x.CustomerReturnId = entity.Id;
                    x.Name = productList.FirstOrDefault(y => y.Id == x.ProductId).Name;

                    if (x.DiscountUnit.HasValue && x.DiscountValue.HasValue && x.DiscountValue.Value > 0)
                        x.TotalPriceAfterDiscount = totalProPrice - (x.DiscountUnit == Enums.MoneyModificationType.VND ? x.DiscountValue : (totalProPrice * (x.DiscountValue / 100)));
                    else
                    {
                        var discountCash = totalProPrice * (precentDiscountEachProduct / 100);
                        x.TotalPriceAfterDiscount = totalProPrice - discountCash;
                    }
                });
                if (totalDiscount == 0)
                {
                    totalDiscount = products.Sum(x => x.TotalPrice - x.TotalPriceAfterDiscount);
                }
                entity.TotalDiscount = totalDiscount;
                await _customerReturnProductRepository.InsertManyAsync(products);
                var updatedEntity = await _customerReturnRepository.UpdateAsync(entity);

                return (_mapper.Map<CustomerReturn, CustomerReturnDTO>(updatedEntity), "Cập nhật thành công", true);

            }catch (Exception ex)
            {
                await uow.RollbackAsync();
                return (new CustomerReturnDTO(),  ex.Message, false);
            }

        }

        public async Task<(CustomerReturnDTO, string, bool)> UpdateNoteAsync(Guid id, string note)
        {
            try
            {
                var entity = await _customerReturnRepository.GetAsync(x => x.Id == id);
                if (entity == null)
                {
                    return (null, "Không tồn tại bản ghi!", false);
                }
                else if (entity.isConfirmed == 1)
                    return (null, "Không thể cập nhật bản ghi đã xác nhận", false);

                entity.PayNote = note;
                await _customerReturnRepository.UpdateAsync(entity);
                return (null, "Cập nhật thành công", true);
            }catch(Exception ex)
            {
                return (null, "Có lỗi xảy ra", false);
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

        public async Task<(string message, bool success, Guid? data, byte[] fileRespon)> ImportBillCustomerReturn(CustomerReturnImportParam param)
        {
            var workbook = new CustomWorkBook();
            var listData = await MappingDataImportBillCustomerProduct(param.File, param.StoreId.GetValueOrDefault());

            var dataToInsert = listData.Where(x => x.Success)
                .Select(x => new CustomerReturnProductDTO
                {
                    ProductId = x.ProductId,
                    Price = x.Price,
                    Quantity = x.Quantity,
                    DiscountUnit = x.DiscountUnit,
                    DiscountValue = x.DiscountValue,
                    StockPrice = x.StockPrice,
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

            var paramCreateBillCustomerReturn = new CreateCustomerReturnRequest
            {
                DiscountValue = param.DiscountValue,
                DiscountUnit = param.DiscountUnit,
                AccountCode = param.AccountCode,
                AccountCodeBanking = param.AccountCodeBanking,
                Banking = param.Banking,
                Cash = param.Cash,
                CustomerId = param.CustomerId,
                EmployeeCare = param.EmployeeCare,
                EmployeeSell = param.EmployeeSell,
                StoreId = param.StoreId,
                PayNote = param.PayNote,
                Products = dataToInsert
            };

            var responCreateBill = await Create(paramCreateBillCustomerReturn);

            if (responCreateBill.Item3)
            {
                var dto = responCreateBill.Item1 as CustomerReturnDTO;
                var responConfirm = await ConfirmAsync(dto.Id);

                if (responConfirm.Item2)
                {
                    var sheet = RenderTemplateAfterImport(listData);
                    workbook.Sheets.Add(sheet);

                    var fileReturn = ExcelHelper.ExportExcel(workbook);
                    return (responConfirm.Item1, true, dto.Id, fileReturn);
                }
                else return (responConfirm.Item1, false, null, null);

            }
            else return (responCreateBill.Item2, false, null, null);
        }

        private async Task<List<CustomerReturnImportRespon>> MappingDataImportBillCustomerProduct(IFormFile file, Guid StoreId)
        {
            var result = new List<CustomerReturnImportRespon>();

            using (var workbook = new XLWorkbook(file.OpenReadStream()))
            {
                var worksheet = workbook.Worksheet(1); // Chỉ định số trang tính trong tệp Excel (index bắt đầu từ 1)

                var firstRowUsed = worksheet.FirstRowUsed();
                var headers = firstRowUsed.CellsUsed()
                    .Select(c => c.Value.ToString().Trim())
                    .ToList();

                var products = await _productsRepository.GetListAsync();
                var storeProduct = await _storeProductRepository.GetListAsync();
                Products objOrigin = null;

                foreach (var row in worksheet.RowsUsed().Skip(1)) // Bỏ qua hàng tiêu đề (hàng đầu tiên)
                {
                    var obj = new CustomerReturnImportRespon();

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
                                obj.StockPrice = objOrigin.StockPrice;
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

        private CustomSheet RenderTemplateAfterImport(List<CustomerReturnImportRespon> list)
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

    }
}
