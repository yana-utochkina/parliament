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
using QRCoder;
using ParliamentInfrastructure.Models.QRCodePOC;
using System.Drawing.Imaging;
using DocumentFormat.OpenXml.Drawing;
using DocumentFormat.OpenXml.Drawing.Wordprocessing;
using PdfSharpCore.Drawing;
using PdfSharpCore.Pdf;
using Picture = DocumentFormat.OpenXml.Drawing.Picture;

using NonVisualGraphicFrameDrawingProperties = DocumentFormat.OpenXml.Drawing.NonVisualGraphicFrameDrawingProperties;
using DocumentFormat.OpenXml.Wordprocessing;
using DocumentFormat.OpenXml;


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

    [HttpGet]
    public IActionResult AddUser()
    {
        return View();
    }
    [HttpPost]
    public async Task<IActionResult> AddUser(RegisterViewModel model)
    {
        if (ModelState.IsValid)
        {
            var user = new DefaultUser
            {
                Email = model.Email,
                UserName = model.Email
            };

            var result = await _userManager.CreateAsync(user, model.Password);

            if (result.Succeeded)
            {
                var token = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var encodedToken = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(token));
                var confirmationLink = Url.Action("ConfirmEmail", "Account",
                    new { userId = user.Id, token = encodedToken }, Request.Scheme);

                // Generate QR code
                byte[] qrCodeBytes;
                using (var qrGenerator = new QRCodeGenerator())
                {
                    var qrData = qrGenerator.CreateQrCode(confirmationLink, QRCodeGenerator.ECCLevel.Q);
                    using (var qrCode = new QRCode(qrData))
                    using (var bitmap = qrCode.GetGraphic(20))
                    using (var ms = new MemoryStream())
                    {
                        bitmap.Save(ms, ImageFormat.Png);
                        qrCodeBytes = ms.ToArray();
                    }
                }

                // Create PDF
                using (var stream = new MemoryStream())
                {
                    var document = new PdfDocument();
                    var page = document.AddPage();
                    var gfx = XGraphics.FromPdfPage(page);
                    var font = new XFont("Arial", 14, XFontStyle.Regular);

                    gfx.DrawString("Інформація для підтвердження акаунту", font, XBrushes.Black,
                        new XRect(0, 20, page.Width, 20), XStringFormats.TopCenter);

                    gfx.DrawString($"Ім'я: {model.FullName}", font, XBrushes.Black,
                        new XRect(20, 60, page.Width, 20), XStringFormats.TopLeft);

                    gfx.DrawString($"Email: {model.Email}", font, XBrushes.Black,
                        new XRect(20, 90, page.Width, 20), XStringFormats.TopLeft);

                    gfx.DrawString("Скануйте QR-код:", font, XBrushes.Black,
                        new XRect(20, 130, page.Width, 20), XStringFormats.TopLeft);

                    using (var imageStream = new MemoryStream(qrCodeBytes))
                    {
                        var qrImage = XImage.FromStream(() => imageStream);
                        gfx.DrawImage(qrImage, 20, 160, 150, 150);
                    }

                    document.Save(stream);
                    stream.Position = 0;

                    return File(stream.ToArray(), "application/pdf", "confirmation.pdf");
                }
            }

            foreach (var error in result.Errors)
                ModelState.AddModelError(string.Empty, error.Description);
        }

        return View(model);
    }

    private Drawing GetImageElement(string relationshipId, string name, long width, long height)
    {
        var drawing = new Drawing(
            new Inline(
                new Extent() { Cx = width * 9525, Cy = height * 9525 },
                new EffectExtent()
                {
                    LeftEdge = 0L,
                    TopEdge = 0L,
                    RightEdge = 0L,
                    BottomEdge = 0L
                },
                new DocProperties() { Id = (UInt32Value)1U, Name = name },
                new NonVisualGraphicFrameDrawingProperties(new GraphicFrameLocks() { NoChangeAspect = true }),
                new Graphic(
                    new GraphicData(
                        new Picture(
                            new NonVisualPictureProperties(
                                new NonVisualDrawingProperties()
                                {
                                    Id = (UInt32Value)0U,
                                    Name = name
                                },
                                new NonVisualPictureDrawingProperties()
                            ),
                            new BlipFill(
                                new Blip() { Embed = relationshipId, CompressionState = BlipCompressionValues.Print },
                                new Stretch(new FillRectangle())
                            ),
                            new ShapeProperties(
                                new Transform2D(
                                    new Offset() { X = 0L, Y = 0L },
                                    new Extents() { Cx = width * 9525, Cy = height * 9525 }
                                ),
                                new PresetGeometry(new AdjustValueList()) { Preset = ShapeTypeValues.Rectangle }
                            )
                        )
                    )
                    { Uri = "http://schemas.openxmlformats.org/drawingml/2006/picture" }
                )
            )
            {
                DistanceFromTop = (UInt32Value)0U,
                DistanceFromBottom = (UInt32Value)0U,
                DistanceFromLeft = (UInt32Value)0U,
                DistanceFromRight = (UInt32Value)0U
            }
        );

        return drawing;
    }


    public byte[] GenerateQrCode(string confirmationLink)
    {
        using var qrGenerator = new QRCodeGenerator();
        using var qrData = qrGenerator.CreateQrCode(confirmationLink, QRCodeGenerator.ECCLevel.Q);
        using var qrCode = new QRCode(qrData);
        using var bitmap = qrCode.GetGraphic(20);
        using var stream = new MemoryStream();

        bitmap.Save(stream, ImageFormat.Png);
        return stream.ToArray(); // Це масив байтів PNG-файлу
    }
}

