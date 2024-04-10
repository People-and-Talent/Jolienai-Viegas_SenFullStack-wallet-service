using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Wallet.Infrastructure.Database.Configuration;

public class WalletConfiguration : IEntityTypeConfiguration<Domain.Entities.Wallet>
{
    public void Configure(EntityTypeBuilder<Domain.Entities.Wallet> builder)
    {
        builder
            .Property(e => e.Version)
            .IsConcurrencyToken()
            .ValueGeneratedOnAddOrUpdate();
        
        builder
            .Property(b => b.Balance)
            .HasPrecision(31, 12);
    }
}