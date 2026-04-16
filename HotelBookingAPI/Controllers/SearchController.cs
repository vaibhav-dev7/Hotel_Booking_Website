using System;
using System.Threading.Tasks;
using HotelBookingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingAPI.Controllers;

[ApiController]
[Route("api/search")]
public class SearchController : ControllerBase
{
    private readonly ISearchService _searchService;

    public SearchController(ISearchService searchService)
    {
        _searchService = searchService;
    }

    // GET: /api/search?location=xx&minPrice=yy...
    [HttpGet]
    public async Task<IActionResult> SearchRooms(
        [FromQuery] string? location,
        [FromQuery] decimal? minPrice,
        [FromQuery] decimal? maxPrice,
        [FromQuery] string? amenities,
        [FromQuery] DateTime? checkIn,
        [FromQuery] DateTime? checkOut)
    {
        try
        {
            var results = await _searchService.SearchRooms(location, minPrice, maxPrice, amenities, checkIn, checkOut);
            return Ok(results);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }
}
