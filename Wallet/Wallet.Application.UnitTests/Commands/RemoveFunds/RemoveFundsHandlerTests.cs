namespace Wallet.Application.UnitTests.Commands.RemoveFunds;

public class RemoveFundsHandlerTests(WalletFixture fixture) : IClassFixture<WalletFixture>
{
    [Theory]
    [InlineData("Withdraw")]
    [InlineData("PlacingBet")]
    public async Task Should_Remove_Funds(string transactionType)
    {
        // Arrange
        var walletId = Guid.NewGuid();
        var userId = Guid.NewGuid().ToString();
        await fixture.DbContext.Set<Domain.Entities.Wallet>().AddAsync(new Domain.Entities.Wallet()
        {
            UserId = userId,
            Id = walletId,
            Balance = 10,
            CreatedAt = DateTime.UtcNow
        });
        await fixture.DbContext.SaveChangesAsync();
        var handler = new RemoveFundsHandler(fixture.DbContext);

        const decimal amount = 10;
        var request = new RemoveFundsRequest
        {
            TransactionType = transactionType,
            Amount = amount,
            WalletId = walletId.ToString()
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);
        
        // Assert
        Assert.True(result);
        Assert.True(true);
        
        var wallet = await fixture.DbContext.Set<Domain.Entities.Wallet>().FirstOrDefaultAsync(x => x.Id.Equals(walletId));
        Assert.NotNull(wallet);
        Assert.Equal(0, wallet.Balance);

        var transactions = await fixture.DbContext.Set<Transaction>().Where(x => x.WalletId == walletId).ToListAsync();
        Assert.NotNull(transactions);
        Assert.NotEmpty(transactions);
        Assert.True(transactions.Count == 1);
        Assert.Equal(amount, transactions.First().Amount);
        Assert.Equal(transactionType, transactions.First().Type.ToString());
        var timeDifference = DateTime.UtcNow - transactions.First().CreatedAt;
        Assert.True(timeDifference.TotalSeconds < 1);
    }
    
    [Theory]
    [InlineData("Withdraw")]
    [InlineData("PlacingBet")]
    public async Task Should_Throw_When_Balance_Is_Negative(string transactionType)
    {
        // Arrange
        var walletId = Guid.NewGuid();
        var userId = Guid.NewGuid().ToString();
        var actualBalance = 10;
        await fixture.DbContext.Set<Domain.Entities.Wallet>().AddAsync(
            new Domain.Entities.Wallet
            {
                UserId = userId,
                Id = walletId,
                Balance = actualBalance,
                CreatedAt = DateTime.UtcNow
            }
        );
        await fixture.DbContext.SaveChangesAsync();
        var handler = new RemoveFundsHandler(fixture.DbContext);

        const decimal amountMoreThanActualBalance = 30;
        var request = new RemoveFundsRequest
        {
            TransactionType = transactionType,
            Amount = amountMoreThanActualBalance,
            WalletId = walletId.ToString()
        };

        // Act
        var ex = await Assert.ThrowsAsync<BalanceNegativeException>(() => handler.Handle(request, CancellationToken.None));
        
        // Assert
        Assert.NotNull(ex);
        Assert.Equal("The balance of a wallet cannot be negative", ex.Message);
        
        var wallet = await fixture.DbContext.Set<Domain.Entities.Wallet>().FirstOrDefaultAsync(x => x.Id.Equals(walletId));
        Assert.NotNull(wallet);
        Assert.Equal(actualBalance, wallet.Balance);

        var transactions = await fixture.DbContext.Set<Transaction>().Where(x => x.WalletId == walletId).ToListAsync();
        Assert.Empty(transactions);
    }
}