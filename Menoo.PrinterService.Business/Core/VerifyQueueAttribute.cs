using PostSharp.Aspects;
using PostSharp.Serialization;

namespace Menoo.PrinterService.Business.Core
{
    [PSerializable]
    public class VerifyQueueAttribute : OnMethodBoundaryAspect
    {
        public override void OnExit(MethodExecutionArgs args)
        {
            while (Firebase.PrintQueue.Count > 0)
            {
                Firebase.PrintQueue.TryDequeue(out Print document);
                ProcessDocument(document);
            }
        }

        private void ProcessDocument(Print document)
        {
            switch (document.PrintEvent)
            {
                case nameof(PrintEvents.TABLE_OPENED):
                    break;
                case nameof(PrintEvents.TABLE_CLOSED):
                    break;
                case nameof(PrintEvents.NEW_BOOKING):
                    break;
                case nameof(PrintEvents.CANCELED_BOOKING):
                    break;
                case nameof(PrintEvents.NEW_TABLE_ORDER):
                    Firebase.OrderFamilyListenCallback(document.Document);
                    break;
                case nameof(PrintEvents.NEW_TAKE_AWAY):
                    break;
                case nameof(PrintEvents.ORDER_CANCELLED):
                    Firebase.OrdersCancelledCallBack(document.Document);
                    break;
            }
        }
    }
}
