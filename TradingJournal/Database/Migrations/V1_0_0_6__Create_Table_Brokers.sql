CREATE TABLE "Brokers" (
    "Id" UUID PRIMARY KEY,
    "BrokerName" VARCHAR(100) NOT NULL,
    "UserID" UUID NOT NULL,
    "ApiKey" VARCHAR(255) NOT NULL,
    "ApiSecret" VARCHAR(255) NOT NULL,
    "AccessToken" VARCHAR(500) NOT NULL,
    "RefreshToken" VARCHAR(500) NOT NULL,
    "TokenExpiry" TIMESTAMP NOT NULL,
    "IsActive" BOOLEAN NOT NULL,
    CONSTRAINT "FK_Brokers_Users_UserID"
        FOREIGN KEY ("UserID") REFERENCES "Users"("Id")
        ON DELETE CASCADE
);

CREATE INDEX "IX_Brokers_UserID"
ON "Brokers"("UserID");

CREATE UNIQUE INDEX "UX_Brokers_UserID_BrokerName"
ON "Brokers"("UserID", "BrokerName");
