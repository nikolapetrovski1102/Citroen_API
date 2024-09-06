
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using System;
using System.Threading;
using System.Threading.Tasks;



namespace CitroenAPI.Controllers
{

    public class LogCleanupBackgroundService : BackgroundService
    {
        private readonly LogCleanupService _logCleanupService;
        private readonly ILogger<LogCleanupBackgroundService> _logger;
        private readonly TimeSpan _cleanupInterval = TimeSpan.FromDays(1); // Run daily

        public LogCleanupBackgroundService(LogCleanupService logCleanupService, ILogger<LogCleanupBackgroundService> logger)
        {
            _logCleanupService = logCleanupService;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    _logger.LogInformation("Starting log cleanup.");
                    _logCleanupService.CleanupOldLogs();
                    _logger.LogInformation("Log cleanup completed.");
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "An error occurred while cleaning up logs.");
                }

                // Wait for the specified interval before running again
                await Task.Delay(_cleanupInterval, stoppingToken);
            }
        }
    }
}