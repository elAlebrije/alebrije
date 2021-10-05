using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Alebrije.Abstractions.Background;
using Alebrije.Extensions;
using Microsoft.Extensions.Logging;

namespace Alebrije.Background
{
    /// <summary>
    /// Creates a General Purpose concrete class of <see cref="IBackgroundProcessQueue"/>
    /// </summary>
    public class BackgroundTaskQueue : IBackgroundProcessQueue
    {
        private readonly ConcurrentQueue<Func<CancellationToken, Task>> _workItems = new();
        private readonly ILogger _logger;
        private SemaphoreSlim _signal = new(1, 1);
        private int _limit = 1;

        public BackgroundTaskQueue(ILogger logger)
        {
            _logger = logger;
        }

        public bool SetQueueLimit(int limit)
        {
            if (_limit > 1)
            {
                return false;
            }
            _limit = limit;
            _signal = new SemaphoreSlim(1, limit);
            return true;
        }

        public bool QueueBackgroundProcess(Func<CancellationToken, Task> workItem)
        {
            if (workItem == null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            try
            {
                _logger.Debug("Attempting to queue new work item.");
                _logger.Debug("Queue is allowed for new work item.");
                _workItems.Enqueue(workItem);
                _signal.Release();
                return true;
            }
            catch (Exception e)
            {
                _logger.Warning("An attempt to enqueue a work Item failed.", e);
            }

            return false;
        }

        public async Task<Func<CancellationToken, Task>> DequeueProcessAsync(CancellationToken cancellationToken)
        {
            _logger.Debug("Waiting for work to process...");
            await _signal.WaitAsync(cancellationToken);
            _logger.Debug("Work received and dequeued.");
            _workItems.TryDequeue(out var workItem);

            return workItem;
        }
    }
}