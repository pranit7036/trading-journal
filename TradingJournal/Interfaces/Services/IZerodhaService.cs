using TradingJournal.Models;

namespace TradingJournal.Interfaces.Services
{
    public interface IZerodhaService
    {
        public Task<Response> GetToken(string requestToken);
        public Task<Response> GetOrders(string apiKey, string accessToken);
        public Task<Response> GetHoldings(string apiKey, string accessToken);
        public Task<Response> GetPositions(string apiKey, string accessToken);
        public Task<Response> SaveAccessToken(string requestToken);
    }
}
