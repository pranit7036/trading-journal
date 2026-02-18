using TradingJournal.Models.Dto;
using TradingJournal.Models.Entity;

namespace TradingJournal.Mappers
{
    public class TradeMapper
    {
        public TradeMapper() { }
        
        public TradesEntity ConvertDtoToEntity (TradesDto tradeDto, decimal profitAndLoss)
        {
            TradesEntity trade = new TradesEntity()
            {
                Id = Guid.NewGuid(),
                Symbol = tradeDto.Symbol.ToUpper(),
                InstrumentType = tradeDto.InstrumentType.ToLower(),
                TradeType = tradeDto.TradeType.ToLower(),
                EntryPrice = tradeDto.EntryPrice,
                ExitPrice = tradeDto.ExitPrice,
                Quantity = tradeDto.Quantity,
                Stoploss = tradeDto.Stoploss,
                Target = tradeDto.Target,
                EntryTime = tradeDto.EntryTime,
                ExitTime = tradeDto.ExitTime,
                Charges = tradeDto.Charges,
                ProfitAndLoss = profitAndLoss,
                Strategy = tradeDto?.Strategy,
                Setup = tradeDto?.Setup,
                Notes = tradeDto?.Notes,
            };
            return trade;
        }

        public TradesDto ConvertEntityToDto (TradesEntity tradeEntity)
        {
            TradesDto trade = new TradesDto()
            {
                Id = tradeEntity.Id,
                Symbol = tradeEntity.Symbol,
                InstrumentType = tradeEntity.InstrumentType,
                TradeType = tradeEntity.TradeType,
                EntryPrice = tradeEntity.EntryPrice,
                ExitPrice = tradeEntity.ExitPrice,
                Quantity = tradeEntity.Quantity,
                Stoploss = tradeEntity.Stoploss,
                Target = tradeEntity.Target,
                EntryTime = tradeEntity.EntryTime,
                ExitTime = tradeEntity.ExitTime,
                Charges = tradeEntity.Charges,
                ProfitAndLoss = tradeEntity.ProfitAndLoss,
                Strategy = tradeEntity?.Strategy,
                Setup = tradeEntity?.Setup,
                Notes = tradeEntity?.Notes
            };
            return trade;
        }
    }
}
