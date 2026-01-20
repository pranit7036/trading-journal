CREATE TABLE "Trades" (
    "Id" VARCHAR(36) PRIMARY KEY,
    "Symbol" VARCHAR(50),
    "InstrumentType" VARCHAR(20),
    "TradeType" VARCHAR(10),    
    "EntryPrice" DECIMAL(10,2),
    "ExitPrice" DECIMAL(10,2),
    "Quantity" INT,
    "Stoploss" DECIMAL(10,2),
    "Target" DECIMAL(10,2),
    "EntryTime" TIMESTAMP,
    "ExitTime" TIMESTAMP,
    "Charges" DECIMAL(10,2),
    "ProfitAndLoss" DECIMAL(12,2),
    "Strategy" VARCHAR(100),
    "Setup" VARCHAR(100),
    "Notes" TEXT
);
