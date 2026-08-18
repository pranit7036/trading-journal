using TradingJournal.Models;
using TradingJournal.Models.Entity;

namespace TradingJournal.Interfaces.Repository
{
    public interface IBrokerRepository
    {
        public Task<BrokerEntity> AddBroker(BrokerEntity brokerEntity);
        public Task<BrokerEntity> UpdateBroker(BrokerEntity brokerEntity);
        public Task<List<BrokerEntity>> GetBrokersByUserId(Guid userId);
        public Task<BrokerEntity?> FindBroker(Guid brokerId, Guid userId);
        public Task<bool> DeleteBroker(BrokerEntity brokerEntity);
        public Task<BrokerEntity?> FindBrokerByApiKey(string apiKey);
    }
}