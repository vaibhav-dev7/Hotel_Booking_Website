using System.Collections.Generic;
using System.Threading.Tasks;
using HotelBookingAPI.DTOs.Room;

namespace HotelBookingAPI.Services.Interfaces;

public interface IRoomService
{
    Task<List<RoomDto>> GetRoomsByHotel(int hotelId);
    Task<RoomDto> AddRoom(CreateRoomDto dto);
}
