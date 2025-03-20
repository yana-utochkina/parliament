using System.ComponentModel.DataAnnotations;

namespace ParliamentDomain.Model;

public partial class Event : Entity
{
    [Display(Name = "Адреса")]
    public int? LocationId { get; set; }

    [Display(Name = "Департамент")]
    public int? DepartmentId { get; set; }

    [Display(Name = "Назва події")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string Title { get; set; }

    [Display(Name = "Хто може прийти?")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string AccessType { get; set; }

    [Display(Name = "Час початку")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public DateTime StartDate { get; set; }

    [Display(Name = "Час завершення")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public DateTime EndDate { get; set; }

    [Display(Name = "Опис події")]
    [Required(ErrorMessage = "Поле не повинно бути порожнім")]
    public string Description { get; set; }

    [Display(Name = "Департамент")]
    public virtual Department? Department { get; set; }

    [Display(Name = "Місце")]
    public virtual Location? Location { get; set; }
    public virtual List<UserEventDetail> UserEventDetails { get; set; } = new List<UserEventDetail>();
}
