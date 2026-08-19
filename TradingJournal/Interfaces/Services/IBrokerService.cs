using TradingJournal.Models;
using TradingJournal.Models.Dto;

namespace TradingJournal.Interfaces.Services
{
    public interface IBrokerService
    {
        public Task<Response> AddBrokerData(BrokerDto brokerDto);
        public Task<Response> UpdateBrokerData(Guid brokerId, Guid userId, BrokerDto brokerDto);
        public Task<Response> GetBrokersByUserId(Guid userId);
        public Task<Response> DeleteBroker(Guid brokerId, Guid userId);
    }
}