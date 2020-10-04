using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Hosting;
using ThrowAwayData;

namespace ThrowAwayDaemon
{
    public class Worker : IHostedService, IDisposable
    {
        private readonly Timer timer;
        public int Interval { get; set; }
        private readonly Database database;
        private readonly IConfigurationRoot configuration;

        public Worker()
        {
            Interval = 1000 * 60 * 5;
            timer = new Timer(Main, "started", Timeout.Infinite, Timeout.Infinite);
            configuration = new ConfigurationBuilder()
                .SetBasePath(Directory.GetParent(AppContext.BaseDirectory).FullName)
                .AddJsonFile("appsettings.json", false)
                .Build();
            database = new Database(ConfigurationBinder.GetValue<string>(configuration, "JackpointDatabase"));
        }

        public void Dispose() => timer.Dispose();

        #region Service Framework

        public Task StartAsync(CancellationToken cancellationToken)
        {
            timer.Change(5000, Interval);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            timer.Change(Timeout.Infinite, Timeout.Infinite);
            return Task.CompletedTask;
        }

        #endregion

        public void Main(object state)
        {
            database.CloseWeekOldThreads();
            database.AllowWeekOldUsersToPost();
        }
    }
}
