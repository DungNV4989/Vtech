using ClosedXML.Excel;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Vinpearl.Modelling.Library.Utility.Excel;
using Volo.Abp.DependencyInjection;
using VTECHERP.Domain.Shared.Helper.Excel.Model;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.PriceTableProduct.Param;
using VTECHERP.DTOs.PriceTableProduct.Respon;
using VTECHERP.DTOs.Suppliers;
using VTECHERP.DTOs.Suppliers.Params;
using VTECHERP.Enums;

namespace VTECHERP.Services
{
    public interface ISupplierService : IScopedDependency
    {
        Task<List<MasterDataDTO>> GetIdCodeNameAsync();

        Task<SupplierDetailDto> GetByIdAsync(Guid id);

        Task<List<MasterDataDTO>> GetChineseSupplierAsync();

        Task<bool> Exist(Guid id);

        Task<(bool success, string message, Guid? data)> Create(CreateSupplierParam param);
        string GetSquenceSupplier();

        Task<(bool success, string message)> Update(Guid Id ,CreateSupplierParam param);
        string GetSupplierTypeText(SupplierOrigin param);

        Task<byte[]> DownloadTemplateImport();
        Task<byte[]> ImportPriceTableProductData(IFormFile file);
    }
}