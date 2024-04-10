namespace Wallet.Application.Queries.Wallet;

public class GetWalletByIdHandler(IWalletDbContext context) : IRequestHandler<GetWalletByIdRequest, GetWalletByIdResponse>
{
    public async Task<GetWalletByIdResponse> Handle(GetWalletByIdRequest request, CancellationToken cancellationToken)
    {
        var wallet = await context.Set<Domain.Entities.Wallet>()
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id.ToString() == request.Id, cancellationToken);

        return new GetWalletByIdResponse
        {
            Id = wallet!.Id,
            UserId = wallet.UserId,
            Balance = wallet.Balance,
            ProviderId = wallet.ProviderId
        };
    }
}