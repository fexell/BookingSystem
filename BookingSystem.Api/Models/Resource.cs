using System.ComponentModel.DataAnnotations;

namespace BookingSystem.Api.Models {
    public class Resource {
        public int Id { get; set; }

        [Required( ErrorMessage = "Name is required." )]
        [MinLength( 1, ErrorMessage = "Name cannot be empty." )]
        public string Name { get; set; } = string.Empty;

        [Required( ErrorMessage = "Description is required." )]
        [MinLength( 1, ErrorMessage = "Description cannot be empty." )]
        public string Description { get; set; } = string.Empty;

        [Required( ErrorMessage = "Type is required." )]
        [MinLength( 1, ErrorMessage = "Type cannot be empty." )]
        public string Type { get; set; } = string.Empty;

        public bool IsAvailable { get; set; } = true;

        public ICollection<Booking> Bookings { get; set; } = new List<Booking>();
    }
}
