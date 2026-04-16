using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelBookingAPI.Data;
using HotelBookingAPI.DTOs.Hotel;
using HotelBookingAPI.Models;
using HotelBookingAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingAPI.Services.Implementations;

public class HotelService : IHotelService
{
    private readonly AppDbContext _context;

    public HotelService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<HotelDto>> GetAllHotels()
    {
        // Fetch all hotels and project them into HotelDto
        return await _context.Hotels
            .Select(h => new HotelDto
            {
                HotelId = h.HotelId,
                Name = h.Name,
                Location = h.Location,
                Description = h.Description
            })
            .ToListAsync();
    }

    public async Task<HotelDto> AddHotel(CreateHotelDto dto)
    {
        // 1. Create Hotel entity
        var hotel = new Hotel
        {
            Name = dto.Name,
            Location = dto.Location,
            Description = dto.Description
        };

        // 2. Add to database
        _context.Hotels.Add(hotel);
        await _context.SaveChangesAsync();

        // 3. Return the mapped DTO
        return new HotelDto
        {
            HotelId = hotel.HotelId,
            Name = hotel.Name,
            Location = hotel.Location,
            Description = hotel.Description
        };
    }
}
