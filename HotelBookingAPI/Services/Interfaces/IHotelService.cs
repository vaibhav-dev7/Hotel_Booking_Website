using System.Collections.Generic;
using System.Threading.Tasks;
using HotelBookingAPI.DTOs.Hotel;

namespace HotelBookingAPI.Services.Interfaces;

public interface IHotelService
{
    Task<List<HotelDto>> GetAllHotels();
    Task<HotelDto> AddHotel(CreateHotelDto dto);
}
