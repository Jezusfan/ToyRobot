using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using ToyRobot.Core;
using ToyRobot.Interface.Movement;

namespace ToyRobot.Console
{
    public static class Program
    {
        static Program()
        {
#if !DEBUG
      Environment.SetEnvironmentVariable("DOTNET_ENVIRONMENT", Environments.Production);
#endif
        }

        public static async Task Main()
        {
            System.Console.WriteLine($"Starting Program (in {Directory.GetCurrentDirectory()})");
            var cancellation = new CancellationTokenSource(); //-- NOTE: Not disposed because this method is expected to be called only once
            AppDomain.CurrentDomain.ProcessExit += (_, _) => cancellation.Cancel();
            AppDomain.CurrentDomain.DomainUnload += (_, _) => cancellation.Cancel();
            await MainAsync(cancellation.Token);
             System.Console.WriteLine($"Program Finished (Process Exit: {cancellation.IsCancellationRequested})");
        }

        public static async Task MainAsync(CancellationToken cancel)
        {
            await MainAsync(b => b, cancel);
        }

        public static async Task MainAsync(Func<IHostBuilder, IHostBuilder> customiseHost, CancellationToken cancel)
        {
            using var host = customiseHost(CreateHostBuilder()).Build();
            var env = host.Services.GetRequiredService<IHostEnvironment>();
            System.Console.WriteLine($"Environment: {env.EnvironmentName}");
            await host.RunAsync(cancel).ConfigureAwait(false);
        }

        public static IHostBuilder CreateHostBuilder(Action<IServiceCollection>? overrides = null)
        {
            return Host.CreateDefaultBuilder()
                .ConfigureServices(ConfigureServices)
                .ConfigureServices(overrides ?? (_ => { }));
        }

        private static void ConfigureServices(IServiceCollection services)
        {
            services
                //-- MainService is the root of the Robot process (*** Start Here ***)
                .AddHostedService<MainService>() 
                //-- this turns configuration sections into POCO configuration classes
                .AddSingleton(sp => sp.GetRequiredService<IConfiguration>().GetSection(Configuration.KnownConfigKeys.Surface).Get<Configuration.SurfaceConfig>())
                .AddLogging()
                //Wire up services
                .AddSingleton<IMovementProcessor, MovementProcessor>()
                .AddSingleton<IRoutePlanner, RoutePlanner>()
                .AddTransient<IMovableObject, Robot>()
                .AddTransient<ConsoleInputProvider>();
        }
    }
}

