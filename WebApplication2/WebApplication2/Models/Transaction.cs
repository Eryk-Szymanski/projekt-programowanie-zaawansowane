using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace WebApplication2.Models
{
    public class Transaction
    {
        [Key]
        public int Id { get; set; }
        [Display(Name = "Portfel Nadawcy")]
        public int? SenderWalletId { get; set; }
        [NotMapped]
        public virtual Wallet? SenderWallet { get; set; }
        public string? SenderId { get; set; }
        public virtual IdentityUser? Sender { get; set; }
        [Display(Name = "Portfel Odbiorcy")]
        public int? RecipientWalletId { get; set; }
        public virtual Wallet? RecipientWallet { get; set; }
        [Display(Name = "Odbiorca")]
        public string? RecipientId { get; set; }
        public virtual IdentityUser? Recipient { get; set; }
        [Display(Name = "Kryptowaluta")]
        public int CryptoId { get; set; }
        [NotMapped]
        public virtual Crypto Crypto { get; set; }
        [Display(Name = "Ilość")]
        public float CryptoQuantity { get; set; }
        [Display(Name = "Wiadomość")]
        public string? Message { get; set; }
    }
}
