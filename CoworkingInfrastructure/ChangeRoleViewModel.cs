using Microsoft.AspNetCore.Identity;

namespace CoworkingInfrastructure
{
    public class ChangeRoleViewModel
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public IList<string> UserRoles { get; set; }
        public List<IdentityRole> AllRoles { get; set; }

        public ChangeRoleViewModel()
        {
            UserRoles = new List<string>();
            AllRoles = new List<IdentityRole>();
        }
    }
}
