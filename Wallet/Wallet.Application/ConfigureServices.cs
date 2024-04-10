using FluentValidation;
using Microsoft.Extensions.DependencyInjection;

namespace Wallet.Application;

public static class ConfigureServices
{
    public static IServiceCollection AddApplication(this IServiceCollection services)
    {
        var assembly = AppDomain.CurrentDomain.Load("Wallet.Application");
        services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(assembly));
        
        services.AddValidatorsFromAssembly(assembly);
        
        return services;
    }
}