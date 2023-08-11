using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Uow;
using VTECHERP.Datas;
using VTECHERP.DTOs.Base;
using VTECHERP.Entities;
using VTECHERP.Enums;

namespace VTECHERP.Services
{
    public class CommonService : ICommonService
    {
        private readonly IRepository<Suppliers> _supplierRepository;
        private readonly IRepository<SupplierOrderReport> _supplierOrderReportRepository;
        private readonly IUnitOfWorkManager _unitOfWorkManager;
        private readonly IRepository<PaymentReceipt> _paymentReceiptRepository;
        private readonly IRepository<SaleOrders> _saleOrderRepository;
        private readonly IRepository<SaleOrderLines> _saleOrderLineRepository;
        private readonly IRepository<SaleOrderLineConfirm> _saleOrderLineConfirmRepository;
        private readonly IRepository<Customer> _customerRepository;

        public CommonService(
            IRepository<Suppliers> supplierRepository,
            IRepository<SupplierOrderReport> supplierOrderReportRepository,
            IUnitOfWorkManager unitOfWorkManager,
            IRepository<PaymentReceipt> paymentReceiptRepository,
            IRepository<SaleOrders> saleOrderRepository,
            IRepository<SaleOrderLines> saleOrderLineRepository,
            IRepository<SaleOrderLineConfirm> saleOrderLineConfirmRepository
            , IRepository<Customer> customerRepository
            )
        {
            _supplierRepository = supplierRepository;
            _supplierOrderReportRepository = supplierOrderReportRepository;
            _unitOfWorkManager = unitOfWorkManager;
            _paymentReceiptRepository = paymentReceiptRepository;
            _saleOrderRepository = saleOrderRepository;
            _saleOrderLineRepository = saleOrderLineRepository;
            _saleOrderLineConfirmRepository = saleOrderLineConfirmRepository;
            _customerRepository = customerRepository;
        }

        public async Task<MasterDataDTO?> GetAudience(AudienceTypes type, Guid? id)
        {
            if (id == null)
            {
                return null;
            }
            switch (type)
            {
                case AudienceTypes.SupplierCN:
                case AudienceTypes.SupplierVN:
                    return (await _supplierRepository
                        .GetQueryableAsync())
                        .Where(p => p.Id == id
                        )
                        .Select(p => new MasterDataDTO { Code = p.Code, Name = p.Name, Phone = p.PhoneNumber, Id = p.Id })
                        .FirstOrDefault();

                case AudienceTypes.Customer:
                    break;

                case AudienceTypes.Employee:
                    break;

                default:
                    return null;
            }

            return null;
        }

        public async Task<List<MasterDataDTO>> GetAudiences(params (AudienceTypes, Guid?)[] request)
        {
            var suppliers = await _supplierRepository.GetListAsync();
            var audiences = new List<MasterDataDTO>();
            foreach (var item in request)
            {
                if (item.Item2 != null && !audiences.Any(p => p.Id == item.Item2))
                {
                    switch (item.Item1)
                    {
                        case AudienceTypes.SupplierCN:
                        case AudienceTypes.SupplierVN:
                            var aud = suppliers.Where(p => p.Id == item.Item2)
                                .Select(p => new MasterDataDTO { Code = p.Code, Name = p.Name, Phone = p.PhoneNumber, Id = p.Id })
                                .FirstOrDefault();
                            if (aud != null)
                            {
                                audiences.Add(aud);
                            }
                            break;

                        case AudienceTypes.Customer:
                            var customer = await _customerRepository.GetListAsync(p => p.Id == item.Item2);
                            if (customer.Any())
                            {
                                var result = customer.Select(p => new MasterDataDTO { Code = p.Code, Name = p.Name, Phone = p.PhoneNumber, Id = p.Id }).FirstOrDefault();
                                audiences.Add(result);
                            }
                            break;

                        case AudienceTypes.Employee:
                            break;

                        default:
                            break;
                    }
                }
            }

            return audiences;
        }

        public string GetDocumentTypeName(DocumentTypes type)
        {
            var data = MasterDatas.DocumentTypes;
            var docType = data.FirstOrDefault(p => p.Id == type);
            return docType?.Name;
        }

        public async Task TriggerCalculateSupplierOrderReport(Guid supplierId, DateTime date, bool isOrder = false)
        {
            var uow = _unitOfWorkManager.Current;
            try
            {
                var allSupplierOrders = await _saleOrderRepository.GetListAsync(p => p.OrderDate.Date >= date.Date && p.SupplierId == supplierId);
                var isCreateReport = isOrder;
                var allDateSupplierOrderCount = allSupplierOrders.Where(p => p.OrderDate.Date == date.Date.Date).Count();
                if (allDateSupplierOrderCount == 0)
                {
                    // delete date report
                    var dateReport = await _supplierOrderReportRepository.FirstOrDefaultAsync(p => p.SupplierId == supplierId && p.Date.Date == date.Date.Date);
                    if (dateReport != null)
                    {
                        isCreateReport = false;
                        await _supplierOrderReportRepository.HardDeleteAsync(dateReport);
                        await uow.SaveChangesAsync();
                    }
                }

                var currentReport = await _supplierOrderReportRepository
                    .FirstOrDefaultAsync(p => p.Date.Date == date.Date.Date && p.SupplierId == supplierId);
                var latestReportBefore =
                    (await _supplierOrderReportRepository
                        .GetQueryableAsync())
                        .Where(p => p.Date.Date < date.Date && p.SupplierId == supplierId)
                        .OrderByDescending(p => p.Date)
                        .FirstOrDefault();

                var allPaidReceipt = await _paymentReceiptRepository.GetListAsync(p =>
                    p.AudienceId == supplierId
                    && (p.TicketType == TicketTypes.DebitNote || p.TicketType == TicketTypes.PaymentVoucher)
                    && (latestReportBefore == null || p.TransactionDate.Date > latestReportBefore.Date)
                );

                var allOrderIds = allSupplierOrders.Select(p => p.Id).ToList();
                var allSupplierOrderLines = await _saleOrderLineRepository.GetListAsync(p => allOrderIds.Contains(p.OrderId));
                var allConfirms = await _saleOrderLineConfirmRepository
                    .GetListAsync(p => p.SupplierId == supplierId
                                        && (latestReportBefore == null || p.ConfirmDate.Date > latestReportBefore.Date));

                var allLackCompleteOrders = await _saleOrderRepository.GetListAsync(p =>
                    p.Confirm == SaleOrder.Confirm.Lack
                    && p.CompleteDate.HasValue
                    && (latestReportBefore == null || p.CompleteDate.Value.Date > latestReportBefore.Date)
                    && p.SupplierId == supplierId
                );
                var allLackCompleteOrderIds = allLackCompleteOrders.Select(p => p.Id).ToList();
                var alllackCompleteOrderLines = (await _saleOrderLineRepository.GetQueryableAsync())
                    .Where(p =>
                        allLackCompleteOrderIds.Contains(p.OrderId)
                    ).ToList();

                var report = await CalculateNextReport(supplierId, date, isCreateReport, currentReport, latestReportBefore, allSupplierOrders, allSupplierOrderLines, allConfirms, allPaidReceipt, allLackCompleteOrders, alllackCompleteOrderLines);
                var nextReportCheckDate = date;
                if (report != null)
                {
                    nextReportCheckDate = report.Date.Date;
                }
                await uow.SaveChangesAsync();
                var nextReports = await _supplierOrderReportRepository.GetListAsync(p => p.SupplierId == supplierId && p.Date > nextReportCheckDate);
                if (nextReports.Any())
                {
                    if (report != null)
                    {
                        nextReports.Add(report);
                        nextReports = nextReports.OrderBy(p => p.Date).ToList();
                    }
                    else if (latestReportBefore != null)
                    {
                        nextReports.Add(latestReportBefore);
                        nextReports = nextReports.OrderBy(p => p.Date).ToList();
                    }
                    else
                    {
                        nextReports = nextReports.OrderBy(p => p.Date).ToList();
                        nextReports.Insert(0, null);
                    }

                    for (int i = 1; i < nextReports.Count; i++)
                    {
                        var curReport = nextReports[i];
                        var latestReport = nextReports[i - 1];
                        await CalculateNextReport(supplierId, curReport.Date, false, curReport, latestReport, allSupplierOrders, allSupplierOrderLines, allConfirms, allPaidReceipt, allLackCompleteOrders, alllackCompleteOrderLines);
                        await uow.SaveChangesAsync();
                    }
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<SupplierOrderReport> CalculateNextReport(
            Guid supplierId,
            DateTime date,
            bool isCreateReport,
            SupplierOrderReport currentReport,
            SupplierOrderReport latestReportBefore,
            List<SaleOrders> allSupplierOrders,
            List<SaleOrderLines> allSupplierOrderLines,
            List<SaleOrderLineConfirm> allConfirms,
            List<PaymentReceipt> allPaidReceipt,
            List<SaleOrders> allLackCompleteOrders,
            List<SaleOrderLines> alllackCompleteOrderLines
            )
        {
            // nếu dữ liệu đầu vào là đơn hàng, tạo hoặc cập nhật báo cáo
            var dateOrders = allSupplierOrders.Where(p => p.OrderDate.Date == date.Date).ToList();
            var dateOrderLines = allSupplierOrderLines.Where(p => dateOrders.Any(o => o.Id == p.OrderId)).ToList();
            // tổng nợ phát sinh trong ngày
            var totalDebtCNY = dateOrderLines.Sum(p => p.RequestPrice * p.RequestQuantity);

            var confirmsToDate = allConfirms.Where(p => p.ConfirmDate.Date < date.Date && (latestReportBefore == null || p.ConfirmDate.Date > latestReportBefore.Date)).ToList();
            var confirmsInDate = allConfirms.Where(p => p.ConfirmDate.Date == date.Date).ToList();

            // tổng đã nhận đến đầu ngày (tính từ kỳ trước)
            var totalConfirmToDateCNY = confirmsToDate.Sum(p => (p.Quantity) * p.EntryPrice);
            var totalConfirmNoSurplusToDateCNY = confirmsToDate.Sum(p =>
            {
                int completeQuantity = 0;
                if (p.IsSurplus)
                {
                    completeQuantity = ((p.Quantity - p.SurplusQuantity) < 0 ? 0 : (p.Quantity - p.SurplusQuantity)) ?? 0;
                }
                else
                {
                    completeQuantity = p.Quantity ?? 0;
                }
                return completeQuantity * (p.EntryPrice ?? 0);
            });

            // tổng đã nhận trong ngày
            var totalConfirmInDateCNY = confirmsInDate.Sum(p => p.Quantity * p.EntryPrice);
            var totalConfirmNoSurplusInDateCNY = confirmsInDate.Sum(p =>
            {
                int completeQuantity = 0;
                var importQuantity = p.Quantity ?? 0;

                if (p.IsSurplus)
                {
                    completeQuantity = ((importQuantity - p.SurplusQuantity) < 0 ? 0 : (importQuantity - p.SurplusQuantity));
                }
                else
                {
                    completeQuantity = p.Quantity ?? 0;
                }
                return completeQuantity * (p.EntryPrice ?? 0);
            });
            // tổng đang giao (thêm) = tổng nợ phát sinh trong ngày
            var totalDeliveryCNY = totalDebtCNY;

            var paidToDate = allPaidReceipt.Where(p => p.TransactionDate.Date < date.Date && (latestReportBefore == null || p.TransactionDate.Date > latestReportBefore.Date)).ToList();
            var paidInDate = allPaidReceipt.Where(p => p.TransactionDate.Date == date.Date);
            // đã thanh toán đến đầu ngày (tính từ kỳ trước)
            var totalPaidToDateCNY = paidToDate.Sum(p => p.AmountCNY ?? 0);
            // đã thanh toán trong ngày
            var totalPaidInDateCNY = paidInDate.Sum(p => p.AmountCNY ?? 0);

            // tính tổng đơn hàng hoàn thành thiếu
            var lackCompleteOrdersToDate = allLackCompleteOrders.Where(p => p.CompleteDate.Value.Date < date.Date);
            var lackCompleteOrdersInDate = allLackCompleteOrders.Where(p => p.CompleteDate.Value.Date == date.Date);

            var lackCompleteOrderToDateIds = lackCompleteOrdersToDate.Select(p => p.Id).ToList();
            var lackCompleteOrdersInDateIds = lackCompleteOrdersInDate.Select(p => p.Id).ToList();

            var completeDeliveryLackToDate = alllackCompleteOrderLines
                .Where(p =>
                    lackCompleteOrderToDateIds.Contains(p.OrderId)
                    && (p.ImportQuantity ?? 0) < (p.RequestQuantity ?? 0)
                ).Sum(p => ((p.RequestQuantity ?? 0) - (p.ImportQuantity ?? 0)) * (p.RequestPrice ?? 0));

            var completeDeliveryLackInDate = alllackCompleteOrderLines
                .Where(p =>
                    lackCompleteOrdersInDateIds.Contains(p.OrderId)
                    && (p.ImportQuantity ?? 0) < (p.RequestQuantity ?? 0)
                ).Sum(p => ((p.RequestQuantity ?? 0) - (p.ImportQuantity ?? 0)) * (p.RequestPrice ?? 0));

            // nếu kỳ đã được tính report
            // Tính lại số tiền đã trả trong kỳ
            if (currentReport != null)
            {
                // Số thanh toán đầu kỳ = tổng số thanh toán đến đầu kỳ
                currentReport.PaidSinceLastReportCNY = totalPaidToDateCNY;
                // Dư nợ đầu kỳ = Dư nợ cuối kỳ trước - Số thanh toán giữa 2 kỳ
                currentReport.DebtToDateCNY = (latestReportBefore?.DebtFinalCNY ?? 0) - currentReport.PaidSinceLastReportCNY;

                // Đã xác nhận đầu kỳ = tổng xác nhận kỳ trước + đã xác nhận đến kỳ
                currentReport.ConfirmedSinceLastReportCNY = totalConfirmToDateCNY ?? 0;
                currentReport.ConfirmedToDateCNY = (latestReportBefore?.ConfirmedFinalCNY ?? 0) + currentReport.ConfirmedSinceLastReportCNY;

                // Đang di chuyển đầu kỳ = Đang di chuyển cuối kỳ trước - đã xác nhận đến kỳ
                // 26/6: đã xác nhận đến kỳ không tính xác nhận thừa
                currentReport.DeliveryToDateCNY = (latestReportBefore?.DeliveryFinalCNY ?? 0) - totalConfirmNoSurplusToDateCNY - completeDeliveryLackToDate;
                if (currentReport.DeliveryToDateCNY < 0)
                {
                    currentReport.DeliveryToDateCNY = 0;
                }
                // tính các dữ liệu phát sinh trong kỳ
                currentReport.DebtInDateCNY = totalDebtCNY ?? 0;
                currentReport.PaidInDateCNY = totalPaidInDateCNY;
                currentReport.ConfirmedInDateCNY = totalConfirmInDateCNY ?? 0;
                currentReport.DeliveryInDateCNY = totalDeliveryCNY ?? 0;

                // tính số cuối kỳ
                // nợ cuối kỳ = nợ đầu kỳ + nợ trong kỳ - thanh toán trong kỳ
                currentReport.DebtFinalCNY = currentReport.DebtToDateCNY + currentReport.DebtInDateCNY - currentReport.PaidInDateCNY;

                // đang di chuyển cuối kỳ = đang di chuyển đầu ky + đang di chuyển giữa kỳ - đã xác nhận giữa kỳ
                // 26/6: đã xác nhận giữa kỳ không tính xác nhận thừa
                currentReport.DeliveryFinalCNY = (currentReport.DeliveryToDateCNY + currentReport.DeliveryInDateCNY) - totalConfirmNoSurplusInDateCNY - completeDeliveryLackInDate;
                if (currentReport.DeliveryFinalCNY < 0)
                {
                    currentReport.DeliveryFinalCNY = 0;
                }
                // tổng đã về
                currentReport.ConfirmedFinalCNY = currentReport.ConfirmedToDateCNY + currentReport.ConfirmedInDateCNY;

                await _supplierOrderReportRepository.UpdateAsync(currentReport);
                await UpdateOrderByReport(dateOrders, dateOrderLines, currentReport);
                return currentReport;
            }
            // tạo report mới
            else if (isCreateReport)
            {
                var report = new SupplierOrderReport
                {
                    SupplierId = supplierId,
                    Date = date,
                };
                // Số thanh toán đầu kỳ = tổng số thanh toán đến đầu kỳ
                report.PaidSinceLastReportCNY = totalPaidToDateCNY;
                // Dư nợ đầu kỳ = Dư nợ cuối kỳ trước - Số thanh toán giữa 2 kỳ
                report.DebtToDateCNY = (latestReportBefore?.DebtFinalCNY ?? 0) - report.PaidSinceLastReportCNY;

                // Đã xác nhận đầu kỳ = tổng xác nhận kỳ trước + đã xác nhận đến kỳ
                report.ConfirmedSinceLastReportCNY = totalConfirmToDateCNY ?? 0;
                report.ConfirmedToDateCNY = (latestReportBefore?.ConfirmedFinalCNY ?? 0) + report.ConfirmedSinceLastReportCNY;

                // Đang di chuyển đầu kỳ = Đang di chuyển cuối kỳ trước - đã xác nhận đến kỳ
                // 26/6: đã xác nhận đến kỳ không tính xác nhận thừa
                report.DeliveryToDateCNY = (latestReportBefore?.DeliveryFinalCNY ?? 0) - totalConfirmNoSurplusToDateCNY;
                if (report.DeliveryToDateCNY < 0)
                {
                    report.DeliveryToDateCNY = 0;
                }
                // tính các dữ liệu phát sinh trong kỳ
                report.DebtInDateCNY = totalDebtCNY ?? 0;
                report.PaidInDateCNY = totalPaidInDateCNY;
                report.ConfirmedInDateCNY = totalConfirmInDateCNY ?? 0;
                report.DeliveryInDateCNY = totalDeliveryCNY ?? 0;

                // tính số cuối kỳ
                // nợ cuối kỳ = nợ đầu kỳ + nợ trong kỳ - thanh toán trong kỳ
                report.DebtFinalCNY = report.DebtToDateCNY + report.DebtInDateCNY - report.PaidInDateCNY;

                // đang di chuyển cuối kỳ = đang di chuyển đầu ky + đang di chuyển giữa kỳ - đã xác nhận giữa kỳ
                // 26/6: đã xác nhận giữa kỳ không tính xác nhận thừa
                report.DeliveryFinalCNY = (report.DeliveryToDateCNY + report.DeliveryInDateCNY) - totalConfirmNoSurplusInDateCNY;
                if (report.DeliveryFinalCNY < 0)
                {
                    report.DeliveryFinalCNY = 0;
                }
                // tổng đã về
                report.ConfirmedFinalCNY = report.ConfirmedToDateCNY + report.ConfirmedInDateCNY;
                await _supplierOrderReportRepository.InsertAsync(report);
                await UpdateOrderByReport(dateOrders, dateOrderLines, report);
                return report;
            }

            return null;
        }

        private async Task UpdateOrderByReport(List<SaleOrders> orders, List<SaleOrderLines> lines, SupplierOrderReport report)
        {
            orders = orders.OrderBy(p => p.Code).ToList();
            if (orders.Count == 1)
            {
                var curOrder = orders.FirstOrDefault();
                curOrder.DebtBeforeCNY = report?.DebtToDateCNY ?? 0;
                if (report.DebtToDateCNY < 0)
                {
                    curOrder.DebtBeforeCNY = 0;
                }
                curOrder.DeliveryBeforeCNY = report?.DeliveryToDateCNY ?? 0;
                curOrder.ConfirmedBeforeCNY = report?.ConfirmedToDateCNY ?? 0;

                curOrder.PaidCNY = report.PaidSinceLastReportCNY + report.PaidInDateCNY;
                curOrder.DeliveryFinalCNY = report.DeliveryFinalCNY;
                curOrder.ConfirmedFinalCNY = report.ConfirmedFinalCNY;
                curOrder.DebtFinalCNY = report.DebtFinalCNY;
            }
            else
            {
                for (int i = 0; i < orders.Count; i++)
                {
                    var curOrder = orders[i];
                    if (i == 0)
                    {
                        curOrder.DebtBeforeCNY = report?.DebtToDateCNY ?? 0;
                        if (report.DebtToDateCNY < 0)
                        {
                            curOrder.DebtBeforeCNY = 0;
                        }
                        curOrder.DeliveryBeforeCNY = report?.DeliveryToDateCNY ?? 0;
                        curOrder.ConfirmedBeforeCNY = report?.ConfirmedToDateCNY ?? 0;
                        curOrder.PaidCNY = report?.PaidSinceLastReportCNY ?? 0;

                        var curOrderLines = lines.Where(p => p.OrderId == curOrder.Id).ToList();
                        var debt = curOrderLines.Sum(p => p.RequestQuantity * p.RequestPrice);

                        curOrder.DebtFinalCNY = curOrder.DebtBeforeCNY + debt ?? 0 - curOrder.PaidCNY;
                        if (curOrder.DebtFinalCNY < 0)
                        {
                            curOrder.DebtFinalCNY = 0;
                        }
                        curOrder.DeliveryFinalCNY = curOrder.DeliveryBeforeCNY + (debt ?? 0);
                        curOrder.ConfirmedFinalCNY = curOrder.ConfirmedBeforeCNY;
                    }
                    else if (i == orders.Count - 1)
                    {
                        var prevOrder = orders[i - 1];
                        curOrder.DebtBeforeCNY = prevOrder.DebtFinalCNY;
                        curOrder.DeliveryBeforeCNY = prevOrder.DebtFinalCNY;
                        curOrder.ConfirmedBeforeCNY = prevOrder.ConfirmedFinalCNY;

                        curOrder.PaidCNY = report.PaidInDateCNY;
                        curOrder.DeliveryFinalCNY = report.DeliveryFinalCNY;
                        curOrder.ConfirmedFinalCNY = report.ConfirmedFinalCNY;
                        curOrder.DebtFinalCNY = report.DebtFinalCNY;
                    }
                    else
                    {
                        var prevOrder = orders[i - 1];
                        curOrder.DebtBeforeCNY = prevOrder.DebtFinalCNY;
                        curOrder.DeliveryBeforeCNY = prevOrder.DeliveryFinalCNY;
                        curOrder.ConfirmedBeforeCNY = prevOrder.ConfirmedFinalCNY;
                        curOrder.PaidCNY = 0;

                        var curOrderLines = lines.Where(p => p.OrderId == curOrder.Id).ToList();
                        var debt = curOrderLines.Sum(p => p.RequestQuantity * p.RequestPrice);

                        curOrder.DebtFinalCNY = curOrder.DebtBeforeCNY + (debt ?? 0);
                        curOrder.DeliveryFinalCNY = curOrder.DeliveryBeforeCNY + (debt ?? 0);
                        curOrder.ConfirmedFinalCNY = curOrder.ConfirmedBeforeCNY;
                    }
                }
            }

            await _saleOrderRepository.UpdateManyAsync(orders);
        }

        public bool ValidatePhoneNumber(string phoneNumber)
        {
            string pattern = @"^(03[2-9]|05[6|8|9]|07[0|6|7|8|9]|08[1-9]|09[0|1|2|3|4|6|7|8|9])+([0-9]{7})$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(phoneNumber);
        }

        public bool ValidateEmail(string email)
        {
            string pattern = @"^[a-zA-Z0-9._%+-]+@[a-zA-Z0-9.-]+\.[a-zA-Z]{2,}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(email);
        }

        /// <summary>
        /// ^: Bắt đầu của chuỗi.
        /// (?=.*[a-z]): Positive lookahead đảm bảo rằng chuỗi phải chứa ít nhất một ký tự chữ thường.
        /// (?=.*[A-Z]): Positive lookahead đảm bảo rằng chuỗi phải chứa ít nhất một ký tự chữ hoa.
        /// (?=.*\d): Positive lookahead đảm bảo rằng chuỗi phải chứa ít nhất một ký tự số.
        /// (?=.*[^a-zA-Z\d]): Positive lookahead đảm bảo rằng chuỗi phải chứa ít nhất một ký tự không phải chữ cái và không phải số.
        /// .{6,}: Chuỗi phải có ít nhất 6 ký tự.
        /// $: Kết thúc của chuỗi.
        /// </summary>
        /// <param name="password"></param>
        /// <returns></returns>
        public bool ValidatePassword(string password)
        {
            string pattern = @"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^a-zA-Z\d]).{6,}$";
            Regex regex = new Regex(pattern);
            return regex.IsMatch(password);
        }

        public async Task ClearDataInTables(string _connectionString, Guid tenantId, string columnName)
        {
            List<string> tableNames = GetTableNames(_connectionString);

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                foreach (string tableName in tableNames)
                {
                    if (ColumnExists(connection, tableName, columnName) && ColumnExists(connection,tableName, "TenantId"))
                    {
                        string deleteQuery = $"Update {tableName} SET IsDeleted = 1 WHERE TenantId = '{tenantId}'";

                        using (SqlCommand command = new SqlCommand(deleteQuery, connection))
                        {
                            command.CommandType = CommandType.Text;
                            command.ExecuteNonQuery();
                        }
                    }
                }
            }
        }

        private bool ColumnExists(SqlConnection connection, string tableName, string columnName)
        {
            string query = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}' AND COLUMN_NAME = '{columnName}'";

            using (SqlCommand command = new SqlCommand(query, connection))
            {
                command.CommandType = CommandType.Text;
                using (SqlDataReader reader = command.ExecuteReader())
                {
                    return reader.HasRows;
                }
            }
        }

        private List<string> GetTableNames(string _connectionString)
        {
            List<string> tableNames = new List<string>();

            using (SqlConnection connection = new SqlConnection(_connectionString))
            {
                connection.Open();

                System.Data.DataTable schema = connection.GetSchema("Tables");

                foreach (DataRow row in schema.Rows)
                {
                    string tableName = row["TABLE_NAME"].ToString();
                    tableNames.Add(tableName);
                }
            }

            return tableNames;
        }

        public string GetTicketTypeName(TicketTypes type)
        {
            var result = "";
            switch (type)
            {
                case TicketTypes.Import:
                    result = "Phiếu nhập";
                    break;
                case TicketTypes.Receipt:
                    result = "Phiếu thu";
                    break ;
                case TicketTypes.CreditNote:
                    result = "Báo có";
                    break;
                case TicketTypes.Export:
                    result = "Phiếu xuất";
                    break;
                case TicketTypes.PaymentVoucher:
                    result = "Phiếu chi";
                    break;
                case TicketTypes.DebitNote:
                    result = "Báo nợ";
                    break;
                case TicketTypes.ClosingEntry:
                    result = "Kết chuyển";
                    break;
                case TicketTypes.Other:
                    result = "Khác";
                    break;
                case TicketTypes.Sales:
                    result = "Bán hàng";
                    break;
                case TicketTypes.Return:
                    result = "Trả hàng";
                    break;
                case TicketTypes.FundTransfer:
                    result = "Chuyển quỹ";
                    break;
                case TicketTypes.ChangeCostPriceProduct:
                    result = "Chuyển giá vốn";
                    break;
            }
            return result;
        }
    }
}