namespace Wallet.Application.UnitTests.Commands.AddFunds;

public class AddFundsHandlerTests(WalletFixture fixture) : IClassFixture<WalletFixture>
{
    [Theory]
    [InlineData("Deposit")]
    [InlineData("WinningBet")]
    [InlineData("Bonus")]
    public async Task Should_Add_Found(string transactionType)
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
        var handler = new AddFundsHandler(fixture.DbContext);

        var request = new AddFundsRequest
        {
            TransactionType = transactionType,
            Amount = 1,
            WalletId = walletId.ToString()
        };

        // Act
        var result = await handler.Handle(request, CancellationToken.None);
        
        // Assert
        Assert.True(result);

        var wallet = await fixture.DbContext.Set<Domain.Entities.Wallet>().FirstOrDefaultAsync(x => x.Id.Equals(walletId));
        Assert.NotNull(wallet);
        Assert.Equal(1, wallet.Balance);

        var transactions = await fixture.DbContext.Set<Transaction>().Where(x => x.WalletId == walletId).ToListAsync();
        Assert.NotNull(transactions);
        Assert.NotEmpty(transactions);
        Assert.True(transactions.Count == 1);
        Assert.Equal(1, transactions.First().Amount);
        Assert.Equal(transactionType, transactions.First().Type.ToString());
        var timeDifference = DateTime.UtcNow - transactions.First().CreatedAt;
        Assert.True(timeDifference.TotalSeconds < 1);
    }
}