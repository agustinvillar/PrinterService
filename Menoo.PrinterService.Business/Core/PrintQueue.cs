using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Menoo.PrinterService.Business.Core
{
    public sealed class PrintQueue<T> : ConcurrentQueue<T> where T : class
    {
        public PrintQueue() : base()
        {
        }

        public PrintQueue(IEnumerable<T> items) : base(items)
        {
        }
    }
}
