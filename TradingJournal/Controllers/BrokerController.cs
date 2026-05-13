using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradingJournal.Interfaces.Services;
using TradingJournal.Models.Dto;
using TradingJournal.Models;

namespace TradingJournal.Controllers
{
    [Route("api")]
    [ApiController]
    [Authorize]
    public class BrokerController : ControllerBase
    {
        private readonly IBrokerService _brokerService;

        public BrokerController(IBrokerService brokerService)
        {
            _brokerService = brokerService;
        }

        [HttpPost]
        [Route("add/broker")]
        public async Task<IActionResult> AddData(BrokerDto brokerDto)
        {
            if(brokerDto == null)
            {
                return BadRequest(new Response{
                    Success = false,
                    Message = "Broker data is null"
                });
            }

            var result = await _brokerService.AddBrokerData(brokerDto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpPatch]
        [Route("update/broker/{id}")]
        public async Task<IActionResult> UpdateData(Guid id, BrokerDto brokerDto)
        {
            if (brokerDto == null)
            {
                return BadRequest(new Response
                {
                    Success = false,
                    Message = "Broker data is null"
                });
            }

            var result = await _brokerService.UpdateBrokerData(brokerDto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }
    }
}