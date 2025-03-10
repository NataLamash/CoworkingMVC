using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using System.Linq;
using System.Threading.Tasks;
using CoworkingDomain.Model;

namespace CoworkingInfrastructure.Controllers
{
        [Authorize(Roles = "admin")]
        public class UsersController : Controller
        {
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<User> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Отримуємо поточного користувача
            var currentUser = await _userManager.GetUserAsync(User);
            var currentUserId = currentUser?.Id; // може бути null, якщо неавторизований (але тоді доступу й так немає)

            // 2. Беремо всіх користувачів, крім себе
            var allUsers = _userManager.Users
                .Where(u => u.Id != currentUserId) // виключаємо поточного адміна
                .ToList();

            // 3. Формуємо список UserViewModel
            var userList = new List<UserViewModel>();
            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Roles = roles.ToList() // якщо у користувача кілька ролей, зберігаємо всі
                });
            }

            return View(userList);
        }

        [HttpPost]
        public async Task<IActionResult> Delete(string userId)
        {
            // Видалити користувача
            var user = await _userManager.FindByIdAsync(userId);
            if (user != null)
            {
                await _userManager.DeleteAsync(user);
            }
            return RedirectToAction("Index");
        }

        [HttpPost]
        public async Task<IActionResult> EditRoles(string userId, List<string> roles)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            // Які ролі вже є в користувача
            var userRoles = await _userManager.GetRolesAsync(user);

            // Список ролей, які було додано
            var addedRoles = roles.Except(userRoles);
            // Список ролей, які було видалено
            var removedRoles = userRoles.Except(roles);

            await _userManager.AddToRolesAsync(user, addedRoles);
            await _userManager.RemoveFromRolesAsync(user, removedRoles);

            return RedirectToAction("Index");
        }

        [HttpGet]
        public async Task<IActionResult> EditRoles(string userId)
        {
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null) return NotFound();

            var userRoles = await _userManager.GetRolesAsync(user);
            var allRoles = _roleManager.Roles.ToList();

            var model = new ChangeRoleViewModel
            {
                UserId = user.Id,
                UserEmail = user.Email,
                UserRoles = userRoles,
                AllRoles = allRoles
            };
            return View(model); // шукатиме Views/Users/EditRoles.cshtml
        }

    }
}
