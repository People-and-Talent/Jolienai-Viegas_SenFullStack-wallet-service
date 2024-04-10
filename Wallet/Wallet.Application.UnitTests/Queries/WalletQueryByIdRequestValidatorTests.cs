namespace Wallet.Application.UnitTests.Queries;

public class WalletQueryByIdRequestValidatorTests (WalletFixture fixture) : IClassFixture<WalletFixture>
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalid-guid")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public async Task Should_Have_Error_For_Invalid_Wallet_Id(string id)
    {
        // Arrange
        var validator = new GetWalletByIdRequestValidator(fixture.DbContext);

        // Act
        var result = await validator.ValidateAsync(new GetWalletByIdRequest(id));

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count == 1);
    }
}

public class WalletQueryByIdHandlerTests(WalletFixture fixture) : IClassFixture<WalletFixture>
{
    [Fact]
    public async Task Should_Return_Wallet()
    {
        // Arrange
        var wallet = new Domain.Entities.Wallet
        {
            Id = Guid.NewGuid(),
            Balance = 1,
            CreatedAt = DateTime.UtcNow,
            UserId = Guid.NewGuid().ToString()
        };
        await fixture.DbContext.Set<Domain.Entities.Wallet>().AddAsync(wallet);
        await fixture.DbContext.SaveChangesAsync();
        var handler = new GetWalletByIdHandler(fixture.DbContext);
        
        // Act
        var response = await handler.Handle(new GetWalletByIdRequest(wallet.Id.ToString()), CancellationToken.None);
        
        // Assert
        Assert.Equal(wallet.Id, response.Id);
        Assert.Equal(wallet.Balance, response.Balance);
        Assert.Equal(wallet.UserId, response.UserId);
    }
}