using Company.App.Domain;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;


namespace Company.App.Persistence.Context.EFCore.Configuration
{
    public class TransactionConfiguration: IEntityTypeConfiguration<FinancialTransaction>
    {
        public void Configure(EntityTypeBuilder<FinancialTransaction> builder)
        {
            builder.ToTable("Transaction");
            builder.HasKey(c => c.Id);
            builder.Property(m => m.CorrelationId).HasColumnName("CorrelationId");
            builder.Property(m => m.SourceAccountId).HasColumnName("SourceAccountId");
            builder.Property(c => c.TargetAccountId).HasColumnName("TargetAccountId");
            builder.Property(c => c.TranferTypeId).HasColumnName("TranferTypeId");
            builder.Property(c => c.Value).HasColumnName("AmountValue");
            builder.Property(c => c.CurrentState).HasColumnName("CurrentState");
            builder.Property(c => c.DateCreation).HasColumnName("DateCreation");
            builder.Property(c => c.DateModification).HasColumnName("DateModification");           
        }
    }
}
