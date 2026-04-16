using System.Collections.Generic;

namespace HotelBookingAPI.DTOs.Room;

public class RoomDto
{
    public int RoomId { get; set; }
    public int HotelId { get; set; }
    public string RoomType { get; set; } = string.Empty;
    public decimal PricePerNight { get; set; }
    public int Capacity { get; set; }
    public bool? IsActive { get; set; }
    public List<string> Amenities { get; set; } = new List<string>();
}
