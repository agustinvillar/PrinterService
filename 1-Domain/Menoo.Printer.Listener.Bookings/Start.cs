using Menoo.PrinterService.Infraestructure;
using Menoo.PrinterService.Infraestructure.Interfaces;

namespace Menoo.Printer.Listener.Bookings
{
    [OnStartUp(Module = "Booking.Listener", Order = 2)]
    public class Start
    {
        public Start()
        {
            var dependencyResolver = GlobalConfig.DependencyResolver;
            dependencyResolver.Register<IFirebaseListener, BookingListener>();
        }
    }
}
