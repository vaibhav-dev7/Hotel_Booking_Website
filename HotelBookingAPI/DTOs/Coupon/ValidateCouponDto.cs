using System.ComponentModel.DataAnnotations;

namespace HotelBookingAPI.DTOs.Coupon;

public class ValidateCouponDto
{
    [Required(ErrorMessage = "Code is required")]
    public string Code { get; set; } = string.Empty;
}
