using TradingJournal.Interfaces.Services;

namespace TradingJournal.Services
{
    public sealed class ZerodhaIstDailyHostedService : BackgroundService
    {
        private const int ScheduledHourIst = 16;
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IConfiguration _configuration;
        private readonly ILogger<ZerodhaIstDailyHostedService> _logger;
        private readonly TimeZoneInfo _indiaTimeZone;

        public ZerodhaIstDailyHostedService(
            IServiceScopeFactory scopeFactory,
            IConfiguration configuration,
            ILogger<ZerodhaIstDailyHostedService> logger)
        {
            _scopeFactory = scopeFactory;
            _configuration = configuration;
            _logger = logger;
            _indiaTimeZone = ResolveIndiaTimeZone();
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation("Zerodha IST hosted service started.");

            while (!stoppingToken.IsCancellationRequested)
            {
                var delay = GetDelayUntilNextRunUtc(DateTime.UtcNow);
                _logger.LogInformation("Next Zerodha run scheduled in {Delay}.", delay);

                try
                {
                    await Task.Delay(delay, stoppingToken);
                }
                catch (TaskCanceledException)
                {
                    break;
                }

                try
                {
                    using var scope = _scopeFactory.CreateScope();
                    var zerodhaService = scope.ServiceProvider.GetRequiredService<IZerodhaService>();

                    var requestToken = _configuration["Zerodha:DailyJobRequestToken"];
                    if (string.IsNullOrWhiteSpace(requestToken))
                    {
                        _logger.LogWarning("Skipping 4 PM IST Zerodha run because Zerodha:DailyJobRequestToken is missing.");
                        continue;
                    }

                    var response = await zerodhaService.GetToken(requestToken);
                    if (response.Success)
                    {
                        _logger.LogInformation("4 PM IST Zerodha scheduled job completed successfully.");
                    }
                    else
                    {
                        _logger.LogWarning("4 PM IST Zerodha scheduled job failed: {Message}", response.Message);
                    }
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Unhandled error in 4 PM IST Zerodha hosted service run.");
                }
            }

            _logger.LogInformation("Zerodha IST hosted service stopped.");
        }

        private TimeSpan GetDelayUntilNextRunUtc(DateTime utcNow)
        {
            var istNow = TimeZoneInfo.ConvertTimeFromUtc(utcNow, _indiaTimeZone);
            var nextIstRun = new DateTime(
                istNow.Year,
                istNow.Month,
                istNow.Day,
                ScheduledHourIst,
                0,
                0,
                DateTimeKind.Unspecified);

            if (istNow >= nextIstRun)
            {
                nextIstRun = nextIstRun.AddDays(1);
            }

            var nextRunUtc = TimeZoneInfo.ConvertTimeToUtc(nextIstRun, _indiaTimeZone);
            var delay = nextRunUtc - utcNow;

            return delay < TimeSpan.Zero ? TimeSpan.Zero : delay;
        }

        private static TimeZoneInfo ResolveIndiaTimeZone()
        {
            try
            {
                return TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            }
            catch (TimeZoneNotFoundException)
            {
                return TimeZoneInfo.FindSystemTimeZoneById("Asia/Kolkata");
            }
        }
    }
}