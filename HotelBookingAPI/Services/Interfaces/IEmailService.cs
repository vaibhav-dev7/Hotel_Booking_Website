using System;
using System.Threading.Tasks;

namespace HotelBookingAPI.Services.Interfaces;

public interface IEmailService
{
    Task SendBookingConfirmation(
        string email, 
        string reservationNumber, 
        string hotelName, 
        string roomType, 
        DateTime checkIn, 
        DateTime checkOut, 
        decimal totalAmount);
}
