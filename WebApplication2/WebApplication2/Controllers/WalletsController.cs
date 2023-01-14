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
    [Authorize]
    public class WalletsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public WalletsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Wallets
        public async Task<IActionResult> Index()
        {
            var wallets = _context.Wallet.Where(s => s.UserId == _userManager.GetUserId(User));
            Dictionary<string, float> walletValues = new Dictionary<string, float>();
            foreach(var wallet in wallets)
            {
                float walletValue = 0;
                if (wallet.Cryptos != null)
                {
                    foreach (var crypto in wallet.Cryptos)
                    {
                        float cryptoValue = crypto.Quantity * _context.Crypto.Where(c => c.Id == crypto.Id).Select(c => c.Value).Single();
                        walletValue += cryptoValue;
                    }
                }
                walletValues[(wallet.Id).ToString()] = walletValue;
            }
            ViewData["WalletValues"] = walletValues;
            return View(await wallets.ToListAsync());
        }

        // GET: Wallets/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Wallet == null)
            {
                return NotFound();
            }

            var wallet = await _context.Wallet.FirstOrDefaultAsync(m => m.Id == id);
            Dictionary<string, float> cryptos = new Dictionary<string, float>();
            if (wallet.Cryptos != null)
            {
                foreach (var crypto in wallet.Cryptos)
                {
                    string name = _context.Crypto.Where(c => c.Id == crypto.Id).Select(c => c.Name).Single();
                    cryptos[name] = crypto.Quantity;
                }
            }
            ViewData["cryptos"] = cryptos;
            if (wallet == null)
            {
                return NotFound();
            }

            return View(wallet);
        }

        // GET: Wallets/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Wallets/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,CashBalance")] Wallet wallet)
        {
            wallet.UserId = _userManager.GetUserId(User);
            if (ModelState.IsValid)
            {
                _context.Add(wallet);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(wallet);
        }

        // GET: Wallets/AddCashBalance/5
        public async Task<IActionResult> AddCashBalance(int? id)
        {
            if (id == null || _context.Wallet == null)
            {
                return NotFound();
            }

            var wallet = await _context.Wallet.FindAsync(id);
            if (wallet == null)
            {
                return NotFound();
            }
            return View(wallet);
        }

        // POST: Wallets/AddCashBalance/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCashBalance(int id, float cashBalance)
        {
            Wallet wallet = _context.Wallet.Where(w => w.Id == id).Single();
            if (id != wallet.Id)
            {
                return NotFound();
            }

            wallet.CashBalance += cashBalance;

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(wallet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WalletExists(wallet.Id))
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
            return View(wallet);
        }

        // GET: Wallets/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Wallet == null)
            {
                return NotFound();
            }

            var wallet = await _context.Wallet.FindAsync(id);
            if (wallet == null)
            {
                return NotFound();
            }
            return View(wallet);
        }

        // POST: Wallets/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name")] Wallet wallet)
        {
            if (id != wallet.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(wallet);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!WalletExists(wallet.Id))
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
            return View(wallet);
        }

        // GET: Wallets/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Wallet == null)
            {
                return NotFound();
            }

            var wallet = await _context.Wallet
                .FirstOrDefaultAsync(m => m.Id == id);
            if (wallet == null)
            {
                return NotFound();
            }

            return View(wallet);
        }

        // POST: Wallets/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Wallet == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Wallet'  is null.");
            }
            var wallet = await _context.Wallet.FindAsync(id);
            if (wallet != null)
            {
                _context.Wallet.Remove(wallet);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool WalletExists(int id)
        {
          return _context.Wallet.Any(e => e.Id == id);
        }
    }
}
