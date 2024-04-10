namespace Wallet.Domain.Entities;

public abstract class Audit
{
    public DateTime CreatedAt { get; set; }
    public uint Version { get; set; }
}