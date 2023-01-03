using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Authorize(Roles = "Admin")]

    public class ExercisesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public ExercisesController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Exercises
        public async Task<IActionResult> Index()
        {
              return View(await _context.Exercise.ToListAsync());
        }

        // GET: Exercises/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Exercise == null)
            {
                return NotFound();
            }

            var exercise = await _context.Exercise
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exercise == null)
            {
                return NotFound();
            }

            return View(exercise);
        }

        // GET: Exercises/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Exercises/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name")] Exercise exercise)
        {
            if (ModelState.IsValid)
            {
                _context.Add(exercise);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(exercise);
        }

        // GET: Exercises/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Exercise == null)
            {
                return NotFound();
            }

            var exercise = await _context.Exercise.FindAsync(id);
            if (exercise == null)
            {
                return NotFound();
            }
            return View(exercise);
        }

        // POST: Exercises/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Exercise exercise)
        {
            if (id != exercise.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(exercise);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ExerciseExists(exercise.Id))
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
            return View(exercise);
        }

        // GET: Exercises/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Exercise == null)
            {
                return NotFound();
            }

            var exercise = await _context.Exercise
                .FirstOrDefaultAsync(m => m.Id == id);
            if (exercise == null)
            {
                return NotFound();
            }

            return View(exercise);
        }

        // POST: Exercises/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Exercise == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Exercise'  is null.");
            }
            var exercise = await _context.Exercise.FindAsync(id);
            if (exercise != null)
            {
                _context.Exercise.Remove(exercise);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ExerciseExists(int id)
        {
          return _context.Exercise.Any(e => e.Id == id);
        }
    }
}
