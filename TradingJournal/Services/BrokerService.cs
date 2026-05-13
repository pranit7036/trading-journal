using TradingJournal.Interfaces.Repository;
using TradingJournal.Interfaces.Services;
using TradingJournal.Mappers;
using TradingJournal.Models;
using TradingJournal.Models.Dto;
using TradingJournal.Models.Entity;

namespace TradingJournal.Services
{
    public class BrokerService : IBrokerService
    {
        private readonly IBrokerRepository _brokerRepository;
        private BrokerMapper _brokerMapper = new BrokerMapper();
        public BrokerService(IBrokerRepository brokerRepository)
        {
            _brokerRepository = brokerRepository;
        }

        public async Task<Response> AddBrokerData(BrokerDto brokerDto)
        {
            var validationResponse = ValidateBrokerData(brokerDto);
            
            if (!validationResponse.Success)
            {
                return validationResponse;
            }

            //if (brokerDto.TokenExpiry == default)
            //{
            //    return new Response
            //    {
            //        Success = false,
            //        Message = "TokenExpiry is required",
            //        Data = null
            //    };
            //}

            BrokerEntity brokerEntity = _brokerMapper.ConvertDtoToEntity(brokerDto);
            var result = await _brokerRepository.AddBroker(brokerEntity);

            return new Response
            {
                Success = true,
                Message = "Broker data is valid",
                Data = null
            };
        }

        public async Task<Response> UpdateBrokerData (BrokerDto brokerDto)
        {
            var validationResponse = ValidateBrokerData(brokerDto);

            if (!validationResponse.Success)
            {
                return validationResponse;
            }
            
            BrokerEntity brokerEntity = _brokerMapper.ConvertDtoToEntity(brokerDto);
            var result = await _brokerRepository.UpdateBroker(brokerEntity);

            return new Response
            {
                Success = true,
                Message = "Broker data is valid",
                Data = null
            };
        }

        private Response ValidateBrokerData(BrokerDto brokerDto)
        {
            if (string.IsNullOrWhiteSpace(brokerDto.BrokerName))
            {
                return new Response
                {
                    Success = false,
                    Message = "BrokerName is required",
                    Data = null
                };
            }

            if (brokerDto.UserID == Guid.Empty)
            {
                return new Response
                {
                    Success = false,
                    Message = "UserID is required",
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(brokerDto.ApiKey))
            {
                return new Response
                {
                    Success = false,
                    Message = "ApiKey is required",
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(brokerDto.ApiSecret))
            {
                return new Response
                {
                    Success = false,
                    Message = "ApiSecret is required",
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(brokerDto.AccessToken))
            {
                return new Response
                {
                    Success = false,
                    Message = "AccessToken is required",
                    Data = null
                };
            }

            if (string.IsNullOrWhiteSpace(brokerDto.RefreshToken))
            {
                return new Response
                {
                    Success = false,
                    Message = "RefreshToken is required",
                    Data = null
                };
            }

            return new Response
            {
                Success = true,
                Message = "Broker data is valid",
                Data = null
            };
        }
    }
}
