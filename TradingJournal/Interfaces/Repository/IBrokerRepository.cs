using TradingJournal.Models;
using TradingJournal.Models.Entity;

namespace TradingJournal.Interfaces.Repository
{
    public interface IBrokerRepository
    {
        public Task<BrokerEntity> AddBroker(BrokerEntity brokerEntity);
        public Task<BrokerEntity> UpdateBroker(BrokerEntity brokerEntity);
    }
}