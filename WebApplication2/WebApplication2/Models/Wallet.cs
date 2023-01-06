﻿using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

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
        public string UserId { get; set; }
        public virtual IdentityUser User { get; set; }
    }

    public class StoredCrypto
    {
        public int Id { get; set; }
        public float Quantity { get; set; }
    }
}