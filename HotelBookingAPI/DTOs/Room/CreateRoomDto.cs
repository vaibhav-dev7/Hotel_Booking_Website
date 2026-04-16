using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace HotelBookingAPI.DTOs.Room;

public class CreateRoomDto
{
    [Required]
    public int HotelId { get; set; }

    [Required(ErrorMessage = "Room Type is required")]
    public string RoomType { get; set; } = string.Empty;

    [Required]
    [Range(1, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
    public decimal PricePerNight { get; set; }

    [Required]
    [Range(1, int.MaxValue, ErrorMessage = "Capacity must be at least 1")]
    public int Capacity { get; set; }

    public List<int>? AmenityIds { get; set; }
}
