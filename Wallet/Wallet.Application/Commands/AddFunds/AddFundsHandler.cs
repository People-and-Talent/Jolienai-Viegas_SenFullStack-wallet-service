namespace Wallet.Application.Commands.AddFunds;

public class AddFundsHandler(IWalletDbContext context) : IRequestHandler<AddFundsRequest, bool>
{
    public async Task<bool> Handle(AddFundsRequest request, CancellationToken cancellationToken)
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

        wallet.Balance += transaction.Amount;
        
        var rowsAffected = await context.SaveChangesAsync(cancellationToken);
        
        return rowsAffected > 0;
    }
}