using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebApplication2.Models;

namespace WebApplication2.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }
        public DbSet<WebApplication2.Models.Exercise> Exercise { get; set; }
        public DbSet<WebApplication2.Models.Session> Session { get; set; }
        public DbSet<WebApplication2.Models.SE> SE { get; set; }
    }
}