using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using CoworkingDomain.Model;
using CoworkingInfrastructure.ViewModels;
using System.Linq;
using System.Threading.Tasks;
using System.Collections.Generic;

namespace CoworkingInfrastructure.Controllers
{
    [Authorize]
    public class UserProfileController : Controller
    {
        private readonly CoworkingDbContext _context;
        private readonly UserManager<User> _userManager;

        public UserProfileController(CoworkingDbContext context, UserManager<User> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null)
                return Challenge();

            // Завантаження бронювань поточного користувача із включенням пов'язаного CoworkingSpace
            var myBookings = await _context.Bookings
                .Include(b => b.CoworkingSpace)
                .Where(b => b.UserId == currentUser.Id)
                .OrderByDescending(b => b.StartTime)
                .ToListAsync();

            // Якщо поточний користувач є адміністратором, завантажуємо усі бронювання
            List<Booking> allBookings = new List<Booking>();
            if (User.IsInRole("admin"))
            {
                allBookings = await _context.Bookings
                    .Include(b => b.CoworkingSpace)
                    .OrderByDescending(b => b.StartTime)
                    .ToListAsync();
            }

            var model = new UserProfileViewModel
            {
                MyBookings = myBookings,
                AllBookings = allBookings
            };

            return View(model);
        }
    }
}
