using System.ComponentModel.DataAnnotations;

namespace CoworkingInfrastructure.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Необхідно вказати Email")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Необхідно вказати ім'я користувача")]
        [Display(Name = "Ім'я користувача")]
        public string? UserName { get; set; }


        [Required(ErrorMessage = "Необхідно вказати пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string? Password { get; set; }

        [Required(ErrorMessage = "Необхідно підтвердити пароль")]
        [Compare("Password", ErrorMessage = "Паролі не співпадають")]
        [DataType(DataType.Password)]
        [Display(Name = "Підтвердження пароля")]
        public string? PasswordConfirm { get; set; }
    }
}
