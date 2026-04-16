using System.Threading.Tasks;
using HotelBookingAPI.DTOs.Auth;

namespace HotelBookingAPI.Services.Interfaces;

public interface IAuthService
{
    Task<string> RegisterUser(RegisterDto dto);
    Task<AuthResponseDto> LoginUser(LoginDto dto);
}
