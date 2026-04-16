using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelBookingAPI.Data;
using HotelBookingAPI.DTOs.Booking;
using HotelBookingAPI.Models;
using HotelBookingAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingAPI.Services.Implementations;

public class BookingService : IBookingService
{
    private readonly AppDbContext _context;

    public BookingService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<BookingResponseDto> CreateBooking(CreateBookingDto dto, int userId)
    {
        // a. Validate: CheckInDate must be >= today
        if (dto.CheckInDate.Date < DateTime.UtcNow.Date)
        {
            throw new ArgumentException("Check-In date cannot be in the past.");
        }

        // b. Validate: CheckOutDate must be > CheckInDate
        if (dto.CheckOutDate.Date <= dto.CheckInDate.Date)
        {
            throw new ArgumentException("Check-Out date must be after Check-In date.");
        }

        // c. Find room by RoomId -> include Hotel -> check if active
        var room = await _context.Rooms
            .Include(r => r.Hotel)
            .FirstOrDefaultAsync(r => r.RoomId == dto.RoomId && r.IsActive == true);
        
        if (room == null)
        {
            throw new Exception("Room not found or is currently inactive.");
        }

        var reqIn = dto.CheckInDate.Date;
        var reqOut = dto.CheckOutDate.Date;

        // d. Check availability for overlapping dates on this room
        var isOverlap = await _context.Bookings.AnyAsync(b => 
            b.RoomId == dto.RoomId && 
            b.CheckInDate.Date < reqOut && 
            b.CheckOutDate.Date > reqIn);

        if (isOverlap)
        {
            throw new Exception("Room is not available for selected dates.");
        }

        // e. Handle coupon (if CouponCode is provided)
        Coupon? appliedCoupon = null; //nullable coupon object  itnulizte it if coupo is provied  and valid 
        if (!string.IsNullOrWhiteSpace(dto.CouponCode))
        {
            var coupon = await _context.Coupons.FirstOrDefaultAsync(c => 
                c.Code.ToLower() == dto.CouponCode.ToLower() && 
                c.IsActive == true);
            
            // Validate expiry
            if (coupon != null && coupon.ExpiryDate.Date >= DateTime.UtcNow.Date)
            {
                appliedCoupon = coupon;
            }
            // we ignore the coupon if it's invalid per the instructions
        }

        // f. Calculate price
        int nights = (dto.CheckOutDate.Date - dto.CheckInDate.Date).Days;
        decimal subtotal = room.PricePerNight * nights;
        decimal discountAmount = 0m;

        if (appliedCoupon != null)
        {
            discountAmount = subtotal * (appliedCoupon.DiscountPercentage / 100);
        }

        decimal totalAmount = subtotal - discountAmount;

        // g. Generate reservation number (RES- + random 8 chars)
        string reservationNumber = "RES-" + Guid.NewGuid().ToString().Substring(0, 8).ToUpper();

        // h. Create Booking entity
        var booking = new Booking
        {
            UserId = userId,
            RoomId = dto.RoomId,
            CheckInDate = reqIn,
            CheckOutDate = reqOut,
            TotalAmount = totalAmount,
            CouponId = appliedCoupon?.CouponId,
            DiscountAmount = discountAmount,
            ReservationNumber = reservationNumber,
            CreatedAt = DateTime.UtcNow
        };

        // i. Save to DB
        _context.Bookings.Add(booking);
        await _context.SaveChangesAsync();

        // j. Map to BookingResponseDto and return
        return new BookingResponseDto
        {
            BookingId = booking.BookingId,
            ReservationNumber = booking.ReservationNumber,
            HotelName = room.Hotel.Name,
            RoomType = room.RoomType,
            CheckInDate = dto.CheckInDate,
            CheckOutDate = dto.CheckOutDate,
            TotalAmount = booking.TotalAmount,
            DiscountAmount = booking.DiscountAmount ?? 0m,
            CouponCode = appliedCoupon?.Code
        };
    }

    public async Task<bool> CheckAvailability(int roomId, DateTime checkIn, DateTime checkOut)
    {
        var reqIn = checkIn.Date;
        var reqOut = checkOut.Date;

        // Query Bookings WHERE RoomId matches AND dates overlap
        // Overlap: existingCheckIn < checkOut AND existingCheckOut > checkIn
        var hasOverlap = await _context.Bookings.AnyAsync(b => 
            b.RoomId == roomId && 
            b.CheckInDate.Date < reqOut && 
            b.CheckOutDate.Date > reqIn);

        // Return true if NO overlap found
        return !hasOverlap;
    }

    public async Task<List<BookingHistoryDto>> GetUserBookings(int userId)
    {
        // Fetch bookings for the specific user
        var bookings = await _context.Bookings
            .Include(b => b.Room)           // Include Room 
                .ThenInclude(r => r.Hotel)  // Then Include Hotel
            .Include(b => b.Coupon)         // Include Coupon
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt) // Order by latest first
            .ToListAsync();

        // Map to BookingHistoryDto
        return bookings.Select(b => new BookingHistoryDto
        {
            BookingId = b.BookingId,
            ReservationNumber = b.ReservationNumber,
            HotelName = b.Room.Hotel.Name,
            HotelLocation = b.Room.Hotel.Location,
            RoomType = b.Room.RoomType,
            RoomId = b.RoomId,
            CheckInDate = b.CheckInDate,
            CheckOutDate = b.CheckOutDate,
            TotalAmount = b.TotalAmount,
            DiscountAmount = b.DiscountAmount ?? 0m,
            CouponCode = b.Coupon?.Code,
            CreatedAt = b.CreatedAt
        }).ToList();
    }

    public async Task<BookingHistoryDto> Rebook(int bookingId, int userId)
    {
        // Find specific previous booking
        var booking = await _context.Bookings
            .Include(b => b.Room)
                .ThenInclude(r => r.Hotel)
            .Include(b => b.Coupon)
            .FirstOrDefaultAsync(b => b.BookingId == bookingId && b.UserId == userId);

        if (booking == null)
        {
            throw new Exception("Booking not found.");
        }

        // Send back the history item. The frontend will take these details and initialize a NEW booking flow.
        return new BookingHistoryDto
        {
            BookingId = booking.BookingId,
            ReservationNumber = booking.ReservationNumber,
            HotelName = booking.Room.Hotel.Name,
            HotelLocation = booking.Room.Hotel.Location,
            RoomType = booking.Room.RoomType,
            RoomId = booking.RoomId,
            CheckInDate = booking.CheckInDate,
            CheckOutDate = booking.CheckOutDate,
            TotalAmount = booking.TotalAmount,
            DiscountAmount = booking.DiscountAmount ?? 0m,
            CouponCode = booking.Coupon?.Code,
            CreatedAt = booking.CreatedAt
        };
    }
}
