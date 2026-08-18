using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using TradingJournal.Interfaces.Repository;
using TradingJournal.Interfaces.Services;
using TradingJournal.Models;

namespace TradingJournal.Controllers
{
    [Route("api/integration")]
    [ApiController]
    [Authorize]
    public class ZerodhaController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IZerodhaService _zerodhaService;
        private readonly IBrokerRepository _brokerRepository;

        public ZerodhaController(IConfiguration configuration, IZerodhaService zerodhaService, IBrokerRepository brokerRepository)
        {
            _configuration = configuration;
            _zerodhaService = zerodhaService;
            _brokerRepository = brokerRepository;
        }

        [HttpGet]
        [Route("kite/connect-url")]
        public IActionResult GetConnectUrl()
        {
            var apiKey = _configuration["Zerodha:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                return BadRequest(new Response
                {
                    Success = false,
                    Message = "Zerodha API key is not configured",
                    Data = null
                });
            }
            var url = $"https://kite.zerodha.com/connect/login?v=3&api_key={apiKey}";

            return Ok(new Response{
                Success = true,
                Message = "Zerodha connect URL retrieved successfully",
                Data = url
            });
        }

        [HttpGet]
        [Route("kite/orders")]
        public async Task<IActionResult> GetOrders(
            [FromHeader(Name = "X-Zerodha-Access-Token")] string? accessToken,
            [FromHeader(Name = "X-Zerodha-Api-Key")] string? apiKey)
        {
            var resolvedApiKey = string.IsNullOrWhiteSpace(apiKey)
                ? _configuration["Zerodha:ApiKey"]
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
        [Route("kite/callback")]
        [AllowAnonymous]
        public async Task<IActionResult> HandleCallback([FromQuery] string request_token, [FromQuery] string? status)
        {
            if (string.IsNullOrEmpty(request_token))
            {
                return BadRequest(new Response
                {
                    Success = false,
                    Message = "Request token is missing in the callback",
                    Data = null
                });
            }

            //if(!string.IsNullOrEmpty(status) && status.ToLower() == "sucesss")
            //{
                var result = _zerodhaService.GetToken(request_token);
            //}

            // Here you would typically exchange the request token for an access token
            // and save it securely for future API calls. For now, we'll just return the token.

            // Save the access token to the database for the specific user
            var saveResult = await _zerodhaService.SaveAccessToken(request_token);

            return Ok(new Response
            {
                Success = saveResult.Success,
                Message = saveResult.Success 
                    ? "Zerodha callback processed and access token saved successfully" 
                    : saveResult.Message,
                Data = new { RequestToken = request_token, Status = status, Result = result, SaveResult = saveResult.Data }
            });
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
