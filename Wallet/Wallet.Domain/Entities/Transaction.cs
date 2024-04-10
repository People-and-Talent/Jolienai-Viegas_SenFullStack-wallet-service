using Wallet.Domain.Enum;

namespace Wallet.Domain.Entities;

public class Transaction : Audit
{
    public Guid Id { get; set; }
    public Guid WalletId { get; set; }
    public virtual Wallet Wallet { get; set; } = default!;
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }
}