using System;
using System.Threading.Tasks;
using HotelBookingAPI.DTOs.Room;
using HotelBookingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingAPI.Controllers;

[ApiController]
[Route("api")] // We map custom routes individually below
public class RoomsController : ControllerBase
{
    private readonly IRoomService _roomService;

    public RoomsController(IRoomService roomService)
    {
        _roomService = roomService;
    }

    // GET: /api/hotels/{hotelId}/rooms
    [HttpGet("hotels/{hotelId}/rooms")]
    public async Task<IActionResult> GetRoomsByHotel(int hotelId)
    {
        try
        {
            var rooms = await _roomService.GetRoomsByHotel(hotelId);
            return Ok(rooms);
        }
        catch (Exception ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }

    // POST: /api/rooms
    [HttpPost("rooms")]
    [Authorize(Roles = "Admin")] // Only admins can add rooms
    public async Task<IActionResult> AddRoom([FromBody] CreateRoomDto dto)
    {
        try
        {
            var room = await _roomService.AddRoom(dto);
            return Created("", room);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
