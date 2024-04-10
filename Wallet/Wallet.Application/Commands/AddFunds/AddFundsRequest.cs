namespace Wallet.Application.Commands.AddFunds;

public record struct AddFundsDto(decimal? Amount, string TransactionType);
public record struct AddFundsRequest(string? WalletId, decimal? Amount, string TransactionType) : IRequest<bool>;