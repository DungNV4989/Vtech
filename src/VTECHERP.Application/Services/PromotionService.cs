using DocumentFormat.OpenXml.Office2010.ExcelAc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Volo.Abp;
using Volo.Abp.Domain.Repositories;
using Volo.Abp.ObjectMapping;
using Volo.Abp.Timing;
using Volo.Abp.Uow;
using Volo.Abp.Users;
using VTECHERP.DTOs.Base;
using VTECHERP.DTOs.Promotions;
using VTECHERP.Entities;
using VTECHERP.Enums.Bills;
using VTECHERP.ServiceInterfaces;

namespace VTECHERP.Services
{
    public class PromotionService: IPromotionService
    {
        private readonly IRepository<Promotion> _promotionRepository;
        private readonly IRepository<Voucher> _voucherRepository;
        private readonly IObjectMapper _objectMapper;
        private readonly IUnitOfWorkManager _uowManager;
        private readonly ICurrentUser _currentUser;
        private readonly IClock _clock;
        private readonly IRepository<UserStore> _userStoreRepository;
        private readonly IRepository<Stores> _storeRepository;
        private readonly IObjectMapper _mapper;
        private readonly IRepository<Products> _productRepository;
        IRepository<ProductCategories> _productCategoriesRepository;

        public PromotionService(
            IRepository<Promotion> promotionRepository,
            IRepository<Voucher> voucherRepository,
            IObjectMapper objectMapper,
            IUnitOfWorkManager uowManager,
            ICurrentUser currentUser,
            IClock clock,
            IRepository<UserStore> userStoreRepository,
            IRepository<Stores> storeRepository,
            IObjectMapper mapper,
            IRepository<Products> productRepository,
            IRepository<ProductCategories> productCategoriesRepository
            )
        {
            _promotionRepository = promotionRepository;
            _voucherRepository = voucherRepository;
            _objectMapper = objectMapper;
            _uowManager = uowManager;
            _currentUser = currentUser;
            _clock = clock;
            _userStoreRepository = userStoreRepository;
            _mapper = mapper;
            _productRepository= productRepository;
            _productCategoriesRepository =productCategoriesRepository;
            _storeRepository= storeRepository;
        }

        public async Task<PagingResponse<PromotionDTO>> SearchPromotin(SearchPromotionRequest request)
        {
            if (request != null && request.FromDate != null)
            {
                request.FromDate = _clock.Normalize(request.FromDate.Value);
            }
            if (request != null && request.ToDate != null)
            {
                request.ToDate = _clock.Normalize(request.ToDate.Value);
            }
            var x = _currentUser.Id;
            var userStores = (await _userStoreRepository.GetQueryableAsync()).Where(x => x.UserId == _currentUser.Id).ToList();
            var storeIds = userStores.Select(x => x.StoreId.ToString()).ToList();
            var promotion = await _promotionRepository.GetListAsync();
            var join = from pro in promotion
                        where storeIds.Any(id => pro.ApplyStoreIds.Contains(id.ToString()))
                        select pro;
            var listpromotion = join.ToList();
            var list = _objectMapper.Map<List<Promotion>, List<PromotionDTO>>(listpromotion);
            //lọc
            var respon = list.WhereIf(!string.IsNullOrEmpty(request.Code), x => (string.IsNullOrEmpty(x.Code) ? false : x.Code.Contains(request.Code)))
                             .WhereIf(!string.IsNullOrEmpty(request.Name), x=> (string.IsNullOrEmpty(x.Name) ? false : x.Name.Contains(request.Name)))
                             .WhereIf(request.FromDate != null, x => x.FromDate.Date >= request.FromDate.Value.Date)
                             .WhereIf(request.ToDate != null, x => x.ToDate.Date <= request.ToDate.Value.Date)
                             .WhereIf(request.DiscountUnit != null, x => x.DiscountUnit==request.DiscountUnit)
                             .WhereIf(request.Status != null, x => x.Status==request.Status)
                .ToList();   
            // lấy ra số lượng đã sử dụng
            if(respon != null && respon.Count>0)
            {
                foreach (var item in respon)
                {
                    var countVoucherUsed = (await _voucherRepository.GetListAsync(x => x.PromotionId == item.Id && x.Status == VoucherStatus.Used))?.Count()??0;
                    item.VoucherUsed=countVoucherUsed;
                }
            }
            var resultPaged = respon.OrderByDescending(p => p.Code).Skip(request.Offset).Take(request.PageSize).ToList();

            return new PagingResponse<PromotionDTO>(respon.Count(), resultPaged);
        }
        public async Task<PagingResponse<VoucherDTO>> SearchVoucherByPromotinId(Guid promotionId,SearchVoucherRequest request)
        {
            
            var voucher = await _voucherRepository.GetListAsync(x=>x.PromotionId==promotionId);
            if (voucher != null && voucher.Count>0)
            {
                var list = _objectMapper.Map<List<Voucher>, List<VoucherDTO>>(voucher);
                var respon = list
                    .WhereIf(!(String.IsNullOrEmpty(request.Code)), x => x.Code == request.Code)
                    .WhereIf(request.Status != null, x => x.Status == request.Status)
                    .ToList();
                var resultPaged = respon.OrderByDescending(p => p.Code).Skip(request.Offset).Take(request.PageSize).ToList();

                return new PagingResponse<VoucherDTO>(list.Count(), resultPaged);
            }    
            else
                return new PagingResponse<VoucherDTO>(0, new List<VoucherDTO>());
        }
        public async Task<PagingResponse<ProductInPromotionDTO>> SearchProductByPromotinId(Guid promotionId, SearchProductInPromotionRequest request)
        {
            var result = new List<ProductInPromotionDTO>();
            var promotion = await _promotionRepository.FindAsync(x => x.Id == promotionId);
            if(promotion != null)
            {
                var listProductId = promotion.ApplyProductIds;
                var listId = JsonConvert.DeserializeObject<List<Guid>>(listProductId);
                if(listId!=null && listId.Count > 0)
                {
                    var listproduct = await _productRepository.GetQueryableAsync();
                    var listCategory = await _productCategoriesRepository.GetQueryableAsync();

                    result = (from pro in listproduct
                             join cate in listCategory
                             on pro.CategoryId equals cate.Id
                             where listId.Contains(pro.Id)
                             select new ProductInPromotionDTO()
                             {
                                 Id = pro.Id,
                                 SequenceId = pro.SequenceId,
                                 Name = pro.Name,
                                 CategoryName = cate.Name,
                            }).ToList();
                    result = result
                        .WhereIf(!(String.IsNullOrEmpty(request.SequenceId)), x => x.SequenceId == request.SequenceId)
                        .WhereIf(!(String.IsNullOrEmpty(request.Name)), x => x.Name.Contains(request.Name))
                        .ToList();
                }
                var resultPaged = result.OrderByDescending(p => p.SequenceId).Skip(request.Offset).Take(request.PageSize).ToList();
                
                return new PagingResponse<ProductInPromotionDTO>(result.Count(), resultPaged);
            }
            else
                return new PagingResponse<ProductInPromotionDTO>(0, new List<ProductInPromotionDTO>());

        }
        public async Task<DetailPromotionDTO> GetPromotionById(Guid Id)
        {
            try
            {
                var promotion = await _promotionRepository.FindAsync(x => x.Id == Id);
                var result = _mapper.Map<Promotion, DetailPromotionDTO>(promotion);
                return result;
            }
            catch(Exception ex)
            {
                var x = ex;
                return null;
            }
        }
        public async Task CreatePromotion(CreatePromotionRequest request)
        {
            await ValidateCreatePromotion(request);
            using var uow = _uowManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                var promotion = _objectMapper.Map<CreatePromotionRequest, Promotion>(request);
                promotion.CreatorName = _currentUser.Name;
                if(request.ApplyStoreIds==null|| request.ApplyStoreIds.Count()==0)
                {
                    var listIdStore = (await _storeRepository.GetListAsync()).Select(x => x.Id).ToList();
                    if(listIdStore.Any())
                    {
                        var listIdStoreString = JsonConvert.SerializeObject(listIdStore);
                        promotion.ApplyStoreIds = listIdStoreString;
                    }     
                }    
                await _promotionRepository.InsertAsync(promotion);
                await uow.SaveChangesAsync();

                if (!request.NotApplyWithDiscount)
                {
                    //string format = string.Empty.PadLeft(request.GenCodeNum, '0');
                    var vouchers = new List<Voucher>();
                    for(int count = 1; count <= request.VoucherNum; count++)
                    {
                        var code = $"{request.Prefix}{GetRandomString(request.GenCodeNum)}{request.Suffix}";
                        // sinh mã random đến khi không trùng với mã đã sinh
                        while (vouchers.Any(p => p.Code == code))
                        {
                            code = $"{request.Prefix}{GetRandomString(request.GenCodeNum)}{request.Suffix}";
                        }
                        var voucher = new Voucher
                        {
                            PromotionId = promotion.Id,
                            Code = code,
                            BillMaxValue = promotion.BillMaxValue,
                            BillMinValue = promotion.BillMinValue,
                            DiscountUnit = promotion.DiscountUnit,
                            DiscountValue = promotion.DiscountValue,
                            MaxDiscountUnit = promotion.MaxDiscountUnit,
                            MaxDiscountValue = promotion.MaxDiscountValue,
                            Status = VoucherStatus.New
                        };

                        vouchers.Add(voucher);
                    }

                    if (vouchers.Any())
                    {
                        await _voucherRepository.InsertManyAsync(vouchers);
                        await uow.SaveChangesAsync();
                    }
                }

                await uow.CompleteAsync();
            }
            catch
            {
                await uow.RollbackAsync();
                throw;
            }
        }
        public async Task<bool> DeletePromotion(Guid id)
        {
            var promotion = await _promotionRepository.FindAsync(p => p.Id == id);
            bool check=true;
            var voucher= await _voucherRepository.GetListAsync(x=>x.PromotionId==id&&x.Status==VoucherStatus.Used);
            if(voucher!=null && voucher.Count>0)
            {
                
                check = false;
            }    
            else
            {
                await _promotionRepository.DeleteAsync(promotion);
            }    
            return check;
        }
        private async Task ValidateCreatePromotion(CreatePromotionRequest request)
        {
            // Check promotion date
            if(request.FromDate > request.ToDate)
            {
                throw new BusinessException("Ngày bắt đầu không thể lớn hơn ngày kết thúc");
            }
            if (DateTime.Now.Date > request.ToDate)
            {
                throw new BusinessException("Không thể tạo chương trình khuyến mãi tại thời điểm quá khứ");
            }
            // check rule tạo voucher 
            // số ký tự không đủ cho tổng số voucher
            var maxCodeNum = int.Parse(string.Empty.PadLeft(request.GenCodeNum, '9'));
            if(maxCodeNum < request.VoucherNum)
            {
                throw new BusinessException($"Không thể tạo {request.VoucherNum} voucher với {request.GenCodeNum} ký tự sinh (mã tối đa {maxCodeNum})");
            }
            await Task.CompletedTask;
            //// voucher có thể bị sinh trùng mã
            //var ruleExist = await _promotionRepository.FirstOrDefaultAsync(p =>
            //    !p.NotApplyWithDiscount
            //    && p.Prefix == request.Prefix
            //    && p.Suffix == request.Suffix
            //    && p.GenCodeNum == request.GenCodeNum
            //    && p.Status == PromotionStatus.Active);

            //if(ruleExist != null)
            //{
            //    throw new BusinessException($"Quy tắc sinh voucher trùng với chương trình KM đang hoạt động: {ruleExist.Name}");
            //}
        }

        public async Task UpdatePromotion(UpdatePromotionRequest request)
        {
            await ValidateUpdatePromotion(request);
            using var uow = _uowManager.Begin(requiresNew: true, isTransactional: true);
            try
            {
                var promotion = await _promotionRepository.GetAsync(p => p.Id == request.Id);

                promotion.Name = request.Name;
                promotion.BillMinValue = request.BillMinValue;
                promotion.BillMaxValue = request.BillMaxValue;
                promotion.ApplyStoreIds = JsonConvert.SerializeObject(request.ApplyStoreIds);
                promotion.ApplyProductIds = JsonConvert.SerializeObject(request.ApplyProductIds);
                promotion.ApplyProductCategoryIds = JsonConvert.SerializeObject(request.ApplyProductCategoryIds);
                promotion.FromDate = request.FromDate;
                promotion.ToDate = request.ToDate;
                promotion.Status = request.Status;
                promotion.Note = request.Note;
                promotion.LastModifierName = _currentUser.Name;

                await _promotionRepository.UpdateAsync(promotion);
                await uow.SaveChangesAsync();
                // cập nhật lại thông tin voucher chưa dùng
                var unusedPromotionVouchers = await _voucherRepository.GetListAsync(
                    p => p.PromotionId == promotion.Id && p.Status == VoucherStatus.New);

                if (unusedPromotionVouchers.Any())
                {
                    unusedPromotionVouchers.ForEach(p =>
                    {
                        p.BillMaxValue = promotion.BillMaxValue;
                        p.BillMinValue = promotion.BillMinValue;
                        if(promotion.Status == PromotionStatus.Inactive)
                        {
                            p.Status = VoucherStatus.Cancel;
                        }
                    });

                    await _voucherRepository.UpdateManyAsync(unusedPromotionVouchers);
                }

                await uow.CompleteAsync();
            }
            catch
            {
                await uow.RollbackAsync();
                throw;
            }
        }

        private async Task ValidateUpdatePromotion(UpdatePromotionRequest request)
        {
            // Check promotion date
            if (request.FromDate > request.ToDate)
            {
                throw new BusinessException("Ngày bắt đầu không thể lớn hơn ngày kết thúc");
            }
            if (DateTime.Now.Date > request.ToDate)
            {
                throw new BusinessException("Không thể cập chương trình khuyến mãi về thời điểm quá khứ");
            }
            await Task.CompletedTask;
        }

        private string GetRandomString(int stringLength)
        {
            StringBuilder sb = new StringBuilder();
            int numGuidsToConcat = (((stringLength - 1) / 32) + 1);
            for (int i = 1; i <= numGuidsToConcat; i++)
            {
                sb.Append(Guid.NewGuid().ToString("N"));
            }

            return sb.ToString(0, stringLength).ToUpper();
        }
    }
}
