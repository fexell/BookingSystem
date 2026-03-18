namespace BookingSystem.Api.Models
{
    public class Resource // det som går att boka i systemet, t.ex. rum, utrustning, etc.
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty; 
        public string Type { get; set; } = string.Empty;
        public bool IsAvilable { get; set; } = true;
        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
