namespace HotelBookingAPI.DTOs.Hotel;

public class HotelDto
{
    public int HotelId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Location { get; set; } = string.Empty;
    public string? Description { get; set; }
}
