namespace Wallet.Application.Queries.Wallet;

public class GetWalletByIdRequestValidator : AbstractValidator<GetWalletByIdRequest>
{
    private readonly IWalletDbContext _context;

    public GetWalletByIdRequestValidator(IWalletDbContext context)
    {
        _context = context;
        RuleFor(x => x.Id)
            .NotEmpty()
            .WithMessage(" Id must not be empty.");

        RuleFor(x => x.Id)
            .Must(ValidId).WithMessage("Invalid id format")
            .When(x => !string.IsNullOrWhiteSpace(x.Id));
        
        RuleFor(x => x.Id)
            .Must(NotEmptyId)
            .WithMessage("Invalid id format")
            .When(x => ValidId(x.Id));

        RuleFor(x => x.Id)
            .Must(WalletExists)
            .WithMessage("Wallet not found")
            .When(x => ValidId(x.Id) && NotEmptyId(x.Id));
    }
    private static bool ValidId(string id) => Guid.TryParse(id, out _);
    private static bool NotEmptyId(string id) => id != Guid.Empty.ToString();
    private bool WalletExists(string id) => _context.Set<Domain.Entities.Wallet>().Any(x => x.Id.ToString() == id);
}