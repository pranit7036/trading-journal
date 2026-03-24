ALTER TABLE "Trades"
ADD COLUMN "UserId" UUID;

ALTER TABLE "Trades"
ADD CONSTRAINT "FK_Trades_Users_UserId"
FOREIGN KEY ("UserId") REFERENCES "Users"("Id")
ON DELETE CASCADE;

CREATE INDEX "IX_Trades_UserId"
ON "Trades"("UserId");
