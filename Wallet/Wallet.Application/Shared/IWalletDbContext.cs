namespace Wallet.Application.Shared;

public interface IWalletDbContext
{
    DbSet<T> Set<T>() where T : class;
    Task<int> SaveChangesAsync(CancellationToken cancellationToken = new());
}