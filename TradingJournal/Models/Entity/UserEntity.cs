namespace TradingJournal.Models.Entity
{
    public class UserEntity
    {
        public Guid Id { get; set; }
        public required string UserName { get; set; }
        public required string Email { get; set; }
        public required string Password { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiry { get; set; }
        public ICollection<TradesEntity> Trades { get; set; } = new List<TradesEntity>();
    }
}
