using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ParliamentDomain.Model;
using ParliamentInfrastructure.Models;
using ParliamentInfrastructure.ViewModels;


namespace ParliamentInfrastructure.Controllers;

public class MyEventsController : Controller
{
    private readonly UserManager<DefaultUser> _userManager;
    private readonly ParliamentDbContext _context;

    public MyEventsController(
    UserManager<DefaultUser> userManager,
    ParliamentDbContext context)
    {
        _userManager = userManager;
        _context = context;
    }

    public async Task<IActionResult> Index()
    {
        var email = _userManager.GetUserName(User);
        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

        var userEventDetails = await _context.UserEventDetails
                                              .Where(u => u.UserId == user.Id)
                                              .Include(e => e.Event)
                                              .ToListAsync();

        var myEvents = new List<MyEventViewModel>();

        foreach (var item in userEventDetails)
        {
            var myEvent = new MyEventViewModel
            {
                Id = item.EventId,
                UserEventDetailId = item.Id,
                Rating = item.Rating,
                StartDate = item.Event.StartDate,
                countUsers = _context.UserEventDetails.Count(t => t.EventId == item.EventId),
                Event = item.Event
            };
            myEvents.Add(myEvent);
        }

        return View(myEvents);
    }


    [HttpPost]
    public async Task<IActionResult> WishToGo(int eventId)
    {
        var email = _userManager.GetUserName(User);
        if (email is null)
        {
            return RedirectToAction("Login", "Account");
        }

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

        var existingEntry = await _context.UserEventDetails
                                           .FirstOrDefaultAsync(x => x.UserId == user.Id && x.EventId == eventId);

        if (existingEntry != null)
        {
            ModelState.AddModelError("", "Ви вже зареєстровані на цю подію.");
            return RedirectToAction("Index");
        }

        var userEventDetail = new UserEventDetail
        {
            EventId = eventId,
            UserId = user.Id,
            Role = "guest"
        };

        await _context.UserEventDetails.AddAsync(userEventDetail);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }

    [HttpPost]
    public async Task<IActionResult> CancelRegistration(int eventId)
    {
        var email = _userManager.GetUserName(User);
        if (email is null)
        {
            return RedirectToAction("Login", "Account");
        }

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

        // Знайдемо запис про реєстрацію користувача на подію
        var existingEntry = await _context.UserEventDetails
                                           .FirstOrDefaultAsync(x => x.UserId == user.Id && x.EventId == eventId);

        if (existingEntry == null)
        {
            ModelState.AddModelError("", "Ви не зареєстровані на цю подію.");
            return RedirectToAction("Index");
        }

        // Якщо є оцінка, видаляємо її
        if (existingEntry.Rating.HasValue)
        {
            existingEntry.Rating = null;
        }

        // Видаляємо реєстрацію
        _context.UserEventDetails.Remove(existingEntry);
        await _context.SaveChangesAsync();

        return RedirectToAction("Index");
    }


    [HttpGet]
    public async Task<IActionResult> RateEvent(int eventId)
    {
        var email = _userManager.GetUserName(User);
        if (email == null)
        {
            return RedirectToAction("Login", "Account");
        }

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

        // Перевіряємо, чи користувач зареєстрований на цю подію
        var userEventDetail = await _context.UserEventDetails
                                             .FirstOrDefaultAsync(x => x.UserId == user.Id && x.EventId == eventId);

        // Отримуємо подію
        var eventDetails = await _context.Events
                                          .Include(e => e.UserEventDetails)
                                          .FirstOrDefaultAsync(e => e.Id == eventId);

        if (eventDetails == null)
        {
            return NotFound();
        }

        // Перевірка на наявність оцінок
        double averageRating = 0;
        int ratingCount = eventDetails.UserEventDetails.Count(d => d.Rating.HasValue);

        if (ratingCount > 0)
        {
            averageRating = eventDetails.UserEventDetails
                                        .Where(d => d.Rating.HasValue)
                                        .Average(d => d.Rating.Value);
        }

        var viewModel = new EventRatingViewModel
        {
            EventId = eventId,
            EventTitle = eventDetails.Title,
            EventDate = eventDetails.StartDate,
            AverageRating = averageRating,
            RatingCount = ratingCount,
            UserRating = userEventDetail?.Rating ?? 0 // Якщо користувач не має оцінки, то 0
        };

        return View(viewModel);
    }


    [HttpPost]
    [Authorize(Roles = "admin, worker, student, guest")]
    public async Task<IActionResult> RateEvent(EventRatingViewModel model)
    {
        if (!ModelState.IsValid)
        {
            return View(model);
        }

        var email = _userManager.GetUserName(User);
        if (email is null)
        {
            return RedirectToAction("Login", "Account");
        }

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

        if (user is not null)
        {
            var userEventDetail = await _context.UserEventDetails
                                             .FirstOrDefaultAsync(x => x.UserId == user.Id && x.EventId == model.EventId);
            if (userEventDetail != null)
            {
                userEventDetail.Rating = model.UserRating;
                _context.UserEventDetails.Update(userEventDetail);
                await _context.SaveChangesAsync();
            }
        }
        return RedirectToAction("Index", "MyEvents");
    }

    [HttpPost]
    public async Task<IActionResult> CancelRating(int eventId)
    {
        var email = _userManager.GetUserName(User);
        if (email is null)
        {
            return RedirectToAction("Login", "Account");
        }

        var user = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);

        // Знайдемо запис про реєстрацію користувача на подію
        var userEventDetail = await _context.UserEventDetails
                                             .FirstOrDefaultAsync(x => x.UserId == user.Id && x.EventId == eventId);

        if (userEventDetail == null)
        {
            ModelState.AddModelError("", "Ви не зареєстровані на цю подію.");
            return RedirectToAction("Index");
        }

        // Якщо є оцінка, видаляємо її
        if (userEventDetail.Rating.HasValue)
        {
            userEventDetail.Rating = null;
            await _context.SaveChangesAsync();
        }

        return RedirectToAction("Index");
    }


    public IActionResult Rate()
    {
        return View();
    }
}
