using System;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingAPI.DTOs.Booking;

public class CreateBookingDto
{
    [Required]
    public int RoomId { get; set; }

    [Required]
    public DateTime CheckInDate { get; set; }

    [Required]
    public DateTime CheckOutDate { get; set; }

    public string? CouponCode { get; set; }
}
