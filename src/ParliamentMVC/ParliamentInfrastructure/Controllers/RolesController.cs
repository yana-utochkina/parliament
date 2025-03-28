using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ParliamentInfrastructure.Models;
using ParliamentInfrastructure.ViewModels;

namespace ParliamentInfrastructure.Controllers
{
    public class RolesController : Controller
    {
        RoleManager<IdentityRole> _roleManager;
        UserManager<DefaultUser> _userManager;

        public RolesController(RoleManager<IdentityRole> roleManager, UserManager<DefaultUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        [Authorize(Roles = "admin, worker")]
        public IActionResult Index() => View(_roleManager.Roles.ToList());
        [Authorize(Roles = "admin, worker")]
        public IActionResult UserList() => View(_userManager.Users.ToList());

        [Authorize(Roles = "admin, worker")]
        public async Task<IActionResult> Edit(string userId)
        {
            DefaultUser user = await _userManager.FindByIdAsync(userId);
            if (user is not null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = _roleManager.Roles.ToList();
                ChangeRoleViewModel model = new ChangeRoleViewModel
                {
                    UserId = user.Id,
                    UserEmail = user.Email,
                    UserRoles = userRoles,
                    AllRoles = allRoles
                };
                return View(model);
            }
            return NotFound();
        }

        [HttpPost]
        [Authorize(Roles = "admin, worker")]
        public async Task<IActionResult> Edit(string userId, List<string> roles)
        {
            DefaultUser user = await _userManager.FindByIdAsync(userId);
            if (user is not null)
            {
                var userRoles = await _userManager.GetRolesAsync(user);
                var allRoles = _roleManager.Roles.ToList();
                var addedRoles = roles.Except(userRoles);
                var removedRoles = userRoles.Except(roles);

                await _userManager.AddToRolesAsync(user, addedRoles);
                await _userManager.RemoveFromRolesAsync(user, removedRoles);

                return RedirectToAction("UserList");
            }
            return NotFound();
        }

    }
}
