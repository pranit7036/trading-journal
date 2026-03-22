namespace TradingJournal.Models.Dto
{
    public class TradesDto
    {
        public Guid? Id { get; set; }
        public required string Symbol { get; set; }
        public required string InstrumentType { get; set; } // Equity, F&O
        public required string TradeType { get; set; } // Buy or sell
        public decimal EntryPrice { get; set; }
        public decimal ExitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Stoploss { get; set; }
        public decimal Target { get; set; }
        public DateTime EntryTime { get; set; }
        public DateTime ExitTime { get; set; }
        public decimal Charges { get; set; } // will calculate this later
        // public decimal ProfitAndLoss { get; set; }
        public string? Strategy { get; set; }
        public string? Setup { get; set; }
        public string? Notes { get; set; }
    }
}
