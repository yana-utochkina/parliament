using Microsoft.AspNetCore.Mvc;
using ParliamentDomain.Model;
using System.ComponentModel.DataAnnotations;

namespace ParliamentInfrastructure.ViewModels;

public class MyEventViewModel
{
    public int Id { get; set; }
    public int UserEventDetailId { get; set; }
    [Display(Name = "Назва")]
    public string Title { get; set; }

    [Display(Name = "Час початку")]
    public DateTime StartDate { get; set; }

    [Display(Name = "Оцінка")]
    public short? Rating { get; set; }
    public int? countUsers { get; set; }

    public Event Event { get; set; } // Подія, щоб мати доступ до її деталей
    public bool IsEventAvailableToJoin { get; set; } // Показує, чи можна приєднатися до події
}
