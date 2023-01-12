using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebApplication2.Models
{
    public class Wallet
    {
        public int Id { get; set; }
        [Display(Name = "Nazwa")]
        public string Name{ get; set; }
        [Display(Name = "Stan Portfela")]
        public IList<StoredCrypto>? Cryptos { get; set; }
        [Display(Name = "Właściciel")]
        public string? UserId { get; set; }
        [NotMapped]
        public virtual IdentityUser? User { get; set; }
    }

    public class StoredCrypto
    {
        public int Id { get; set; }
        public float Quantity { get; set; }
    }
}
