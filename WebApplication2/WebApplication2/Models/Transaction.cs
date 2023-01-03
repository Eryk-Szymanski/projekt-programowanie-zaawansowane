using Microsoft.AspNetCore.Identity;

namespace WebApplication2.Models
{
	public class Transaction
	{
		public string ExerciseName { get; set; }
		public int NumOfSessions { get; set; }
		public int BestResult { get; set; }
        public string? UserId { get; set; }
        public virtual IdentityUser? User { get; set; }
    }
}
