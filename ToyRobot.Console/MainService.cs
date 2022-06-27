using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace ToyRobot.Console
{
    internal class MainService : BackgroundService
    {
        private readonly IHostApplicationLifetime _hostApplicationLifetime;
        private readonly IServiceProvider _services;
        private readonly ILogger _logger;

        public MainService(IHostApplicationLifetime hostApplicationLifetime, IServiceProvider services, ILogger<MainService> logger)
        {
            _hostApplicationLifetime = hostApplicationLifetime;
            _services = services;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            using var cancelSource = CancellationTokenSource.CreateLinkedTokenSource(stoppingToken);
            try
            {
                var waitForCancellationTask = Task.Delay(-1, cancelSource.Token); //-- Wait until stoppingToken is triggered
                //-- Wait for sync to complete (should not return unless crash or cancellation)
                var waitForSyncToEndTask = StartConsoleMenuAsync(cancelSource);
                var stoppedTask = await Task.WhenAny(waitForCancellationTask, waitForSyncToEndTask); //-- wait until something completes
                await stoppedTask; //-- await the stopped task to unwrap any exceptions, bubbling up the cause of stoppage if any
                if (!cancelSource.IsCancellationRequested)
                {
                    //-- in this scenario, if cancellation is not requested, but the task is completed, then we should log a warning (at least)
                    //   since it is expected the sync would run for the duration of the application lifetime
                    _logger.LogWarning($"{Assembly.GetExecutingAssembly().GetName()} stopped unexpectedly");
                    cancelSource.Cancel();
                    await waitForSyncToEndTask; //-- at point we know cancellation has been triggered and we need to wait until sync finishes
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"{Assembly.GetExecutingAssembly().GetName()} encountered an unexpected error.");
                throw; //-- bubble the exception up for anyone that cares
            }
            finally
            {
                _logger.LogInformation($"Stopping {Assembly.GetExecutingAssembly().GetName()}");
                _hostApplicationLifetime.StopApplication();
            }
        }
        
        private async Task StartConsoleMenuAsync(CancellationTokenSource cancellationTokenSource)
        {
            await Task.Yield(); //-- force asynchronous execution (useful in testing)
            var consoleInputProvider = _services.GetRequiredService<ConsoleInputProvider>();
            
            await consoleInputProvider.RunConsoleAsync(cancellationTokenSource);
        }
    }
}
