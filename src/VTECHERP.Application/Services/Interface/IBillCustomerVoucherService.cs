using System;
using System.Collections.Generic;
using System.Text;
using Volo.Abp.DependencyInjection;
using VTECHERP.DTOs.BillCustomerVoucher.Dto;
using VTECHERP.DTOs.BillCustomerVoucher.Param;
using VTECHERP.Entities;

namespace VTECHERP.Services.Interface
{
    public interface IBillCustomerVoucherService : IScopedDependency
    {
        Voucher GetVoucherByCode(string VoucherCode);
        Promotion GetPromotionById(Guid PromotionId);
        (BillVoucher, string, bool) GetVoucher(GetVoucherParam param);
        bool CheckExpireVoucherByPromotion(Promotion Promotion);
        bool CheckPromotionAmount(Promotion Promotion, decimal Amount);
        bool CheckStorePromotion(Promotion Promotion, Guid StoreId);
        List<Guid> ProductsApplyVoucher(Promotion Promotion, List<Guid> ProductIds);
        List<Guid> ProductsApplyVoucherByCategory(Promotion Promotion, List<Guid> ProductIds);
        List<Guid> ProductsApplyVoucher(string ProductVoucher, List<Guid> ProductIds);
        List<Guid> ProductsApplyVoucherByCategory(string CategoryProductVoucher, List<Guid> ProductIds);
    }
}
