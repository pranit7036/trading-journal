using TradingJournal.Models;
using TradingJournal.Models.Entity;

namespace TradingJournal.Interfaces.Repository
{
    public interface ITradeRepository
    {
        public Task<bool> Save(TradesEntity tradesEntity);
        public Task<List<TradesEntity>> GetData();
        public Task<TradesEntity> UpdateTrade(TradesEntity tradesEntity);
        public Task<TradesEntity?> FindTradeEntry(Guid id);
        public Task<int> DeleteAllTrades();
        public Task<Response> DeleteTrade(Guid id);
    }
}
