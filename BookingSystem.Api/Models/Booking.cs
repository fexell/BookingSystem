namespace BookingSystem.Api.Models
{
    public class Booking
    {
        public int Id { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string Status { get; set; } = "Active";

        public int UserId { get; set; }
        public User User { get; set; } = null!;

        public int ResourceId { get; set; }
        public Resource Resource { get; set; } = null!;
        public string? Notes { get; set; }
        public int PartySize { get; set; } = 1;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    }
}
