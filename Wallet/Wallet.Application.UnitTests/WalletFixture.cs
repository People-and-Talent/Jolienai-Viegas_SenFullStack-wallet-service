using Wallet.Infrastructure.Database;

namespace Wallet.Application.UnitTests;

public class WalletFixture : IDisposable
{
    public readonly WalletDbContext DbContext = new(
        new DbContextOptionsBuilder<WalletDbContext>()
            .UseInMemoryDatabase($"Database_{Guid.NewGuid()}").Options);
    
    public void Dispose()
    {
        DbContext.Dispose();
    }
}