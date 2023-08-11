using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.BillCustomers.Respons;

namespace VTECHERP.ServiceInterfaces
{
    public interface IBillCustomerReturnService : IScopedDependency
    {
        Task<(BillCustomerDto, string, bool)> AutoCreateCustomerBillForReturnProduct(Guid id);
        Task<(BillCustomerDto, string, bool)> AutoDeleteCustomerBillForReturnProduct(Guid id);
    }
}
