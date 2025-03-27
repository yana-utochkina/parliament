using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using ParliamentDomain.Model;
using ParliamentInfrastructure.Models;
using ParliamentInfrastructure.ViewModels;

namespace ParliamentInfrastructure.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<DefaultUser> _userManager;
        private readonly SignInManager<DefaultUser> _signInManager;

        private readonly ParliamentDbContext _context;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AccountController(UserManager<DefaultUser> userManager, SignInManager<DefaultUser> signInManager, ParliamentDbContext context, IHttpContextAccessor httpContextAccessor)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
            _httpContextAccessor = httpContextAccessor;
        }

        [HttpGet]
        public IActionResult Register()
        {
            return View();
        }

        [HttpPost]
        public async Task<ActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                DefaultUser defaultUser = new DefaultUser { Email = model.Email, UserName = model.Email, };
                var result = await _userManager.CreateAsync(defaultUser, model.Password);
                if (result.Succeeded)
                {
                    await _signInManager.SignInAsync(defaultUser, false);
                    User user = new User { Email = model.Email, FullName = model.FullName, Faculty = model.Faculty, University = model.University };
                    user.Id = _userManager.FindByIdAsync(_userManager.GetUserId(_httpContextAccessor.HttpContext?.User)).Id;
                    await _context.Users.AddAsync(user);
                    await _context.SaveChangesAsync();
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
            }
            return View(model);
        }

        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
                    {
                        return Redirect(model.ReturnUrl);
                    }
                    else
                    {
                        return RedirectToAction("Index", "Home");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "Неправильна пошта чи (та) пароль");
                }
            }
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }
    }
}
