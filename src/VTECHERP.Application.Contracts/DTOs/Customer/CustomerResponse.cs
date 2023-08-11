using System;
using System.Collections.Generic;

namespace VTECHERP.DTOs.Customer
{
    public class CustomerResponse
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string PhoneNumber { get; set; }
        public Guid? HandlerEmployeeId { get; set; }
        public string HandlerEmployeeName { get; set; }
        public Guid? HandlerStoreIds { get; set; }
        public string HandlerStoreNames { get; set; }
        public Guid? CreatorId { get; set; }
        public string CreateName { get; set; }
    }
}