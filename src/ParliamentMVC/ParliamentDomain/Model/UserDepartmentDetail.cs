using System.ComponentModel.DataAnnotations;

namespace ParliamentDomain.Model;

public partial class UserDepartmentDetail : Entity
{
    [Display(Name = "Працівник")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public int WorkerId { get; set; }

    [Display(Name = "Департамент")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public int DepartmentId { get; set; }

    [Display(Name = "Позиція")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string Position { get; set; } = null!;

    [Display(Name = "Доєднаний з")]
    public DateOnly JoinedAt { get; set; }
    public User User { get; set; }
    public Department Department { get; set; }
}
