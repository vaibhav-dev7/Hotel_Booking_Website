using System;
using System.Collections.Generic;

namespace HotelBookingAPI.Models;

public partial class Booking
{
    public int BookingId { get; set; }

    public int UserId { get; set; }

    public int RoomId { get; set; }

    public DateTime CheckInDate { get; set; }

    public DateTime CheckOutDate { get; set; }

    public decimal TotalAmount { get; set; }

    public int? CouponId { get; set; }

    public decimal? DiscountAmount { get; set; }

    public string ReservationNumber { get; set; } = null!;

    public DateTime? CreatedAt { get; set; }

    public virtual Coupon? Coupon { get; set; }

    public virtual Room Room { get; set; } = null!;

    public virtual User User { get; set; } = null!;
}
