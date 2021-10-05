using System;
using System.Threading;
using System.Threading.Tasks;
using Alebrije.Abstractions.Background;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace Alebrije.Framework.Background
{
    public class QueuedHostedService : BackgroundService
    {
        private readonly ILogger _logger;
        private readonly IBackgroundProcessQueue _taskQueue;

        public QueuedHostedService(ILogger logger,IBackgroundProcessQueue taskQueue)
        {
            _logger = logger;
            _taskQueue = taskQueue;
        }

        protected override async Task ExecuteAsync(
            CancellationToken cancellationToken)
        {
            _logger.LogTrace("Queued Hosted Service is starting.");

            while (!cancellationToken.IsCancellationRequested)
            {
                _logger.LogDebug($"Waiting for incoming work requests...");
                var workItem = await _taskQueue.DequeueProcessAsync(cancellationToken);
                _logger.LogDebug($"Process dequeued and preparing to start.");

                try
                {
                    await workItem(cancellationToken);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing dequeued process.");
                    throw;
                }
            }

            _logger.LogTrace("Queued Hosted Service is stopping.");
        }
    }
}