using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HotelBookingAPI.Data;
using HotelBookingAPI.DTOs.Search;
using HotelBookingAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingAPI.Services.Implementations;

public class SearchService : ISearchService
{
    private readonly AppDbContext _context;

    public SearchService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<SearchResultDto>> SearchRooms(
        string? location, 
        decimal? minPrice, 
        decimal? maxPrice, 
        string? amenities, 
        DateTime? checkIn, 
        DateTime? checkOut)
    {
        // a & b. Start with base query and make it queryable for dynamic appending
        var query = _context.Rooms
            .Include(r => r.Hotel)
            .Include(r => r.Amenities)
            .Where(r => r.IsActive == true)
            .AsQueryable(); //convert normal data into query from  so that we can add more filters to it 

        // c. Location Filter
        if (!string.IsNullOrWhiteSpace(location))
        {
            query = query.Where(r => r.Hotel.Location.ToLower().Contains(location.ToLower()));
        }

        // d. Minimum Price Filter
        if (minPrice.HasValue)
        {
            query = query.Where(r => r.PricePerNight >= minPrice.Value);
        }

        // e. Maximum Price Filter
        if (maxPrice.HasValue)
        {
            query = query.Where(r => r.PricePerNight <= maxPrice.Value);
        }

        // f. Amenities Filter (e.g. "?amenities=WiFi,Pool")
        if (!string.IsNullOrWhiteSpace(amenities))
        {
            var amenityList = amenities.Split(',').Select(a => a.Trim().ToLower()).ToList();
            
            // Ensure the room has AT LEAST ONE of the requested amenities
            query = query.Where(r => r.Amenities.Any(a => amenityList.Contains(a.Name.ToLower())));
        }

        // g. Availability / Overlap Filter (Date checking)  if room is booked at patuclar date it will nto be shown in search result 
        if (checkIn.HasValue && checkOut.HasValue)
        {
            var reqIn = checkIn.Value.Date;
            var reqOut = checkOut.Value.Date;

            // EXCLUDE rooms that have ANY booking overlapping with our requested dates
            query = query.Where(room => !room.Bookings.Any(b => 
                b.CheckInDate.Date < reqOut && b.CheckOutDate.Date > reqIn
            ));
        }

        // h. Execute query against the database
        var rooms = await query.ToListAsync();

        // i. Map results to DTOs
        var results = rooms.Select(r => new SearchResultDto
        {
            HotelId = r.HotelId,
            HotelName = r.Hotel.Name,
            Location = r.Hotel.Location,
            RoomId = r.RoomId,
            RoomType = r.RoomType,
            PricePerNight = r.PricePerNight,
            Capacity = r.Capacity,
            Amenities = r.Amenities.Select(a => a.Name).ToList()
        }).ToList();

        // j. Return results
        return results;
    }
}
