using Microsoft.EntityFrameworkCore;
using TradingJournal.Context;
using TradingJournal.Interfaces.Repository;
using TradingJournal.Models;
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
                var result  = await _dbContext.Trades.AddAsync(tradesEntity);
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

        public async Task<TradesEntity> UpdateTrade(TradesEntity tradesEntity)
        {
            try
            {
                var result =  _dbContext.Trades.Update(tradesEntity);
                await _dbContext.SaveChangesAsync();
                return result.Entity;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<TradesEntity?> FindTradeEntry (Guid id)
        {
            try
            {
                var result = await _dbContext.Trades.FindAsync(id);
                if (result != null)
                {
                    // we can add model builder in dbContext to get the dafault time Zone as utc
                    result.EntryTime = DateTime.SpecifyKind(result.EntryTime, DateTimeKind.Utc);
                    result.ExitTime = DateTime.SpecifyKind(result.ExitTime, DateTimeKind.Utc);
                }
                return result;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<int> DeleteAllTrades()
        {
            try
            {
                var allTrades = await _dbContext.Trades.ToListAsync();
                _dbContext.Trades.RemoveRange(allTrades);
                var count = await _dbContext.SaveChangesAsync();
                return count;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task<Response> DeleteTrade(Guid id)
        {
            try
            {
                var entity = await _dbContext.Trades.FindAsync(id);
                if (entity != null)
                {
                    _dbContext.Trades.Remove(entity);
                    await _dbContext.SaveChangesAsync();

                }
                else
                {
                    return new Response
                    {
                        Success = false,
                        Message = "Id not found",
                        Data = null
                    };
                }

                return new Response
                {
                    Success = true,
                    Message = "Entity Delete",
                    Data = null
                };
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        //use DB context and save data directly, all the validations will and calculation will be done till this process
    }
}