using Microsoft.AspNetCore.Mvc;
using SportsBookingSystem.Data;
using SportsBookingSystem.Models;

namespace SportsBookingSystem.Controllers
{
    public class GuestController : Controller
    {
        private readonly ApplicationDbContext _context;

        public GuestController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: /Guest/Inquiry
        // Open to everyone -- no login required.
        [HttpGet]
        public IActionResult Inquiry()
        {
            return View(new GuestInquiryViewModel());
        }

        // POST: /Guest/Inquiry
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Inquiry(GuestInquiryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var inquiry = new Inquiry
            {
                Name = model.Name,
                Email = model.Email,
                Message = model.Message,
                SubmittedDate = DateTime.Now
            };

            _context.Inquiries.Add(inquiry);
            await _context.SaveChangesAsync();

            TempData["Success"] = "Thanks for reaching out! We'll get back to you soon.";
            return RedirectToAction(nameof(Inquiry));
        }
    }
}
