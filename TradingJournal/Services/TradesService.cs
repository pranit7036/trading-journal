using Microsoft.AspNetCore.Mvc;
using TradingJournal.Models.Dto;
using TradingJournal.Models.Entity;
using TradingJournal.Mappers;
using TradingJournal.Repository;
using TradingJournal.Interfaces.Services;
using TradingJournal.Interfaces.Repository;
using TradingJournal.Models;
using System.Diagnostics;
namespace TradingJournal.Services
{
    public class TradesService : ITradeService
    {
        private readonly TradeMapper _tradeMapper;
        private readonly ITradeRepository _tradesRepository;
        public TradesService(ITradeRepository tradeRepository) 
        {
            _tradesRepository = tradeRepository;
            _tradeMapper = new TradeMapper();
            // add DBContext and save and extract data.
        }

        // can make a simple validation function to validate the trades or input and simple save and update to svae nad update the trade.

        public async Task<Response> ValidateTrades (TradesDto inputTrade, Guid userId)
        {
            if(inputTrade.EntryPrice <= 0 || inputTrade.ExitPrice <=0)
            {
                return (new Response {
                    Success= false,
                    Message = "Price entered is negitive",
                    Data = null
                });
            }

            if(inputTrade.Quantity <= 0 )
            {
                return (new Response
                {
                    Success = false,
                    Message = "Price enter a valid Quantity",
                    Data = null
                });
            }
                
            if(inputTrade.Stoploss <= 0 || inputTrade.Target <= 0 )
            {
                return (new Response
                {
                    Success = false,
                    Message = "Price enter a valid target and stoploss",
                    Data = null
                });
            }


            if (!IsWithinMarketHours(inputTrade.EntryTime))
            {
                return (new Response
                {
                    Success = false,
                    Message = "Entry time is outside market hours (9:15 AM to 3:30 PM IST)",
                    Data = null
                });
            }

            if (!IsWithinMarketHours(inputTrade.ExitTime))
            {
                return (new Response
                {
                    Success = false,
                    Message = "Exit time is outside market hours (9:15 AM to 3:30 PM IST)",
                    Data = null
                });
            }

            if(inputTrade.EntryTime>inputTrade.ExitTime)
            {
                return (new Response
                {
                    Success = false,
                    Message = "Please enter valid Entry and Exit time",
                    Data = null
                });
            }

            decimal profitAndLoss = CalculateProfitAndLoss(inputTrade.TradeType.ToLower(), inputTrade.EntryPrice, inputTrade.ExitPrice, inputTrade.Quantity);

            TradesEntity tradesEntity = _tradeMapper.ConvertDtoToEntity(inputTrade,profitAndLoss, userId);

            // save data 
            var result = await _tradesRepository.Save(tradesEntity);
            if (result)
            {
                return (new Response
                {
                    Success = true,
                    Message = "Data Saved",
                    Data = null
                });
            }
            else
            {
                return (new Response
                {
                    Success = false,
                    Message = "Error while saving data",
                    Data = null
                });
            }
        }

        public async Task<Response> GetTrades ()
        {
            var result = await _tradesRepository.GetData();
            List<TradesDto> tradeList = new List<TradesDto>();
            foreach(var trade in result)
            {
               tradeList.Add(_tradeMapper.ConvertEntityToDto(trade));
            }

            return (new Response
            {
                Success = true,
                Message = "Data fetched successfull",
                Data = tradeList
            });
        }

        // we can validate trades in one fucntion and write a separate function to save and edit 

        public async Task<Response> EditTrade(Guid id, UpdateTradeDto tradeData)
        {
            TradesEntity tradesEntity = await _tradesRepository.FindTradeEntry(id);

            if(tradesEntity == null)
            {
                return new Response
                {
                    Success = false,
                    Message = "Update data not found",
                    Data = null
                };
            }

            if(!IsNullOrEmpty(tradeData.Symbol) && tradesEntity.Symbol != tradeData.Symbol)
            {
                tradesEntity.Symbol = tradeData.Symbol;
            }

            if(!IsNullOrEmpty(tradeData.InstrumentType) && tradesEntity.InstrumentType != tradeData.InstrumentType)
            {
                tradesEntity.InstrumentType = tradeData.InstrumentType;
            }

            if(!IsNullOrEmpty(tradeData.TradeType) && tradesEntity.TradeType != tradeData.TradeType)
            {
                tradesEntity.TradeType = tradeData.TradeType;
            }

            if(tradeData.EntryPrice.HasValue && tradesEntity.EntryPrice != tradeData.EntryPrice.Value)
            {
                if(tradeData.EntryPrice.Value > 0)
                {
                    tradesEntity.EntryPrice = tradeData.EntryPrice.Value;
                }
            }

            if(tradeData.ExitPrice.HasValue && tradesEntity.ExitPrice != tradeData.ExitPrice.Value)
            {
                if(tradeData.ExitPrice.Value > 0)
                {
                    tradesEntity.ExitPrice = tradeData.ExitPrice.Value;
                }
            }

            if(tradeData.Quantity.HasValue && tradesEntity.Quantity != tradeData.Quantity.Value)
            {
                if(tradeData.Quantity.Value > 0)
                {
                    tradesEntity.Quantity = tradeData.Quantity.Value;
                }
            }

            if(tradeData.Stoploss.HasValue && tradesEntity.Stoploss != tradeData.Stoploss.Value)
            {
                if(tradeData.Stoploss.Value > 0)
                {
                    tradesEntity.Stoploss = tradeData.Stoploss.Value;
                }
            }

            if(tradeData.Target.HasValue && tradesEntity.Target != tradeData.Target.Value)
            {
                if(tradeData.Target.Value > 0)
                {
                    tradesEntity.Target = tradeData.Target.Value;
                }
            }

            // validate date and time in this part, also check the entry time is less than exit time or not.
            if (tradeData.EntryTime.HasValue && tradesEntity.EntryTime != tradeData.EntryTime.Value)
            {
                tradesEntity.EntryTime = tradeData.EntryTime.Value;
            }

            if (tradeData.ExitTime.HasValue && tradesEntity.ExitTime != tradeData.ExitTime.Value)
            {
                tradesEntity.ExitTime = tradeData.ExitTime.Value;
            }


            if (tradeData.Charges.HasValue && tradesEntity.Charges != tradeData.Charges.Value)
            {
                if(tradeData.Charges.Value > 0)
                {
                    tradesEntity.Charges = tradeData.Charges.Value; 
                }
            }

            if(!IsNullOrEmpty(tradeData.Strategy) && tradesEntity.Strategy != tradeData.Strategy)
            {
                tradesEntity.Strategy = tradeData.Strategy;
            }

            if(!IsNullOrEmpty(tradeData.Setup) && tradesEntity.Setup != tradeData.Setup)
            {
                tradesEntity.Setup = tradeData.Setup;
            }

            if(!IsNullOrEmpty(tradeData.Notes) && tradesEntity.Notes != tradeData.Notes)
            {
                tradesEntity.Notes = tradeData.Notes;
            }

            var result = await _tradesRepository.UpdateTrade(tradesEntity);
            if(result!=null)
            {
                return new Response
                {
                    Success = true,
                    Message = "Trade is Updated",
                    Data = result
                };
            }
            else
            {
                return new Response
                {
                    Success = false,
                    Message = "Error while updating data",
                    Data = null
                };
            }
        }

        public async Task<Response> DeleteAllTrades()
        {
            try
            {
                var count = await _tradesRepository.DeleteAllTrades();
                
                if (count > 0)
                {
                    return new Response
                    {
                        Success = true,
                        Message = $"Successfully deleted {count} trade(s)",
                        Data = null
                    };
                }
                else
                {
                    return new Response
                    {
                        Success = true,
                        Message = "No trades found to delete",
                        Data = null
                    };
                }
            }
            catch (Exception ex)
            {
                return new Response
                {
                    Success = false,
                    Message = $"Error deleting trades: {ex.Message}",
                    Data = null
                };
            }
        }

        public async Task<Response> DeleteTrade(Guid id)
        {
            var result = await _tradesRepository.DeleteTrade(id);
            return result;
        }

        private static decimal  CalculateProfitAndLoss (string tradeType, decimal entryPrice, decimal exitPrice, int quantity)
        {
            if(tradeType == "buy")
            {
                return (exitPrice - entryPrice) * quantity;
            }
            else
            {
                return (entryPrice - exitPrice) * quantity;
            }
        }
        // also do the calculations in this part, also 
        // get dto + extra data and map it with entity and call repository to save the final data 

        // target to calculate exact charges for Equity Intraday, Equity dilevery, ,F&O  

        private static bool IsWithinMarketHours(DateTime time)
        {
            TimeSpan marketOpen = new TimeSpan(9, 15, 0);
            TimeSpan marketClose = new TimeSpan(15, 30, 0);
            TimeSpan timeOfDay = time.TimeOfDay;

            return timeOfDay >= marketOpen && timeOfDay <= marketClose;
        }

        private static bool IsNullOrEmpty(string value)
        {
            if (String.IsNullOrEmpty(value))
            {
                return true;
            }

            return false;
        }
    }
}
