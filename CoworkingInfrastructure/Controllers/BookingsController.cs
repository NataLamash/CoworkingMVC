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
                .Include(b => b.CoworkingSpace) // Ось тут підвантажуємо дані про коворкінг
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

            // Знаходимо коворкінг
            var coworkingSpace = await _context.CoworkingSpaces.FindAsync(model.CoworkingSpaceId);
            if (coworkingSpace == null)
            {
                ModelState.AddModelError("", "CoworkingSpace not found.");
                return View(model);
            }

            // Примітивна перевірка, що бронювання не в минулому
            DateTime now = DateTime.Now;

            DateTime startDateTime, endDateTime;
            double durationHours = 0;
            bool isSingleDay = model.BookingType == "SingleDay";

            if (isSingleDay)
            {
                // Одноденне бронювання: комбінуємо StartDate з рядковими StartTime і EndTime
                try
                {
                    // Парсимо рядки у TimeSpan
                    TimeSpan startTimeSpan = TimeSpan.Parse(model.StartTime);
                    TimeSpan endTimeSpan = TimeSpan.Parse(model.EndTime);

                    // Формуємо повні DateTime
                    startDateTime = model.StartDate.Date.Add(startTimeSpan);
                    endDateTime = model.StartDate.Date.Add(endTimeSpan);

                    // Перевірка: бронювання має починатися не в минулому
                    if (startDateTime < now)
                    {
                        ModelState.AddModelError("", "Booking start time cannot be in the past.");
                        return View(model);
                    }

                    // Перевірка: час початку < час завершення
                    if (endDateTime <= startDateTime)
                    {
                        ModelState.AddModelError("", "End time must be after start time.");
                        return View(model);
                    }

                    // Обмеження: доступний часовий проміжок тільки між 08:00 та 20:00
                    if (startTimeSpan < TimeSpan.FromHours(8))
                    {
                        ModelState.AddModelError("", "For single-day bookings, the start time must be no earlier than 08:00.");
                        return View(model);
                    }
                    if (endTimeSpan > TimeSpan.FromHours(20) || (endTimeSpan == TimeSpan.FromHours(20) && model.EndTime.Contains(":") && !model.EndTime.EndsWith(":00")))
                    {
                        ModelState.AddModelError("", "For single-day bookings, the end time must be no later than 20:00.");
                        return View(model);
                    }
                    // Переконаємося, що хвилини рівні нулю (чисто цілих годин)
                    if (startTimeSpan.Minutes != 0 || endTimeSpan.Minutes != 0)
                    {
                        ModelState.AddModelError("", "For single-day bookings, please select times on the full hour.");
                        return View(model);
                    }

                    durationHours = (endDateTime - startDateTime).TotalHours;
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Invalid time format. Please select valid times.");
                    return View(model);
                }
            }
            else if (model.BookingType == "MultipleDays")
            {
                // Для бронювання на декілька днів: EndDate має бути вказаний
                if (!model.EndDate.HasValue)
                {
                    ModelState.AddModelError("", "Please provide an end date for multiple-day bookings.");
                    return View(model);
                }

                // Формуємо час, автоматично встановлюючи:
                // Початок: 08:00 на даті StartDate; Завершення: 20:00 на даті EndDate.
                startDateTime = model.StartDate.Date.AddHours(8);
                endDateTime = model.EndDate.Value.Date.AddHours(20);

                // Перевірка: початок не в минулому
                if (startDateTime < now)
                {
                    ModelState.AddModelError("", "Booking start time cannot be in the past.");
                    return View(model);
                }
                if (endDateTime <= startDateTime)
                {
                    ModelState.AddModelError("", "End date must be after start date.");
                    return View(model);
                }

                // Обчислюємо кількість днів. Додаємо 1, щоб врахувати, що бронювання включає обидва дні.
                int days = (model.EndDate.Value.Date - model.StartDate.Date).Days + 1;
                durationHours = days * 12; // Кожен день має 12 годин (з 08:00 до 20:00)
            }
            else
            {
                ModelState.AddModelError("", "Invalid booking type.");
                return View(model);
            }

            if (durationHours <= 0)
            {
                ModelState.AddModelError("", "Invalid time range.");
                return View(model);
            }

            // Перевірка на перекриття бронювання
            bool overlap = _context.Bookings.Any(b =>
                b.CoworkingSpaceId == model.CoworkingSpaceId &&
                !(b.EndTime <= startDateTime || b.StartTime >= endDateTime)
            );
            if (overlap)
            {
                ModelState.AddModelError("", "This coworking space is already booked in the selected time range.");
                return View(model);
            }

            // Отримуємо поточного користувача
            var currentUser = await _userManager.GetUserAsync(User);

            // Створюємо бронювання
            var booking = new Booking
            {
                UserId = currentUser.Id,
                CoworkingSpaceId = model.CoworkingSpaceId,
                StartTime = startDateTime,
                EndTime = endDateTime,
                Duration = (int)durationHours,
                Status = "Pending"
            };

            // Обчислюємо вартість бронювання за погодинною ставкою
            decimal basePrice = coworkingSpace.HourlyRate * (decimal)durationHours;
            booking.TotalPrice = basePrice;

            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();

            return RedirectToAction("MyBookings");
        }



        // GET: Bookings/Details/5
        public async Task<IActionResult> Details(int id)
        {
            // Завантажуємо разом із CoworkingSpace
            var booking = _context.Bookings
                .Include(b => b.CoworkingSpace)
                .FirstOrDefault(b => b.Id == id);

            if (booking == null)
                return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (!User.IsInRole("admin") && booking.UserId != currentUser.Id)
            {
                return Forbid();
            }

            return View(booking);
        }

        // POST: Bookings/Delete/5
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
            return RedirectToAction("Index", "UserProfile");
        }
    }
}
