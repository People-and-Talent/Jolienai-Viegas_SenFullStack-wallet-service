namespace Wallet.Domain.Entities;

public class Wallet : Audit
{
    public Guid Id { get; set; }
    public string UserId { get; set; } = default!;
    public decimal Balance { get; set; }
    
    /// <summary>
    /// Can use for third-party integration, but not mandatory.
    /// </summary>
    public string? ProviderId { get; set; } = null;
}