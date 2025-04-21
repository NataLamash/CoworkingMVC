using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CoworkingDomain.Model;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;

namespace CoworkingInfrastructure.ViewComponents
{
    public class UserChartViewComponent : ViewComponent
    {
        [ViewComponent(Name = "UserChart")]
        public record UsersByRoleResponseItem(string Role, int Count);
        private readonly UserManager<User> _userManager;
        public UserChartViewComponent(UserManager<User> userManager)
        {
            _userManager = userManager;
        }
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var responseItems = new List<UsersByRoleResponseItem>();
            var roles = new[] { "admin", "user" };
            foreach (var role in roles)
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role);
                responseItems.Add(new UsersByRoleResponseItem(role, usersInRole.Count));
            }
            return View(responseItems);
        }
    }
}
