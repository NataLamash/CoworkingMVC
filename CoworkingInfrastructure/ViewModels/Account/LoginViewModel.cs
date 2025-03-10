using System.ComponentModel.DataAnnotations;

namespace CoworkingInfrastructure.ViewModels
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Вкажіть Email")]
        [Display(Name = "Email")]
        public string? Email { get; set; }

        [Required(ErrorMessage = "Вкажіть пароль")]
        [DataType(DataType.Password)]
        [Display(Name = "Пароль")]
        public string? Password { get; set; }

        [Display(Name = "Запам'ятати мене")]
        public bool RememberMe { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
