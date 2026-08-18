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

        public async Task<Response> GetBrokersByUserId(Guid userId)
        {
            if (userId == Guid.Empty)
            {
                return new Response
                {
                    Success = false,
                    Message = "UserID is required",
                    Data = null
                };
            }

            var brokers = await _brokerRepository.GetBrokersByUserId(userId);
            var brokerList = new List<BrokerSummaryDto>();

            foreach (var broker in brokers)
            {
                brokerList.Add(new BrokerSummaryDto
                {
                    Id = broker.Id,
                    BrokerName = broker.BrokerName,
                    ApiKey = broker.ApiKey,
                    IsActive = broker.IsActive,
                    TokenExpiry = broker.TokenExpiry
                });
            }

            return new Response
            {
                Success = true,
                Message = "Brokers fetched successfully",
                Data = brokerList
            };
        }

        public async Task<Response> DeleteBroker(Guid brokerId, Guid userId)
        {
            if (brokerId == Guid.Empty)
            {
                return new Response
                {
                    Success = false,
                    Message = "BrokerId is required",
                    Data = null
                };
            }

            if (userId == Guid.Empty)
            {
                return new Response
                {
                    Success = false,
                    Message = "UserID is required",
                    Data = null
                };
            }

            var broker = await _brokerRepository.FindBroker(brokerId, userId);

            if (broker == null)
            {
                return new Response
                {
                    Success = false,
                    Message = "Broker not found",
                    Data = null
                };
            }

            await _brokerRepository.DeleteBroker(broker);

            return new Response
            {
                Success = true,
                Message = "Broker deleted",
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

            return new Response
            {
                Success = true,
                Message = "Broker data is valid",
                Data = null
            };
        }
    }
}
