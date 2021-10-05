using System;
using System.Threading;
using System.Threading.Tasks;
using Alebrije.Abstractions.Background;
using Alebrije.Extensions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Alebrije.Background
{
    public class QueuedHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IBackgroundProcessQueue _taskQueue;

        public QueuedHostedService(ILogger logger, IBackgroundProcessQueue taskQueue)
        {
            _logger = logger;
            _taskQueue = taskQueue;
        }

        protected override async Task ExecuteAsync(
            CancellationToken cancellationToken)
        {
            _logger.Verbose("Queued Hosted Service is starting.");

            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.Debug($"Waiting for incoming work requests...");
                var workItem = await _taskQueue.DequeueProcessAsync(cancellationToken);
                _logger.Debug($"Process dequeued and preparing to start.");

                try
                {
                    await workItem(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.Error("Error occurred executing dequeued process.", ex);
                    throw;
                }
            }

            _logger.Verbose("Queued Hosted Service is stopping.");
        }
    }
}