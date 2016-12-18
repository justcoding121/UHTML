using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;

namespace UHtml.Tests.WPF.Utility
{
    public class TestFileManager
    {
        public static string GetHtmlTestFilePath(string testFileName)
        {
            var executingDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            return Path.Combine(executingDir, "TestFiles", testFileName + ".html");
        }

        internal static string GetTestImagePath(string testFileName)
        {
            var executingDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            return Path.Combine(executingDir, "TestFiles",  testFileName + ".png");
        }

        internal static void WriteResultFile(string resultFileName, Bitmap result)
        {
            var executingDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var resultFilePath = Path.Combine(executingDir, "TestFiles", resultFileName + "_result.png");

            new Bitmap(result).Save(resultFilePath, ImageFormat.Png);
        }
    }
}
