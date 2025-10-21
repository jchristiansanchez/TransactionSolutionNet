using Company.App.Domain;
using Company.App.Domain.Common;
using Company.App.Persistence.Context.EFCore.Configuration;
using Microsoft.EntityFrameworkCore;

namespace Company.App.Persistence.Context.EFCore
{
    public class EFCoreContext : DbContext
    {
        public EFCoreContext(DbContextOptions<EFCoreContext> options)
          : base(options)
        {            
        }
        protected override void OnModelCreating(ModelBuilder builder)
        {
            builder.ApplyConfiguration(new TransactionConfiguration());            
        }
        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            SaveAuditableEntity();
            return base.SaveChangesAsync(cancellationToken);
        }

        private void SaveAuditableEntity()
        {

            foreach (var entry in ChangeTracker.Entries<BaseAuditableEntity>().Where(e => e.State == EntityState.Added))
            {
                entry.Entity.DateCreation = DateTime.UtcNow;
            }

            foreach (var entry in ChangeTracker.Entries<BaseAuditableEntity>().Where(e => e.State == EntityState.Modified))
            {
                entry.Entity.DateModification = DateTime.UtcNow;
            }
        }

        public DbSet<FinancialTransaction> FinancialTransaction { get; set; }   
    }
}
