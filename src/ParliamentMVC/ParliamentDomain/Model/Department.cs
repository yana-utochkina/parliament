using System.ComponentModel.DataAnnotations;

namespace ParliamentDomain.Model;

public partial class Department : Entity
{
    [Required(ErrorMessage="Поле не повинно бути порожнім")]
    [Display(Name="Назва департаменту")]
    public string Name { get; set; } = null!;

    [Display(Name="Інформація про департамент")]
    public string? Description { get; set; }

    [Display(Name="Електронна пошта")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public int ContactId { get; set; }

    [Display(Name = "Контакт")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public virtual Contact Contact { get; set; } = null!;

    [Display(Name = "Події")]
    public virtual ICollection<Event> Events { get; set; } = new List<Event>();

    [Display(Name = "Новини")]
    public virtual ICollection<News> News { get; set; } = new List<News>();
}
