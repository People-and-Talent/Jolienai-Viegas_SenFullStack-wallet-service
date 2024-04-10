using MediatR;
using Wallet.Application.Shared;

namespace Wallet.Application.Commands.CreateWallet;

public class CreateWalletHandler(IWalletDbContext context) : IRequestHandler<CreateWalletRequest, Guid>
{
    public async Task<Guid> Handle(CreateWalletRequest request, CancellationToken cancellationToken)
    {
        var wallet = new Domain.Entities.Wallet
        {
            UserId = request.UserId,
            ProviderId = request.ProviderId,
            CreatedAt = DateTime.UtcNow
        };
        await context.Set<Domain.Entities.Wallet>().AddAsync(wallet, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
        return wallet.Id;
    }
}