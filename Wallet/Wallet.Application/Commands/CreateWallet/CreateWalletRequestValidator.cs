namespace Wallet.Application.Commands.CreateWallet;

public class CreateWalletRequestValidator : AbstractValidator<CreateWalletRequest>
{
    private readonly IWalletDbContext _context;

    public CreateWalletRequestValidator(IWalletDbContext context)
    {
        _context = context;
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User Id cannot be empty")
            .Must(ValidUserId).WithMessage("Invalid user Id format")
            .Must(NotEmptyUserId).WithMessage("Invalid user id");

        RuleFor(x => x.UserId)
            .Must(WalletNotExistsForUserId).WithMessage("Wallet already exists for user id provided")
            .When(x => ValidUserId(x.UserId))
            .When(x => NotEmptyUserId(x.UserId));
    }
    private static bool ValidUserId(string userId) => Guid.TryParse(userId, out _);
    private static bool NotEmptyUserId(string userId) => userId != Guid.Empty.ToString();
    private bool WalletNotExistsForUserId(string userId) => !_context.Set<Domain.Entities.Wallet>().Any(x => x.UserId == userId);
}