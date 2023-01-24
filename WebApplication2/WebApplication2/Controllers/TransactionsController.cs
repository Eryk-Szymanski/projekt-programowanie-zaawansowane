using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using NuGet.Protocol.Core.Types;
using WebApplication2.Data;
using WebApplication2.Models;

namespace WebApplication2.Controllers
{
    [Authorize]
    public class TransactionsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public TransactionsController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: Transactions
        public async Task<IActionResult> Index()
        {
            var transactions = _context.Transaction.Where(s => s.SenderId == _userManager.GetUserId(User) || s.RecipientId == _userManager.GetUserId(User));
            foreach(var transaction in transactions)
            {
                transaction.Sender = _context.Users.Where(u => u.Id == transaction.SenderId).SingleOrDefault();
                transaction.SenderWallet = _context.Wallet.Where(w => w.Id == transaction.SenderWalletId).SingleOrDefault();
                transaction.Recipient = _context.Users.Where(u => u.Id == transaction.RecipientId).SingleOrDefault();
                transaction.RecipientWallet= _context.Wallet.Where(w => w.Id == transaction.RecipientWalletId).SingleOrDefault();
                transaction.Crypto = _context.Crypto.Where(c => c.Id == transaction.CryptoId).SingleOrDefault();
            }
            ViewData["IsAdmin"] = User.IsInRole("Admin");
            return View(await transactions.ToListAsync());
        }

        // GET: Transaction/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null || _context.Transaction == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transaction
                .FirstOrDefaultAsync(m => m.Id == id);
            transaction.Sender = _context.Users.Where(u => u.Id == transaction.SenderId).Single();
            transaction.SenderWallet = _context.Wallet.Where(w => w.Id == transaction.SenderWalletId).Single();
            transaction.Recipient = _context.Users.Where(u => u.Id == transaction.RecipientId).Single();
            transaction.RecipientWallet = _context.Wallet.Where(w => w.Id == transaction.RecipientWalletId).Single();
            transaction.Crypto = _context.Crypto.Where(c => c.Id == transaction.CryptoId).Single();
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // GET: Transactions/Create
        public IActionResult Create()
        {
            string userId = _userManager.GetUserId(User);
            List<Wallet> userWallets = _context.Wallet.Where(w => w.UserId == userId).ToList();
            Wallet dummyWallet = new Wallet();
            dummyWallet.Id = -1;
            dummyWallet.Name = "Wybierz porftel";
            dummyWallet.Cryptos = null;
            userWallets.Insert(0, dummyWallet);
            var wallets = _context.Wallet.Where(w => w.UserId != userId);
            if (userWallets == null || wallets == null)
            {
                return NotFound();
            }
            ViewData["UserWalletIds"] = new SelectList(userWallets, "Id", "Name");
            ViewData["AllWallets"] = new SelectList(wallets, "Id", "Name");
            return View();
        }

        [HttpPost]
        public IActionResult GetWalletCrypto(int selectedWallet)
        {
            var cryptos = _context.Wallet.Where(w => w.Id == selectedWallet).Select(w => w.Cryptos).Single();
            List<Crypto> walletCrypto = new List<Crypto>();
            if (cryptos != null)
            {
                foreach (var crypto in cryptos)
                {
                    walletCrypto.Add(_context.Crypto.Where(c => c.Id == crypto.Id).Single());
                }
            }
            return Json(walletCrypto);
        }

        // POST: Transactions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,SenderWalletId,RecipientWalletId,CryptoId,CryptoQuantity,Message")] Transaction transaction)
        {
            if (transaction.CryptoQuantity > 0) { 
                transaction.SenderId = _userManager.GetUserId(User);
                transaction.RecipientId = _context.Wallet.Where(w => w.Id == transaction.RecipientWalletId).Select(w => w.UserId).Single();
                Wallet senderWallet = _context.Wallet.Where(w => w.Id == transaction.SenderWalletId).Single();
                Wallet recipientWallet = _context.Wallet.Where(w => w.Id == transaction.RecipientWalletId).Single();
                StoredCrypto crypto = senderWallet.Cryptos.Where(c => c.Id == transaction.CryptoId).Single();
                if (crypto.Quantity >= transaction.CryptoQuantity)
                {
                    crypto.Quantity -= transaction.CryptoQuantity;
                    StoredCrypto crypto1 = new StoredCrypto(0, 0);
                    if (recipientWallet.Cryptos != null)
                    {
                        crypto1 = recipientWallet.Cryptos.SingleOrDefault(c => c.Id == transaction.CryptoId);
                        if (crypto1 != null)
                        {
                            crypto1.Quantity += transaction.CryptoQuantity;
                        } else
                        {
                            crypto1 = new StoredCrypto(transaction.CryptoId, transaction.CryptoQuantity);
                            recipientWallet.Cryptos.Add(crypto1);
                        }
                    }
                    else
                    {
                        recipientWallet.Cryptos = new List<StoredCrypto>();
                        crypto1 = new StoredCrypto(transaction.CryptoId, transaction.CryptoQuantity);
                        recipientWallet.Cryptos.Add(crypto1);
                    }
                    if (ModelState.IsValid)
                    {
                        try
                        {
                            _context.Update(senderWallet);
                            await _context.SaveChangesAsync();
                            _context.Update(recipientWallet);
                            await _context.SaveChangesAsync();
                            _context.Add(transaction);
                            await _context.SaveChangesAsync();
                            return RedirectToAction(nameof(Index));
                        }
                        catch (DbUpdateConcurrencyException)
                        {
                            if (!TransactionExists(transaction.Id))
                            {
                                return NotFound();
                            }
                            else
                            {
                                throw;
                            }
                        }
                    }
                }
            }
            return View(transaction);
        }

        // GET: Transactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null || _context.Transaction == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transaction.FindAsync(id);
            if (transaction == null)
            {
                return NotFound();
            }
            return View(transaction);
        }

        // POST: Transactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,SenderWalletId,SenderId,RecipientWalletId,RecipientId,CryptoId,CryptoQuantity,Message")] Transaction transaction)
        {
            if (id != transaction.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(transaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TransactionExists(transaction.Id))
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
            return View(transaction);
        }

        // GET: Transactions/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null || _context.Transaction == null)
            {
                return NotFound();
            }

            var transaction = await _context.Transaction
                .FirstOrDefaultAsync(m => m.Id == id);
            if (transaction == null)
            {
                return NotFound();
            }

            return View(transaction);
        }

        // POST: Transactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (_context.Transaction == null)
            {
                return Problem("Entity set 'ApplicationDbContext.Transaction'  is null.");
            }
            var transaction = await _context.Transaction.FindAsync(id);
            if (transaction != null)
            {
                _context.Transaction.Remove(transaction);
            }
            
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TransactionExists(int id)
        {
          return _context.Transaction.Any(e => e.Id == id);
        }
    }
}
