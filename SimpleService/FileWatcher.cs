using SimpleService.TaskQueue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleService
{
    public class FileWatcher
    {
        private readonly IBackgroundTaskQueue _taskQueue;
        private readonly ILogger<FileWatcher> _logger;
        private readonly CancellationToken _cancellationToken;
        private readonly Random _random;
        private FileSystemWatcher _watcher;

        public FileWatcher(ILogger<FileWatcher> logger, IBackgroundTaskQueue taskQueue, IHostApplicationLifetime applicationLifetime)
        {
            _logger = logger;
            _taskQueue = taskQueue;
            _cancellationToken = applicationLifetime.ApplicationStopping;
            _random = new Random();
        }

        public void WatchFolder(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                throw new ArgumentNullException("path is not valid");
            }

            if (_watcher is not null)
            {
                _watcher.Dispose();
            }

            ConfigureWatcher(path);
        }

        private void ConfigureWatcher(string path)
        {
            _watcher = new FileSystemWatcher(path);

            _watcher.NotifyFilter = NotifyFilters.Attributes
                                | NotifyFilters.CreationTime
                                | NotifyFilters.DirectoryName
                                | NotifyFilters.FileName
                                | NotifyFilters.LastAccess
                                | NotifyFilters.LastWrite
                                | NotifyFilters.Security
                                | NotifyFilters.Size;

            _watcher.Created += OnCreated;

            _watcher.Filter = "*.txt";
            _watcher.IncludeSubdirectories = true;
            _watcher.EnableRaisingEvents = true;
        }

        private async void OnCreated(object sender, FileSystemEventArgs e)
        {
            UploadTask uploadTask = CreateUploadTask();

            _logger.LogInformation("Queuing new task: {0}", uploadTask.Id);
            await _taskQueue.QueueBackgroundWorkItemAsync(uploadTask);
        }

        private UploadTask CreateUploadTask()
        {
            return new UploadTask(BuildWorkItemAsync);
        }

        private async ValueTask BuildWorkItemAsync(CancellationToken token)
        {
            // Simulate three 5-second tasks to complete
            // for each enqueued work item

            int delayLoop = 0;

            bool success = _random.Next() % 2 == 0;

            while (!token.IsCancellationRequested && delayLoop < 3)
            {
                if (!success)
                {
                    throw new UploadTaskFailedException();
                }

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(5), token);
                }
                catch (OperationCanceledException)
                {
                    // Prevent throwing if the Delay is cancelled
                }

                ++delayLoop;
            }
        }
    }
}
