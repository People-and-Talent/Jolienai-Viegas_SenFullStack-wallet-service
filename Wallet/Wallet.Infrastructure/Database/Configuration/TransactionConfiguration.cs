namespace Wallet.Infrastructure.Database.Configuration;

public class TransactionConfiguration : IEntityTypeConfiguration<Transaction>
{
    public void Configure(EntityTypeBuilder<Transaction> builder)
    {
        builder
            .Property(e => e.Version)
            .IsConcurrencyToken()
            .ValueGeneratedOnAddOrUpdate();
        
        builder
            .Property(b => b.Amount)
            .HasPrecision(31, 12);

        builder.Property(x => x.Type)
            .HasConversion(v => v.ToString(), v => (TransactionType)Enum.Parse(typeof(TransactionType), v));
    }
}