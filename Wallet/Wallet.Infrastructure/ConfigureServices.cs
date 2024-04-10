using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Wallet.Application.Shared;
using Wallet.Infrastructure.Database;

namespace Wallet.Infrastructure;

public static class ConfigureServices
{
    public static IServiceCollection AddInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("ServiceDatabase");
        services.AddDbContext<WalletDbContext>(options => options.UseNpgsql(connectionString));

        services.AddScoped<IWalletDbContext>(
            provider => provider.GetRequiredService<WalletDbContext>()
        );
        return services;
    }
}