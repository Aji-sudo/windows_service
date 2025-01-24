using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

public class Program
{
    public static async Task Main(string[] args)
    {
        if (Environment.UserInteractive)
        {
            // Running in interactive mode (e.g., Visual Studio debugging)
            Console.WriteLine("Running in interactive mode...");
            var host = CreateHostBuilder(args).Build();

            // Start the Worker service manually
            using var scope = host.Services.CreateScope();
            var worker = scope.ServiceProvider.GetRequiredService<Worker>();

            // Simulate worker execution (as if it's a console app)
            await worker.StartAsync(new System.Threading.CancellationToken());
            Console.WriteLine("Press Enter to exit...");
            Console.ReadLine();
            await worker.StopAsync(new System.Threading.CancellationToken());
        }
        else
        {
            // Running as a Windows Service
            CreateHostBuilder(args).Build().Run();
        }
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
            .ConfigureServices((hostContext, services) =>
            {
                services.AddHostedService<Worker>();
                services.AddSingleton<Worker>();
            });
}


