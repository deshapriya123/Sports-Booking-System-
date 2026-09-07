using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Data;
using SportsBookingSystem.Models;

namespace SportsBookingSystem.Controllers
{
    public class ReviewController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ReviewController(ApplicationDbContext context)
        {
            _context = context;
        }

        private int? CurrentMemberId => HttpContext.Session.GetInt32("MemberID");

        // GET: /Review/Create?bookingId=4
        [HttpGet]
        public async Task<IActionResult> Create(int bookingId)
        {
            if (CurrentMemberId == null)
                return RedirectToAction("Login", "Account");

            var booking = await _context.Bookings
                .Include(b => b.Facility)
                    .ThenInclude(f => f!.Sport)
                .Include(b => b.Review)
                .FirstOrDefaultAsync(b => b.BookingID == bookingId && b.MemberID == CurrentMemberId);

            if (booking == null) return NotFound();

            if (booking.Status != "Completed")
            {
                TempData["Error"] = "You can only review a facility after your booking is completed.";
                return RedirectToAction("MyBookings", "Booking");
            }

            if (booking.Review != null)
            {
                TempData["Error"] = "You've already reviewed this booking.";
                return RedirectToAction("MyBookings", "Booking");
            }

            var model = new ReviewCreateViewModel
            {
                BookingID = booking.BookingID,
                FacilityName = booking.Facility?.FacilityName ?? "",
                SportName = booking.Facility?.Sport?.SportName ?? "",
                BookingDate = booking.BookingDate
            };

            return View(model);
        }

        // POST: /Review/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ReviewCreateViewModel model)
        {
            if (CurrentMemberId == null)
                return RedirectToAction("Login", "Account");

            var booking = await _context.Bookings
                .Include(b => b.Facility)
                    .ThenInclude(f => f!.Sport)
                .Include(b => b.Review)
                .FirstOrDefaultAsync(b => b.BookingID == model.BookingID && b.MemberID == CurrentMemberId);

            if (booking == null) return NotFound();

            // Re-populate display fields in case we need to redisplay the form
            model.FacilityName = booking.Facility?.FacilityName ?? "";
            model.SportName = booking.Facility?.Sport?.SportName ?? "";
            model.BookingDate = booking.BookingDate;

            if (booking.Status != "Completed")
            {
                ModelState.AddModelError(string.Empty, "You can only review a facility after your booking is completed.");
            }

            if (booking.Review != null)
            {
                ModelState.AddModelError(string.Empty, "You've already reviewed this booking.");
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var review = new Review
            {
                MemberID = CurrentMemberId!.Value,
                FacilityID = booking.FacilityID,
                BookingID = booking.BookingID,
                Rating = model.Rating,
                Comment = model.Comment,
                ReviewDate = DateTime.Now
            };

            _context.Reviews.Add(review);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Thanks for your review!";
            return RedirectToAction("MyBookings", "Booking");
        }
    }
}
