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


    public IActionResult Rate()
    {
        return View();
    }
}
