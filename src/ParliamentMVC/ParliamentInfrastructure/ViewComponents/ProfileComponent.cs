using Microsoft.AspNetCore.Mvc;
using ParliamentInfrastructure.ViewModels;

namespace ParliamentInfrastructure.ViewComponents
{
    [ViewComponent]
    public class ProfileComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(ProfileViewModel model)
        {
            return View(model);
        }
    }
}
