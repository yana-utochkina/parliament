using Microsoft.AspNetCore.Mvc;
using ParliamentInfrastructure.Models;

namespace ParliamentInfrastructure.ViewComponents
{
    [ViewComponent]
    public class EventCardComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(EventCardViewModel model)
        {
            return View(model);
        }
    }
}
