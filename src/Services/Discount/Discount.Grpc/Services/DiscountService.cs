using AutoMapper;
using Discount.Grpc.Entities;
using Discount.Grpc.Protos;
using Discount.Grpc.Repositories;
using Grpc.Core;
using Microsoft.Extensions.Logging;
using System;
using System.Threading.Tasks;

namespace Discount.Grpc
{
    public class DiscountService : DiscountProtoService.DiscountProtoServiceBase
    {
        private readonly IDiscountRepository _discountRepository;
        private readonly IMapper _mapper;
        private readonly ILogger<DiscountService> _logger;
        
        public DiscountService(IDiscountRepository discountRepository, IMapper mapper, ILogger<DiscountService> logger)
        {
            _discountRepository = discountRepository;
            _mapper = mapper;
            _logger = logger;
        }

        public override async Task<CouponModel> GetDiscount(GetDiscountRequest request, ServerCallContext context)
        {
            Coupon coupon = await _discountRepository.GetDiscount(request.ProductName);
            CouponModel couponModel = GetCouponModel(coupon, request.ProductName);

            _logger.LogInformation("Discount is retrieved for ProductName : {productName}, Amount : {amount}", coupon.ProductName, coupon.Amount);

            return couponModel;
        }

        public override async Task<CouponModel> CreateDiscount(CreateDiscountRequest request, ServerCallContext context)
        {
            Coupon coupon = GetCoupon(request.Coupon);
            await _discountRepository.CreateDiscount(coupon);
            CouponModel couponModel = GetCouponModel(coupon);

            _logger.LogInformation("Discount is successfully created. ProductName : {productName}", coupon.ProductName);

            return couponModel;
        }

        public override async Task<CouponModel> UpdateDiscount(UpdateDiscountRequest request, ServerCallContext context)
        {
            Coupon coupon = GetCoupon(request.Coupon);
            await _discountRepository.CreateDiscount(coupon);
            CouponModel couponModel = GetCouponModel(coupon);

            _logger.LogInformation("Discount is successfully updated. ProductName : {productName}", coupon.ProductName);

            return couponModel;
        }

        public override async Task<DeleteDiscountResponse> DeleteDiscount(DeleteDiscountRequest request, ServerCallContext context)
        {
            bool deleted = await _discountRepository.DeleteDiscount(request.ProductName);

            return new DeleteDiscountResponse{ Success = deleted };
        }

        private Coupon GetCoupon(string coupon)
        {
            if (coupon is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount not found."));
            }

            return _mapper.Map<Coupon>(coupon);
        }

        private CouponModel GetCouponModel(Coupon coupon, string productName = null)
        {
            if (coupon is null)
            {
                throw new RpcException(new Status(StatusCode.NotFound, $"Discount with ProductName={productName ?? "unknown"} is not found."));
            }

            return _mapper.Map<CouponModel>(coupon);
        }
    }
}
