using System;
using System.Security.Claims;
using System.Threading.Tasks;
using HotelBookingAPI.DTOs.Booking;
using HotelBookingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingAPI.Controllers;

[ApiController]
[Route("api/bookings")]
public class BookingsController : ControllerBase
{
    private readonly IBookingService _bookingService;
    private readonly IEmailService _emailService;

    public BookingsController(IBookingService bookingService, IEmailService emailService)
    {
        _bookingService = bookingService;
        _emailService = emailService;
    }

    // POST: /api/bookings
    [HttpPost]
    [Authorize] // Must be logged in
    public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto dto)
    {
        try
        {
            // Extract UserId and Email perfectly securely from the incoming JWT token
            int userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            string email = User.FindFirst(ClaimTypes.Email)?.Value ?? "";

            var booking = await _bookingService.CreateBooking(dto, userId);

            // Trigger Mock Email confirmation directly after safe insertion
            await _emailService.SendBookingConfirmation(
                email, 
                booking.ReservationNumber, 
                booking.HotelName, 
                booking.RoomType, 
                booking.CheckInDate, 
                booking.CheckOutDate, 
                booking.TotalAmount);

            return Created("", booking);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // GET: /api/bookings/availability?roomId=1&checkIn=...&checkOut=...
    [HttpGet("availability")]
    public async Task<IActionResult> CheckAvailability(
        [FromQuery] int roomId, 
        [FromQuery] DateTime checkIn, 
        [FromQuery] DateTime checkOut)
    {
        try
        {
            var isAvailable = await _bookingService.CheckAvailability(roomId, checkIn, checkOut);
            return Ok(new { IsAvailable = isAvailable });
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // GET: /api/bookings/my
    [HttpGet("my")]
    [Authorize]
    public async Task<IActionResult> GetUserBookings()
    {
        try
        {
            int userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var bookings = await _bookingService.GetUserBookings(userId);
            return Ok(bookings);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // POST: /api/bookings/rebook/{bookingId}
    [HttpPost("rebook/{bookingId}")]
    [Authorize]
    public async Task<IActionResult> Rebook(int bookingId)
    {
        try
        {
            int userId = int.Parse(User.FindFirst("UserId")?.Value ?? "0");
            var rebookDto = await _bookingService.Rebook(bookingId, userId);
            
            // This returns the history format, which the frontend uses to pre-fill the search/booking form
            return Ok(rebookDto);
        }
        catch (Exception ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}
