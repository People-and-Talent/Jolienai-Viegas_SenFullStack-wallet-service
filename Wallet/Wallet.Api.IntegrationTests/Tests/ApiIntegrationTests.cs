using System.Net;
using System.Net.Http.Json;
using Microsoft.EntityFrameworkCore;
using Wallet.Api.IntegrationTests.Api.Responses;
using Wallet.Application.Commands.AddFunds;
using Wallet.Application.Commands.CreateWallet;
using Wallet.Application.Commands.RemoveFunds;
using Wallet.Application.Queries.Transaction;
using Wallet.Application.Queries.Wallet;

namespace Wallet.Api.IntegrationTests.Tests;

// bad request scenarios were omitted for simplicity, the idea is to show how can we do integration tests
// with TestContainers in place, so we dont need use docker-compose file for it.
public class ApiIntegrationTests(IntegrationTestWebAppFactory factory) : BaseIntegrationTest(factory)
{
    [Fact]
    public async Task Should_Create_Wallet()
    {
        // Arrange
        var client = factory.CreateClient();
        var request = new CreateWalletRequest
        {
            UserId = Guid.NewGuid().ToString(),
            ProviderId = "any_provider_id"
        };
        
        // Action
        var response = await client.PostAsJsonAsync("/api/v1/wallets", request);
        
        // Assertions
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var createdWalletResponse = await response.Content.ReadFromJsonAsync<ApiCreatedWalletResponse>();
        Assert.NotNull(createdWalletResponse);
        Assert.True(Guid.TryParse(createdWalletResponse.Id, out _));
        
        var wallet = await DbContext.Set<Domain.Entities.Wallet>().FirstOrDefaultAsync(x => x.UserId == request.UserId);
        Assert.NotNull(wallet);
        Assert.Equal(request.ProviderId, wallet.ProviderId);
        Assert.Equal(0, wallet.Balance);
    }

    [Fact]
    public async Task Should_Get_Wallet_By_Id()
    {
        // Arrange
        var command = new CreateWalletRequest { UserId = Guid.NewGuid().ToString(), ProviderId = "any_provider_id" };
        var walletId = await Sender.Send(command);
        var client = factory.CreateClient();
        
        // Action
        var response = await client.GetAsync($"api/v1/wallets/{walletId}");
        var wallet = await response.Content.ReadFromJsonAsync<GetWalletByIdResponse>();
        
        // Assertions
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(command.UserId, wallet.UserId);
        Assert.Equal(command.ProviderId, wallet.ProviderId);
        Assert.Equal(0, wallet.Balance);
    }

    [Fact]
    public async Task Should_Add_Funds()
    {
        // Arrange
        var walletId = await Sender.Send(new CreateWalletRequest { UserId = Guid.NewGuid().ToString(), ProviderId = "any_provider_id" });
        var id = walletId.ToString();
        var amount = 1000;
        var client = factory.CreateClient();
        var request = new AddFundsDto(amount, "Deposit");
        
        // Action
        var response = await client.PutAsJsonAsync($"api/v1/wallets/{id}/add-funds", request);
        
        // Assertions
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var wallet = await DbContext.Set<Domain.Entities.Wallet>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == walletId);
        Assert.NotNull(wallet);
        Assert.Equal(amount, wallet.Balance);
    }
    
    [Fact]
    public async Task Should_Remove_Funds()
    {
        // Arrange
        var walletId = await Sender.Send(new CreateWalletRequest { UserId = Guid.NewGuid().ToString(), ProviderId = "any_provider_id" });
        var id = walletId.ToString();
        const decimal amount = 1000;
        _ = await Sender.Send(new AddFundsRequest(id, amount, "Deposit"));
        
        const decimal withdrawAmount = 100;
        var client = factory.CreateClient();
        var request = new RemoveFundsDto(withdrawAmount, "Withdraw");
        
        // Action
        var response = await client.PutAsJsonAsync($"api/v1/wallets/{id}/remove-funds", request);
        
        // Assertions
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        
        var wallet = await DbContext.Set<Domain.Entities.Wallet>().AsNoTracking().FirstOrDefaultAsync(x => x.Id == walletId);
        Assert.NotNull(wallet);
        Assert.Equal(amount - withdrawAmount, wallet.Balance);
    }

    [Fact]
    public async Task Should_Get_Transactions()
    {
        // Arrange
        var walletId = await Sender.Send(new CreateWalletRequest { UserId = Guid.NewGuid().ToString(), ProviderId = "any_provider_id" });
        var id = walletId.ToString();
        const decimal depositAmount = 1000;
        _ = await Sender.Send(new AddFundsRequest(id, depositAmount, "Deposit"));
        
        const decimal withdrawAmount = 100;
        _ = await Sender.Send(new RemoveFundsRequest(id, withdrawAmount, "Withdraw"));
        
         var client = factory.CreateClient();
         
         // Action
         var response = await client.GetAsync($"api/v1/wallets/{id}/transactions");
         var transactions = await response.Content.ReadFromJsonAsync<List<GetTransactionsByWalletIdResponse>>();
         
         // Assertions
         Assert.Equal(HttpStatusCode.OK, response.StatusCode);
         Assert.NotNull(transactions);
         Assert.True(transactions.Count == 2);

         var deposit = transactions.FirstOrDefault(x => x.WalletId == walletId && x.Type == "Deposit");
         Assert.Equal(depositAmount, deposit.Amount);
         
         var withdraw = transactions.FirstOrDefault(x => x.WalletId == walletId && x.Type == "Withdraw");
         Assert.Equal(withdrawAmount, withdraw.Amount);
    }
}