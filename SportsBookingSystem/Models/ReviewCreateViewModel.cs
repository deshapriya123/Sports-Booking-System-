using System.ComponentModel.DataAnnotations;

namespace SportsBookingSystem.Models
{
    public class ReviewCreateViewModel
    {
        public int BookingID { get; set; }
        public string FacilityName { get; set; } = string.Empty;
        public string SportName { get; set; } = string.Empty;
        public DateTime BookingDate { get; set; }

        [Required]
        [Range(1, 5, ErrorMessage = "Rating must be between 1 and 5.")]
        public int Rating { get; set; }

        [MaxLength(1000)]
        public string? Comment { get; set; }
    }
}
