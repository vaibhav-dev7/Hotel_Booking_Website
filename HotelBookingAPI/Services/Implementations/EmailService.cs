using System;
using System.Threading.Tasks;
using HotelBookingAPI.Services.Interfaces;

namespace HotelBookingAPI.Services.Implementations;

public class EmailService : IEmailService
{
    public Task SendBookingConfirmation(
        string email, 
        string reservationNumber, 
        string hotelName, 
        string roomType, 
        DateTime checkIn, 
        DateTime checkOut, 
        decimal totalAmount)
    {
        // Replace with real SMTP implementation (like SendGrid or MailKit) for production
        // This is a MOCK email service for the hackathon demo to avoid blocking deployment

        Console.WriteLine("\n==================================================");
        Console.WriteLine($"📧 MOCK EMAIL SENT TO: {email}");
        Console.WriteLine("==================================================");
        Console.WriteLine($"Subject: Your Booking Confirmation - {reservationNumber}");
        Console.WriteLine($"Hello! Your booking at {hotelName} is confirmed.");
        Console.WriteLine($"Room Type: {roomType}");
        Console.WriteLine($"Dates: {checkIn:MMM dd, yyyy} to {checkOut:MMM dd, yyyy}");
        Console.WriteLine($"Total Amount: ₹{totalAmount}");
        Console.WriteLine("==================================================\n");

        return Task.CompletedTask;
    }
}
