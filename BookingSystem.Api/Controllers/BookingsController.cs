using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Authorization;
using System.Security.Claims;
using BookingSystem.Api.Models;
using BookingSystem.Api.Repositories;

namespace BookingSystem.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize] // Kräver inloggning för allt här inne
    public class BookingsController : ControllerBase
    {
        private readonly IBookingRepository _bookingRepository;

        public BookingsController(IBookingRepository bookingRepository)
        {
            _bookingRepository = bookingRepository;
        }

        // HÄMTA MINA BOKNINGAR: /api/bookings/user/me
        [HttpGet("user/me")]
        public async Task<IActionResult> GetMyBookings()
        {
            // Hämta ID från token
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId)) return Unauthorized("Kunde inte verifiera dig.");

            var bookings = await _bookingRepository.GetByUserIdWithIncludesAsync(userId);

            // Mappar till DTO (som din frontend vill ha)
            var dtos = bookings.Select(b => new {
                Id = b.Id,
                ResourceId = b.ResourceId,
                ResourceName = b.Resource?.Name ?? "Okänd",
                StartTime = b.StartTime,
                EndTime = b.EndTime,
                Status = b.Status ?? "Bekräftad"
            });

            return Ok(dtos);
        }

        // SKAPA BOKNING: /api/bookings
        [HttpPost]
        public async Task<IActionResult> CreateBooking([FromBody] Booking request)
        {
            // MAGIN SOM LÖSER FELET: Hämta ID från JWT-token!
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (!int.TryParse(userIdString, out int userId))
            {
                return Unauthorized("Kunde inte hitta ditt användar-ID i inloggningen.");
            }

            // Skapa bokningen och sätt UserId!!
            var newBooking = new Booking
            {
                ResourceId = request.ResourceId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                PartySize = request.PartySize,
                Notes = request.Notes,
                Status = "Bekräftad",
                UserId = userId // <--- DET ÄR DENNA RAD SOM GÖR ATT DET INTE KRASCHAR LÄNGRE!
            };

            await _bookingRepository.AddAsync(newBooking);
            return Ok(newBooking);
        }

        // TA BORT BOKNING: /api/bookings/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteBooking(int id)
        {
            // Hämta ID från token
            var userIdString = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
            if (!int.TryParse(userIdString, out int userId)) return Unauthorized();

            var booking = await _bookingRepository.GetByIdAsync(id);
            if (booking == null) return NotFound();

            // Samma användare som skapat bokningen kan ta bort den
            if (booking.UserId != userId) return Forbid();

            await _bookingRepository.DeleteAsync(id);
            return Ok();
        }
    }
}