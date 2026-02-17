using TradingJournal.Models.Entity;

namespace TradingJournal.Interfaces.Repository
{
    public interface ITradeRepository
    {
        public Task<bool> Save(TradesEntity tradesEntity);
        public Task<List<TradesEntity>> GetData();
    }
}
