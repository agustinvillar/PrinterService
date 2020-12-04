using Google.Cloud.Firestore;

namespace Menoo.PrinterService.Business.Core
{
    public sealed class Print
    {
        public string PrintEvent { get; set; }

        public DocumentSnapshot Document { get; set; }
    }
}
