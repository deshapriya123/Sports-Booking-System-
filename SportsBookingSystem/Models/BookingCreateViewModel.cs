using System.ComponentModel.DataAnnotations;

namespace SportsBookingSystem.Models
{
    public class BookingCreateViewModel
    {
        public int FacilityID { get; set; }
        public string FacilityName { get; set; } = string.Empty;
        public string SportName { get; set; } = string.Empty;
        public string Location { get; set; } = string.Empty;

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Booking Date")]
        public DateTime BookingDate { get; set; } = DateTime.Today.AddDays(1);

        [Required]
        [Display(Name = "Start Time")]
        public string StartTime { get; set; } = string.Empty;

        [Required]
        [Display(Name = "End Time")]
        public string EndTime { get; set; } = string.Empty;
    }
}
