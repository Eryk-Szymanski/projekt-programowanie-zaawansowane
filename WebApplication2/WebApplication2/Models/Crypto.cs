using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Xml.Linq;

namespace WebApplication2.Models
{
    public class Crypto
    {
        public int Id { get; set; }
        [Display(Name = "Nazwa")]
        public string Name { get; set; }
        [Display(Name = "Opis")]
        public string Description { get; set; }
        [Display(Name = "Wartość")]
        public float Value { get; set; }
        [NotMapped]
        public IFormFile? Image { get; set; }
        public string? ImageName { get; set; }
    }
}
