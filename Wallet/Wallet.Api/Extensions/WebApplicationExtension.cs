using Microsoft.EntityFrameworkCore;
using Wallet.Infrastructure.Database;

namespace Wallet.Api.Extensions;

public static class WebApplicationExtension
{
    public static void ApplyDbMigrations(this WebApplication app)
    {
        // maybe we dont want to apply migration on the service startup for other environments
        if (!app.Environment.IsDevelopment())
        {
            return;
        }
        
        using var scope = app.Services.CreateScope();
        var db = scope.ServiceProvider.GetRequiredService<WalletDbContext>();
        db.Database.Migrate();
    }
}