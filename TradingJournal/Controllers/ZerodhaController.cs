using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using TradingJournal.Interfaces.Services;
using TradingJournal.Models;
using TradingJournal.Services;

namespace TradingJournal.Controllers
{
    [Route("api/integration")]
    [ApiController]
    // [Authorize]
    public class ZerodhaController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IZerodhaService _zerodhaService;
        public ZerodhaController(IConfiguration configuration, IZerodhaService zerodhaService)
        {
            _configuration = configuration;
            _zerodhaService = zerodhaService;
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

            return Ok(new Response
            {
                Success = true,
                Message = "Zerodha callback received successfully",
                Data = new { RequestToken = request_token, Status = status, Result = result }
            });
        }
    }
}