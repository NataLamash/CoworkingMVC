using Microsoft.AspNetCore.Identity;

namespace CoworkingInfrastructure
{
    public class ChangeRoleViewModel
    {
        public string UserId { get; set; }
        public string UserEmail { get; set; }
        public List<IdentityRole> AllRoles { get; set; } = new List<IdentityRole>();
        public IList<string> UserRoles { get; set; } = new List<string>();

        // Нове поле для збереження обраної ролі (тільки один варіант)
        public string SelectedRole { get; set; }
    }
}
