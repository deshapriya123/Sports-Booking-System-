using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Data;
using SportsBookingSystem.Models;

namespace SportsBookingSystem.Controllers
{
    public class FacilityController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FacilityController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Facility/Search
        // Works for both members and guests — no login required.
        // Query string params: keyword, sportId, location, availableOnly
        [HttpGet]
        public async Task<IActionResult> Search(string? keyword, int? sportId, string? location, bool availableOnly = true)
        {
            var query = _context.Facilities
                .Include(f => f.Sport)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(keyword))
            {
                query = query.Where(f => f.FacilityName.Contains(keyword)
                                       || (f.Description != null && f.Description.Contains(keyword)));
            }

            if (sportId.HasValue)
            {
                query = query.Where(f => f.SportID == sportId.Value);
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                query = query.Where(f => f.Location.Contains(location));
            }

            if (availableOnly)
            {
                query = query.Where(f => f.Status == "Available");
            }

            var model = new FacilitySearchViewModel
            {
                Keyword = keyword,
                SportId = sportId,
                Location = location,
                AvailableOnly = availableOnly,
                Sports = await _context.Sports.OrderBy(s => s.SportName).ToListAsync(),
                Results = await query.OrderBy(f => f.FacilityName).ToListAsync()
            };

            return View(model);
        }

        // GET: /Facility/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var facility = await _context.Facilities
                .Include(f => f.Sport)
                .Include(f => f.Reviews)
                    .ThenInclude(r => r.Member)
                .FirstOrDefaultAsync(f => f.FacilityID == id);

            if (facility == null) return NotFound();

            return View(facility);
        }
    }
}
