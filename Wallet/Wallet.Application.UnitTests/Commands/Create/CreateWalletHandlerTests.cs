using Wallet.Application.Commands.CreateWallet;

namespace Wallet.Application.UnitTests.Commands.Create;

public class CreateWalletHandlerTests (WalletFixture fixture) : IClassFixture<WalletFixture>
{
    [Theory]
    [InlineData("")]
    [InlineData(null)]
    [InlineData("provider-id")]
    public async Task Should_Create_Wallet(string providerId)
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var handler = new CreateWalletHandler(fixture.DbContext);

        // Act
        var walletId = await handler.Handle(new CreateWalletRequest(userId, providerId), CancellationToken.None);

        // Assert
        Assert.True(walletId != Guid.Empty);
        var wallet = fixture.DbContext.Set<Domain.Entities.Wallet>().FirstOrDefault(x => x.UserId == userId);
        Assert.NotNull(wallet);
        Assert.True(wallet.Id == walletId);
        Assert.True(wallet.Balance == 0);
        Assert.Equal(providerId, wallet.ProviderId);
    }
}