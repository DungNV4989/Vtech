using Microsoft.AspNetCore.Hosting;
using SixLabors.ImageSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using Volo.Abp.ObjectMapping;
using VTECHERP.DTOs.Attachment;
using VTECHERP.DTOs.Base;
using VTECHERP.Entities;
using VTECHERP.Enums;
using VTECHERP.Models;
namespace VTECHERP.Services
{
    public class AttachmentService : IAttachmentService
    {
        private readonly IRepository<Attachment> _attachmentRepository;
        private readonly IObjectMapper _objectMapper;

        #region VBN

        private readonly IIdentityUserRepository _userRepository;
        private readonly IHostingEnvironment _hostingEnvironment;
        private const string FolderUpload = "uploads";
        private static readonly string[] ImageExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".bmp" };

        private readonly IRepository<Products> _productsRepository;

        /// <summary>
        /// Lịch sử vận đơn, Phiếu vận chuyển sinh ra từ phiếu bán hàng
        /// </summary>
        private readonly IRepository<TransportInformation> _transportInfomationRepository;

        /// <summary>
        /// Hóa đơn bán hàng
        /// </summary>
        private readonly IRepository<BillCustomer> _billCustomerRepository;

        /// <summary>
        /// Phiếu chuyển kho
        /// </summary>
        private readonly IRepository<WarehouseTransferBill> _warehouseTransferBillRepository;

        /// <summary>
        /// Phiếu xuất, nhập chuyển kho
        /// </summary>
        private readonly IRepository<Entities.WarehousingBill> _warehousingBillRepository;

        /// <summary>
        /// Phiếu thu, chi, báo nợ, báo có, chuyển quỹ
        /// </summary>
        private readonly IRepository<PaymentReceipt> _paymentReceiptRepository;

        /// <summary>
        /// Trả hàng
        /// </summary>
        private readonly IRepository<CustomerReturn> _customerReturnRepository;

        /// <summary>
        /// Bút toán
        /// </summary>
        private readonly IRepository<Entry> _entryRepository;

        /// <summary>
        /// Phiếu đặt hàng
        /// </summary>
        private readonly IRepository<SaleOrders> _saleOrderRepository;

        /// <summary>
        /// Đơn vận chuyển TQ
        /// </summary>
        private readonly IRepository<OrderTransport> _orderTransportRepository;

        #endregion VBN

        public AttachmentService(
            IRepository<Attachment> attachmentRepository,
            IHostingEnvironment hostingEnvironment,
            IObjectMapper objectMapper,
            IIdentityUserRepository userRepository,
            IRepository<Products> productsRepository,
            IRepository<TransportInformation> transportInfomationRepository,
            IRepository<BillCustomer> billCustomerRepository,
            IRepository<WarehouseTransferBill> warehouseTransferBillRepository,
            IRepository<Entities.WarehousingBill> warehousingBillRepository,
            IRepository<PaymentReceipt> paymentReceiptRepository,
            IRepository<CustomerReturn> customerReturnRepository,
            IRepository<Entry> entryRepository,
            IRepository<SaleOrders> saleOrderRepository,
            IRepository<OrderTransport> orderTransportRepository)
        {
            _attachmentRepository = attachmentRepository;
            _hostingEnvironment = hostingEnvironment;
            _objectMapper = objectMapper;
            _userRepository = userRepository;
            _productsRepository = productsRepository;
            _transportInfomationRepository = transportInfomationRepository;
            _billCustomerRepository = billCustomerRepository;
            _warehouseTransferBillRepository = warehouseTransferBillRepository;
            _warehousingBillRepository = warehousingBillRepository;
            _paymentReceiptRepository = paymentReceiptRepository;
            _customerReturnRepository = customerReturnRepository;
            _entryRepository = entryRepository;
            _saleOrderRepository = saleOrderRepository;
            _orderTransportRepository = orderTransportRepository;
        }

        public async Task<PagingResponse<AttachmentResponse>> Save(UploadAttachmentRequest request, UploadFileResult uploadFile)
        {
            var listAtt = new List<Entities.Attachment>();
            foreach (var item in uploadFile.Files)
            {
                listAtt.Add(new Entities.Attachment()
                {
                    Name = item.Name,
                    Url = item.Url,
                    ObjectId = request.ObjectId,
                    ObjectType = request.ObjectType,
                    Extensions = item.Extensions,
                    Path = item.Path,
                });
            }

            await _attachmentRepository.InsertManyAsync(listAtt);

            return new PagingResponse<AttachmentResponse>(listAtt.Count, listAtt.Select(x => new AttachmentResponse()
            {
                Id = x.Id,
                Url = x.Url,
                Path = x.Path,
                Extensions = x.Extensions,
                Name = x.Name,
                ObjectId = x.ObjectId,
                ObjectType = x.ObjectType,
                CreatedDate = x.CreationTime,
            }));
        }

        public async Task<string> GetPathFileById(Guid id)
        {
            var item = await _attachmentRepository.GetAsync(x => x.Id == id);
            return item.Path;
        }

        public async Task Delete(Guid id)
        {
            await _attachmentRepository.DeleteAsync(x => x.Id == id);
        }

        public async Task<PagingResponse<AttachmentResponse>> Search(SearchAttachmentRequest request)
        {
            var query = await _attachmentRepository.GetQueryableAsync();

            query = query.WhereIf(!string.IsNullOrEmpty(request.Keyword), x => x.Name.Contains(request.Keyword) || x.Extensions.Contains(request.Keyword));

            var pagingQuery = query.Skip(request.Offset).Take(request.PageSize).Select(x => new AttachmentResponse()
            {
                Id = x.Id,
                Url = x.Url,
                Path = x.Path,
                Extensions = x.Extensions,
                Name = x.Name,
                ObjectId = x.ObjectId,
                ObjectType = x.ObjectType,
                CreatedDate = x.CreationTime,
            });

            return new PagingResponse<AttachmentResponse>()
            {
                Total = query.Count(),
                Data = pagingQuery.ToList(),
            };
        }

        public async Task<List<AttachmentDetailDto>> GetByObjectId(Guid objectId)
        {
            var result = new List<AttachmentDetailDto>();
            var attachments = (await _attachmentRepository.GetQueryableAsync()).Where(x => x.ObjectId == objectId);
            if (!attachments.Any())
                return result;
            result = _objectMapper.Map<List<Attachment>, List<AttachmentDetailDto>>(attachments.ToList());
            return result;
        }

        #region VBN

        public async Task<List<DetailAttachmentDto>> GetAttachmentByObjectIdAsync(Guid objectId)
        {
            var result = new List<DetailAttachmentDto>();
            var attachments = await _attachmentRepository.GetListAsync(x => x.ObjectId == objectId) ?? new List<Attachment>();

            if (!attachments.Any())
                return result;

            result = attachments.Select(x => new DetailAttachmentDto
            {
                Id = x.Id,
                ObjectId = x.ObjectId,
                ObjectType = x.ObjectType,
                FileName = x.Name,
                FileUrl = x.Url,
                CreationTime = x.CreationTime
            })
            .OrderBy(x => x.CreationTime)
            .ToList();

            foreach (var item in result)
            {
                if (!string.IsNullOrEmpty(item.FileName))
                {
                    var splitFileName = item.FileName.Split('.');
                    var extensionFile = splitFileName[splitFileName.Length - 1];

                    if (extensionFile.Trim().ToLower() == "pdf")
                        item.TypeFile = TypeFile.Pdf;

                    if (extensionFile.Trim().ToLower() == "xlsx" || extensionFile.Trim().ToLower() == "xls")
                        item.TypeFile = TypeFile.Excel;

                    if (extensionFile.Trim().ToLower() == "docx" || extensionFile.Trim().ToLower() == "doc")
                        item.TypeFile = TypeFile.Word;
                }
            }

            return result;
        }

        public async Task<List<DetailAttachmentDto>> ListAttachmentByObjectIdAsync(List<Guid> objectIds)
        {
            var result = new List<DetailAttachmentDto>();
            var attachments = await _attachmentRepository.GetListAsync(x => objectIds.Any(id => id == x.ObjectId.Value)) ?? new List<Attachment>();

            if (!attachments.Any())
                return result;

            result = attachments.Select(x => new DetailAttachmentDto
            {
                Id = x.Id,
                ObjectId = x.ObjectId,
                FileName = x.Name,
                FileUrl = x.Url,
                CreationTime = x.CreationTime
            })
            .OrderBy(x => x.CreationTime)
            .ToList();

            foreach (var item in result)
            {
                if (!string.IsNullOrEmpty(item.FileName))
                {
                    var splitFileName = item.FileName.Split('.');
                    var extensionFile = splitFileName[splitFileName.Length - 1];

                    if (extensionFile.Trim().ToLower() == "pdf")
                        item.TypeFile = TypeFile.Pdf;

                    if (extensionFile.Trim().ToLower() == "xlsx" || extensionFile.Trim().ToLower() == "xls")
                        item.TypeFile = TypeFile.Excel;

                    if (extensionFile.Trim().ToLower() == "docx" || extensionFile.Trim().ToLower() == "doc")
                        item.TypeFile = TypeFile.Word;
                }
            }

            return result;
        }

        public async Task<List<AttachmentModuleResponse>> AttachmentByObjectIdAsync(Guid objectId, AttachmentObjectType? objectType = null)
        {
            //Lấy các file con vả bản thân
            var result = await GetReferenceFileAsync(objectId);

            #region Lấy các file cha
            var entry = (await _entryRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == objectId);
            if (entry != null)
            {
                result.AddRange(await GetReferenceFileAsync(entry.SourceId ?? Guid.Empty));
                return result;
            }

            var transportInfomation = (await _transportInfomationRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == objectId);
            if (transportInfomation != null)
            {
                result.AddRange(await GetReferenceFileAsync(transportInfomation.SourceId ?? Guid.Empty));
                return result.DistinctBy(x => x.Id).ToList();
            }

            var customerReturn = (await _customerReturnRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == objectId);
            if (customerReturn != null)
            {
                result.AddRange(await GetReferenceFileAsync(customerReturn.BillCustomerId ?? Guid.Empty));
                return result;
            }

            var paymentReceipt = (await _paymentReceiptRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == objectId);
            if (paymentReceipt != null)
            {
                result.AddRange(await GetReferenceFileAsync(paymentReceipt.SourceId ?? Guid.Empty));
                return result;
            }

            var warehousingBill = (await _warehousingBillRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == objectId);
            if (warehousingBill != null)
            {
                result.AddRange(await GetReferenceFileAsync(warehousingBill.SourceId ?? Guid.Empty));
                return result;
            }
            #endregion

            return result;
        }

        private async Task<List<AttachmentModuleResponse>> GetAttachmentAsync(Guid objectId, AttachmentObjectType? objectType)
        {
            var result = await AttachmentByObjectIdAsync(new List<Guid> { objectId });
            switch (objectType)
            {
                #region Bút toán

                case AttachmentObjectType.Entry:
                    var entry = (await _entryRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == objectId);
                    if (entry != null)
                    {
                        var e_paymentReceipts = (await _paymentReceiptRepository.GetQueryableAsync()).Where(x => x.SourceId.Value == entry.Id || x.Id == (entry.SourceId ?? Guid.Empty)).ToList();
                        if (e_paymentReceipts.Any())
                            result.AddRange(await AttachmentByObjectIdAsync(e_paymentReceipts.Select(x => x.Id).ToList()));

                        var e_billCustomers = (await _billCustomerRepository.GetQueryableAsync()).Where(x => x.Id == (entry.SourceId ?? Guid.Empty)).ToList();
                        if (e_billCustomers.Any())
                            result.AddRange(await AttachmentByObjectIdAsync(e_billCustomers.Select(x => x.Id).ToList()));

                        var e_customerReturns = (await _customerReturnRepository.GetQueryableAsync()).Where(x => x.Id == (entry.SourceId ?? Guid.Empty)).ToList();
                        if (e_customerReturns.Any())
                            result.AddRange(await AttachmentByObjectIdAsync(e_customerReturns.Select(x => x.Id).ToList()));

                        var e_warehousingBills = (await _warehousingBillRepository.GetQueryableAsync()).Where(x => x.Id == (entry.SourceId ?? Guid.Empty)).ToList();
                        if (e_warehousingBills.Any())
                            result.AddRange(await AttachmentByObjectIdAsync(e_warehousingBills.Select(x => x.Id).ToList()));

                        var e_saleOrders = (await _saleOrderRepository.GetQueryableAsync()).Where(x => x.Id == (entry.SourceId ?? Guid.Empty)).ToList();
                        if (e_saleOrders.Any())
                            result.AddRange(await AttachmentByObjectIdAsync(e_saleOrders.Select(x => x.Id).ToList()));

                        var e_orderTransports = (await _orderTransportRepository.GetQueryableAsync()).Where(x => x.Id == (entry.SourceId ?? Guid.Empty)).ToList();
                        if (e_orderTransports.Any())
                            result.AddRange(await AttachmentByObjectIdAsync(e_orderTransports.Select(x => x.Id).ToList()));
                    }
                    break;

                #endregion Bút toán

                #region Sản phẩm

                case AttachmentObjectType.Product:
                    var products = (await _productsRepository.GetQueryableAsync()).Where(x => x.Id == objectId);
                    if (products.Any())
                        result = (await AttachmentByObjectIdAsync(products.Select(x => x.Id).ToList()));
                    break;

                #endregion Sản phẩm

                #region Chuyển kho

                case AttachmentObjectType.WarehouseTransferBill:
                    var warehouseTransferBill = (await _warehouseTransferBillRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == objectId);
                    if (warehouseTransferBill != null)
                    {
                        var e_warehousingBills = (await _warehousingBillRepository.GetQueryableAsync()).Where(x => x.SourceId.Value == warehouseTransferBill.Id).ToList();
                        if (e_warehousingBills.Any())
                            result.AddRange(await AttachmentByObjectIdAsync(e_warehousingBills.Select(x => x.Id).ToList()));
                    }
                    break;

                #endregion Chuyển kho

                #region Hóa đơn bán hàng

                case AttachmentObjectType.BillCustomer:
                    var billCustomer = (await _billCustomerRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == objectId);
                    if (billCustomer != null)
                    {
                        var e_warehousingBills = (await _warehousingBillRepository.GetQueryableAsync()).Where(x => x.SourceId.Value == billCustomer.Id).ToList();
                        if (e_warehousingBills.Any())
                            result.AddRange(await AttachmentByObjectIdAsync(e_warehousingBills.Select(x => x.Id).ToList()));
                        var e_paymentReceipts = (await _paymentReceiptRepository.GetQueryableAsync()).Where(x => x.SourceId.Value == billCustomer.Id).ToList();
                        if (e_paymentReceipts.Any())
                            result.AddRange(await AttachmentByObjectIdAsync(e_paymentReceipts.Select(x => x.Id).ToList()));
                        var e_entries = (await _entryRepository.GetQueryableAsync()).Where(x => x.SourceId.Value == billCustomer.Id).ToList();
                        if (e_entries.Any())
                            result.AddRange(await AttachmentByObjectIdAsync(e_entries.Select(x => x.Id).ToList()));
                        var e_transportInfomations = (await _transportInfomationRepository.GetQueryableAsync()).Where(x => x.SourceId.Value == billCustomer.Id).ToList();
                        if (e_transportInfomations.Any())
                            result.AddRange(await AttachmentByObjectIdAsync(e_transportInfomations.Select(x => x.Id).ToList()));
                        var e_customerReturns = (await _customerReturnRepository.GetQueryableAsync()).Where(x => x.BillCustomerId.Value == billCustomer.Id).ToList();
                        if (e_customerReturns.Any())
                            result.AddRange(await AttachmentByObjectIdAsync(e_customerReturns.Select(x => x.Id).ToList()));
                    }
                    break;

                #endregion Hóa đơn bán hàng

                #region Lịch sử vận đơn

                case AttachmentObjectType.BillHistory:
                    var transportInfomation = (await _transportInfomationRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == objectId);
                    if (transportInfomation != null)
                    {
                        var e_billCustomers = (await _billCustomerRepository.GetQueryableAsync()).Where(x => x.Id == (transportInfomation.SourceId ?? Guid.Empty)).ToList();
                        if (e_billCustomers.Any())
                            result.AddRange(await AttachmentByObjectIdAsync(e_billCustomers.Select(x => x.Id).ToList()));

                        var e_transportInfomations = (await _transportInfomationRepository.GetQueryableAsync()).Where(x => x.Id == (transportInfomation.SourceId ?? Guid.Empty)).ToList();
                        if (e_transportInfomations.Any())
                            result.AddRange(await AttachmentByObjectIdAsync(e_transportInfomations.Select(x => x.Id).ToList()));
                    }
                    break;

                #endregion Lịch sử vận đơn

                #region Trả hàng

                case AttachmentObjectType.CustomerReturn:
                    var customerReturn = (await _customerReturnRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == objectId);
                    if (customerReturn != null)
                    {
                        //var ids = customerReturns.Select(x => x.Id).ToList();
                        var e_warehousingBills = (await _warehousingBillRepository.GetQueryableAsync()).Where(x => x.SourceId.Value == customerReturn.Id).ToList();
                        if (e_warehousingBills.Any())
                            result.AddRange(await AttachmentByObjectIdAsync(e_warehousingBills.Select(x => x.Id).ToList()));
                        var e_paymentReceipts = (await _paymentReceiptRepository.GetQueryableAsync()).Where(x => x.SourceId.Value == customerReturn.Id).ToList();
                        if (e_paymentReceipts.Any())
                            result.AddRange(await AttachmentByObjectIdAsync(e_paymentReceipts.Select(x => x.Id).ToList()));
                        var e_billCustomers = (await _billCustomerRepository.GetQueryableAsync()).Where(x => x.Id == (customerReturn.BillCustomerId ?? Guid.Empty)).ToList();
                        if (e_billCustomers.Any())
                            result.AddRange(await AttachmentByObjectIdAsync(e_billCustomers.Select(x => x.Id).ToList()));
                        var e_entries = (await _entryRepository.GetQueryableAsync()).Where(x => x.SourceId.Value == customerReturn.Id).ToList();
                        if (e_entries.Any())
                            result.AddRange(await AttachmentByObjectIdAsync(e_entries.Select(x => x.Id).ToList()));
                    }
                    break;

                #endregion Trả hàng

                #region Thu chi

                case AttachmentObjectType.PaymentReceipt:
                    var paymentReceipt = (await _paymentReceiptRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == objectId);
                    if (paymentReceipt != null)
                    {
                        var e_entries = (await _entryRepository.GetQueryableAsync()).Where(x => x.SourceId.Value == paymentReceipt.Id || x.Id == (paymentReceipt.SourceId ?? Guid.Empty)).ToList();
                        if (e_entries.Any())
                            result.AddRange(await AttachmentByObjectIdAsync(e_entries.Select(x => x.Id).ToList()));
                        var e_billCustomers = (await _billCustomerRepository.GetQueryableAsync()).Where(x => x.Id == (paymentReceipt.SourceId ?? Guid.Empty)).ToList();
                        if (e_billCustomers.Any())
                            result.AddRange(await AttachmentByObjectIdAsync(e_billCustomers.Select(x => x.Id).ToList()));
                        var e_customerReturns = (await _customerReturnRepository.GetQueryableAsync()).Where(x => x.Id == (paymentReceipt.SourceId ?? Guid.Empty)).ToList();
                        if (e_customerReturns.Any())
                            result.AddRange(await AttachmentByObjectIdAsync(e_customerReturns.Select(x => x.Id).ToList()));
                        var e_warehousingBills = (await _warehousingBillRepository.GetQueryableAsync()).Where(x => x.Id == (paymentReceipt.SourceId ?? Guid.Empty)).ToList();
                        if (e_warehousingBills.Any())
                            result.AddRange(await AttachmentByObjectIdAsync(e_warehousingBills.Select(x => x.Id).ToList()));
                    }
                    break;

                #endregion Thu chi

                #region Phiếu xuất nhập kho

                case AttachmentObjectType.WarehousingBill:
                    var warehousingBill = (await _warehousingBillRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == objectId);
                    if (warehousingBill != null)
                    {
                        var e_paymentReceipts = (await _paymentReceiptRepository.GetQueryableAsync()).Where(x => x.SourceId.Value == warehousingBill.Id || x.Id == (warehousingBill.SourceId ?? Guid.Empty)).ToList();
                        if (e_paymentReceipts.Any())
                            result.AddRange(await AttachmentByObjectIdAsync(e_paymentReceipts.Select(x => x.Id).ToList()));
                        var e_entries = (await _entryRepository.GetQueryableAsync()).Where(x => x.SourceId.Value == warehousingBill.Id || x.Id == (warehousingBill.SourceId ?? Guid.Empty)).ToList();
                        if (e_entries.Any())
                            result.AddRange(await AttachmentByObjectIdAsync(e_entries.Select(x => x.Id).ToList()));
                        var e_warehouseTransferBills = (await _warehouseTransferBillRepository.GetQueryableAsync()).Where(x => x.Id == (warehousingBill.SourceId ?? Guid.Empty)).ToList();
                        if (e_warehouseTransferBills.Any())
                            result.AddRange(await AttachmentByObjectIdAsync(e_warehouseTransferBills.Select(x => x.Id).ToList()));
                        var e_customerReturns = (await _customerReturnRepository.GetQueryableAsync()).Where(x => x.Id == (warehousingBill.SourceId ?? Guid.Empty)).ToList();
                        if (e_customerReturns.Any())
                            result.AddRange(await AttachmentByObjectIdAsync(e_customerReturns.Select(x => x.Id).ToList()));
                    }
                    break;

                #endregion Phiếu xuất nhập kho

                #region Phiếu đặt hàng

                case AttachmentObjectType.SaleOrder:
                    var saleOrder = (await _saleOrderRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == objectId);
                    if (saleOrder != null)
                    {
                        var e_entries = (await _entryRepository.GetQueryableAsync()).Where(x => x.SourceId.Value == saleOrder.Id).ToList();
                        if (e_entries.Any())
                            result.AddRange(await AttachmentByObjectIdAsync(e_entries.Select(x => x.Id).ToList()));
                    }
                    break;

                #endregion Phiếu đặt hàng

                #region Đơn vận chuyển TQ

                case AttachmentObjectType.ShippingForm:
                    var orderTransport = (await _orderTransportRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == objectId);
                    if (orderTransport != null)
                    {
                        var e_entries = (await _entryRepository.GetQueryableAsync()).Where(x => x.SourceId.Value == orderTransport.Id).ToList();
                        if (e_entries.Any())
                            result.AddRange(await AttachmentByObjectIdAsync(e_entries.Select(x => x.Id).ToList()));
                    }
                    break;

                #endregion Đơn vận chuyển TQ

                #region Giao hàng nội bộ

                case AttachmentObjectType.TnternalDelivery:
                    var _transportInfomation = (await _transportInfomationRepository.GetQueryableAsync()).FirstOrDefault(x => x.Id == objectId);
                    if (_transportInfomation != null)
                    {
                        var e_billCustomers = (await _billCustomerRepository.GetQueryableAsync()).Where(x => x.Id == (_transportInfomation.SourceId ?? Guid.Empty)).ToList();
                        if (e_billCustomers.Any())
                            result.AddRange(await AttachmentByObjectIdAsync(e_billCustomers.Select(x => x.Id).ToList()));

                        var e_transportInfomations = (await _transportInfomationRepository.GetQueryableAsync()).Where(x => x.Id == (_transportInfomation.SourceId ?? Guid.Empty)).ToList();
                        if (e_transportInfomations.Any())
                            result.AddRange(await AttachmentByObjectIdAsync(e_transportInfomations.Select(x => x.Id).ToList()));
                    }
                    break;

                #endregion Giao hàng nội bộ

                default:
                    break;
            }

            return result;
        }

        private async Task<List<AttachmentModuleResponse>> GetReferenceFileAsync(Guid sourceId)
        {
            var result = new List<AttachmentModuleResponse>();

            #region Sản phẩm

            var e_products = (await _productsRepository.GetQueryableAsync()).Where(x => x.Id == sourceId).ToList();
            if (e_products.Any())
                result.AddRange(await AttachmentByObjectIdAsync(e_products.Select(x => x.Id).ToList()));

            #endregion Sản phẩm

            #region Lịch sử vận đơn/Giao hàng nội bộ

            var e_transportInfomations = (await _transportInfomationRepository.GetQueryableAsync()).Where(x => x.Id == sourceId || x.SourceId.Value == sourceId).ToList();
            if (e_transportInfomations.Any())
                result.AddRange(await AttachmentByObjectIdAsync(e_transportInfomations.Select(x => x.Id).ToList()));

            #endregion Lịch sử vận đơn/Giao hàng nội bộ

            #region Chuyển kho

            var e_warehouseTransferBills = (await _warehouseTransferBillRepository.GetQueryableAsync()).Where(x => x.Id == sourceId || x.DraftTicketId == sourceId).ToList();
            if (e_warehouseTransferBills.Any())
                result.AddRange(await AttachmentByObjectIdAsync(e_warehouseTransferBills.Select(x => x.Id).ToList()));

            #endregion Chuyển kho

            #region Bán hàng

            var e_billCustomers = (await _billCustomerRepository.GetQueryableAsync()).Where(x => x.Id == sourceId).ToList();
            if (e_billCustomers.Any())
                result.AddRange(await AttachmentByObjectIdAsync(e_billCustomers.Select(x => x.Id).ToList()));

            #endregion Bán hàng

            #region Trả hàng

            var e_customerReturns = (await _customerReturnRepository.GetQueryableAsync()).Where(x => x.Id == sourceId || x.BillCustomerId.Value == sourceId).ToList();
            if (e_customerReturns.Any())
                result.AddRange(await AttachmentByObjectIdAsync(e_customerReturns.Select(x => x.Id).ToList()));

            #endregion Trả hàng

            #region Phiếu thu chi

            var e_paymentReceipts = (await _paymentReceiptRepository.GetQueryableAsync()).Where(x => x.Id == sourceId || x.SourceId.Value == sourceId).ToList();
            if (e_paymentReceipts.Any())
                result.AddRange(await AttachmentByObjectIdAsync(e_paymentReceipts.Select(x => x.Id).ToList()));

            #endregion Phiếu thu chi

            #region bút toán

            var e_entries = (await _entryRepository.GetQueryableAsync()).Where(x => x.Id == sourceId || x.SourceId.Value == sourceId).ToList();
            if (e_entries.Any())
                result.AddRange(await AttachmentByObjectIdAsync(e_entries.Select(x => x.Id).ToList()));

            #endregion bút toán

            #region Xuất nhập kho

            var e_warehousingBills = (await _warehousingBillRepository.GetQueryableAsync()).Where(x => x.Id == sourceId || x.SourceId.Value == sourceId).ToList();
            if (e_warehousingBills.Any())
                result.AddRange(await AttachmentByObjectIdAsync(e_warehousingBills.Select(x => x.Id).ToList()));

            #endregion Xuất nhập kho

            #region Phiếu đặt hàng

            var e_saleOrders = (await _saleOrderRepository.GetQueryableAsync()).Where(x => x.Id == sourceId).ToList();
            if (e_saleOrders.Any())
                result.AddRange(await AttachmentByObjectIdAsync(e_saleOrders.Select(x => x.Id).ToList()));

            #endregion Phiếu đặt hàng

            #region Đơn vận chuyển TQ

            var e_orderTransports = (await _orderTransportRepository.GetQueryableAsync()).Where(x => x.Id == sourceId).ToList();
            if (e_orderTransports.Any())
                result.AddRange(await AttachmentByObjectIdAsync(e_orderTransports.Select(x => x.Id).ToList()));

            #endregion Đơn vận chuyển TQ

            return result;
        }

        private async Task<List<AttachmentModuleResponse>> AttachmentByObjectIdAsync(List<Guid> objectIds)
        {
            var result = new List<AttachmentModuleResponse>();
            var attachments = await _attachmentRepository.GetListAsync(x => objectIds.Any(objectId => objectId == x.ObjectId.Value)) ?? new List<Attachment>();

            if (!attachments.Any())
                return result;

            result = attachments.Select(x => new AttachmentModuleResponse
            {
                Id = x.Id,
                ObjectId = x.ObjectId,
                ObjectType = x.ObjectType,
                ObjectTypeName = SetObjectTypeNameAsync(x.ObjectType.Value),
                FileName = x.Name,
                FileUrl = x.Url,
                CreatorId = x.CreatorId,
                CreationTime = x.CreationTime
            })
            .OrderBy(x => x.CreationTime)
            .ToList();
            var users = (await _userRepository.GetListAsync()).Where(x => result.Any(r => r.CreatorId == x.Id)).ToList() ?? new List<IdentityUser>();
            foreach (var item in result)
            {
                if (!string.IsNullOrEmpty(item.FileName))
                {
                    var splitFileName = item.FileName.Split('.');
                    var extensionFile = splitFileName[splitFileName.Length - 1];

                    if (extensionFile.Trim().ToLower() == "pdf")
                        item.TypeFile = TypeFile.Pdf;

                    if (extensionFile.Trim().ToLower() == "xlsx" || extensionFile.Trim().ToLower() == "xls")
                        item.TypeFile = TypeFile.Excel;

                    if (extensionFile.Trim().ToLower() == "docx" || extensionFile.Trim().ToLower() == "doc")
                        item.TypeFile = TypeFile.Word;
                }
                var user = users.FirstOrDefault(x => x.Id == item.CreatorId);
                item.CreatorName = user == null ? null : user.Name;
            }

            return result;
        }

        private string SetObjectTypeNameAsync(AttachmentObjectType objectType)
        {
            switch (objectType)
            {
                case AttachmentObjectType.Entry:
                    return "Bút toán";

                case AttachmentObjectType.Product:
                    return "Sản phẩm";

                case AttachmentObjectType.WarehouseTransferBill:
                    return "Chuyển kho";

                case AttachmentObjectType.BillCustomer:
                    return "Hóa đơn bán hàng";

                case AttachmentObjectType.BillHistory:
                    return "Lịch sử vận đơn";

                case AttachmentObjectType.CustomerReturn:
                    return "Trả hàng";

                case AttachmentObjectType.PaymentReceipt:
                    return "Thu chi";

                case AttachmentObjectType.WarehousingBill:
                    return "Phiếu xuất nhập kho";

                case AttachmentObjectType.SaleOrder:
                    return "Phiếu đặt hàng";

                case AttachmentObjectType.ShippingForm:
                    return "Đơn vận chuyển TQ";

                case AttachmentObjectType.TnternalDelivery:
                    return "Giao hàng nội bộ";

                default:
                    return string.Empty;
            }
        }

        #endregion VBN
    }
}