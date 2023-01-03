using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;

namespace WebApplication2.Models
{
    public class SE
    {
        public int Id { get; set; }
        [Display(Name = "Ciężar")]
        public int Weight { get; set; }
        [Display(Name = "Ilość serii")]
        public int NumOfSeries { get; set; }
        [Display(Name = "Ilość powtórzeń")]
        public int NumOfReps { get; set; }
        public int? ExerciseId { get; set; }
        [Display(Name = "Ćwiczenie")]
        public virtual Exercise? Exercise { get; set; }
        public int? SessionId { get; set; }
        [Display(Name = "Sesja")]
        public virtual Session? Session { get; set; }
        public string? UserId { get; set; }
        public virtual IdentityUser? User { get; set; }
    }
}
