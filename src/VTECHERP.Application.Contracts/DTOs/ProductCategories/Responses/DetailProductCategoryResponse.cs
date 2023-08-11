using VTECHERP.DTOs.ProductCategories.Requests;

namespace VTECHERP.DTOs.ProductCategories.Responses
{
    public class DetailProductCategoryResponse : UpdateProductCategoryRequest
    {
        public string ParentName { get; set; }
    }
}