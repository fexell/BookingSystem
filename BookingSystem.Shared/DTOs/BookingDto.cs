namespace BookingSystem.Shared.DTOs;

public class BookingDto
{
    public int Id { get; set; }
    public DateTime StartTime { get; set; }
    public DateTime EndTime { get; set; }
    public string Status { get; set; } = "Active";

    public int UserId { get; set; }
    public int ResourceId { get; set; }
}
