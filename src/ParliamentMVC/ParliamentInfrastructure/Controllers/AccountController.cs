using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParliamentDomain.Model;
using ParliamentInfrastructure.Models;
using ParliamentInfrastructure.ViewModels;

namespace ParliamentInfrastructure.Controllers;
public class AccountController : Controller
{
    private readonly UserManager<DefaultUser> _userManager;
    private readonly SignInManager<DefaultUser> _signInManager;

    private readonly ParliamentDbContext _context;

    public AccountController(
        UserManager<DefaultUser> userManager, 
        SignInManager<DefaultUser> signInManager, 
        ParliamentDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _context = context;
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
            DefaultUser defaultUser = new DefaultUser 
            { 
                Email = model.Email,    
                UserName = model.Email 
            };
            var result = await _userManager.CreateAsync(defaultUser, model.Password);

            defaultUser.EmailConfirmed = true;
            await _userManager.UpdateAsync(defaultUser);
            if (result.Succeeded)
            {
                await _signInManager.SignInAsync(defaultUser, false);
                User user = new User
                {
                    Email = model.Email,
                    FullName = model.FullName,
                    Faculty = model.Faculty,
                    University = model.University
                };

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
    public async Task<IActionResult> Login([Bind("Email,Password,RememberMe,ReturnUrl")] LoginViewModel model)
    {
        var user = await _userManager.FindByEmailAsync(model.Email);
        user.EmailConfirmed = true;
        await _userManager.UpdateAsync(user);
        if (user == null)
        {
            ModelState.AddModelError(string.Empty, "Користувач не знайдений.");
            return View(model);
        }

        var result = await _signInManager.PasswordSignInAsync(user, model.Password, model.RememberMe, false);
        if (result.Succeeded)
        {
            return RedirectToAction("Index", "Home");
        }
        else
        {
            ModelState.AddModelError(string.Empty, "Невірний email або пароль.");
            return View(model);
        }
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await _signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }


    public async Task<IActionResult> Profile()
    {
        try
        {
            var currentUser = await _userManager.GetUserAsync(User);
            var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == currentUser.Email);
            var profile = new ProfileViewModel(user);
            return View(profile);
        }
        catch (Exception ex) 
        {
            return NotFound();
        }
    }

    [HttpPost]
    public async Task<IActionResult> Edit([Bind("Email,FullName,University,Faculty")] ProfileViewModel model)
    {
        var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
        user.FullName = model.FullName;
        user.University = model.University;
        user.Faculty = model.Faculty;

        _context.Update(user);
        _context.SaveChanges();
        return RedirectToAction("Profile", "Account");
    }
}

