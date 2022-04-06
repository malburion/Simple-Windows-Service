using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleService
{
    public class UploadTaskFailedException : Exception
    {
        public UploadTaskFailedException() { }
        public UploadTaskFailedException(string message) : base(message) { }
        public UploadTaskFailedException(string message, Exception innerException) : base(message, innerException) { }
    }
}
