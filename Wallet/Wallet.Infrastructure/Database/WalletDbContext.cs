namespace Wallet.Infrastructure.Database;

public class WalletDbContext(DbContextOptions options) : DbContext(options), IWalletDbContext
{
    public DbSet<Domain.Entities.Wallet> Wallet { get; set; } = default!;
    public DbSet<Domain.Entities.Transaction> Transaction { get; set; } = default!;
    public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = new CancellationToken())
    {
        return await base.SaveChangesAsync(cancellationToken);
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(WalletDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
}