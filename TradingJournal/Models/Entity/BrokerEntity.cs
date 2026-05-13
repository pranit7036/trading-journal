namespace TradingJournal.Models.Entity
{
    public class BrokerEntity
    {
        public Guid Id { get; set; }
        public string BrokerName { get; set; }
        public Guid UserID { get; set; }
        public string ApiKey { get; set; }
        public string ApiSecret { get; set; } //encrypted
        public string AccessToken { get; set; } //encrypted
        public string RefreshToken { get; set; } //encrypted
        public DateTime TokenExpiry { get; set; }
        public bool IsActive { get; set; }
        public UserEntity User { get; set; } = null!;
    }
}
