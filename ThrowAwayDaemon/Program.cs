using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace ThrowAwayDaemon
{
    public class Program
    {
        public static void Main(string[] args) => CreateHostBuilder(args).Build().Run();

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .UseWindowsService()
                .ConfigureServices(services =>
                {
                    services.AddHostedService<Worker>();
                });
    }
}
