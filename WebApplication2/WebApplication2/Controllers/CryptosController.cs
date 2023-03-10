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
using Microsoft.AspNetCore.Http;
using System.IO;

namespace WebApplication2.Controllers
{
    public class CryptosController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly IWebHostEnvironment hostingEnvironment;

        public CryptosController(ApplicationDbContext context, UserManager<IdentityUser> userManager, IWebHostEnvironment environment)
        {
            _context = context;
            _userManager = userManager;
            hostingEnvironment = environment;
        }

        // GET: Cryptos
        public async Task<IActionResult> Index()
        {
            ViewData["IsAdmin"] = User.IsInRole("Admin");
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
            return View(new Crypto());
        }

        // POST: Cryptos/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create([Bind("Id,Name,Description,Value")] Crypto crypto, IFormFile file)
        {
            if (file != null)
            {
                var fileExtension = Path.GetExtension(file.FileName);
                if (string.Equals(fileExtension, ".jpg", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(fileExtension, ".png", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(fileExtension, ".gif", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(fileExtension, ".jpeg", StringComparison.OrdinalIgnoreCase))
                {
                    crypto.Image = file;
                    crypto.ImageName = file.FileName;
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", file.FileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
            }
            if (crypto.Value > 0)
            {
                if (ModelState.IsValid)
                {
                    _context.Add(crypto);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(crypto);
        }

        private string GetUniqueFileName(string fileName)
        {
            fileName = Path.GetFileName(fileName);
            return Path.GetFileNameWithoutExtension(fileName)
                      + "_"
                      + Guid.NewGuid().ToString().Substring(0, 4)
                      + Path.GetExtension(fileName);
        }

        // GET: Cryptos/Buy/5
        [Authorize]
        public async Task<IActionResult> Buy(int? id)
        {
            if (id == null || _context.Crypto == null)
            {
                return NotFound();
            }

            var crypto = await _context.Crypto.FindAsync(id);
            var wallets = _context.Wallet.Where(w => w.UserId == _userManager.GetUserId(User));
            ViewData["WalletId"] = new SelectList(wallets, "Id", "Name");
            if (crypto == null)
            {
                return NotFound();
            }
            return View(crypto);
        }

        // POST: Cryptos/Buy/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Buy(int id, float quantity, int walletId)
        {
            Crypto crypto = _context.Crypto.SingleOrDefault(c => c.Id == id);
            if (quantity > 0 && walletId != null)
            {
                if (id != crypto.Id)
                {
                    return NotFound();
                }
                Wallet wallet = _context.Wallet.SingleOrDefault(w => w.Id == walletId);
                if (wallet.CashBalance >= quantity * crypto.Value)
                {
                    bool cryptoInWallet = false;
                    wallet.CashBalance -= quantity * crypto.Value;
                    if (wallet.Cryptos == null)
                    {
                        wallet.Cryptos = new List<StoredCrypto>();
                    }
                    else
                    {
                        foreach (var walletCrypto in wallet.Cryptos)
                        {
                            if (walletCrypto.Id == id)
                            {
                                cryptoInWallet = true;
                                walletCrypto.Quantity += quantity;
                            }
                        }
                    }
                    if (!cryptoInWallet)
                    {
                        wallet.Cryptos.Add(new StoredCrypto(id, quantity));
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
                }
            }
            return View(crypto);
        }

        // GET: Cryptos/Sell/5
        [Authorize]
        public async Task<IActionResult> Sell(int? id)
        {
            if (id == null || _context.Crypto == null)
            {
                return NotFound();
            }

            var crypto = await _context.Crypto.FindAsync(id);
            var wallets = _context.Wallet.Where(w => w.UserId == _userManager.GetUserId(User));
            List<Wallet> correctWallets = new List<Wallet>();
            foreach (var wallet in wallets)
            {
                if (wallet.Cryptos != null)
                {
                    foreach (var c in wallet.Cryptos)
                    {
                        if (c.Id == crypto.Id)
                        {
                            correctWallets.Add(wallet);
                            break;
                        }
                    }
                }
            }
            ViewData["WalletId"] = new SelectList(correctWallets, "Id", "Name");
            if (crypto == null)
            {
                return NotFound();
            }
            return View(crypto);
        }

        // POST: Cryptos/Sell/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public async Task<IActionResult> Sell(int id, float quantity, int walletId)
        {
            Crypto crypto = _context.Crypto.Where(c => c.Id == id).Single();
            if (quantity > 0)
            {
                if (id != crypto.Id)
                {
                    return NotFound();
                }
                Wallet wallet = _context.Wallet.Where(w => w.Id == walletId).Single();
                StoredCrypto walletCrypto = null;
                foreach (var c in wallet.Cryptos)
                {
                    if (c.Id == crypto.Id)
                    {
                        walletCrypto = c;
                        break;
                    }
                }
                if (walletCrypto.Quantity >= quantity)
                {
                    walletCrypto.Quantity -= quantity;
                    if (walletCrypto.Quantity == 0)
                    {
                        wallet.Cryptos.Remove(walletCrypto);
                    }
                    wallet.CashBalance += crypto.Value * quantity;
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
                }
            }
            return View(crypto);
        }

        [HttpPost]
        public IActionResult GetWallets(int selectedCrypto)
        {
            var wallets = _context.Wallet.Where(w => w.UserId == _userManager.GetUserId(User));
            List<Wallet> correctWallets = new List<Wallet>();
            foreach(var wallet in wallets)
            {
                foreach(var crypto in wallet.Cryptos)
                {
                    if(crypto.Id == selectedCrypto)
                    {
                        correctWallets.Add(wallet);
                        break;
                    }
                }
            }
            return Json(correctWallets);
        }

        private bool WalletExists(int id)
        {
            return _context.Wallet.Any(e => e.Id == id);
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
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Description,Value")] Crypto crypto, IFormFile file)
        {
            if (id != crypto.Id)
            {
                return NotFound();
            }

            if (file != null)
            {
                var fileExtension = Path.GetExtension(file.FileName);
                if (string.Equals(fileExtension, ".jpg", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(fileExtension, ".png", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(fileExtension, ".gif", StringComparison.OrdinalIgnoreCase)
                    || string.Equals(fileExtension, ".jpeg", StringComparison.OrdinalIgnoreCase))
                {
                    crypto.Image = file;
                    crypto.ImageName = file.FileName;
                    var path = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/uploads", file.FileName);
                    using (var stream = new FileStream(path, FileMode.Create))
                    {
                        await file.CopyToAsync(stream);
                    }
                }
            }

            if (crypto.Value > 0)
            {
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
            }
            return View(crypto);
        }

        // GET: Cryptos/Delete/5
        [Authorize(Roles = "Admin")]
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
        [Authorize(Roles = "Admin")]
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
