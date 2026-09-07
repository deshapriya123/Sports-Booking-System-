using SportsBookingSystem.Models;

namespace SportsBookingSystem.Models
{
    public class FacilitySearchViewModel
    {
        // Filter inputs
        public string? Keyword { get; set; }
        public int? SportId { get; set; }
        public string? Location { get; set; }
        public bool AvailableOnly { get; set; } = true;

        // Dropdown source
        public List<Sport> Sports { get; set; } = new();

        // Results
        public List<Facility> Results { get; set; } = new();
    }
}
