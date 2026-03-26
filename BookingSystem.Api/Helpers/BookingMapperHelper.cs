using BookingSystem.Api.Models;

using BookingSystem.Shared.DTOs;

namespace BookingSystem.Api.Helpers;

public static class  BookingMapperHelper {
    public static BookingResponse ToResponse( Booking b ) => new(
    b.Id,
    b.StartTime,
    b.EndTime,
    b.Status,
    b.UserId,
    b.User.UserName,
    b.FirstName,
    b.Surname,
    b.ResourceId,
    b.Resource.Name,
    b.PartySize,
    b.Notes
    );
}