using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ParliamentDomain.Model;

namespace ParliamentInfrastructure.Controllers
{
    public class EventsController : Controller
    {
        private readonly ParliamentDbContext _context;

        public EventsController(ParliamentDbContext context)
        {
            _context = context;
        }

        // GET: Events
        public async Task<IActionResult> Index(int? locationId, int? departmentId)
        {
            if (locationId == null && departmentId == null)
            {
                var dbparliamentContext = _context.Events.Include(e => e.Department).Include(e => e.Location);
                return View(await dbparliamentContext.ToListAsync());
            }
            if (departmentId == null)
            {
                var eventsByLocation = _context.Events.Where(e => e.LocationId == locationId)
                    .Include(e => e.Location);
                return View(await eventsByLocation.ToListAsync());
            }

            var eventsByDepartment = _context.Events.Where(e => e.DepartmentId == departmentId)
                .Include(e => e.Department);
            return View(await eventsByDepartment.ToListAsync());
        }

        // GET: Events/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.Department)
                .Include(e => e.Location)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // GET: Events/Create
        [Authorize(Roles = "admin, worker")]
        public IActionResult Create()
        {
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name");
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Address");
            return View();
        }

        // POST: Events/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, worker")]
        public async Task<IActionResult> Create([Bind("LocationId,DepartmentId,Title,AccessType,StartDate,EndDate,Description,Id")] Event @event)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@event);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", @event.DepartmentId);
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Address", @event.LocationId);
            return View(@event);
        }

        // GET: Events/Edit/5
        [Authorize(Roles = "admin, worker")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events.FindAsync(id);
            if (@event == null)
            {
                return NotFound();
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", @event.DepartmentId);
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Address", @event.LocationId);
            return View(@event);
        }

        // POST: Events/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, worker")]
        public async Task<IActionResult> Edit(int id, [Bind("LocationId,DepartmentId,Title,AccessType,StartDate,EndDate,Description,Id")] Event @event)
        {
            if (id != @event.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@event);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EventExists(@event.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            ViewData["DepartmentId"] = new SelectList(_context.Departments, "Id", "Name", @event.DepartmentId);
            ViewData["LocationId"] = new SelectList(_context.Locations, "Id", "Address", @event.LocationId);
            return View(@event);
        }

        // GET: Events/Delete/5
        [Authorize(Roles = "admin, worker")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @event = await _context.Events
                .Include(e => e.Department)
                .Include(e => e.Location)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (@event == null)
            {
                return NotFound();
            }

            return View(@event);
        }

        // POST: Events/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, worker")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @event = await _context.Events.FindAsync(id);
            if (@event != null)
            {
                _context.Events.Remove(@event);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EventExists(int id)
        {
            return _context.Events.Any(e => e.Id == id);
        }
    }
}
