using System;

namespace VTECHERP.DTOs.Customer
{
    public class UpdateCustomerRequest:CreateCustomerRequest
    {
        public Guid? Id { get; set; }
    }
}
