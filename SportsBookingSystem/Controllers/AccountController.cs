using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SportsBookingSystem.Data;
using SportsBookingSystem.Models;

namespace SportsBookingSystem.Controllers
{
    public class AccountController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly PasswordHasher<Member> _passwordHasher = new();

        public AccountController(ApplicationDbContext context)
        {
            _context = context;
        }

        // =========================================================
        // GET: /Account/Register
        // =========================================================
        [HttpGet]
        public async Task<IActionResult> Register()
        {
            ViewBag.Sports = await _context.Sports
                .OrderBy(s => s.SportName)
                .ToListAsync();

            return View(new RegisterViewModel());
        }


        // =========================================================
        // POST: /Account/Register
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            // Validate form
            if (!ModelState.IsValid)
            {
                ViewBag.Sports = await _context.Sports
                    .OrderBy(s => s.SportName)
                    .ToListAsync();

                return View(model);
            }


            // =====================================================
            // Check if email already exists
            //
            // DO NOT use AnyAsync() here with your current
            // Oracle EF Core configuration.
            // =====================================================

            var existingMember = await _context.Members
                .FirstOrDefaultAsync(m => m.Email == model.Email);

            if (existingMember != null)
            {
                ModelState.AddModelError(
                    nameof(model.Email),
                    "An account with this email already exists."
                );

                ViewBag.Sports = await _context.Sports
                    .OrderBy(s => s.SportName)
                    .ToListAsync();

                return View(model);
            }


            // =====================================================
            // Create Member
            // =====================================================

            var member = new Member
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Phone = model.Phone,
                Address = model.Address,
                RegistrationDate = DateTime.Now
            };


            // =====================================================
            // Hash password
            // =====================================================

            member.PasswordHash =
                _passwordHasher.HashPassword(
                    member,
                    model.Password
                );


            // =====================================================
            // Save Member
            // =====================================================

            _context.Members.Add(member);

            await _context.SaveChangesAsync();


            // =====================================================
            // Save Preferred Sports
            // =====================================================

            if (model.PreferredSportIds != null &&
                model.PreferredSportIds.Any())
            {
                foreach (var sportId in model.PreferredSportIds)
                {
                    var memberSport = new MemberSport
                    {
                        MemberID = member.MemberID,
                        SportID = sportId
                    };

                    _context.MemberSports.Add(memberSport);
                }

                await _context.SaveChangesAsync();
            }


            // =====================================================
            // Registration successful
            // =====================================================

            TempData["Success"] =
                "Registration successful. Please sign in.";

            return RedirectToAction(nameof(Login));
        }


        // =========================================================
        // GET: /Account/Login
        // =========================================================
        [HttpGet]
        public IActionResult Login()
        {
            return View(new LoginViewModel());
        }


        // =========================================================
        // POST: /Account/Login
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }


            // =====================================================
            // Find member by email
            // =====================================================

            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.Email == model.Email);


            if (member == null)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email or password."
                );

                return View(model);
            }


            // =====================================================
            // Verify password
            // =====================================================

            var result =
                _passwordHasher.VerifyHashedPassword(
                    member,
                    member.PasswordHash,
                    model.Password
                );


            if (result == PasswordVerificationResult.Failed)
            {
                ModelState.AddModelError(
                    string.Empty,
                    "Invalid email or password."
                );

                return View(model);
            }


            // =====================================================
            // Store logged-in member in Session
            // =====================================================

            HttpContext.Session.SetInt32(
                "MemberID",
                member.MemberID
            );

            HttpContext.Session.SetString(
                "MemberName",
                $"{member.FirstName} {member.LastName}"
            );


            return RedirectToAction(
                "Index",
                "Home"
            );
        }


        // =========================================================
        // POST: /Account/Logout
        // =========================================================
        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();

            return RedirectToAction(
                "Index",
                "Home"
            );
        }
    }
}