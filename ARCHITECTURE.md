# TradingJournal Architecture

## High-level structure
The backend is a layered ASP.NET Core Web API organized as:

1) API layer (Controllers) -> 2) Service layer (business logic) -> 3) Repository layer (data access) -> 4) EF Core + PostgreSQL.

Startup wiring, DI, and middleware live in [TradingJournal/Program.cs](TradingJournal/Program.cs).

## Runtime flow
A typical request flows like this:

- HTTP request hits a controller in [TradingJournal/Controllers](TradingJournal/Controllers).
- Controller validates basic input and calls a service via an interface.
- Service performs domain validation, mapping, and orchestration.
- Repository executes database operations via EF Core in [TradingJournal/Context/DBContext.cs](TradingJournal/Context/DBContext.cs).
- Service returns a common `Response` object.

## Layers and responsibilities
### Controllers (API layer)
- Define routes and HTTP verbs.
- Extract auth user ID from JWT claims when needed.
- Return `Response` wrapper to clients.

Implemented controllers:
- Trades: [TradingJournal/Controllers/TradesController.cs](TradingJournal/Controllers/TradesController.cs)
- Users: [TradingJournal/Controllers/UserController.cs](TradingJournal/Controllers/UserController.cs)
- Brokers: [TradingJournal/Controllers/BrokerController.cs](TradingJournal/Controllers/BrokerController.cs)
- Zerodha: [TradingJournal/Controllers/ZerodhaController.cs](TradingJournal/Controllers/ZerodhaController.cs)

### Services (business logic)
- Validation and business rules.
- Calculations like PnL.
- Mapping DTOs <-> Entities.
- Token generation and refresh.

Implemented services:
- Trades: [TradingJournal/Services/TradesService.cs](TradingJournal/Services/TradesService.cs)
- Users: [TradingJournal/Services/UserService.cs](TradingJournal/Services/UserService.cs)
- Brokers: [TradingJournal/Services/BrokerService.cs](TradingJournal/Services/BrokerService.cs)
- Zerodha: [TradingJournal/Services/ZerodhaService.cs](TradingJournal/Services/ZerodhaService.cs)

### Repositories (data access)
- CRUD operations through EF Core DB context.
- No business logic; persistence only.

Implemented repositories:
- Trades: [TradingJournal/Repository/TradesRepository.cs](TradingJournal/Repository/TradesRepository.cs)
- Users: [TradingJournal/Repository/UserRepository.cs](TradingJournal/Repository/UserRepository.cs)
- Brokers: [TradingJournal/Repository/BrokerRepository.cs](TradingJournal/Repository/BrokerRepository.cs)

### Models and mapping
- DTOs define API payloads in [TradingJournal/Models/Dto](TradingJournal/Models/Dto).
- Entities define database shape in [TradingJournal/Models/Entity](TradingJournal/Models/Entity).
- Mappers translate between DTOs and entities in [TradingJournal/Mappers](TradingJournal/Mappers).
- Common response model: [TradingJournal/Models/Reponse.cs](TradingJournal/Models/Reponse.cs).

## Data layer and migrations
- EF Core DbContext defines relationships in [TradingJournal/Context/DBContext.cs](TradingJournal/Context/DBContext.cs).
- PostgreSQL schema is versioned with Evolve SQL scripts in [TradingJournal/Database/Migrations](TradingJournal/Database/Migrations).
- Migrations run at startup using Evolve in [TradingJournal/Program.cs](TradingJournal/Program.cs).

## Authentication and security
- JWT Bearer auth configured in [TradingJournal/Program.cs](TradingJournal/Program.cs).
- User login issues access + refresh tokens in [TradingJournal/Services/UserService.cs](TradingJournal/Services/UserService.cs).
- Refresh tokens are stored with expiry in the user table.
- Passwords are hashed using BCrypt.

## Integrations and background jobs
- Zerodha integration:
  - OAuth-like connect flow and callback in [TradingJournal/Controllers/ZerodhaController.cs](TradingJournal/Controllers/ZerodhaController.cs).
  - Token exchange implemented in [TradingJournal/Services/ZerodhaService.cs](TradingJournal/Services/ZerodhaService.cs).
- Daily background job:
  - 4 PM IST scheduled task in [TradingJournal/Services/ZerodhaIstDailyHostedService.cs](TradingJournal/Services/ZerodhaIstDailyHostedService.cs).

## Configuration
- App configuration in [TradingJournal/appsettings.json](TradingJournal/appsettings.json) and [TradingJournal/appsettings.Development.json](TradingJournal/appsettings.Development.json).
- Includes connection string, JWT settings, and Zerodha settings.

## Operational notes
- Swagger enabled in development via [TradingJournal/Program.cs](TradingJournal/Program.cs).
- CORS policy allows any origin for now.
- Example HTTP calls are in [TradingJournal/TradingJournal.http](TradingJournal/TradingJournal.http).
