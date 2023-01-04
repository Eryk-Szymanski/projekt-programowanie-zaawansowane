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
    public class CryptosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public CryptosController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Cryptos
        public async Task<IActionResult> Index()
        {
              return View(await _context.Crypto.ToListAsync());
        }

        // GET: Cryptos/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Crypto == null)
            {
                return NotFound();
            }

            var crypto = await _context.Crypto
                .FirstOrDefaultAsync(m => m.Id == id);
            if (crypto == null)
            {
                return NotFound();
            }

            return View(crypto);
        }

        // GET: Cryptos/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: Cryptos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Value")] Crypto crypto)
        {
            if (ModelState.IsValid)
            {
                _context.Add(crypto);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(crypto);
        }

        // GET: Cryptos/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Crypto == null)
            {
                return NotFound();
            }

            var crypto = await _context.Crypto.FindAsync(id);
            if (crypto == null)
            {
                return NotFound();
            }
            return View(crypto);
        }

        // POST: Cryptos/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Value")] Crypto crypto)
        {
            if (id != crypto.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(crypto);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CryptoExists(crypto.Id))
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
            return View(crypto);
        }

        // GET: Cryptos/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Crypto == null)
            {
                return NotFound();
            }

            var crypto = await _context.Crypto
                .FirstOrDefaultAsync(m => m.Id == id);
            if (crypto == null)
            {
                return NotFound();
            }

            return View(crypto);
        }

        // POST: Cryptos/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Crypto == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Exercise'  is null.");
            }
            var crypto = await _context.Crypto.FindAsync(id);
            if (crypto != null)
            {
                _context.Crypto.Remove(crypto);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CryptoExists(int id)
        {
          return _context.Crypto.Any(e => e.Id == id);
        }
    }
}
