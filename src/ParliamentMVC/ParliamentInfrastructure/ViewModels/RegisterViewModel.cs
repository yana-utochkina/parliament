using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Security.Cryptography;

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

    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Compare("Password", ErrorMessage =  "Паролі не співпадають")]
    [DisplayName("Підтвердження паролю")]
    [DataType(DataType.Password)]
    public string PasswordConfirm { get; set; }

    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [DisplayName("Ім'я повністю")]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [DisplayName("Університет")]
    public string University {  get; set; }

    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [DisplayName("Факультет")]
    public string Faculty {  get; set; }


    public static string GenerateRandomPassword(int length = 12)
    {
        const string validChars = "ABCDEFGHJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@$?_-";
        char[] password = new char[length];
        using (var rng = RandomNumberGenerator.Create())
        {
            byte[] uintBuffer = new byte[sizeof(uint)];

            for (int i = 0; i < length; i++)
            {
                rng.GetBytes(uintBuffer);
                uint num = BitConverter.ToUInt32(uintBuffer, 0);
                password[i] = validChars[(int)(num % (uint)validChars.Length)];
            }
        }

        return new string(password);
    }

}
