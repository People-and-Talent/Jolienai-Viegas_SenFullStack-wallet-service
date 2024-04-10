using FluentValidation;
using Wallet.Application.Shared;
using Wallet.Domain.Enum;

namespace Wallet.Application.Commands.AddFunds;

public class AddFundsRequestValidator : AbstractValidator<AddFundsRequest>
{
    private readonly IWalletDbContext _context;

    public AddFundsRequestValidator(IWalletDbContext context)
    {
        _context = context;
        RuleFor(x => x.TransactionType).NotNull().NotEmpty().WithMessage("Transaction type must be not null or empty");

        RuleFor(x => x.TransactionType)
            .Must(ValidTransactionType).WithMessage("Invalid transaction type value.")
            .When(x => !string.IsNullOrWhiteSpace(x.TransactionType));

        RuleFor(x => x.Amount).NotNull().WithMessage("Amount must be greater than 0").GreaterThan(0).WithMessage("Amount must be greater than 0");

        RuleFor(x => x.WalletId)
            .Must(ValidWalletId)
            .WithMessage("Wallet id format invalid");
            
        RuleFor(x => x.WalletId)
            .Must(NotEmptyWalletId)
            .WithMessage("Wallet id must be an empty guid");

        RuleFor(x => x.WalletId)
            .Must(WalletExists).WithMessage("Wallet not found for the id provided")
            .When(x => ValidWalletId(x.WalletId))
            .When(x => NotEmptyWalletId(x.WalletId));
    }
    
    private static bool ValidWalletId(string id) => Guid.TryParse(id, out _);
    
    private static bool NotEmptyWalletId(string userId) => userId != Guid.Empty.ToString();
    
    private static bool ValidTransactionType(string value)
    {
        if (!Enum.TryParse<TransactionType>(value, out var type))
        {
            return false;
        }

        return type is TransactionType.Deposit or TransactionType.WinningBet or TransactionType.Bonus;
    }
    
    private bool WalletExists(string id) => _context.Set<Domain.Entities.Wallet>().Any(x => x.Id.ToString() == id);
}