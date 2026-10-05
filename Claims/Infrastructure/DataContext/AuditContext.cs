using Claims.Domain.Models.Audit;
using Microsoft.EntityFrameworkCore;
using MongoDB.EntityFrameworkCore.Extensions;

namespace Claims.Infrastructure.DataContext
{
    public class AuditContext : DbContext
    {
        public DbSet<ClaimAudit> ClaimAudits { get; set; }
        public DbSet<CoverAudit> CoverAudits { get; set; }

        public AuditContext(DbContextOptions<AuditContext> options) :
            base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<ClaimAudit>().ToCollection("claims_audits");
            modelBuilder.Entity<CoverAudit>().ToCollection("covers_audits");
        }
    }
}
