using Microsoft.AspNetCore.Mvc;
using ParliamentInfrastructure.ViewModels;

namespace ParliamentInfrastructure.ViewComponents
{
    [ViewComponent]
    public class RegisterCardComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(RegisterViewModel model)
        {
            return View(model);
        }
    }
}
