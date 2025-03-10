using Microsoft.AspNetCore.Mvc;
using ParliamentInfrastructure.Models;

namespace ParliamentInfrastructure.ViewComponents
{
    [ViewComponent]
    public class NewsCardComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(NewsCardModel model)
        {
            return View(model);
        }
    }
}
