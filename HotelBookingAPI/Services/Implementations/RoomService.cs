using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelBookingAPI.Data;
using HotelBookingAPI.DTOs.Room;
using HotelBookingAPI.Models;
using HotelBookingAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingAPI.Services.Implementations;

public class RoomService : IRoomService
{
    private readonly AppDbContext _context;

    public RoomService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<RoomDto>> GetRoomsByHotel(int hotelId)
    {
        // a. Check if hotel exists
        var hotelExists = await _context.Hotels.AnyAsync(h => h.HotelId == hotelId);
        if (!hotelExists)
        {
            throw new Exception("Hotel not found");
        }

        // b & c. Fetch active rooms for this hotel and INCLUDE Amenities
        // Note: EF Core 8 handles the RoomAmenities junction table automatically
        var rooms = await _context.Rooms
            .Include(r => r.Amenities)
            .Where(r => r.HotelId == hotelId && r.IsActive == true)
            .ToListAsync();

        // d. Map to RoomDto (converting amenities to simple List<string> for frontend)
        var roomDtos = rooms.Select(r => new RoomDto
        {
            RoomId = r.RoomId,
            HotelId = r.HotelId,
            RoomType = r.RoomType,
            PricePerNight = r.PricePerNight,
            Capacity = r.Capacity,
            IsActive = r.IsActive,
            Amenities = r.Amenities.Select(a => a.Name).ToList() // Pluck just the amenity names
        }).ToList();

        return roomDtos;
    }

    public async Task<RoomDto> AddRoom(CreateRoomDto dto)
    {
        // a. Check if hotel exists
        var hotelExists = await _context.Hotels.AnyAsync(h => h.HotelId == dto.HotelId);
        if (!hotelExists)
        {
            throw new Exception("Hotel not found");
        }

        // b. Create Room entity
        var room = new Room
        {
            HotelId = dto.HotelId,
            RoomType = dto.RoomType,
            PricePerNight = dto.PricePerNight,
            Capacity = dto.Capacity,
            IsActive = true
        };

        // c. Attach amenities to the intermediate mapping
        if (dto.AmenityIds != null && dto.AmenityIds.Any())
        {
            // Fetch the genuine amenities from DB
            var selectedAmenities = await _context.Amenities
                .Where(a => dto.AmenityIds.Contains(a.AmenityId))
                .ToListAsync();

            // Tell EF Core to link them to this Room
            foreach (var amenity in selectedAmenities)
            {
                room.Amenities.Add(amenity);
            }
        }

        // d. Save to DB
        _context.Rooms.Add(room);
        await _context.SaveChangesAsync();

        // e. Return mapped DTO
        return new RoomDto
        {
            RoomId = room.RoomId,
            HotelId = room.HotelId,
            RoomType = room.RoomType,
            PricePerNight = room.PricePerNight,
            Capacity = room.Capacity,
            IsActive = room.IsActive,
            Amenities = room.Amenities.Select(a => a.Name).ToList()
        };
    }
}
