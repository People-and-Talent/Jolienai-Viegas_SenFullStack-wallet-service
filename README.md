# wallet-service
Simple rest API in net core 8 for managing wallets following Clear architecture, CQRS with Postgres database, and MediaTR, EF core, Fluent Validation, and TestContainer are some packages used.

## Database
Run the docker compose file to get the database ready before starting the api.

```docker-compose -f docker-compose-local.yml up -d ```

## Database migrations
Just an example how to run the command to create the migration as I am using the code-first approach.

``` dotnet ef migrations add "Initial" --project Wallet.Infrastructure --startup-project Wallet.Api --output-dir Database/Migrations ```


## Unit tests
I am using an in-memory database, as the focus is to validate the business requirements are correctly implemented. 

## Integration tests
I am using Testcontainers and Testcontainers.PostgreSql nuget packages to test using a real Postgres database instance in place for each test isolated.


Note: 
    There is a wallet.postman_collection.json file with all endpoints calls, anyway we can also do using the swagger page displayed when start the application in development environment.