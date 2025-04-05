using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ParliamentInfrastructure.ViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [DisplayName("Електронна пошта")]
    public string Email { get; set; }

    [Required(ErrorMessage = "Полен не повинно бути порожнім")]
    [DisplayName("Пароль")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [DisplayName("Запам'ятати?")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}
