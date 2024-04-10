using MediatR;

namespace Wallet.Application.Commands.CreateWallet;

public record struct CreateWalletRequest(string UserId, string? ProviderId = null) : IRequest<Guid>;
