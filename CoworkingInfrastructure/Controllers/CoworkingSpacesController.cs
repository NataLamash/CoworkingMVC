using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using CoworkingDomain.Model;
using CoworkingInfrastructure;
using Microsoft.AspNetCore.Authorization;

namespace CoworkingInfrastructure.Controllers
{
    public class CoworkingSpacesController : Controller
    {
        private readonly CoworkingDbContext _context;

        public CoworkingSpacesController(CoworkingDbContext context)
        {
            _context = context;
        }

        // GET: CoworkingSpaces
        public async Task<IActionResult> Index()
        {
            return View(await _context.CoworkingSpaces.ToListAsync());
        }

        // GET: CoworkingSpaces/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coworkingSpace = await _context.CoworkingSpaces
                .FirstOrDefaultAsync(m => m.Id == id);
            if (coworkingSpace == null)
            {
                return NotFound();
            }

            return View(coworkingSpace);
        }

        // GET: CoworkingSpaces/Create

        [Authorize(Roles = "admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: CoworkingSpaces/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Location,Description,Capacity,HourlyRate,Id")] CoworkingSpace coworkingSpace)
        {
            if (ModelState.IsValid)
            {
                _context.Add(coworkingSpace);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(coworkingSpace);
        }


        [Authorize(Roles = "admin")]
        // GET: CoworkingSpaces/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coworkingSpace = await _context.CoworkingSpaces.FindAsync(id);
            if (coworkingSpace == null)
            {
                return NotFound();
            }
            return View(coworkingSpace);
        }

        // POST: CoworkingSpaces/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.

        [Authorize(Roles = "admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Name,Location,Description,Capacity,HourlyRate,Id")] CoworkingSpace coworkingSpace)
        {
            if (id != coworkingSpace.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(coworkingSpace);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CoworkingSpaceExists(coworkingSpace.Id))
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
            return View(coworkingSpace);
        }

        // GET: CoworkingSpaces/Delete/5

        [Authorize(Roles = "admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var coworkingSpace = await _context.CoworkingSpaces
                .FirstOrDefaultAsync(m => m.Id == id);
            if (coworkingSpace == null)
            {
                return NotFound();
            }

            return View(coworkingSpace);
        }

        // POST: CoworkingSpaces/Delete/5

        [Authorize(Roles = "admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var coworkingSpace = await _context.CoworkingSpaces.FindAsync(id);
            if (coworkingSpace != null)
            {
                _context.CoworkingSpaces.Remove(coworkingSpace);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CoworkingSpaceExists(int id)
        {
            return _context.CoworkingSpaces.Any(e => e.Id == id);
        }
    }
}
