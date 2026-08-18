using System.Text.Json.Serialization;

namespace TradingJournal.Models.Dto
{
    public class HoldingDto
    {
        [JsonPropertyName("tradingsymbol")]
        public string TradingSymbol { get; set; } = string.Empty;

        [JsonPropertyName("exchange")]
        public string Exchange { get; set; } = string.Empty;

        [JsonPropertyName("instrument_token")]
        public long InstrumentToken { get; set; }

        [JsonPropertyName("isin")]
        public string Isin { get; set; } = string.Empty;

        [JsonPropertyName("product")]
        public string Product { get; set; } = string.Empty;

        [JsonPropertyName("price")]
        public decimal Price { get; set; }

        [JsonPropertyName("quantity")]
        public long Quantity { get; set; }

        [JsonPropertyName("used_quantity")]
        public long UsedQuantity { get; set; }

        [JsonPropertyName("t1_quantity")]
        public long T1Quantity { get; set; }

        [JsonPropertyName("realised_quantity")]
        public long RealisedQuantity { get; set; }

        [JsonPropertyName("authorised_quantity")]
        public long AuthorisedQuantity { get; set; }

        [JsonPropertyName("authorised_date")]
        public string? AuthorisedDate { get; set; }

        [JsonPropertyName("opening_quantity")]
        public long OpeningQuantity { get; set; }

        [JsonPropertyName("short_quantity")]
        public long ShortQuantity { get; set; }

        [JsonPropertyName("collateral_quantity")]
        public long CollateralQuantity { get; set; }

        [JsonPropertyName("collateral_type")]
        public string? CollateralType { get; set; }

        [JsonPropertyName("discrepancy")]
        public bool Discrepancy { get; set; }

        [JsonPropertyName("average_price")]
        public decimal AveragePrice { get; set; }

        [JsonPropertyName("last_price")]
        public decimal LastPrice { get; set; }

        [JsonPropertyName("close_price")]
        public decimal ClosePrice { get; set; }

        [JsonPropertyName("pnl")]
        public decimal Pnl { get; set; }

        [JsonPropertyName("day_change")]
        public decimal DayChange { get; set; }

        [JsonPropertyName("day_change_percentage")]
        public decimal DayChangePercentage { get; set; }
    }
}
