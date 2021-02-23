using Unity;

namespace Menoo.PrinterService.Infraestructure
{
    public static class Bootstrapper
    {
        public static IUnityContainer UnityContainer = new UnityContainer();

        public static void Bootstrap()
        {
            Utils.PreloadAssemblies();
            Utils.GetBootstrapClasses();
        }
    }
}
