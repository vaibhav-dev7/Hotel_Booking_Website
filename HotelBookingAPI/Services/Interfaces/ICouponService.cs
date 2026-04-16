using System.Threading.Tasks;
using HotelBookingAPI.DTOs.Coupon;

namespace HotelBookingAPI.Services.Interfaces;

public interface ICouponService
{
    Task<CouponResponseDto> ValidateCoupon(string code);
    Task<CouponResponseDto> CreateCoupon(CreateCouponDto dto);
    Task<string> ToggleCoupon(int couponId);
}
