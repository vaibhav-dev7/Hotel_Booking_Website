namespace HotelBookingAPI.DTOs.Coupon;

public class CouponResponseDto
{
    public bool IsValid { get; set; }
    public string? Code { get; set; }
    public decimal DiscountPercentage { get; set; }
}
