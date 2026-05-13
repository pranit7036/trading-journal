using TradingJournal.Models.Dto;
using TradingJournal.Models.Entity;

namespace TradingJournal.Mappers
{
    public class BrokerMapper
    {
           
        public BrokerEntity ConvertDtoToEntity (BrokerDto brokerDto)
        {
            BrokerEntity broker = new BrokerEntity()
            {
                Id = Guid.NewGuid(),
                BrokerName = brokerDto.BrokerName,
                UserID = brokerDto.UserID,
                ApiKey = brokerDto.ApiKey,
                ApiSecret = brokerDto.ApiSecret,
                AccessToken = brokerDto.AccessToken,
                RefreshToken = brokerDto.RefreshToken,
                TokenExpiry = brokerDto.TokenExpiry,
                IsActive = brokerDto.IsActive
            };
            return broker;
        }

        public BrokerDto ConvertEntityToDto (BrokerEntity brokerEntity)
        {
            BrokerDto broker = new BrokerDto()
            {
                Id = brokerEntity.Id,
                BrokerName = brokerEntity.BrokerName,
                UserID = brokerEntity.UserID,
                ApiKey = brokerEntity.ApiKey,
                ApiSecret = brokerEntity.ApiSecret,
                AccessToken = brokerEntity.AccessToken,
                RefreshToken = brokerEntity.RefreshToken,
                TokenExpiry = brokerEntity.TokenExpiry,
                IsActive = brokerEntity.IsActive
            };
            return broker;
        }
    }
}