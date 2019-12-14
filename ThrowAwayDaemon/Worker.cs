using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;

namespace ThrowAwayDaemon
{
    public class Worker : IHostedService, IDisposable
    {
        private readonly Timer timer;
        public int Interval { get; set; }

        public Worker()
        {
            Interval = 1000 * 5;
            timer = new Timer(Main, "started", Timeout.Infinite, Timeout.Infinite);
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
            //TODO: All SQLite Jobs would go here.
        }
    }
}
