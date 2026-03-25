using System.ComponentModel.DataAnnotations;

namespace BookingSystem.Shared.DTOs;

public record CreateBookingRequest(
    [Required] DateTime StartTime,
    [Required] DateTime EndTime,
    [Required] int ResourceId,
    [Range( 1, 20 )] int PartySize = 1,
    string? Notes = null
);

public record BookingResponse (
    int Id,
    DateTime StartTime,
    DateTime EndTime,
    string Status,
    int UserId,
    string Username,
    int ResourceId,
    string ResourceName,
    int PartySize,
    string? Notes
);