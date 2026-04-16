using System;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingAPI.DTOs.Coupon;

public class CreateCouponDto
{
    [Required(ErrorMessage = "Code is required")]
    public string Code { get; set; } = string.Empty;

    [Required]
    [Range(1, 100, ErrorMessage = "Discount must be between 1 and 100 percentage")]
    public decimal DiscountPercentage { get; set; }

    [Required]
    public DateTime ExpiryDate { get; set; }
}
