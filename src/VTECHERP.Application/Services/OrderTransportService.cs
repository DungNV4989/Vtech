using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vinpearl.Modelling.Library.Utility.Excel;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Users;
using VTECHERP.Domain.Shared.Helper.Excel.Model;
using VTECHERP.DTOs.OrderTransports;
using VTECHERP.DTOs.OrderTransports.Params;
using VTECHERP.DTOs.SaleOrders;
using VTECHERP.Entities;
using Cell = VTECHERP.Domain.Shared.Helper.Excel.Model.Cell;

namespace VTECHERP.Services
{
    public class OrderTransportService : IOrderTransportService
    {
        private readonly IRepository<OrderTransport> _orderTransportRepository;
        private readonly IRepository<OrderTransportSale> _orderTransportSaleRepository;
        private readonly IRepository<SaleOrders> _saleOrderRepository;
        private readonly IRepository<Suppliers> _supplierRepository;
        private readonly IRepository<UserStore> _userStoreRepository;
        private readonly ICurrentUser _currentUser;

        private readonly IAttachmentService _attachmentService;
        public OrderTransportService(
            IRepository<OrderTransport> orderTransportRepository
            , IRepository<OrderTransportSale> orderTransportSaleRepository,
            IRepository<SaleOrders> saleOrderRepository,
            IRepository<Suppliers> supplierRepository,
            IRepository<UserStore> userStoreRepository,
            ICurrentUser currentUser,
            IAttachmentService attachmentService)
        {
            _orderTransportRepository = orderTransportRepository;
            _orderTransportSaleRepository = orderTransportSaleRepository;
            _saleOrderRepository = saleOrderRepository;
            _supplierRepository = supplierRepository;
            _userStoreRepository = userStoreRepository;
            _currentUser = currentUser;
            _attachmentService = attachmentService;
        }

        public async Task<Guid> Create(List<Guid> OrderSalesId)
        {
            //var orderTransportId = Guid.NewGuid();
            var orderTransport = new OrderTransport();
            await _orderTransportRepository.InsertAsync(orderTransport);
            var orderTransportId = orderTransport.Id;
            var orderTransportSales = new List<OrderTransportSale>();

            foreach (var item in OrderSalesId)
            {
                var orderTransportSale = new OrderTransportSale();
                orderTransportSale.OrderTransportId = orderTransportId;
                orderTransportSale.OrderSaleId = item;

                orderTransportSales.Add(orderTransportSale);
            }


            await _orderTransportSaleRepository.InsertManyAsync(orderTransportSales);

            return orderTransportId;
        }

        public async Task<byte[]> ExportAsync(GetListOrderTransportParm param)
        {
            var workbook = new CustomWorkBook();
            var orderTransports = await GetList(param);
            var sheet = RenderTemplate(orderTransports.ToList());
            workbook.Sheets.Add(sheet);

            return ExcelHelper.ExportExcel(workbook);
        }

        private CustomSheet RenderTemplate(List<OrderTransportItemList> data)
        {
            var sheet = new CustomSheet("Danh sách đơn vận chuyển");

            var startRow = 2;

            var header = new CustomDataTable()
            {
                StartRowIndex = startRow,
                StartColumnIndex = 1,
                RowDirection = Directions.Horizontal,
                Rows = new List<DataRow>
                    {
                        new DataRow(
                            new HeaderCell("STT"),
                            new HeaderCell("ID"),
                            new HeaderCell("Nhà cung cấp"),
                            new HeaderCell("Số hóa đơn"),
                            new HeaderCell("Tổng số tiền"),
                            new HeaderCell("Nhà vận chuyển"),
                            new HeaderCell("Mã hóa đơn"),
                            new HeaderCell("Trạng thái")
                            )
                    }
            };

            sheet.Tables.Add(header);

            var indexFirstColumn = startRow + 1;
            for (int i = 0; i < data.Count; i++)
            {
                var index = i + 1;
                var firstColumn = new CustomDataTable()
                {
                    StartRowIndex = indexFirstColumn,
                    StartColumnIndex = 1,
                    RowDirection = Directions.Horizontal,
                    Rows = new List<DataRow>
                    {
                        new DataRow(
                            new Cell(index){
                                RowSpan = data[i].SaleOrders.Count,
                            }
                            )
                    }
                };

                sheet.Tables.Add(firstColumn);
                indexFirstColumn += data[i].SaleOrders.Count;
            }

            var indexSaleOrderColumn = startRow + 1;
            foreach (var item in data)
            {
                foreach (var itemSaleOrder in item.SaleOrders)
                {
                    var row = new CustomDataTable()
                    {
                        StartRowIndex = indexSaleOrderColumn++,
                        StartColumnIndex = 2,
                        RowDirection = Directions.Horizontal,
                        Rows = new List<DataRow>
                        {
                        new DataRow(
                            new Cell(itemSaleOrder.Code) { Format = "@"},
                            new Cell(itemSaleOrder.SupplierName),
                            new Cell(itemSaleOrder.InvoiceNumber)
                            )
                    }
                    };

                    sheet.Tables.Add(row);
                }
            }

            var threeTableIndex = startRow + 1;
            for (int i = 0; i < data.Count; i++)
            {
                var firstColumn = new CustomDataTable()
                {
                    StartRowIndex = threeTableIndex,
                    StartColumnIndex = 5,
                    RowDirection = Directions.Horizontal,
                    Rows = new List<DataRow>
                    {
                        new DataRow(
                            new Cell(data[i].TotalPrice){
                                RowSpan = data[i].SaleOrders.Count,
                            },
                            new Cell(data[i].TransporterText){
                                RowSpan = data[i].SaleOrders.Count,
                            },
                            new Cell(data[i].TransportCode){
                                RowSpan = data[i].SaleOrders.Count,
                            },
                            new Cell(data[i].Status == Enums.OrderTransportStatus.Received ? "Đã nhận" : "Chưa nhận"){
                                RowSpan = data[i].SaleOrders.Count,
                            }
                            )
                    }
                };

                sheet.Tables.Add(firstColumn);
                threeTableIndex += data[i].SaleOrders.Count;
            }

            return sheet;
        }

        public async Task<IQueryable<OrderTransportItemList>> GetList(GetListOrderTransportParm param)
        {
            // var result = new List<OrderTransportItemList>();
            var orderTransports = await GetOrderTransportFull();

            if (param.Status.HasValue)
                orderTransports = orderTransports.Where(x => x.Status == param.Status);

            if (!string.IsNullOrEmpty(param.Transporter))
                orderTransports = (orderTransports.Where(delegate (OrderTransportItemList x)
                {
                    if (!string.IsNullOrEmpty(x.TransporterText))
                        return x.TransporterText.Trim().ToLower().Contains(param.Transporter.Trim().ToLower());

                    return false;
                })).AsQueryable();

            if (!string.IsNullOrEmpty(param.OrderTransportCode))
                orderTransports = (orderTransports.Where(delegate (OrderTransportItemList x)
                {
                    if (!string.IsNullOrEmpty(x.TransportCode))
                        return x.TransportCode.Trim().ToLower().Contains(param.OrderTransportCode.Trim().ToLower());

                    return false;
                })).AsQueryable();

            if (param.FromDate.HasValue)
                orderTransports = orderTransports.Where(x => x.CreateTime.Date >= param.FromDate.Value.Date);

            if (param.ToDate.HasValue)
                orderTransports = orderTransports.Where(x => x.CreateTime.Date <= param.ToDate.Value.Date);

            if (!string.IsNullOrEmpty(param.SaleOrderCode))
                orderTransports = orderTransports.Where(delegate (OrderTransportItemList x)
                {
                    var codes = x.SaleOrders.Select(x => x.Code.Trim()).ToList();

                    return codes.Any(x => x.Contains(param.SaleOrderCode.Trim()));
                }).AsQueryable();

            if (!string.IsNullOrEmpty(param.Suplier))
                orderTransports = orderTransports.Where(x => x.SaleOrders.Select(x => x.SupplierName.Trim().ToLower()).Any(x => x.Contains(param.Suplier.Trim().ToLower())));

            return orderTransports.OrderByDescending(x => x.CreateTime);

        }

        public async Task<IQueryable<OrderTransportItemList>> GetOrderTransportFull()
        {
            var userStore = (await _userStoreRepository.GetQueryableAsync()).Where(x => x.UserId == _currentUser.GetId());

            var saleOrders = (await _saleOrderRepository.GetQueryableAsync()).Where(x => userStore.Any(us => us.StoreId == x.StoreId));
            var orderTransportSales = (await _orderTransportSaleRepository.GetQueryableAsync()).Where(x=> saleOrders.Any(sO => sO.Id == x.OrderSaleId));
            var orderTransports = (await _orderTransportRepository.GetQueryableAsync()).Where(x=>orderTransportSales.Any(oTS => oTS.OrderTransportId == x.Id));
            
            var suppliers = await _supplierRepository.GetListAsync();
            #region
            //var result = (from transport in orderTransports

            //              join transporter in suppliers
            //              on transport.TransporterId equals transporter.Id
            //              into transporterGr
            //              from transporter in transporterGr.DefaultIfEmpty()

            //              let saleOrderss = (from saleOrder in saleOrders

            //                                join transportSale in orderTransportSales
            //                                on transport.Id equals transportSale.OrderTransportId

            //                                join suplier in suppliers
            //                                on saleOrder.SupplierId equals suplier.Id
            //                                into suplierGr
            //                                from suplier in suplierGr.DefaultIfEmpty()

            //                                where transportSale.OrderSaleId == saleOrder.Id
            //                                select new SaleOrderCommonDto
            //                                {
            //                                    InvoiceNumber = saleOrder.InvoiceNumber,
            //                                    Code = saleOrder.Code,
            //                                    Id = saleOrder.Id,
            //                                    SupplierId = saleOrder.SupplierId,
            //                                    SupplierName = suplier.Name,
            //                                }).ToList()

            //              select new OrderTransportItemList
            //              {
            //                  Id = transport.Id,
            //                  CreateTime = transport.CreationTime,
            //                  Status = transport.Status,
            //                  TransportCode = transport.TransportCode,
            //                  TransporterText = transporter.Name,
            //                  SaleOrders = saleOrderss,
            //                  TotalPrice = transport.TotalPrice,
            //                  DateArrive = transport.DateArrive,
            //                  DateTransport = transport.DateTransport,
            //                  TransporterId = transport.TransporterId
            //              });
            #endregion

            var r = new List<OrderTransportItemList>();
            foreach(var transport in orderTransports)
            {
                var saleOrderCommonDtos = new List<SaleOrderCommonDto>();
                var oTSs = orderTransportSales.Where(x => x.OrderTransportId == transport.Id);
                var sOs = saleOrders.Where(x => oTSs.Any(oTS => oTS.OrderSaleId == x.Id));
                foreach (var sO in sOs)
                {
                    var supplier = suppliers.FirstOrDefault(x => x.Id == sO.SupplierId);
                    saleOrderCommonDtos.Add(new SaleOrderCommonDto()
                    {
                        InvoiceNumber = sO.InvoiceNumber,
                        Code = sO.Code,
                        Id = sO.Id,
                        SupplierId = sO.SupplierId,
                        SupplierName = supplier == null ? null : supplier.Name,
                        CreationTime = sO.CreationTime,
                    });
                }

                r.Add(new OrderTransportItemList()
                {
                    Id = transport.Id,
                    CreateTime = transport.CreationTime,
                    Status = transport.Status,
                    TransportCode = transport.TransportCode,
                    TransporterText = suppliers.FirstOrDefault(x => x.Id == transport.TransporterId)?.Name,
                    SaleOrders = saleOrderCommonDtos,
                    TotalPrice = transport.TotalPrice,
                    DateArrive = transport.DateArrive,
                    DateTransport = transport.DateTransport,
                    TransporterId = transport.TransporterId
                });
            }


            return r.AsQueryable();
        }
    }
}
