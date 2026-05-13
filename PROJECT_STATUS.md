# TradingJournal Project Status (2026-05-12)

## Overview
- ASP.NET Core Web API for trade journaling with JWT auth, refresh tokens, EF Core + PostgreSQL, and Zerodha integration.
- Layered design: Controllers -> Services -> Repositories -> EF Core.
- Migrations managed by Evolve on startup.

## Startup and configuration
- DI and middleware are configured in [TradingJournal/Program.cs](TradingJournal/Program.cs).
- JWT settings are in [TradingJournal/appsettings.json](TradingJournal/appsettings.json) and [TradingJournal/appsettings.Development.json](TradingJournal/appsettings.Development.json).
- Evolve runs SQL migrations from [TradingJournal/Database/Migrations](TradingJournal/Database/Migrations).

## Data model
- Entities are defined in [TradingJournal/Models/Entity](TradingJournal/Models/Entity): users, trades, brokers.
- Relationships are defined in [TradingJournal/Context/DBContext.cs](TradingJournal/Context/DBContext.cs): users to trades (1-many), users to brokers (1-many), unique broker per user and broker name.

## API endpoints
### Users
- POST /api/user/register -> create user
- POST /api/user/login -> returns access and refresh tokens
- POST /api/user/refresh-token -> rotates tokens

### Trades (JWT required)
- POST /api/trades/add -> validate and save
- GET /api/trades/getdata -> list all trades
- PATCH /api/trades/edit/{id} -> partial update
- DELETE /api/trades/deleteall -> delete all
- DELETE /api/trades/delete/{id} -> delete one

### Brokers (JWT required)
- POST /api/add/broker -> save broker credentials
- PATCH /api/update/broker/{id} -> update broker credentials

### Zerodha integration
- GET /api/integration/kite/connect-url -> returns connect login URL
- GET /api/integration/kite/callback -> exchanges request_token for access_token

## Services and main logic
- Trades: validation, market hours check, PnL calc, CRUD in [TradingJournal/Services/TradesService.cs](TradingJournal/Services/TradesService.cs).
- Users: register, login, refresh tokens in [TradingJournal/Services/UserService.cs](TradingJournal/Services/UserService.cs).
- Brokers: input validation and save/update in [TradingJournal/Services/BrokerService.cs](TradingJournal/Services/BrokerService.cs).
- Zerodha: token exchange in [TradingJournal/Services/ZerodhaService.cs](TradingJournal/Services/ZerodhaService.cs).
- Daily job: 4 PM IST token refresh in [TradingJournal/Services/ZerodhaIstDailyHostedService.cs](TradingJournal/Services/ZerodhaIstDailyHostedService.cs).

## Repositories
- Trades data access in [TradingJournal/Repository/TradesRepository.cs](TradingJournal/Repository/TradesRepository.cs).
- Users data access in [TradingJournal/Repository/UserRepository.cs](TradingJournal/Repository/UserRepository.cs).
- Brokers data access in [TradingJournal/Repository/BrokerRepository.cs](TradingJournal/Repository/BrokerRepository.cs).

## Migrations history
- Trades table created and adjusted in [TradingJournal/Database/Migrations](TradingJournal/Database/Migrations) (table creation, time zone adjustments, user foreign key).
- Users table and refresh token columns in [TradingJournal/Database/Migrations](TradingJournal/Database/Migrations).
- Brokers table and unique index per user and broker name in [TradingJournal/Database/Migrations](TradingJournal/Database/Migrations).

## Notable behaviors implemented
- Trades validate entry and exit time in market hours and entry time before exit time.
- PnL calculation for buy vs sell trades.
- Passwords are stored hashed with BCrypt.
- JWT access tokens include sub and email claims.
- Refresh tokens are stored and rotated on refresh.

## Gaps and TODO candidates
- Charges calculation is still TODO; currently accepted from the client.
- Edit trade does not re-validate market hours or recalc PnL when prices or times change.
- Trade listing is not filtered by user.
- Zerodha callback returns token data but does not persist tokens to broker records.
- Broker update ignores the route id and uses only the DTO.
