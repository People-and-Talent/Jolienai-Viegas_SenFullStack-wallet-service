using FluentValidation;
using MediatR;
using Wallet.Api.Extensions;
using Wallet.Application;
using Wallet.Application.Commands.AddFunds;
using Wallet.Application.Commands.CreateWallet;
using Wallet.Application.Commands.RemoveFunds;
using Wallet.Application.Queries.Transaction;
using Wallet.Application.Queries.Wallet;
using Wallet.Infrastructure;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddApplication();
builder.Services.AddInfrastructure(builder.Configuration);

var app = builder.Build();
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}
app.UseHttpsRedirection();

app.MapPost("api/v1/wallets", async (CreateWalletRequest request, IValidator<CreateWalletRequest> validator, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid) 
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }
            var id = await mediator.Send(request, cancellationToken);
            return Results.Created($"api/v1/wallet/{id}", new {id});
        }
    )
    .WithName("CreateWallet")
    .WithOpenApi();

app.MapGet("api/v1/wallets/{id}", async(IValidator<GetWalletByIdRequest> validator, IMediator mediator, string id, CancellationToken cancellationToken) =>
    {
        var request = new GetWalletByIdRequest(id);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid) 
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }
        var response = await mediator.Send(request, cancellationToken);
        return Results.Ok(response);
    })
    .WithName("GetWalletById")
    .WithOpenApi();

app.MapPut("api/v1/wallets/{id}/add-funds", async (string id, AddFundsDto dto, IValidator<AddFundsRequest> validator, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var request = new AddFundsRequest(id, dto.Amount, dto.TransactionType);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid) 
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }
            var response = await mediator.Send(request, cancellationToken);
            return Results.Ok(response);
        }
    )
    .WithName("AddFunds")
    .WithOpenApi();

app.MapPut("api/v1/wallets/{id}/remove-funds", async (string id, RemoveFundsDto dto, IValidator<RemoveFundsRequest> validator, IMediator mediator, CancellationToken cancellationToken) =>
        {
            var request = new RemoveFundsRequest(id, dto.Amount, dto.TransactionType);
            var validationResult = await validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid) 
            {
                return Results.ValidationProblem(validationResult.ToDictionary());
            }
            var response = await mediator.Send(request, cancellationToken);
            return Results.Ok(response);
        }
    )
    .WithName("RemoveFunds")
    .WithOpenApi();


app.MapGet("api/v1/wallets/{id}/transactions", async(string id, IValidator<GetTransactionsByWalletId> validator,  IMediator mediator, CancellationToken cancellationToken) =>
    {
        var request = new GetTransactionsByWalletId(id);
        var validationResult = await validator.ValidateAsync(request, cancellationToken);
        if (!validationResult.IsValid) 
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }
        var response = await mediator.Send(request, cancellationToken);
        return Results.Ok(response);
    })
    .WithName("GetTransactionsByWalletId")
    .WithOpenApi();

app.ApplyDbMigrations();
app.Run();


// needed for integration tests
public partial class Program { }