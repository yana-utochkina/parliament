using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace ParliamentInfrastructure.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [DisplayName("Електронна пошта")]
    [DataType(DataType.EmailAddress)]
    public string Email { get; set; }

    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [DisplayName("Пароль")]
    [DataType(DataType.Password)]
    public string Password { get; set; }

    [Required]
    [Compare("Password", ErrorMessage =  "Паролі не співпадають")]
    [DisplayName("Підтвердження паролю")]
    [DataType(DataType.Password)]
    public string PasswordConfirm { get; set; }

    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [DisplayName("Ім'я повністю")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Полен не повинно бути порожнім")]
    [DisplayName("Університет")]
    public string University {  get; set; }

    [Required(ErrorMessage = "Полен не повинно бути порожнім")]
    [DisplayName("Факультет")]
    public string Faculty {  get; set; }
}
