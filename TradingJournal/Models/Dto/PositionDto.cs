using System.Text.Json.Serialization;

namespace TradingJournal.Models.Dto
{
    public class PositionDto
    {
        [JsonPropertyName("tradingsymbol")]
        public string TradingSymbol { get; set; } = string.Empty;

        [JsonPropertyName("exchange")]
        public string Exchange { get; set; } = string.Empty;

        [JsonPropertyName("instrument_token")]
        public long InstrumentToken { get; set; }

        [JsonPropertyName("product")]
        public string Product { get; set; } = string.Empty;

        [JsonPropertyName("quantity")]
        public long Quantity { get; set; }

        [JsonPropertyName("overnight_quantity")]
        public long OvernightQuantity { get; set; }

        [JsonPropertyName("multiplier")]
        public long Multiplier { get; set; }

        [JsonPropertyName("average_price")]
        public decimal AveragePrice { get; set; }

        [JsonPropertyName("close_price")]
        public decimal ClosePrice { get; set; }

        [JsonPropertyName("last_price")]
        public decimal LastPrice { get; set; }

        [JsonPropertyName("value")]
        public decimal Value { get; set; }

        [JsonPropertyName("pnl")]
        public decimal Pnl { get; set; }

        [JsonPropertyName("m2m")]
        public decimal M2m { get; set; }

        [JsonPropertyName("unrealised")]
        public decimal Unrealised { get; set; }

        [JsonPropertyName("realised")]
        public decimal Realised { get; set; }

        [JsonPropertyName("buy_quantity")]
        public long BuyQuantity { get; set; }

        [JsonPropertyName("buy_price")]
        public decimal BuyPrice { get; set; }

        [JsonPropertyName("buy_value")]
        public decimal BuyValue { get; set; }

        [JsonPropertyName("buy_m2m")]
        public decimal BuyM2m { get; set; }

        [JsonPropertyName("sell_quantity")]
        public long SellQuantity { get; set; }

        [JsonPropertyName("sell_price")]
        public decimal SellPrice { get; set; }

        [JsonPropertyName("sell_value")]
        public decimal SellValue { get; set; }

        [JsonPropertyName("sell_m2m")]
        public decimal SellM2m { get; set; }

        [JsonPropertyName("day_buy_quantity")]
        public long DayBuyQuantity { get; set; }

        [JsonPropertyName("day_buy_price")]
        public decimal DayBuyPrice { get; set; }

        [JsonPropertyName("day_buy_value")]
        public decimal DayBuyValue { get; set; }

        [JsonPropertyName("day_sell_quantity")]
        public long DaySellQuantity { get; set; }

        [JsonPropertyName("day_sell_price")]
        public decimal DaySellPrice { get; set; }

        [JsonPropertyName("day_sell_value")]
        public decimal DaySellValue { get; set; }
    }

    public class PositionsResponseDto
    {
        [JsonPropertyName("net")]
        public List<PositionDto> Net { get; set; } = new();

        [JsonPropertyName("day")]
        public List<PositionDto> Day { get; set; } = new();
    }
}
