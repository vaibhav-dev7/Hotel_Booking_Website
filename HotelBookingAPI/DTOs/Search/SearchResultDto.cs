using System.Collections.Generic;

namespace HotelBookingAPI.DTOs.Search;

public class SearchResultDto
{
    public int HotelId { get; set; }
    public string HotelName { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public int RoomId { get; set; }
    public string RoomType { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public int Capacity { get; set; }
    public List<string> Amenities { get; set; } = new List<string>();
}
