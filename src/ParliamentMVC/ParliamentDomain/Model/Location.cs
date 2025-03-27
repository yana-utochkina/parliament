using System.ComponentModel.DataAnnotations;

namespace ParliamentDomain.Model;

public partial class Location : Entity
{
    [Display(Name = "Назва")]
    public string? Name { get; set; }

    [Display(Name = "Адреса")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string Address { get; set; }

    [Display(Name = "Контакт")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public int ContactId { get; set; }

    [Display(Name = "Посилання на GoogleMaps")]
    public string? GoogleMapsLink { get; set; }

    [Display(Name = "Контакт")]
    public virtual Contact Contact { get; set; } = null!;

    [Display(Name = "Події")]
    public virtual ICollection<Event> Events { get; set; } = new List<Event>();
}
