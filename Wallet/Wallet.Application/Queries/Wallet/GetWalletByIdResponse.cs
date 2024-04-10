namespace Wallet.Application.Queries.Wallet;

public record struct GetWalletByIdResponse(Guid Id, string UserId, decimal Balance, string? ProviderId = null);