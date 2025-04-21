using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CoworkingDomain.Model;
using CoworkingInfrastructure.ViewModels;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CoworkingInfrastructure.ViewComponents
{
    public class UsersListViewComponent : ViewComponent
    {
        private readonly UserManager<User> _userManager;

        public UsersListViewComponent(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var currentUser = await _userManager.GetUserAsync(HttpContext.User);
            var currentUserId = currentUser?.Id;

            // Отримуємо всіх користувачів, крім поточного (якщо потрібно)
            var allUsers = _userManager.Users.Where(u => u.Id != currentUserId).ToList();
            var userList = new List<UserViewModel>();
            foreach (var user in allUsers)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userList.Add(new UserViewModel
                {
                    Id = user.Id,
                    Email = user.Email,
                    Roles = roles.ToList()
                });
            }

            return View(userList);
        }
    }
}
