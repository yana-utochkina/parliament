using System.ComponentModel.DataAnnotations;

namespace ParliamentDomain.Model;

public partial class News : Entity
{
    [Display(Name = "Видавець")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public int PublisherId { get; set; }

    [Display(Name = "Департамент")]
    public int? DepartmentId { get; set; }

    [Display(Name = "Заголовок новини")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string Title { get; set; } = null!;

    [Display(Name = "Дата публікації")]
    [Required(ErrorMessage = "Полене повинно бути порожнім")]
    public DateTime PublicationDate { get; set; }

    [Display(Name = "Текст новини")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string Description { get; set; } = null!;

    [Display(Name = "Департамент")]
    public virtual Department? Department { get; set; }
}
