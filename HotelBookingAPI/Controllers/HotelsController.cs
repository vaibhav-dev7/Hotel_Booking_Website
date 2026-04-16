using System;
using System.Threading.Tasks;
using HotelBookingAPI.DTOs.Hotel;
using HotelBookingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingAPI.Controllers;

[ApiController]
[Route("api/hotels")]
public class HotelsController : ControllerBase
{
    private readonly IHotelService _hotelService;

    public HotelsController(IHotelService hotelService)
    {
        _hotelService = hotelService;
    }

    // GET: /api/hotels
    [HttpGet]
    public async Task<IActionResult> GetAllHotels()
    {
        try
        {
            var hotels = await _hotelService.GetAllHotels();
            return Ok(hotels);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { error = ex.Message });
        }
    }

    // POST: /api/hotels
    [HttpPost]
    [Authorize(Roles = "Admin")] // Only admins can add hotels
    public async Task<IActionResult> AddHotel([FromBody] CreateHotelDto dto)
    {
        try
        {
            var hotel = await _hotelService.AddHotel(dto);
            return Created("", hotel);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
