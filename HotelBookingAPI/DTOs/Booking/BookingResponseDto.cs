using System;

namespace HotelBookingAPI.DTOs.Booking;

public class BookingResponseDto
{
    public int BookingId { get; set; }
    public string ReservationNumber { get; set; } = string.Empty;
    public string HotelName { get; set; } = string.Empty;
    public string RoomType { get; set; } = string.Empty;
    public DateTime CheckInDate { get; set; }
    public DateTime CheckOutDate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public string? CouponCode { get; set; }
}
