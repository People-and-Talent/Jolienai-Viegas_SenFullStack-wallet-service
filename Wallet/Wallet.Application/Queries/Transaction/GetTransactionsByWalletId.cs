using MediatR;
using Microsoft.EntityFrameworkCore;
using Wallet.Application.Shared;

namespace Wallet.Application.Queries.Transaction;

public record struct GetTransactionsByWalletId(string Id) : IRequest<IEnumerable<GetTransactionsByWalletIdResponse>>;
public record struct GetTransactionsByWalletIdResponse(Guid Id, Guid WalletId, string Type, decimal Amount, DateTime Timestamp);

public class GetTransactionsByWalletIdHandler (IWalletDbContext context) : IRequestHandler<GetTransactionsByWalletId, IEnumerable<GetTransactionsByWalletIdResponse>>
{
    public async Task<IEnumerable<GetTransactionsByWalletIdResponse>> Handle(GetTransactionsByWalletId request, CancellationToken cancellationToken)
    {
        var transactions = await context.Set<Domain.Entities.Transaction>()
            .AsNoTracking()
            .Where(x => x.WalletId.ToString() == request.Id)
            .ToListAsync(cancellationToken);
        
        if (!transactions.Any())
        {
            return Array.Empty<GetTransactionsByWalletIdResponse>();
        }
        
        return transactions.Select(item =>
            new GetTransactionsByWalletIdResponse
            {
                Id = item.Id,
                WalletId = item.WalletId,
                Type = item.Type.ToString(),
                Amount = item.Amount,
                Timestamp = item.CreatedAt,
            }
        ).ToArray();
    }
}