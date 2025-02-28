using System.ComponentModel.DataAnnotations;

namespace ParliamentDomain.Model;

public partial class Contact : Entity
{
    [Display(Name = "Номер телефону")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    [Display(Name = "Електронна пошта")]
    public string Email { get; set; } = null!;

    [Display(Name = "Посилання на інстаграм")]
    public string? InstagramLink { get; set; }

    [Display(Name = "Департаменти")]
    public virtual ICollection<Department> Departments { get; set; } = new List<Department>();

    [Display(Name = "Локації")]
    public virtual ICollection<Location> Locations { get; set; } = new List<Location>();
}
