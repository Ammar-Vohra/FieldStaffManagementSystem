using FieldStaff.Application.Features.ServiceTasks.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace FieldStaff.API.BackgroundServices
{
    public class OverdueTaskBackgroundService : BackgroundService
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly ILogger<OverdueTaskBackgroundService> _logger;

        public OverdueTaskBackgroundService(
            IServiceScopeFactory scopeFactory,
            ILogger<OverdueTaskBackgroundService> logger)
        {
            _scopeFactory = scopeFactory;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(
            CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                "Overdue Task Background Service started.");

            using var timer = new PeriodicTimer(
                TimeSpan.FromMinutes(1));

            // Run once immediately when the application starts
            await ProcessOverdueTasksAsync(stoppingToken);

            // Then run every minute
            while (await timer.WaitForNextTickAsync(stoppingToken))
            {
                await ProcessOverdueTasksAsync(stoppingToken);
            }
        }

        private async Task ProcessOverdueTasksAsync(
            CancellationToken stoppingToken)
        {
            try
            {
                using var scope = _scopeFactory.CreateScope();

                var serviceTaskService = scope.ServiceProvider
                    .GetRequiredService<IServiceTaskService>();

                var count = await serviceTaskService
                    .ProcessOverdueTasksAsync();

                if (count > 0)
                {
                    _logger.LogInformation(
                        "{Count} task(s) automatically marked as Overdue.",
                        count);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                // Application is shutting down
            }
            catch (Exception ex)
            {
                _logger.LogError(
                    ex,
                    "An error occurred while processing overdue tasks.");
            }
        }
    }
}