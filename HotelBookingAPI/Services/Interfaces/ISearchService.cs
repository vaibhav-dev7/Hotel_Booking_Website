using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using HotelBookingAPI.DTOs.Search;

namespace HotelBookingAPI.Services.Interfaces;

public interface ISearchService
{
    Task<List<SearchResultDto>> SearchRooms(
        string? location, 
        decimal? minPrice, 
        decimal? maxPrice, 
        string? amenities, 
        DateTime? checkIn, 
        DateTime? checkOut);
}
