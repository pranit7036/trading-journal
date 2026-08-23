using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using TradingJournal.Interfaces.Repository;
using TradingJournal.Interfaces.Services;
using TradingJournal.Models;
using TradingJournal.Models.Dto;

namespace TradingJournal.Services
{
    public class ZerodhaService : IZerodhaService
    {
        private readonly IConfiguration _configuration;
        private readonly IHttpClientFactory _httpClientFactory;
        private readonly IBrokerRepository _brokerRepository;
        
        private static readonly JsonSerializerOptions _jsonOptions = new()
        {
            PropertyNameCaseInsensitive = true
        };

        public ZerodhaService(IConfiguration configuration, IHttpClientFactory httpClientFactory, IBrokerRepository brokerRepository)
        {
            _configuration = configuration;
            _httpClientFactory = httpClientFactory;
            _brokerRepository = brokerRepository;
        }

        /// <summary>
        /// Used by the hosted service (reads ApiKey/ApiSecret from appsettings).
        /// Will be updated when the daily job is fully implemented.
        /// </summary>
        public async Task<Response> GetToken(string requestToken)
        {
            var apiKey = _configuration["Zerodha:ApiKey"];
            var apiSecret = _configuration["Zerodha:ApiSecret"];
            return await GetToken(requestToken, apiKey ?? string.Empty, apiSecret ?? string.Empty);
        }

        private async Task<Response> GetToken(string requestToken, string apiKey, string apiSecret)
        {
            if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(apiSecret))
            {
                return new Response { Success = false, Message = "ApiKey or ApiSecret is missing", Data = null };
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
            var zerodhaUserId = doc.RootElement.GetProperty("data").GetProperty("user_id").GetString();

            return new Response { Success = true, Message = "Token generated", Data = new { accessToken, zerodhaUserId } };
        }

        public async Task<Response> GetOrders(string apiKey, string accessToken)
        {
            if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(accessToken))
            {
                return new Response { Success = false, Message = "ApiKey or AccessToken is missing", Data = null };
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("X-Kite-Version", "3");
            client.DefaultRequestHeaders.Add("Authorization", $"token {apiKey}:{accessToken}");

            var resp = await client.GetAsync("https://api.kite.trade/orders");
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                return new Response { Success = false, Message = "Orders fetch failed", Data = body };
            }

            try
            {
                using var doc = JsonDocument.Parse(body);
                return new Response { Success = true, Message = "Orders fetched", Data = doc.RootElement.Clone() };
            }
            catch (JsonException)
            {
                return new Response { Success = true, Message = "Orders fetched", Data = body };
            }
        }

        public async Task<Response> GetHoldings(string apiKey, string accessToken)
        {
            if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(accessToken))
            {
                return new Response { Success = false, Message = "ApiKey or AccessToken is missing", Data = null };
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("X-Kite-Version", "3");
            client.DefaultRequestHeaders.Add("Authorization", $"token {apiKey}:{accessToken}");

            var resp = await client.GetAsync("https://api.kite.trade/portfolio/holdings");
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                return new Response { Success = false, Message = "Holdings fetch failed", Data = body };
            }

            try
            {
                using var doc = JsonDocument.Parse(body);
                var dataElement = doc.RootElement.GetProperty("data");
                var holdings = JsonSerializer.Deserialize<List<HoldingDto>>(dataElement.GetRawText(), _jsonOptions);

                return new Response { Success = true, Message = "Holdings fetched", Data = holdings };
            }
            catch (JsonException)
            {
                return new Response { Success = false, Message = "Failed to parse holdings response", Data = body };
            }
        }

        public async Task<Response> GetPositions(string apiKey, string accessToken)
        {
            if (string.IsNullOrWhiteSpace(apiKey) || string.IsNullOrWhiteSpace(accessToken))
            {
                return new Response { Success = false, Message = "ApiKey or AccessToken is missing", Data = null };
            }

            var client = _httpClientFactory.CreateClient();
            client.DefaultRequestHeaders.Add("X-Kite-Version", "3");
            client.DefaultRequestHeaders.Add("Authorization", $"token {apiKey}:{accessToken}");

            var resp = await client.GetAsync("https://api.kite.trade/portfolio/positions");
            var body = await resp.Content.ReadAsStringAsync();

            if (!resp.IsSuccessStatusCode)
            {
                return new Response { Success = false, Message = "Positions fetch failed", Data = body };
            }

            try
            {
                using var doc = JsonDocument.Parse(body);
                var dataElement = doc.RootElement.GetProperty("data");
                var positions = JsonSerializer.Deserialize<PositionsResponseDto>(dataElement.GetRawText(), _jsonOptions);

                return new Response { Success = true, Message = "Positions fetched", Data = positions };
            }
            catch (JsonException)
            {
                return new Response { Success = false, Message = "Failed to parse positions response", Data = body };
            }
        }

        public async Task<Response> SaveAccessToken(string requestToken, Guid brokerId, Guid userId)
        {
            // Fetch broker from DB — verifies it belongs to the authenticated user
            var broker = await _brokerRepository.FindBroker(brokerId, userId);

            if (broker == null)
            {
                return new Response { Success = false, Message = "Broker not found for the given brokerId", Data = null };
            }

            if (string.IsNullOrWhiteSpace(broker.ApiKey) || string.IsNullOrWhiteSpace(broker.ApiSecret))
            {
                return new Response { Success = false, Message = "Broker ApiKey or ApiSecret is not set in the database", Data = null };
            }

            // Exchange the request token using the broker's own ApiKey and ApiSecret from DB
            var tokenResponse = await GetToken(requestToken, broker.ApiKey, broker.ApiSecret);
            if (!tokenResponse.Success)
            {
                return tokenResponse;
            }

            // Extract access token from the response
            var data = tokenResponse.Data;
            var accessToken = data?.GetType().GetProperty("accessToken")?.GetValue(data)?.ToString();

            if (string.IsNullOrWhiteSpace(accessToken))
            {
                return new Response { Success = false, Message = "Access token not found in response", Data = null };
            }

            // Update the broker record with the new access token
            broker.AccessToken = accessToken;
            broker.RefreshToken = requestToken; // Store request token for future re-auth
            broker.TokenExpiry = DateTime.UtcNow.AddHours(8); // Zerodha tokens expire at 6 AM next day
            broker.IsActive = true;

            await _brokerRepository.UpdateBroker(broker);

            return new Response
            {
                Success = true,
                Message = "Access token saved successfully",
                Data = new { accessToken, brokerId = broker.Id, userId = broker.UserID }
            };
        }

    }
}
