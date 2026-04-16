using System;
using System.Linq;
using System.Threading.Tasks;
using HotelBookingAPI.Data;
using HotelBookingAPI.DTOs.Coupon;
using HotelBookingAPI.Models;
using HotelBookingAPI.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace HotelBookingAPI.Services.Implementations;

public class CouponService : ICouponService
{
    private readonly AppDbContext _context;

    public CouponService(AppDbContext context)
    {
        _context = context;
    }

    public async Task<CouponResponseDto> ValidateCoupon(string code)
    {
        // a. Find coupon by code (case-insensitive)
        var coupon = await _context.Coupons.FirstOrDefaultAsync(c => c.Code.ToLower() == code.ToLower());

        // b. If not found
        if (coupon == null)
        {
            return new CouponResponseDto { IsValid = false };
        }

        // c. If found but not active
        if (coupon.IsActive == false)
        {
            return new CouponResponseDto { IsValid = false };
        }

        // d. If found but expired (ExpiryDate < today)
        if (coupon.ExpiryDate.Date < DateTime.UtcNow.Date)
        {
            return new CouponResponseDto { IsValid = false };
        }

        // e. If valid
        return new CouponResponseDto
        {
            IsValid = true,
            Code = coupon.Code,
            DiscountPercentage = coupon.DiscountPercentage
        };
    }

    public async Task<CouponResponseDto> CreateCoupon(CreateCouponDto dto)
    {
        // a. Check if code already exists
        var exists = await _context.Coupons.AnyAsync(c => c.Code.ToLower() == dto.Code.ToLower());
        if (exists)
        {
            throw new Exception("Coupon code already exists");
        }

        // b. Create Coupon entity
        var coupon = new Coupon
        {
            Code = dto.Code.ToUpper(), // Store as uppercase
            DiscountPercentage = dto.DiscountPercentage,
            ExpiryDate = dto.ExpiryDate,
            IsActive = true
        };

        // c. Save to DB and return
        _context.Coupons.Add(coupon);
        await _context.SaveChangesAsync();

        return new CouponResponseDto
        {
            IsValid = true,
            Code = coupon.Code,
            DiscountPercentage = coupon.DiscountPercentage
        };
    }

    public async Task<string> ToggleCoupon(int couponId)
    {
        // a. Find coupon by id
        var coupon = await _context.Coupons.FindAsync(couponId);
        if (coupon == null)
        {
            throw new Exception("Coupon not found");
        }

        // b. Flip IsActive state
        coupon.IsActive = !coupon.IsActive;

        // c. Save and return message
        await _context.SaveChangesAsync();
        
        return coupon.IsActive == true ? "Coupon activated" : "Coupon deactivated";
    }
}
