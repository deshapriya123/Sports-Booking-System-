using System.ComponentModel.DataAnnotations;

namespace SportsBookingSystem.Models
{
    public class GuestInquiryViewModel
    {
        [Required, MaxLength(100)]
        public string Name { get; set; } = string.Empty;

        [Required, EmailAddress, MaxLength(100)]
        public string Email { get; set; } = string.Empty;

        [Required, MaxLength(1000)]
        public string Message { get; set; } = string.Empty;
    }
}
