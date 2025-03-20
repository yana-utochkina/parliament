using System.ComponentModel.DataAnnotations;

namespace ParliamentDomain.Model;

public partial class UserEventDetail : Entity
{
    [Display(Name = "Подія")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public int EventId { get; set; }

    [Display(Name = "Студент")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public int UserId { get; set; }

    [Display(Name = "Роль")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string Role { get; set; }

    [Display(Name = "Оцінка")]
    public short? Rating { get; set; }

    public Event Event { get; set; }
    public User User { get; set; }
}
