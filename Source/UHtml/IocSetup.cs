using SimpleInjector;

namespace UHtml
{
    public static class IocModule
    {
        internal static Container Container;

        public static void Register(Container iocContainer)
        {
            Container = iocContainer;
        }
    }
}
