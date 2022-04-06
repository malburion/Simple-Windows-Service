using SimpleService.TaskQueue;

namespace SimpleService
{
    public class QueueHostedService : BackgroundService
    {
        private readonly ILogger<QueueHostedService> _logger;
        private readonly IBackgroundTaskQueue _taskQueue;
        private FileSystemWatcher _watcher;

        public QueueHostedService(ILogger<QueueHostedService> logger, IBackgroundTaskQueue taskQueue)
        {
            _logger = logger;
            _taskQueue = taskQueue;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            while (!stoppingToken.IsCancellationRequested)
            {
                UploadTask? workItem = await _taskQueue.DequeueAsync(stoppingToken);

                try
                {
                    _logger.LogInformation("Starting task: {0}", workItem.Id);
                    await workItem.Execute(stoppingToken);
                    _logger.LogInformation("Task {0} ended", workItem.Id);
                }
                catch (OperationCanceledException)
                {
                    // Prevent throwing if stoppingToken was signaled
                }
                catch (UploadTaskFailedException ex) {
                    _logger.LogWarning("Task {0} failed and has been requeued", workItem.Id);

                    await _taskQueue.QueueBackgroundWorkItemAsync(workItem);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error occurred executing task work item.");
                }
            }
        }

        public override async Task StopAsync(CancellationToken stoppingToken)
        {
            _logger.LogInformation(
                $"{nameof(QueueHostedService)} is stopping.");

            await base.StopAsync(stoppingToken);
        }
    }
}