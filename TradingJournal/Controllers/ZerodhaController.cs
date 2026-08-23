using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TradingJournal.Interfaces.Repository;
using TradingJournal.Interfaces.Services;
using TradingJournal.Models;
using TradingJournal.Models.Dto;

namespace TradingJournal.Controllers
{
    [Route("api/integration")]
    [ApiController]
    [Authorize]
    public class ZerodhaController : ControllerBase
    {
        private readonly IZerodhaService _zerodhaService;
        private readonly IBrokerRepository _brokerRepository;

        public ZerodhaController(IZerodhaService zerodhaService, IBrokerRepository brokerRepository)
        {
            _zerodhaService = zerodhaService;
            _brokerRepository = brokerRepository;
        }

        /// <summary>
        /// Returns the Zerodha Kite Connect login URL for the specified broker.
        /// The URL contains the broker's ApiKey (from DB) and brokerId as state.
        /// The redirect target is the frontend callback page (configured in Zerodha developer console).
        /// </summary>
        [HttpGet]
        [Route("kite/connect-url")]
        public async Task<IActionResult> GetConnectUrl([FromQuery] Guid brokerId)
        {
            if (brokerId == Guid.Empty)
            {
                return BadRequest(new Response
                {
                    Success = false,
                    Message = "brokerId is required",
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

            // Verify broker belongs to this user and fetch its ApiKey
            var broker = await _brokerRepository.FindBroker(brokerId, userId);
            if (broker == null)
            {
                return BadRequest(new Response
                {
                    Success = false,
                    Message = "Broker not found or does not belong to this user",
                    Data = null
                });
            }

            if (string.IsNullOrWhiteSpace(broker.ApiKey))
            {
                return BadRequest(new Response
                {
                    Success = false,
                    Message = "Broker ApiKey is not set. Please update your broker details first.",
                    Data = null
                });
            }

            // state=brokerId lets the frontend callback know which broker to pass to POST /kite/token
            var url = $"https://kite.zerodha.com/connect/login?v=3&api_key={broker.ApiKey}&state={brokerId}";

            return Ok(new Response
            {
                Success = true,
                Message = "Zerodha connect URL retrieved successfully",
                Data = url
            });
        }

        /// <summary>
        /// Called by the frontend after Zerodha redirects back with request_token.
        /// Exchanges the request_token for an access_token using the broker's own ApiKey/ApiSecret from DB.
        /// Requires JWT — broker ownership is validated via userId from claims.
        /// </summary>
        [HttpPost]
        [Route("kite/token")]
        public async Task<IActionResult> ExchangeToken([FromBody] ZerodhaTokenRequestDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.RequestToken) || dto.BrokerId == Guid.Empty)
            {
                return BadRequest(new Response
                {
                    Success = false,
                    Message = "RequestToken and BrokerId are required",
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

            var result = await _zerodhaService.SaveAccessToken(dto.RequestToken, dto.BrokerId, userId);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("kite/orders")]
        public async Task<IActionResult> GetOrders(
            [FromHeader(Name = "X-Zerodha-Access-Token")] string? accessToken,
            [FromHeader(Name = "X-Zerodha-Api-Key")] string? apiKey)
        {
            var resolvedApiKey = string.IsNullOrWhiteSpace(apiKey)
                ? null
                : apiKey;

            if (string.IsNullOrWhiteSpace(resolvedApiKey) || string.IsNullOrWhiteSpace(accessToken))
            {
                return BadRequest(new Response
                {
                    Success = false,
                    Message = "ApiKey or AccessToken is missing",
                    Data = null
                });
            }

            var result = await _zerodhaService.GetOrders(resolvedApiKey, accessToken);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("kite/portfolio/holdings")]
        public async Task<IActionResult> GetHoldings()
        {
            var credentialsResult = await GetBrokerCredentials();
            if (credentialsResult.Error != null)
            {
                return credentialsResult.Error;
            }

            var result = await _zerodhaService.GetHoldings(credentialsResult.ApiKey!, credentialsResult.AccessToken!);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        [HttpGet]
        [Route("kite/portfolio/positions")]
        public async Task<IActionResult> GetPositions()
        {
            var credentialsResult = await GetBrokerCredentials();
            if (credentialsResult.Error != null)
            {
                return credentialsResult.Error;
            }

            var result = await _zerodhaService.GetPositions(credentialsResult.ApiKey!, credentialsResult.AccessToken!);

            if (!result.Success)
            {
                return BadRequest(result);
            }

            return Ok(result);
        }

        /// <summary>
        /// Extracts the user ID from JWT claims and fetches broker credentials from the database.
        /// </summary>
        private async Task<(string? ApiKey, string? AccessToken, IActionResult? Error)> GetBrokerCredentials()
        {
            var userIdClaim = User.FindFirstValue(ClaimTypes.NameIdentifier)
                ?? User.FindFirstValue(JwtRegisteredClaimNames.Sub)
                ?? User.FindFirstValue("sub");

            if (string.IsNullOrWhiteSpace(userIdClaim) || !Guid.TryParse(userIdClaim, out var userId))
            {
                return (null, null, Unauthorized(new Response
                {
                    Success = false,
                    Message = "Invalid user token",
                    Data = null
                }));
            }

            var brokers = await _brokerRepository.GetBrokersByUserId(userId);
            var broker = brokers.FirstOrDefault(b => b.IsActive && !string.IsNullOrWhiteSpace(b.AccessToken));

            if (broker == null)
            {
                return (null, null, BadRequest(new Response
                {
                    Success = false,
                    Message = "No active broker with a valid access token found. Please connect your Zerodha account first.",
                    Data = null
                }));
            }

            return (broker.ApiKey, broker.AccessToken, null);
        }
    }
}
