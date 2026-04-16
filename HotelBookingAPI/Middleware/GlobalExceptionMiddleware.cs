using System;
using System.Collections.Generic;
using System.Net;
using System.Text.Json;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace HotelBookingAPI.Middleware;

public class GlobalExceptionMiddleware
{
    private readonly RequestDelegate _next;

    public GlobalExceptionMiddleware(RequestDelegate next)
    {
        _next = next;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            // Let the request pass through the pipeline to controllers
            await _next(context);
        }
        catch (Exception ex)
        {
            // If a controller or service throws an error, catch it here and format cleanly
            await HandleExceptionAsync(context, ex);
        }
    }

    private static Task HandleExceptionAsync(HttpContext context, Exception exception)
    {
        // Print the error directly to the console for easy debugging during the hackathon
        Console.WriteLine($"\n[GLOBAL ERROR] {exception.Message}\n{exception.StackTrace}\n");

        context.Response.ContentType = "application/json";

        // Map standard C# exceptions to HTTP status codes
        context.Response.StatusCode = exception switch
        {
            KeyNotFoundException => (int)HttpStatusCode.NotFound,               // 404
            UnauthorizedAccessException => (int)HttpStatusCode.Unauthorized,    // 401
            InvalidOperationException => (int)HttpStatusCode.BadRequest,        // 400
            ArgumentException => (int)HttpStatusCode.BadRequest,                // 400
            _ => (int)HttpStatusCode.InternalServerError                        // 500 Fallback
        };

        // For security, if it's a 500 error, we hide the inner stack trace from the client,
        // but if it's a 400 bad request, we pass along the safe message (like "Room is fully booked")
        var message = context.Response.StatusCode == (int)HttpStatusCode.InternalServerError
            ? "Something went wrong on the server."
            : exception.Message;

        var result = JsonSerializer.Serialize(new { error = message });

        return context.Response.WriteAsync(result);
    }
}
