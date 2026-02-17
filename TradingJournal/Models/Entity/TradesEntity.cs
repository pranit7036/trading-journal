namespace TradingJournal.Models.Entity
{
    public class TradesEntity
    {
        public string Id { get; set; } //guid
        public string Symbol { get; set; } // name of stock i.e. symbol of stock
        public string InstrumentType { get; set; } // Equity, F&O 
        // order type MIS or NORMAL
        public string TradeType { get; set; } // Buy or sell
        public decimal EntryPrice { get; set; } 
        public decimal ExitPrice { get; set; }
        public int Quantity { get; set; }
        public decimal Stoploss { get; set; }
        public decimal Target { get; set; }
        // Risk to Reward
        public DateTime EntryTime { get; set; }
        //validate date and time
        public DateTime ExitTime { get; set; }
        //validate date and time
        public decimal Charges { get; set; }
        public decimal ProfitAndLoss { get; set; }
        public string? Strategy { get; set; }
        public string? Setup { get; set; }
        public string? Notes { get; set; }
    }
}
