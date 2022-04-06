using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleService.TaskQueue
{
    public  interface IBackgroundTaskQueue
    {
        ValueTask QueueBackgroundWorkItemAsync(UploadTask workItem);

        ValueTask<UploadTask> DequeueAsync(CancellationToken cancellationToken);
    }
}
