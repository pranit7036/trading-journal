namespace TradingJournal.Models
{
    public class Trades
    {
        public string Id { get; set; }
        public string Symbol { get; set; }
        public string InstrumentType { get; set; } // Equity, F&O
        public string TradeType { get; set; } // Buy or sell
        public decimal EntryPrice { get; set; }
        public decimal ExitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Stoploss { get; set; }
        public decimal Target { get; set; }
        // Risk to Reward
        public DateTime EntryTime { get; set; }
        public DateTime ExitTime { get; set; }
        public decimal Charges { get; set; }
        public decimal ProfitAndLoss { get; set; }
        public string? Strategy { get; set; }
        public string? Setup { get; set; }
        public string? Notes { get; set; }
    }
}
