using Claims.Domain.Models.Audit;
using Claims.Domain.Models.Insurance;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Claims.Infrastructure.DataContext
{
    public class MainContext : DbContext
    {

        public DbSet<Claim> Claims { get; init; }
        public DbSet<Cover> Covers { get; init; }

        public DbSet<ClaimAudit> ClaimAudits { get; set; }
        public DbSet<CoverAudit> CoverAudits { get; set; }

        public MainContext(DbContextOptions options)
            : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Claim>().ToCollection("Claims");
            modelBuilder.Entity<Cover>().ToCollection("Covers");
            modelBuilder.Entity<ClaimAudit>().ToCollection("ClaimAudits");
            modelBuilder.Entity<CoverAudit>().ToCollection("CoverAudits");
        }
    }
}
