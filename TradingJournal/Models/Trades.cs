namespace TradingJournal.Models
{
    public class Trades
    {
        public string Id { get; set; }
        public string Symbol { get; set; }
        public string InstrumentType { get; set; } // Equity, F&O
        public string TradeType { get; set; } // Buy or sell
        public float EntryPrice { get; set; }
        public float ExitPrice { get; set; }
        public int Quantity { get; set; }
        public float Stoploss { get; set; }
        public float Target { get; set; }
        // Risk to Reward
        public DateTime Entry { get; set; }
        public DateTime Exit { get; set; }
        public float Charges { get; set; }
        public float ProfitAndLoss { get; set; }
        public string Strategy { get; set; }
        public string Setup { get; set; }
        public string Notes { get; set; }
    }
}
