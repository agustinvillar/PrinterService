using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public static void QueuePrintEvent(string printEvent, T document) 
        {
            
        }
    }
}
