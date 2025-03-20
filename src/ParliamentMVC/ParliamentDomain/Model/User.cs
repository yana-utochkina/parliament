using System.ComponentModel.DataAnnotations;

namespace ParliamentDomain.Model;

public partial class User : Entity
{
    [Display(Name = "Електронна пошта")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string Email {  get; set; }

    [Display(Name = "Повне ім'я")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string FullName { get; set; }

    [Display(Name = "Університет")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string University { get; set; }

    [Display(Name = "Факультет")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string Faculty { get; set; }

    public List<UserDepartmentDetail> UserDepartmentDetails { get; set; }
    public List<UserEventDetail> UserEventDetails { get; set; }
}
