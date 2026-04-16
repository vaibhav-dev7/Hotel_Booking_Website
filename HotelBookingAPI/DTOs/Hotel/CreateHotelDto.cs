using System.ComponentModel.DataAnnotations;

namespace HotelBookingAPI.DTOs.Hotel;

public class CreateHotelDto
{
    [Required(ErrorMessage = "Name is required")]
    public string Name { get; set; } = string.Empty;

    [Required(ErrorMessage = "Location is required")]
    public string Location { get; set; } = string.Empty;

    public string? Description { get; set; }
}
