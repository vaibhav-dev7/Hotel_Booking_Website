using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotelBookingAPI.DTOs.Booking;

namespace HotelBookingAPI.Services.Interfaces;

public interface IBookingService
{
    Task<BookingResponseDto> CreateBooking(CreateBookingDto dto, int userId);
    Task<bool> CheckAvailability(int roomId, DateTime checkIn, DateTime checkOut);
    Task<List<BookingHistoryDto>> GetUserBookings(int userId);
    Task<BookingHistoryDto> Rebook(int bookingId, int userId);
}
