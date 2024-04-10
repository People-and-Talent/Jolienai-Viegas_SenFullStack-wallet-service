namespace Wallet.Application.UnitTests.Commands.AddFunds;

public class AddFundsRequestValidatorTests(WalletFixture fixture) : IClassFixture<WalletFixture>
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalid-transaction-type")]
    [InlineData("Withdraw")]
    [InlineData("PlacingBet")]
    public async Task Should_Return_Error_When_TransactionType_Invalid(string transactionType)
    {
        // Arrange
        var walletId = Guid.NewGuid();
        var userId = Guid.NewGuid().ToString();
        await fixture.DbContext.Set<Domain.Entities.Wallet>().AddAsync(new Domain.Entities.Wallet()
        {
            UserId = userId,
            Id = walletId,
            Balance = 0,
            CreatedAt = DateTime.UtcNow
        });
        await fixture.DbContext.SaveChangesAsync();
        
        var validator = new AddFundsRequestValidator(fixture.DbContext);
        
        // Act
        var result = await validator.ValidateAsync(new AddFundsRequest(walletId.ToString(), 10, transactionType));

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count > 0);
    }
    
    [Fact]
    public async Task Should_Return_Error_When_Wallet_Not_Exists()
    {
        // Arrange
        var walletId = Guid.NewGuid();
        var validator = new AddFundsRequestValidator(fixture.DbContext);
        
        // Act
        var result = await validator.ValidateAsync(new AddFundsRequest(walletId.ToString(), 10, TransactionType.Deposit.ToString()));

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count > 0);
        Assert.Equal("Wallet not found for the id provided", result.Errors[0].ErrorMessage);
    }
    
    [Fact]
    public async Task Should_Return_Error_When_Amount_Invalid()
    {
        // Arrange
        var walletId = Guid.NewGuid();
        var userId = Guid.NewGuid().ToString();
        await fixture.DbContext.Set<Domain.Entities.Wallet>().AddAsync(new Domain.Entities.Wallet()
        {
            UserId = userId,
            Id = walletId,
            Balance = 0,
            CreatedAt = DateTime.UtcNow
        });
        await fixture.DbContext.SaveChangesAsync();
        var validator = new AddFundsRequestValidator(fixture.DbContext);
        
        // Act
        var result = await validator.ValidateAsync(new AddFundsRequest(walletId.ToString(), 0, TransactionType.Deposit.ToString()));

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count > 0);
        Assert.Equal("Amount must be greater than 0", result.Errors[0].ErrorMessage);
        
        // Act
        result = await validator.ValidateAsync(new AddFundsRequest(walletId.ToString(), null, TransactionType.Deposit.ToString()));
        
        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count > 0);
        Assert.Equal("Amount must be greater than 0", result.Errors[0].ErrorMessage);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalid-guid")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public async Task Should_Return_Error_When_WalletId_Invalid(string walletId)
    {
        // Arrange
        var validator = new AddFundsRequestValidator(fixture.DbContext);
        
        // Act
        var result = await validator.ValidateAsync(new AddFundsRequest(walletId, 100, TransactionType.Deposit.ToString()));
        
        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count > 0);
    }
    
    [Theory]
    [InlineData("Deposit")]
    [InlineData("WinningBet")]
    [InlineData("Bonus")]
    public async Task Should_Return_No_Error(string transactionType)
    {
        // Arrange
        var walletId = Guid.NewGuid();
        var userId = Guid.NewGuid().ToString();
        await fixture.DbContext.Set<Domain.Entities.Wallet>().AddAsync(new Domain.Entities.Wallet()
        {
            UserId = userId,
            Id = walletId,
            Balance = 0,
            CreatedAt = DateTime.UtcNow
        });
        await fixture.DbContext.SaveChangesAsync();
        
        var validator = new AddFundsRequestValidator(fixture.DbContext);
        
        // Act
        var result = await validator.ValidateAsync(new AddFundsRequest(walletId.ToString(), 10, transactionType));

        // Assert
        Assert.True(result.IsValid);
        Assert.True(result.Errors.Count == 0);
    }
}