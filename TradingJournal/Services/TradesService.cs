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

        public async Task<Response> ValidateTrades (TradesDto inputTrade)
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

            // Validate Date and Time in this service.
            // also entry time should be less than the exit time 

            decimal profitAndLoss = CalculateProfitAndLoss(inputTrade.InstrumentType.ToLower(), inputTrade.EntryPrice, inputTrade.ExitPrice, inputTrade.Quantity);

            TradesEntity tradesEntity = _tradeMapper.ConvertDtoToEntity(inputTrade,profitAndLoss);

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
