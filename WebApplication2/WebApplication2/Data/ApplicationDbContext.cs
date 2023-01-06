using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using WebApplication2.Models;

namespace WebApplication2.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.Entity<Wallet>()
                .Property(b => b.Cryptos)
                .HasConversion(
                    v => JsonConvert.SerializeObject(v),
                    v => JsonConvert.DeserializeObject<IList<StoredCrypto>>(v));

            var valueComparer = new ValueComparer<IList<StoredCrypto>>(
                (c1, c2) => c1.SequenceEqual(c2),
                c => c.Aggregate(0, (a, v) => HashCode.Combine(a, v.GetHashCode())),
                c => c.ToList());

            modelBuilder
                .Entity<Wallet>()
                .Property(e => e.Cryptos)
                .Metadata
                .SetValueComparer(valueComparer);

            modelBuilder.Entity<Transaction>()
                .HasOne(x => x.Sender)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);

            modelBuilder.Entity<Transaction>()
                .HasOne(x => x.Recipient)
                .WithMany()
                .OnDelete(DeleteBehavior.Restrict);
        }

        public DbSet<WebApplication2.Models.Crypto> Crypto { get; set; }
        public DbSet<WebApplication2.Models.Transaction> Transaction { get; set; }
        public DbSet<WebApplication2.Models.Wallet> Wallet { get; set; }
    }
}