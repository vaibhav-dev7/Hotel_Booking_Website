using System;
using System.Threading.Tasks;
using HotelBookingAPI.DTOs.Coupon;
using HotelBookingAPI.Services.Interfaces;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace HotelBookingAPI.Controllers;

[ApiController]
[Route("api/coupons")]
public class CouponsController : ControllerBase
{
    private readonly ICouponService _couponService;

    public CouponsController(ICouponService couponService)
    {
        _couponService = couponService;
    }

    // POST: /api/coupons/validate
    [HttpPost("validate")]
    [Authorize] // Must be logged in to test coupons
    public async Task<IActionResult> ValidateCoupon([FromBody] ValidateCouponDto dto)
    {
        try
        {
            var result = await _couponService.ValidateCoupon(dto.Code);
            return Ok(result);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // POST: /api/coupons
    [HttpPost]
    [Authorize(Roles = "Admin")] // Admins only
    public async Task<IActionResult> CreateCoupon([FromBody] CreateCouponDto dto)
    {
        try
        {
            var coupon = await _couponService.CreateCoupon(dto);
            return Created("", coupon);
        }
        catch (Exception ex)
        {
            return BadRequest(new { error = ex.Message });
        }
    }

    // PUT: /api/coupons/{id}/toggle
    [HttpPut("{id}/toggle")]
    [Authorize(Roles = "Admin")] // Admins only
    public async Task<IActionResult> ToggleCoupon(int id)
    {
        try
        {
            var result = await _couponService.ToggleCoupon(id);
            return Ok(new { message = result });
        }
        catch (Exception ex)
        {
            return NotFound(new { error = ex.Message });
        }
    }
}
