using Microsoft.EntityFrameworkCore;
using TradingJournal.Context;
using TradingJournal.Interfaces.Repository;
using TradingJournal.Models;
using TradingJournal.Models.Entity;

namespace TradingJournal.Repository
{
    public class BrokerRepository : IBrokerRepository
    {
        private readonly DBContext _context;

        public BrokerRepository(DBContext context)
        {
            _context = context;
        }

        public async Task<BrokerEntity> AddBroker(BrokerEntity brokerEntity)
        {
            var result = await _context.Brokers.AddAsync(brokerEntity);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<BrokerEntity> UpdateBroker(BrokerEntity brokerEntity)
        {
            var result = _context.Brokers.Update(brokerEntity);
            await _context.SaveChangesAsync();
            return result.Entity;
        }

        public async Task<List<BrokerEntity>> GetBrokersByUserId(Guid userId)
        {
            return await _context.Brokers
                .Where(broker => broker.UserID == userId)
                .ToListAsync();
        }

        public async Task<BrokerEntity?> FindBroker(Guid brokerId, Guid userId)
        {
            return await _context.Brokers
                .FirstOrDefaultAsync(broker => broker.Id == brokerId && broker.UserID == userId);
        }

        public async Task<bool> DeleteBroker(BrokerEntity brokerEntity)
        {
            _context.Brokers.Remove(brokerEntity);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<BrokerEntity?> FindBrokerByApiKey(string apiKey)
        {
            return await _context.Brokers
                .FirstOrDefaultAsync(broker => broker.ApiKey == apiKey);
        }
    }
}