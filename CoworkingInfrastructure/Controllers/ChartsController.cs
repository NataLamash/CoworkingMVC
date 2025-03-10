using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Identity;
using CoworkingDomain.Model;

namespace CoworkingInfrastructure.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ChartsController : ControllerBase
    {
        private record UsersByRoleResponseItem(string Role, int Count);

        private readonly UserManager<User> _userManager;

        public ChartsController(UserManager<User> userManager)
        {
            _userManager = userManager;
        }

        [HttpGet("usersByRole")]
        public async Task<JsonResult> GetUsersByRoleAsync()
        {
            var responseItems = new List<UsersByRoleResponseItem>();
            var roles = new[] { "admin", "user" };

            foreach (var role in roles)
            {
                var usersInRole = await _userManager.GetUsersInRoleAsync(role);
                responseItems.Add(new UsersByRoleResponseItem(role, usersInRole.Count));
            }

            return new JsonResult(responseItems);
        }
    }
}
