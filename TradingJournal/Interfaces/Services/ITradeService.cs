using TradingJournal.Models;
using TradingJournal.Models.Dto;

namespace TradingJournal.Interfaces.Services
{
    public interface ITradeService
    {
        public Task<Response> ValidateTrades(TradesDto inputTrade);
        public Task<Response> GetTrades();
    }
}