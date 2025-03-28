using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ParliamentDomain.Model;

namespace ParliamentInfrastructure.Controllers
{
    public class DepartmentsController : Controller
    {
        private readonly ParliamentDbContext _context;

        public DepartmentsController(ParliamentDbContext context)
        {
            _context = context;
        }

        // GET: Departments
        public async Task<IActionResult> Index(int? id)
        {
            var dbparliamentContext = _context.Departments.Include(d => d.Contact);
            return View(await dbparliamentContext.ToListAsync());
        }

        // GET: Departments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.Contact)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // GET: Departments/Create
        [Authorize(Roles = "admin, worker")]
        public IActionResult Create()
        {
            ViewData["ContactId"] = new SelectList(_context.Contacts, "Id", "Email");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, worker")]
        public async Task<IActionResult> Create([Bind("Name,Description,ContactId,Id")] Department department)
        {
            Contact contact = _context.Contacts.FirstOrDefault(c => c.Id == department.ContactId);
            department.Contact = contact;
            ModelState.Clear();
            TryValidateModel(contact);

            if (ModelState.IsValid)
            {
                _context.Add(department);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Details), new { id = department.Id });
            }
            ViewData["ContactId"] = new SelectList(_context.Contacts, "Id", "Email", department.ContactId);
            return View(department);
        }

        // GET: Departments/Edit/5
        [Authorize(Roles = "admin, worker")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments.FindAsync(id);
            if (department == null)
            {
                return NotFound();
            }
            ViewData["ContactId"] = new SelectList(_context.Contacts, "Id", "Email", department.ContactId);
            return View(department);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, worker")]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Description,ContactId,Id")] Department department)
        {
            if (id != department.Id)
            {
                return NotFound();
            }

            Contact contact = _context.Contacts.FirstOrDefault(c => c.Id == department.ContactId);
            department.Contact = contact;
            ModelState.Clear();
            TryValidateModel(contact);

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(department);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!DepartmentExists(department.Id))
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
            ViewData["ContactId"] = new SelectList(_context.Contacts, "Id", "Email", department.ContactId);
            return View(department);
        }

        // GET: Departments/Delete/5
        [Authorize(Roles = "admin, worker")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var department = await _context.Departments
                .Include(d => d.Contact)
                .FirstOrDefaultAsync(m => m.Id == id);
            if (department == null)
            {
                return NotFound();
            }

            return View(department);
        }

        // POST: Departments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "admin, worker")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var department = await _context.Departments.FindAsync(id);
            if (department != null)
            {
                _context.Departments.Remove(department);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool DepartmentExists(int id)
        {
            return _context.Departments.Any(e => e.Id == id);
        }
    }
}
