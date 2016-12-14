using SimpleInjector;

namespace UHtml.Demo.WPF
{
    public static class IocModule
    {
        internal static Container Container;

        public static void Register(Container iocContainer)
        {
            Container = iocContainer;


            UHtml.WPF.IocModule.Register(iocContainer);
        }
    }
}
