using Microsoft.AspNetCore.Identity;

namespace WebApplication2.Models
{
    public class Session
    {
        public int Id { get; set; }
        public DateTime DateTimeStart { get; set; }
        public DateTime DateTimeEnd { get; set; }
        public string? UserId { get; set; }
        public virtual IdentityUser? User { get; set; }
    }
}
