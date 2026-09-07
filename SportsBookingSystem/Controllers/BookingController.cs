using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Data;
using SportsBookingSystem.Models;

namespace SportsBookingSystem.Controllers
{
    public class BookingController : Controller
    {
        private readonly ApplicationDbContext _context;

        public BookingController(ApplicationDbContext context)
        {
            _context = context;
        }

        // Every action here requires a logged-in member.
        // Returns the current MemberID, or null if not logged in.
        private int? CurrentMemberId => HttpContext.Session.GetInt32("MemberID");

        // GET: /Booking/Create?facilityId=5
        [HttpGet]
        public async Task<IActionResult> Create(int facilityId)
        {
            if (CurrentMemberId == null)
                return RedirectToAction("Login", "Account");

            var facility = await _context.Facilities
                .Include(f => f.Sport)
                .FirstOrDefaultAsync(f => f.FacilityID == facilityId);

            if (facility == null) return NotFound();

            if (facility.Status != "Available")
            {
                TempData["Error"] = "This facility is not currently available for booking.";
                return RedirectToAction("Details", "Facility", new { id = facilityId });
            }

            var model = new BookingCreateViewModel
            {
                FacilityID = facility.FacilityID,
                FacilityName = facility.FacilityName,
                SportName = facility.Sport?.SportName ?? "",
                Location = facility.Location
            };

            return View(model);
        }

        // POST: /Booking/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(BookingCreateViewModel model)
        {
            if (CurrentMemberId == null)
                return RedirectToAction("Login", "Account");

            var facility = await _context.Facilities
                .Include(f => f.Sport)
                .FirstOrDefaultAsync(f => f.FacilityID == model.FacilityID);

            if (facility == null) return NotFound();

            // Re-populate display fields in case we need to redisplay the form
            model.FacilityName = facility.FacilityName;
            model.SportName = facility.Sport?.SportName ?? "";
            model.Location = facility.Location;

            if (string.Compare(model.StartTime, model.EndTime, StringComparison.Ordinal) >= 0)
            {
                ModelState.AddModelError(string.Empty, "End time must be after start time.");
            }

            if (model.BookingDate.Date < DateTime.Today)
            {
                ModelState.AddModelError(string.Empty, "Booking date cannot be in the past.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // Prevent double-booking: check for overlapping, non-cancelled bookings
            // on the same facility and date.
            // NOTE: fetch same-facility/same-date/non-cancelled bookings with simple,
            // fully-translatable filters first, then do the time-range overlap check
            // in memory (LINQ to Objects). Doing the CompareTo()-based overlap logic
            // directly inside the SQL query can cause the Oracle provider to emit an
            // unsupported boolean literal (ORA-00904: "FALSE": invalid identifier) on
            // Oracle versions before 23c, which don't support SQL boolean literals.
            var bookingDateOnly = model.BookingDate.Date;
            var candidateBookings = await _context.Bookings
                .Where(b => b.FacilityID == model.FacilityID
                         && b.BookingDate == bookingDateOnly
                         && b.Status != "Cancelled")
                .ToListAsync();

            bool overlaps = candidateBookings.Any(b =>
                model.StartTime.CompareTo(b.EndTime) < 0 &&
                model.EndTime.CompareTo(b.StartTime) > 0);

            if (overlaps)
            {
                ModelState.AddModelError(string.Empty, "This facility is already booked for part of that time slot. Please choose a different time.");
                return View(model);
            }

            var booking = new Booking
            {
                MemberID = CurrentMemberId!.Value,
                FacilityID = model.FacilityID,
                BookingDate = model.BookingDate.Date,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                Status = "Confirmed",
                CreatedAt = DateTime.Now
            };

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Booking confirmed!";
            return RedirectToAction(nameof(MyBookings));
        }

        // GET: /Booking/MyBookings
        [HttpGet]
        public async Task<IActionResult> MyBookings()
        {
            if (CurrentMemberId == null)
                return RedirectToAction("Login", "Account");

            var bookings = await _context.Bookings
                .Include(b => b.Facility)
                    .ThenInclude(f => f!.Sport)
                .Include(b => b.Review)
                .Where(b => b.MemberID == CurrentMemberId)
                .OrderByDescending(b => b.BookingDate)
                .ToListAsync();

            return View(bookings);
        }

        // POST: /Booking/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            if (CurrentMemberId == null)
                return RedirectToAction("Login", "Account");

            var booking = await _context.Bookings
                .FirstOrDefaultAsync(b => b.BookingID == id && b.MemberID == CurrentMemberId);

            if (booking == null) return NotFound();

            if (booking.BookingDate.Date < DateTime.Today)
            {
                TempData["Error"] = "Past bookings cannot be cancelled.";
                return RedirectToAction(nameof(MyBookings));
            }

            booking.Status = "Cancelled";
            await _context.SaveChangesAsync();

            TempData["Success"] = "Booking cancelled.";
            return RedirectToAction(nameof(MyBookings));
        }
    }
}
