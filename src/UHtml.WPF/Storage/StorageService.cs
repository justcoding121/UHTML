using System;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;
using PCLStorage;

namespace UHtml.WPF.Storage
{
    public class StorageService
    {
        private static readonly Lazy<IFolder> cache = new Lazy<IFolder>(() => FileSystem.Current.GetFolderFromPathAsync(AssemblyDirectory).Result);

        public static IFolder GetStorageFolder()
        {
            return cache.Value;
        }

        private static string AssemblyDirectory
        {
            get
            {
                string codeBase = Assembly.GetExecutingAssembly().CodeBase;
                UriBuilder uri = new UriBuilder(codeBase);
                string path = Uri.UnescapeDataString(uri.Path);
                return Path.GetDirectoryName(path);
            }
        }
    }
}
