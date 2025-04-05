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
        var userEventDetails = await _context.UserEventDetails.Where(u => u.UserId == user.Id)
            .Include(e => e.Event)
            .ToListAsync();
        var myEvents = new List<MyEventViewModel>();
        foreach (var item in userEventDetails)
        {
            var myEvent = new MyEventViewModel();
            myEvent.Id = item.EventId;
            myEvent.UserEventDetailId = item.Id;
            myEvent.Rating = item.Rating;
            myEvent.StartDate = item.Event.StartDate;
            myEvent.countUsers = _context.UserEventDetails.Where(t => t.EventId == item.EventId).Count();
        }
        return View(myEvents);
    }

    [HttpPost]
    public async Task<IActionResult> WishToGo(string email, int eventId)
    {
        var userEventDetail = new UserEventDetail();
        userEventDetail.EventId = eventId;
        userEventDetail.Event = await _context.Events.FirstOrDefaultAsync(x => x.Id == eventId);
        userEventDetail.User = await _context.Users.FirstOrDefaultAsync(x => x.Email == email);
        userEventDetail.Role = "guest";
        await _context.UserEventDetails.AddAsync(userEventDetail);
        _context.SaveChanges();
        return RedirectToAction("Index");
    }

    public IActionResult Rate()
    {
        return View();
    }
}
