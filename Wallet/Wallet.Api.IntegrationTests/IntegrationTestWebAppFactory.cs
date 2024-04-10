using DotNet.Testcontainers.Builders;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Testcontainers.PostgreSql;
using Wallet.Infrastructure.Database;

namespace Wallet.Api.IntegrationTests;

public class IntegrationTestWebAppFactory : WebApplicationFactory<Program>,
    IAsyncLifetime
{
    private readonly PostgreSqlContainer _dbContainer = new PostgreSqlBuilder()
        .WithDatabase("wallet_db_test")
        .WithUsername("postgres")
        .WithPassword("example")
        .WithPortBinding(5555,5432)
        .WithWaitStrategy(Wait.ForUnixContainer().UntilCommandIsCompleted("pg_isready"))
        .WithCleanUp(true)
        .Build();
    
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureTestServices(services =>
        {
            var descriptorType =
                typeof(DbContextOptions<WalletDbContext>);

            var descriptor = services
                .SingleOrDefault<ServiceDescriptor>(s => s.ServiceType == descriptorType);

            if (descriptor is not null)
            {
                services.Remove(descriptor);
            }
            
            EntityFrameworkServiceCollectionExtensions.AddDbContext<WalletDbContext>(services, options =>
                NpgsqlDbContextOptionsBuilderExtensions.UseNpgsql(options, _dbContainer.GetConnectionString()));
        });
    }

    public Task InitializeAsync()
    {
        return _dbContainer.StartAsync();
    }

    public new Task DisposeAsync()
    {
        return _dbContainer.StopAsync();
    }
}