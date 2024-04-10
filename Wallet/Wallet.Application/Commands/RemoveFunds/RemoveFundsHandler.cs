using MediatR;
using Microsoft.EntityFrameworkCore;
using Wallet.Application.Shared;
using Wallet.Domain.Entities;
using Wallet.Domain.Enum;
using Wallet.Domain.Exceptions;

namespace Wallet.Application.Commands.RemoveFunds;

public class RemoveFundsHandler(IWalletDbContext context) : IRequestHandler<RemoveFundsRequest, bool>
{
    public async Task<bool> Handle(RemoveFundsRequest request, CancellationToken cancellationToken)
    {
        var wallet = await context.Set<Domain.Entities.Wallet>()
            .SingleAsync(x => x.Id.ToString() == request.WalletId, cancellationToken: cancellationToken);

        var transaction = new Transaction
        {
            WalletId = wallet.Id,
            Type = Enum.Parse<TransactionType>(request.TransactionType),
            Amount = request.Amount ?? 0,
            CreatedAt = DateTime.UtcNow
        };
        await context.Set<Transaction>().AddAsync(transaction, cancellationToken);
        
        var newBalance = wallet.Balance - transaction.Amount;
        if (newBalance < 0)
        {
            throw new BalanceNegativeException("The balance of a wallet cannot be negative");
        }
        
        wallet.Balance -= transaction.Amount;
        
        var rowsAffected = await context.SaveChangesAsync(cancellationToken);
        
        return rowsAffected > 0;
    }
}