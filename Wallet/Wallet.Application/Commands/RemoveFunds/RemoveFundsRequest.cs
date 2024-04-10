namespace Wallet.Application.Commands.RemoveFunds;

public record struct RemoveFundsDto(decimal? Amount, string TransactionType);
public record struct RemoveFundsRequest(string WalletId, decimal? Amount, string TransactionType) : IRequest<bool>;