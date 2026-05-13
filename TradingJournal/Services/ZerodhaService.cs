using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TradingJournal.Interfaces.Services;
using TradingJournal.Models;

namespace TradingJournal.Services
{
    public class ZerodhaService : IZerodhaService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        
        public ZerodhaService(IConfiguration configuration, IHttpClientFactory httpClientFactory)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
        }

        public async Task<Response> GetToken(string requestToken)
        {
            var apiKey = _configuration["Zerodha:ApiKey"];
            var apiSecret = _configuration["Zerodha:ApiSecret"];

            if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(apiSecret))
            {
                return new Response { Success = false, Message = "Zerodha config missing", Data = null };
            }

            var checksumInput = $"{apiKey}{requestToken}{apiSecret}";
            var checksum = Convert.ToHexString(SHA256.HashData(Encoding.UTF8.GetBytes(checksumInput))).ToLowerInvariant();
            
            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("X-Kite-Version", "3");
            var form = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                ["api_key"] = apiKey,
                ["request_token"] = requestToken,
                ["checksum"] = checksum
            });

            var resp = await client.PostAsync("https://api.kite.trade/session/token", form);
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                return new Response { Success = false, Message = "Token exchange failed", Data = body };
            }

            using var doc = JsonDocument.Parse(body);
            var accessToken = doc.RootElement.GetProperty("data").GetProperty("access_token").GetString();
            var userId = doc.RootElement.GetProperty("data").GetProperty("user_id").GetString();

            return new Response { Success = true, Message = "Token generated", Data = new { accessToken, userId } };
        }

    }
}
