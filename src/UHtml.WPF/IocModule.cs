using SimpleInjector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using PCLStorage;
using UHtml.WPF.Storage;

namespace UHtml.WPF
{
    public static class IocModule
    {
        internal static Container Container;

        public static void Register(Container iocContainer)
        {
            Container = iocContainer;

            iocContainer.Register<HttpClientHandler>();
            iocContainer.Register(StorageService.GetStorageFolder);
            UHtml.IocModule.Register(iocContainer);
        }
    }
}
