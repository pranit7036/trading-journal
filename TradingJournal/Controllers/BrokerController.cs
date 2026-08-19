using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
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

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? User.FindFirstValue("sub");

            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new Response
                {
                    Success = false,
                    Message = "Invalid user token",
                    Data = null
                });
            }

            brokerDto.UserID = userId;
            brokerDto.AccessToken = null;
            brokerDto.RefreshToken = null;
            brokerDto.TokenExpiry = null;

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
            if (id == Guid.Empty)
            {
                return BadRequest(new Response
                {
                    Success = false,
                    Message = "BrokerId is required"
                });
            }

            if (brokerDto == null)
            {
                return BadRequest(new Response
                {
                    Success = false,
                    Message = "Broker data is null"
                });
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? User.FindFirstValue("sub");

            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new Response
                {
                    Success = false,
                    Message = "Invalid user token",
                    Data = null
                });
            }

            var result = await _brokerService.UpdateBrokerData(id, userId, brokerDto);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("get/brokers")]
        public async Task<IActionResult> GetBrokers()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? User.FindFirstValue("sub");

            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new Response
                {
                    Success = false,
                    Message = "Invalid user token",
                    Data = null
                });
            }

            var result = await _brokerService.GetBrokersByUserId(userId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpDelete]
        [Route("delete/broker/{id}")]
        public async Task<IActionResult> DeleteBroker(Guid id)
        {
            if (id == Guid.Empty)
            {
                return BadRequest(new Response
                {
                    Success = false,
                    Message = "BrokerId is required",
                    Data = null
                });
            }

            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? User.FindFirstValue("sub");

            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return Unauthorized(new Response
                {
                    Success = false,
                    Message = "Invalid user token",
                    Data = null
                });
            }

            var result = await _brokerService.DeleteBroker(id, userId);

            if (!result.Success)
            {
                return NotFound(result);
            }

            return Ok(result);
        }
    }
}