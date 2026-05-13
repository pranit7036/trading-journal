using TradingJournal.Models;
using TradingJournal.Models.Dto;

namespace TradingJournal.Interfaces.Services
{
    public interface IBrokerService
    {
        public Task<Response> AddBrokerData(BrokerDto brokerDto);
        public Task<Response> UpdateBrokerData(BrokerDto brokerDto);
    }
}