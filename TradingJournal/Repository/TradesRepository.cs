using Microsoft.EntityFrameworkCore;
using TradingJournal.Context;
using TradingJournal.Interfaces.Repository;
using TradingJournal.Models.Entity;

namespace TradingJournal.Repository
{
    public class TradesRepository : ITradeRepository
    {
        private readonly DBContext _dbContext;
        public TradesRepository(DBContext dbContext) {
            _dbContext = dbContext;
        }

        public async Task<bool> Save(TradesEntity tradesEntity)
        {
            try
            {
                await _dbContext.Trades.AddAsync(tradesEntity);
                await _dbContext.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<List<TradesEntity>> GetData()
        {
            try
            {
                var result = await _dbContext.Trades.ToListAsync();
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //use DB context and save data directly, all the validations will and calculation will be done till this process
    }
}