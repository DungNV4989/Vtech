using DocumentFormat.OpenXml.Math;
using DocumentFormat.OpenXml.Office2010.Excel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Application.Services;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.Identity;
using VTECHERP.DTOs.BillCustomers.Params;
using VTECHERP.DTOs.BillCustomers.Respons;
using VTECHERP.Entities;
using VTECHERP.ServiceInterfaces;

namespace VTECHERP.Applications.BillCustomerLogs
{
    [Authorize]
    public class BillCustomerLogApplication : ApplicationService
    {
        private readonly IRepository<BillCustomerLog> _billCustomerLogRepository;
        private readonly IRepository<BillCustomer> _billCustomerRepository;
        private readonly IRepository<Customer> _customerRepository;
        private readonly IRepository<Provinces> _provinceRepository;
        private readonly IRepository<Employee> _employeeRepository;
        private readonly IRepository<Products> _productRepository;
        private readonly IdentityUserManager _userManager;
        private readonly IBillCustomerService _billCustomerService;
        public BillCustomerLogApplication(
            IRepository<BillCustomerLog> billCustomerLogRepository
            , IdentityUserManager userManager
            , IRepository<BillCustomer> billCustomerRepository
            , IRepository<Customer> customerRepository
            , IRepository<Provinces> provinceRepository
            , IRepository<Employee> employeeRepository
            , IRepository<Products> productRepository
            , IBillCustomerService billCustomerService
            )
        {
            _billCustomerLogRepository = billCustomerLogRepository;
            _userManager = userManager;
            _billCustomerRepository = billCustomerRepository;
            _customerRepository = customerRepository;
            _provinceRepository = provinceRepository;
            _employeeRepository = employeeRepository;
            _productRepository = productRepository;
            _billCustomerService = billCustomerService;
        }

        [HttpGet]
        public async Task<IActionResult> GetBillCustomerLogs(Guid BillCustomerId)
        {
            var billLogs = await _billCustomerLogRepository.GetListAsync(x => x.BillCustomerId == BillCustomerId);

            var results = billLogs.Select(x => new BillCustomerLogItem
            {
                Id = x.Id,
                CreateTime = x.CreationTime,
                CreatorId = x.CreatorId,
            })
                .ToList();

            foreach (var item in results)
            {
                if (item.CreatorId.HasValue)
                {
                    var user = await _userManager.FindByIdAsync(item.CreatorId.ToString());
                    item.CreatorText = user == null ? "" : user.Name;
                }
            }

            return new OkObjectResult(new { count = results.Count, data = results });
        }

        [HttpGet]
        public async Task<IActionResult> GetDetail(Guid BillCustomerLogId)
        {
            var log = await _billCustomerLogRepository.FindAsync(x => x.Id == BillCustomerLogId);
            if (log == null)
            {
                return new BadRequestObjectResult(new
                {
                    message = $"Không tìm thấy log có id là {BillCustomerLogId}"
                });
            }

            var billCustomer = await _billCustomerRepository.FindAsync(x => x.Id == log.BillCustomerId);
            if (billCustomer == null)
            {
                return new BadRequestObjectResult(new
                {
                    message = $"Không tìm thấy hóa đơn có id là {log.BillCustomerId}"
                });
            }

            var result = new BillCustomerLogDetail()
            {
                BillCustomerCode= billCustomer.Code,
                CreatorId= log.CreatorId,
                CreatTime= log.CreationTime,
                ToValue = JsonConvert.DeserializeObject<BillCustomerLogJson>(log.ToValue),  
                FromValue= JsonConvert.DeserializeObject<BillCustomerLogJson>(log.FromValue),
            };

            if (result.CreatorId.HasValue)
            {
                var user = await _userManager.FindByIdAsync(result.CreatorId.ToString());
                result.CreatorText = user == null ? "" : user.Name;
            }

            // Lấy thông tin khách hàng
            await GetInfoCustomer(result.FromValue);
            await GetInfoCustomer(result.ToValue);

            result.FromValue.BillCustomerStatusText = _billCustomerService.MapBillCustomerStatus(result.FromValue.BillCustomerStatus);
            result.ToValue.BillCustomerStatusText = _billCustomerService.MapBillCustomerStatus(result.ToValue.BillCustomerStatus);

            result.FromValue.TransportFormText = _billCustomerService.MapBillCustomerTransportForm(result.FromValue.TransportForm);
            result.ToValue.TransportFormText = _billCustomerService.MapBillCustomerTransportForm(result.ToValue.TransportForm);

            Type type = typeof(BillCustomerLogJson);
            foreach (PropertyInfo property in type.GetProperties())
            {
                object value1 = property.GetValue(result.FromValue);
                object value2 = property.GetValue(result.ToValue);

                if (Equals(value1, value2))
                {
                    property.SetValue(result.FromValue, null);
                    property.SetValue(result.ToValue, null);
                }
            }

            var listFromValue = new List<BillCustomerProductItem>();    
            var listToValue = new List<BillCustomerProductItem>();   
            listToValue = result.ToValue.Products.Where(p => p.Id == null || p.Id == Guid.Empty || !listFromValue.Select(x => x.Id).Contains(p.Id)).ToList();

            foreach (var item in result.FromValue.Products)
            {
                var itemInToValue = result.ToValue.Products.FirstOrDefault(x => x.Id == item.Id);
                // Case remove
                if (itemInToValue == null)
                    listFromValue.Add(item);

                // Case update
                if (itemInToValue != null &&
                    (item.ProductId != itemInToValue.ProductId
                    || item.Quantity != itemInToValue.Quantity
                    || item.Price != itemInToValue.Price
                    || item.DiscountUnit!= itemInToValue.DiscountUnit
                    || item.DiscountValue!= itemInToValue.DiscountValue)
                    )
                {
                    listFromValue.Add(item);
                    listToValue.Add(itemInToValue);
                }
            }

            var productOrigin = await _productRepository.GetListAsync(x => listToValue.Select(x => x.ProductId).Contains(x.Id)
                                                                        || listFromValue.Select(x => x.ProductId).Contains(x.Id));


            foreach (var item in listFromValue)
            {
                var product = productOrigin.FirstOrDefault(x => x.Id == item.ProductId);
                item.ProductName = product == null ? "" : product.Name;
            }

            foreach (var item in listToValue)
            {
                var product = productOrigin.FirstOrDefault(x => x.Id == item.ProductId);
                item.ProductName = product == null ? "" : product.Name;
            }

            result.FromValue.Products = listFromValue;
            result.ToValue.Products = listToValue;

            return new OkObjectResult(result);
        }

        private async Task GetInfoCustomer(BillCustomerLogJson param)
        {
            if (param.CustomerId.HasValue)
            {
                var customer = await _customerRepository.FindAsync(x => x.Id == param.CustomerId);
                if (customer != null)
                {
                    param.CustomerName = customer.Name;
                    param.CustomerAddress = customer.Address;
                    param.CustomerNote = customer.Note;

                    if (customer.ProvinceId.HasValue)
                    {
                        var province = await _provinceRepository.FindAsync(x => x.Id == customer.ProvinceId);
                        param.ProvinceName = province == null ? "" : province.Name;
                    }

                    if (customer.HandlerEmployeeId.HasValue)
                    {
                        var employee = await _employeeRepository.FindAsync(x => x.Id == customer.HandlerEmployeeId);
                        param.ProvinceName = employee == null ? "" : employee.Name;
                    }

                    if (customer.SupportEmployeeId.HasValue)
                    {
                        var employee = await _employeeRepository.FindAsync(x => x.Id == customer.SupportEmployeeId);
                        param.ProvinceName = employee == null ? "" : employee.Name;
                    }
                }
            }
        }
    }
}
