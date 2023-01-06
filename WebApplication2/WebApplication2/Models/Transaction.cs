using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Xml.Linq;

namespace WebApplication2.Models
{
    public class Transaction
    {
		public int Id { get; set; }
        [Display(Name = "Nadawca")]
        public int SenderWalletId { get; set; }
        public virtual Wallet SenderWallet { get; set; }
        public string SenderId { get; set; }
        public virtual IdentityUser Sender { get; set; }
        [Display(Name = "Odbiorca")]
        public int RecipientWalletId { get; set; }
        public virtual Wallet RecipientWallet { get; set; }
        public string RecipientId { get; set; }
        public virtual IdentityUser Recipient { get; set; }
        public int CryptoId { get; set; }
        public float CryptoQuantity { get; set; }
        public string? Message { get; set; }
    }
}
