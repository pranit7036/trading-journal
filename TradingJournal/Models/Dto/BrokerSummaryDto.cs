namespace TradingJournal.Models.Dto
{
    public class BrokerSummaryDto
    {
        public Guid Id { get; set; }
        public string BrokerName { get; set; } = string.Empty;
        public string ApiKey { get; set; } = string.Empty;
        public bool IsActive { get; set; }
        public DateTime? TokenExpiry { get; set; }
    }
}
