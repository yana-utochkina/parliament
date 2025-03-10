using Microsoft.AspNetCore.Mvc;
using ParliamentInfrastructure.Models;

namespace ParliamentInfrastructure.ViewComponents
{
    [ViewComponent]
    public class EventCardComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(EventCardModel model)
        {
            return View(model);
        }
    }
}
