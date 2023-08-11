using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.PriceTableProduct.Param;
using VTECHERP.DTOs.PriceTableProduct.Respon;

namespace VTECHERP.ServiceInterfaces
{
    public interface IPriceTableService: IScopedDependency
    {
        /// <summary>
        /// Chi tiết bảng giá
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<PriceTableDetail> GetPriceTable(Guid id);
        /// <summary>
        /// tạo bảng giá
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task CreatePriceTable(CreatePriceTableRequest request);
        /// <summary>
        /// Cập nhật thông tin bảng giá
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task UpdatePriceTable(UpdatePriceTableRequest request);
        /// <summary>
        /// DS Bảng giá (màn hình quản lý)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<PagingResponse<PriceTableDetail>> SearchPriceTable(SearchPriceTableRequest request);
       
        /// <summary>
        /// DS tất cả bảng giá (mã + tên)
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<List<MasterDataDTO>> ListAllPriceTable(ListAllPriceTableRequest request);
        /// <summary>
        /// DS tất cả sản phẩm kèm với bảng giá
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<SearchProductPriceResponse> SearchProductPrice(SearchPriceProductRequest request);
        /// <summary>
        /// DS sản phẩm khi lấy chi tiết bảng giá
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<SearchProductPriceResponse> SearchProductPriceByTableId(SearchPriceProductByTableIdRequest request);
        /// <summary>
        /// Lọc các sản phẩm chưa được add vào bảng giá
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task<List<MasterDataDTO>> SearchProductNotInPriceTable(SearchPriceProductNotInPriceTableRequest request);
        /// <summary>
        /// Cập nhật SP trong bảng giá
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task UpdateProductPrice(UpdatePriceProductRequest request);
        /// <summary>
        /// Xóa SP trong bảng giá
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task DeleteProductPrice(DeletePriceProductRequest request);
        /// <summary>
        /// Thêm mới SP vào bảng giá
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task AddProductPrice(AddPriceProductRequest request);
        /// <summary>
        /// Xóa sản phẩm trên tất cả bảng giá
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        Task DeleteMultipleProductPrice(DeleteMultiplePriceProductRequest request);

        Task<byte[]> ExportProductPrice(SearchPriceProductRequest request);
    }
}
