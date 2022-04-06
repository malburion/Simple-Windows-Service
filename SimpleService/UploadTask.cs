using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleService
{
    public class UploadTask
    {
        private readonly Guid _id;
        private readonly Func<CancellationToken, ValueTask> _task;

        public UploadTask(Func<CancellationToken, ValueTask> task)
        {
            _id = Guid.NewGuid();
            _task = task;
        }

        public Guid Id { get { return _id; } }

        public ValueTask Execute(CancellationToken cancellationToken)
        {
            return _task(cancellationToken);
        }
    }
}
