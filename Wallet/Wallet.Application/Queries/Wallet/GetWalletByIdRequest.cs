namespace Wallet.Application.Queries.Wallet;

public record struct GetWalletByIdRequest(string Id) : IRequest<GetWalletByIdResponse>;