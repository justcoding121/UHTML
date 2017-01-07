using Newtonsoft.Json;
using PCLStorage;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHtml.Core.Dom;
using UHtml.Core.Utils;

namespace UHtml.Debug
{
    public class DOMDebugger
    {
        internal static void WriteDOMToJson(CssBox root)
        {
            var serialized = JsonConvert.SerializeObject(root, Formatting.Indented);
            var tempFile = StorageUtils.GetTempFileName();

            var file = FileSystem.Current.GetFileFromPathAsync(tempFile).Result;

            file.WriteAllTextAsync(serialized);

        }
    }
}
