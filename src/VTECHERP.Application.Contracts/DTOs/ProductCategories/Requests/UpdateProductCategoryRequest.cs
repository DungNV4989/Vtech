using System;
using VTECHERP.Enums;

namespace VTECHERP.DTOs.ProductCategories.Requests
{
    public class UpdateProductCategoryRequest : CreateProductCategoryRequest
    {
        public Guid? Id { get; set; }
    }
}
