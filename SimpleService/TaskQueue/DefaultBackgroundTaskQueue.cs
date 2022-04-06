using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;

namespace SimpleService.TaskQueue
{
    public class DefaultBackgroundTaskQueue : IBackgroundTaskQueue
    {
        private readonly Channel<UploadTask> _queue;

        public DefaultBackgroundTaskQueue(int capacity)
        {
            BoundedChannelOptions options = new(capacity)
            {
                FullMode = BoundedChannelFullMode.Wait
            };
            _queue = Channel.CreateBounded<UploadTask>(options);
        }

        public async ValueTask QueueBackgroundWorkItemAsync(UploadTask workItem)
        {
            if (workItem is null)
            {
                throw new ArgumentNullException(nameof(workItem));
            }

            await _queue.Writer.WriteAsync(workItem);
        }

        public async ValueTask<UploadTask> DequeueAsync(CancellationToken cancellationToken)
        {
            UploadTask? workItem = await _queue.Reader.ReadAsync(cancellationToken);

            return workItem;
        }
    }
}
