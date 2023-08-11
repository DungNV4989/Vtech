using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp.Domain.Repositories;
using VTECHERP.DTOs.BillCustomerVoucher.Dto;
using VTECHERP.DTOs.BillCustomerVoucher.Param;
using VTECHERP.Entities;
using VTECHERP.Enums.Bills;
using VTECHERP.Services.Interface;

namespace VTECHERP.Services
{
    public class BillCustomerVoucherService : IBillCustomerVoucherService
    {
        private readonly IRepository<Voucher> _voucherRepository;
        private readonly IRepository<Promotion> _promtionRepository;
        private readonly IRepository<Products> _productRepository;
        public BillCustomerVoucherService(
             IRepository<Voucher> voucherRepository
            , IRepository<Promotion> promtionRepository
            , IRepository<Products> productRepository
            )
        {
            _voucherRepository = voucherRepository;
            _promtionRepository = promtionRepository;
            _productRepository = productRepository; 
        }
        public Voucher GetVoucherByCode(string VoucherCode)
                   => _voucherRepository.FirstOrDefaultAsync(x => x.Code == VoucherCode).Result;

        public Promotion GetPromotionById(Guid PromotionId)
                   => _promtionRepository.FirstOrDefaultAsync(x => x.Id == PromotionId && x.Status == PromotionStatus.Active).Result;

        public bool CheckExpireVoucherByPromotion(Promotion Promotion)
                   => Promotion.FromDate.Date <= DateTime.Now.Date && Promotion.ToDate.Date >= DateTime.Now.Date;

        public bool CheckPromotionAmount(Promotion Promotion, decimal Amount)
        {
            if (!Promotion.BillMinValue.HasValue && !Promotion.BillMaxValue.HasValue)
                return true;

            if (!Promotion.BillMinValue.HasValue && Promotion.BillMinValue > Amount)
                return false;

            if (!Promotion.BillMaxValue.HasValue && Promotion.BillMaxValue < Amount)
                return false;

            return Promotion.BillMinValue <= Amount && Promotion.BillMaxValue >= Amount;
        }
                   

        /// <summary>
        /// Check cửa hàng có nằm trong cửa hàng áp dụng của chương trình không
        /// </summary>
        /// <param name="Promotion"></param>
        /// <param name="StoreId"></param>
        /// <returns></returns>
        public bool CheckStorePromotion(Promotion Promotion, Guid StoreId)
        {
            var storeId = JsonConvert.DeserializeObject<List<Guid>>(Promotion.ApplyStoreIds);
            if (storeId == null || !storeId.Any()) return true;

            return storeId.Contains(StoreId);
        }

        /// <summary>
        /// Trả về danh sách id của sản phẩm mà thuộc danh sách sản phẩm của chương trình
        /// </summary>
        /// <param name="Promotion"></param>
        /// <param name="ProductIds"></param>
        /// <returns></returns>
        public List<Guid> ProductsApplyVoucher(Promotion Promotion, List<Guid> ProductIds)
        {
            if (string.IsNullOrEmpty(Promotion.ApplyProductIds))
                return ProductIds;

            var products = JsonConvert.DeserializeObject<List<Guid>>(Promotion.ApplyProductIds);
            if (products == null || !products.Any()) return new List<Guid>();

            return ProductIds.Where(x => products.Contains(x)).ToList();
        }

        /// <summary>
        /// Trả về danh sách id của sản phẩm mà thuộc loại sản phẩm trong chương trình
        /// </summary>
        /// <param name="Promotion"></param>
        /// <param name="ProductIds"></param>
        /// <returns></returns>
        public List<Guid> ProductsApplyVoucherByCategory(Promotion Promotion, List<Guid> ProductIds)
        {
            if (string.IsNullOrEmpty(Promotion.ApplyProductCategoryIds))
                return ProductIds;

            var productCategories = JsonConvert.DeserializeObject<List<Guid>>(Promotion.ApplyProductCategoryIds);
            if (productCategories == null || productCategories.Any()) return new List<Guid>();

            var products = _productRepository.GetListAsync(x => ProductIds.Contains(x.Id)).Result.Select(x => new {
                CategoryId = x.CategoryId,
                ProductId = x.Id
            }).ToList();

            return products.Where(x => productCategories.Contains(x.CategoryId)).Select(x => x.ProductId).ToList();
        }

        public (BillVoucher, string, bool) GetVoucher(GetVoucherParam param)
        {
            var voucher = GetVoucherByCode(param.Code);
            if (voucher == null)
                return (null, $"Không tìm thấy voucher có mã là {param.Code}", false);

            if(voucher.Status == VoucherStatus.Used)
                return (null, $"Voucher đã được sử dụng", false);

            if (voucher.Status == VoucherStatus.Cancel)
                return (null, $"Voucher đã bị hủy", false);

            var promotion = GetPromotionById(voucher.PromotionId);
            if (promotion == null)
                return (null, $"Không tìm thấy chương trình của voucher", false);

            var checkExpire = CheckExpireVoucherByPromotion(promotion);
            if (!checkExpire)
                return (null, $"Thời gian hiệu lực chương trình không hợp lệ", false);

            var checkAmount = CheckPromotionAmount(promotion, param.BillAmount);
            if (!checkAmount)
                return (null, $"Số tiền hóa đơn không hợp lệ", false);

            var checkStore = CheckStorePromotion(promotion, param.StoreId.GetValueOrDefault());
            if (!checkStore)
                return (null, $"Voucher không được áp dụng cho cửa hàng", false);

            List<Guid> checkProducts = null;
            if (promotion.ApplyFor == ApplyFor.Product)
            {
                 checkProducts = ProductsApplyVoucher(promotion, param.ProductIds);
                if (checkProducts == null || !checkProducts.Any())
                    return (null, $"Voucher không được áp dụng cho danh sách sản phẩm", false);
            }

            if (promotion.ApplyFor == ApplyFor.ProductCategory)
            {
                 checkProducts = ProductsApplyVoucherByCategory(promotion, param.ProductIds);
                if (checkProducts == null || !checkProducts.Any())
                    return (null, $"Voucher không được áp dụng cho danh sách sản phẩm", false);
            }

            var result = new BillVoucher()
            {
                Code = voucher.Code,
                Id = voucher.Id,
                MaxDiscountValue = promotion.MaxDiscountValue,
                BillMaxValue= promotion.BillMaxValue,
                BillMinValue= promotion.BillMinValue,
                DiscountUnit = promotion.DiscountUnit,
                DiscountValue = promotion.DiscountValue,    
                NotApplyWithDiscount = promotion.NotApplyWithDiscount,
                ApplyFor = promotion.ApplyFor,
                PromotionId = promotion.Id,
                Products = checkProducts
            };

            return (result, "", true);
        }

        public List<Guid> ProductsApplyVoucher(string ProductVoucher, List<Guid> ProductIds)
        {
            var products = JsonConvert.DeserializeObject<List<Guid>>(ProductVoucher);
            if (products == null || !products.Any()) return new List<Guid>();

            return ProductIds.Where(x => products.Contains(x)).ToList();
        }

        public List<Guid> ProductsApplyVoucherByCategory(string CategoryProductVoucher, List<Guid> ProductIds)
        {
            var productCategories = JsonConvert.DeserializeObject<List<Guid>>(CategoryProductVoucher);
            if (productCategories == null || productCategories.Any()) return new List<Guid>();

            var products = _productRepository.GetListAsync(x => ProductIds.Contains(x.Id)).Result.Select(x => new {
                CategoryId = x.CategoryId,
                ProductId = x.Id
            }).ToList();

            return products.Where(x => productCategories.Contains(x.CategoryId)).Select(x => x.ProductId).ToList();
        }
    }
}
