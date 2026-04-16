using System;
using System.IdentityModel.Tokens.Jwt; //token object 
using System.Security.Claims;
using System.Text; //utf 8 encoding
using System.Threading.Tasks; //task return type 
using HotelBookingAPI.Data;
using HotelBookingAPI.DTOs.Auth;
using HotelBookingAPI.Models;
using HotelBookingAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; //iconfiguration to read appsetting .json
using Microsoft.IdentityModel.Tokens; //symmetric security key control jwt lifetyme creation singing etc 

namespace HotelBookingAPI.Services.Implementations;

public class AuthService : IAuthService
{
    private readonly AppDbContext _context;
    private readonly IConfiguration _configuration;

    // Constructor injection for DB context and configuration (appsettings)
    public AuthService(AppDbContext context, IConfiguration configuration)
    {
        _context = context;
        _configuration = configuration;
    }

    public async Task<string> RegisterUser(RegisterDto dto)
    {
        try
        {
            // a. Check if email already exists
            var existingUser = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (existingUser != null)
            {
                throw new InvalidOperationException("Email already registered");
            }

            // b. Hash password using BCrypt
            string hashedPassword = BCrypt.Net.BCrypt.HashPassword(dto.Password);

            // c. Create User object with Role = "User"
            var user = new User
            {
                Name = dto.Name,
                Email = dto.Email,
                PasswordHash = hashedPassword,
                Role = "User", // Default role for standard customers
                CreatedAt = DateTime.UtcNow
            };

            // d. Add to DB and save
            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            // e. Return success message
            return "User registered successfully";
        }
        catch (Exception ex)
        {
            throw new Exception($"Error during registration: {ex.Message}");
        }
    }

    public async Task<AuthResponseDto> LoginUser(LoginDto dto)
    {
        try
        {
            // a. Find user by email
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == dto.Email);
            if (user == null)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            // b. Verify password using BCrypt
            bool isPasswordValid = BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash);
            if (!isPasswordValid)
            {
                throw new UnauthorizedAccessException("Invalid credentials");
            }

            // c. Generate JWT token with claims
            var claims = new[]
            {
                new Claim("UserId", user.UserId.ToString()),
                new Claim(ClaimTypes.Name, user.Name),
                new Claim(ClaimTypes.Role, user.Role),
                new Claim(ClaimTypes.Email, user.Email)
            };

            // d. Read settings from IConfiguration (appsettings.json)
            var keyStr = _configuration["Jwt:Key"];
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(keyStr!));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            int expiryHours = Convert.ToInt32(_configuration["Jwt:ExpiryHours"]);

            var tokenDescriptor = new JwtSecurityToken(
                issuer: _configuration["Jwt:Issuer"],
                audience: _configuration["Jwt:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddHours(expiryHours),
                signingCredentials: creds
            );

            // Create the encoded string token
            var tokenString = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            // e. Return AuthResponseDto
            return new AuthResponseDto
            {
                Token = tokenString,
                Name = user.Name,
                Role = user.Role
            };
        }
        catch (Exception ex)
        {
            throw new Exception($"Error during login: {ex.Message}");
        }
    }
}
