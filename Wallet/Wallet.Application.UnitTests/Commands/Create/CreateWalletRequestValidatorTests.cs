using Wallet.Application.Commands.CreateWallet;

namespace Wallet.Application.UnitTests.Commands.Create;

public class CreateWalletRequestValidatorTests(WalletFixture fixture) : IClassFixture<WalletFixture>
{
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("invalid-guid")]
    [InlineData("00000000-0000-0000-0000-000000000000")]
    public async Task Should_Have_Error_For_Invalid_UserId(string userId)
    {
        // Arrange
        var validator = new CreateWalletRequestValidator(fixture.DbContext);

        // Act
        var result = await validator.ValidateAsync(new CreateWalletRequest(userId));

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count > 0);
    }

    [Fact]
    public async Task Should_Have_Error_When_User_Has_Wallet_Already()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        await fixture.DbContext.Set<Domain.Entities.Wallet>().AddAsync(new () { UserId = userId });
        await fixture.DbContext.SaveChangesAsync();
        var validator = new CreateWalletRequestValidator(fixture.DbContext);

        // Act
        var result = await validator.ValidateAsync(new CreateWalletRequest(userId));

        // Assert
        Assert.False(result.IsValid);
        Assert.True(result.Errors.Count > 0);
        Assert.Equal("Wallet already exists for user id provided", result.Errors[0].ErrorMessage);
    }

    [Fact]
    public async Task Should_Create_Wallet()
    {
        // Arrange
        var userId = Guid.NewGuid().ToString();
        var validator = new CreateWalletRequestValidator(fixture.DbContext);

        // Act
        var result = await validator.ValidateAsync(new CreateWalletRequest(userId));

        // Assert
        Assert.True(result.IsValid);
        Assert.True(result.Errors.Count == 0);
    }
}