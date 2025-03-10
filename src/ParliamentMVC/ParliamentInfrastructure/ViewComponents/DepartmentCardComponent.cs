using Microsoft.AspNetCore.Mvc;
using ParliamentInfrastructure.Models;

namespace ParliamentInfrastructure.ViewComponents
{
    [ViewComponent]
    public class DepartmentCardComponent : ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync(DepartmentCardModel model)
        {
            return View(model);
        }
    }
}
