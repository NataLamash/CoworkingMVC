using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using CoworkingDomain.Model;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CoworkingInfrastructure;
using CoworkingInfrastructure.ViewModels.Bookings;

namespace CoworkingInfrastructure.Controllers
{
    [Authorize]
    public class BookingsController : Controller
    {
        private readonly CoworkingDbContext _context;
        private readonly UserManager<User> _userManager;

        public BookingsController(CoworkingDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Bookings/MyBookings
        public async Task<IActionResult> MyBookings()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var myBookings = _context.Bookings
                .Where(b => b.UserId == currentUser.Id)
                .OrderByDescending(b => b.StartTime)
                .ToList();
            return View(myBookings);
        }

        // GET: Bookings (admin-only)
        [Authorize(Roles = "admin")]
        public IActionResult Index()
        {
            var allBookings = _context.Bookings
                .OrderByDescending(b => b.StartTime)
                .ToList();
            return View(allBookings);
        }

        // GET: Bookings/Create?coworkingSpaceId=...
        public IActionResult Create(int coworkingSpaceId)
        {
            // Можна зберегти coworkingSpaceId у ViewBag або в окремій моделі
            ViewBag.CoworkingSpaceId = coworkingSpaceId;
            return View();

        }

        // POST: Bookings/Create
        [HttpPost]
        public async Task<IActionResult> Create(BookingCreateViewModel model)
        {
            if (!ModelState.IsValid)
                return View(model);

            var coworkingSpace = _context.CoworkingSpaces.Find(model.CoworkingSpaceId);
            if (coworkingSpace == null)
            {
                ModelState.AddModelError("", "CoworkingSpace not found.");
                return View(model);
            }

            var overlap = _context.Bookings.Any(b =>
                b.CoworkingSpaceId == model.CoworkingSpaceId &&
                // Перевірка, чи інтервал [StartTime, EndTime] перетинається
                !(b.EndTime <= model.StartTime || b.StartTime >= model.EndTime)
);
            if (overlap)
            {
                ModelState.AddModelError("", "This coworking space is already booked in the selected time range.");
                return View(model);
            }
            // Отримуємо поточного користувача
            var currentUser = await _userManager.GetUserAsync(User);

            // Обчислюємо тривалість (у годинах)
            var durationHours = (model.EndTime - model.StartTime).TotalHours;
            if (durationHours <= 0)
            {
                ModelState.AddModelError("", "Invalid time range.");
                return View(model);
            }

            // Формуємо бронювання
            var booking = new Booking
            {
                UserId = currentUser.Id,
                CoworkingSpaceId = model.CoworkingSpaceId,
                StartTime = model.StartTime,
                EndTime = model.EndTime,
                Duration = (int)durationHours, // або з точністю до хвилин
                Status = "Pending"
            };

            // Підрахунок TotalPrice
            decimal basePrice = coworkingSpace.HourlyRate * (decimal)durationHours;
            // Якщо треба додати додаткові послуги:
            // booking.BookingsFacilities = ...
            booking.TotalPrice = basePrice;

            // Зберігаємо
            _context.Bookings.Add(booking);
            _context.SaveChanges();

            return RedirectToAction("MyBookings");
        }

        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var booking = _context.Bookings.Find(id);
            if (booking == null)
                return NotFound();

            // Якщо це не адмін і не власник бронювання - 403
            var currentUser = await _userManager.GetUserAsync(User);
            if (!User.IsInRole("admin") && booking.UserId != currentUser.Id)
            {
                return Forbid();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5 (admin-only або власник)
        [HttpPost]
        [Authorize(Roles = "admin")]
        public IActionResult Delete(int id)
        {
            var booking = _context.Bookings.Find(id);
            if (booking != null)
            {
                _context.Bookings.Remove(booking);
                _context.SaveChanges();
            }
            return RedirectToAction("Index");
        }
    }
}
