using FluentValidation;
using Wallet.Application.Shared;
using Wallet.Domain.Enum;

namespace Wallet.Application.Commands.RemoveFunds;

public class RemoveFundsRequestValidator : AbstractValidator<RemoveFundsRequest>
{
    private readonly IWalletDbContext _context;
    
    public RemoveFundsRequestValidator(IWalletDbContext context)
    {
        _context = context;
        RuleFor(x => x.WalletId).NotNull().NotEmpty().WithMessage("Wallet id cannot be null or empty");

        RuleFor(x => x.Amount)
            .NotNull()
            .WithMessage("Amount must not be empty")
            .NotEmpty()
            .WithMessage("Amount must not be empty");
            
        RuleFor(x => x.Amount)
            .GreaterThan(0)
            .WithMessage("Amount must be greater than 0");
        
        RuleFor(x => x.WalletId)
            .Must(ValidWalletId)
            .WithMessage("Wallet id format invalid");

        RuleFor(x => x.WalletId)
            .Must(NotEmptyWalletId)
            .WithMessage("Wallet id must be an empty guid")
            .When(x => ValidWalletId(x.WalletId));
        
        RuleFor(x => x.TransactionType)
            .NotNull().WithMessage("Transaction type must be not null or empty")
            .NotEmpty().WithMessage("Transaction type must be not null or empty");

        RuleFor(x => x.TransactionType)
            .Must(ValidTransactionType).WithMessage("Invalid transaction type value.");
        
        RuleFor(x => x.WalletId)
            .Must(WalletExists).WithMessage("Wallet not found for the id provided")
            .When(x => ValidWalletId(x.WalletId))
            .When(x => NotEmptyWalletId(x.WalletId));

        RuleFor(x => x.WalletId)
            .Must(BalanceGreaterThanZero).WithMessage("Insufficient funds")
            .When(x => WalletExists(x.WalletId));
        
        RuleFor(x => x.WalletId)
            .Must(BalanceGreaterThanZero).WithMessage("Insufficient funds")
            .When(x => WalletExists(x.WalletId));
    }
    private static bool ValidWalletId(string id) => Guid.TryParse(id, out _);
    private static bool NotEmptyWalletId(string userId) => userId != Guid.Empty.ToString();
    private static bool ValidTransactionType(string value)
    {
        if (!Enum.TryParse<TransactionType>(value, out var type))
        {
            return false;
        }
        return type is TransactionType.Withdraw or TransactionType.PlacingBet;
    }
    private bool WalletExists(string id) => _context.Set<Domain.Entities.Wallet>().Any(x => x.Id.ToString() == id);
    private bool BalanceGreaterThanZero(string id)
    {
        var wallet = _context.Set<Domain.Entities.Wallet>().First(x => x.Id.ToString() == id);
        return wallet!.Balance > 0;
    }
}