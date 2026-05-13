using TradingJournal.Models;

namespace TradingJournal.Interfaces.Services
{
    public interface IZerodhaService
    {
        public Task<Response> GetToken(string requestToken);
    }
}
