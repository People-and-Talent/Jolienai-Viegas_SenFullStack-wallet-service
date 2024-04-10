using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Wallet.Infrastructure.Database;

namespace Wallet.Api.IntegrationTests;

public abstract class BaseIntegrationTest
    : IClassFixture<IntegrationTestWebAppFactory>,
        IDisposable
{
    private readonly IServiceScope _scope;
    protected readonly IMediator Sender;
    protected readonly WalletDbContext DbContext;

    protected BaseIntegrationTest(IntegrationTestWebAppFactory factory)
    {
        _scope = factory.Services.CreateScope();

        Sender = _scope.ServiceProvider.GetRequiredService<IMediator>();

        DbContext = _scope.ServiceProvider
            .GetRequiredService<WalletDbContext>();
    }

    public void Dispose()
    {
        _scope?.Dispose();
        DbContext?.Dispose();
    }
}