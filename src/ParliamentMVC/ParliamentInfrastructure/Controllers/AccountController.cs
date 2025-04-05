using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParliamentDomain.Model;
using ParliamentInfrastructure.Models;
using ParliamentInfrastructure.ViewModels;
using System.Net.Mail;
using System.Net;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.WebUtilities;
using System.Text;

namespace ParliamentInfrastructure.Controllers;
public class AccountController : Controller
{
    private readonly UserManager<DefaultUser> _userManager;
    private readonly SignInManager<DefaultUser> _signInManager;
    private readonly SmtpSettings _smtpSettings;

    private readonly ParliamentDbContext _context;

    public AccountController(
        UserManager<DefaultUser> userManager, 
        SignInManager<DefaultUser> signInManager,
        IOptions<SmtpSettings> smtpSettings,
        ParliamentDbContext context)
    {
        _userManager = userManager;
        _signInManager = signInManager;
        _smtpSettings = smtpSettings.Value;
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

            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(defaultUser);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                var confirmationLink = Url.Action("ConfirmEmail", "Account",
                    new { userId = defaultUser.Id, token = encodedToken }, Request.Scheme);


                await SendEmailAsync(defaultUser.Email, "Confirm your email", confirmationLink);

                TempData["FullName"] = model.FullName;
                TempData["Faculty"] = model.Faculty;
                TempData["University"] = model.University;
                TempData["Email"] = model.Email;

                return View("RegisterConfirmation");


                //User user = new User
                //{
                //    Email = model.Email,
                //    FullName = model.FullName,
                //    Faculty = model.Faculty,
                //    University = model.University
                //};

                //await _context.Users.AddAsync(user);
                //await _context.SaveChangesAsync();

                //return RedirectToAction("Index", "Home");
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

    public async Task SendEmailAsync(string email, string subject, string message)
    {
        var smtpClient = new SmtpClient(_smtpSettings.Host)
        {
            Port = 587,
            Credentials = new NetworkCredential(_smtpSettings.Username, _smtpSettings.Password),
            EnableSsl = _smtpSettings.EnableSSL,
        };

        var mailMessage = new MailMessage
        {
            From = new MailAddress(_smtpSettings.Username),
            Subject = subject,
            Body = message,
            IsBodyHtml = true,
        };
        mailMessage.To.Add(email);

        await smtpClient.SendMailAsync(mailMessage);
    }

    [AllowAnonymous]
    public async Task<IActionResult> ConfirmEmail(string userId, string token)
    {
        if (userId == null || token == null)
            return View("Error");

        var user = await _userManager.FindByIdAsync(userId);
        if (user == null)
            return View("Error");

        var decodedToken = Encoding.UTF8.GetString(WebEncoders.Base64UrlDecode(token));
        var result = await _userManager.ConfirmEmailAsync(user, decodedToken);

        if (result.Succeeded)
        {
            var fullName = TempData["FullName"]?.ToString() ?? "NoName";
            var faculty = TempData["Faculty"]?.ToString() ?? "Unknown";
            var university = TempData["University"]?.ToString() ?? "Unknown";

            var fullUser = new User
            {
                Email = user.Email,
                FullName = fullName,
                Faculty = faculty,
                University = university
            };

            _context.Users.Add(fullUser);
            await _context.SaveChangesAsync();

            return View("ConfirmEmailSuccess");
        }

        return View("Error");
    }



    [HttpGet]
    public IActionResult Login(string returnUrl = null)
    {
        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = await _userManager.FindByEmailAsync(model.Email);
            if (user != null)
            {
                if (!await _userManager.IsEmailConfirmedAsync(user))
                {
                    ModelState.AddModelError("", "You need to confirm your email before logging in.");
                    return View(model);
                }

                var result = await _signInManager.PasswordSignInAsync(model.Email, model.Password, model.RememberMe, false);
                if (result.Succeeded)
                {
                    return RedirectToAction("Index", "Home");
                }
            }
            ModelState.AddModelError("", "Invalid login attempt.");
        }
        else
        {
            // Якщо ModelState не дійсна, вивести помилки
            foreach (var error in ModelState.Values.SelectMany(v => v.Errors))
            {
                Console.WriteLine(error.ErrorMessage);
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

    public IActionResult RegisterConfirmation()
    {
        return View();
    }

}

