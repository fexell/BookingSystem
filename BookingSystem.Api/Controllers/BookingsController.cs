using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

using BookingSystem.Api.Models;
using BookingSystem.Api.Services;
using BookingSystem.Api.Filters;
using BookingSystem.Api.Helpers;

using BookingSystem.Shared.DTOs;

namespace BookingSystem.Api.Controllers
{
    [Authorize]
    [ApiController]
    [Route("api/[controller]")]
    public class BookingsController : ControllerBase
    {
        private readonly IBookingService _bookingService;

        public BookingsController(IBookingService bookingService)
        {
            _bookingService = bookingService;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetAll()
        {
            var bookings = await _bookingService.GetAllBookingsAsync();
            return Ok(bookings.Select(BookingMapperHelper.ToResponse));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<BookingDto>> GetById(int id)
        {
            var booking = await _bookingService.GetBookingByIdAsync(id);
            if (booking == null)
                return NotFound();

            return Ok( BookingMapperHelper.ToResponse( booking ) );
        }

        [ServiceFilter(typeof(SameUserFilter))]
        [HttpGet("user/{userId}")]
        public async Task<ActionResult<IEnumerable<BookingDto>>> GetByUserId(int userId)
        {
            var bookings = await _bookingService.GetBookingsByUserIdAsync(userId);
            return Ok(bookings);
        }

        [HttpPost]
        public async Task<IActionResult> Create(CreateBookingRequest request)
        {
            var userId = int.Parse( User.FindFirstValue( ClaimTypes.NameIdentifier )! );
            var booking = new Booking {
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                ResourceId = request.ResourceId,
                PartySize = request.PartySize,
                Notes = request.Notes,
                UserId = userId
            };

            try {
                var created = await _bookingService.CreateBookingAsync( booking );

                return CreatedAtAction( nameof( GetById ), new { id = created.Id }, BookingMapperHelper.ToResponse( created ) );
            } catch(InvalidOperationException ex) {
                return BadRequest(new { message = ex.Message });
            }
        }

        [HttpPut("{id}")]
        public async Task<IActionResult> Update(int id, Booking booking)
        {
            if (id != booking.Id)
                return BadRequest("ID does not match.");

            await _bookingService.UpdateBookingAsync(booking);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            await _bookingService.DeleteBookingAsync(id);
            return NoContent();
        }
    }
}