using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;

namespace UHtml.Tests.WPF.Utility
{
    public class TestFileManager
    {
        public static string GetTestFile(string fileName, string fileNumber)
        {
            return string.Format(@"{0}\{0}_{1}\{0}_{1}", fileName, fileNumber);
        }

        public static string GetHtmlTestFilePath(string testFileName)
        {
            var executingDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            return Path.Combine(executingDir, "CssTestFiles", testFileName + ".html");
        }

        public static string GetExpectedImagePath(string testFileName)
        {
            var executingDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            return Path.Combine(executingDir, "CssTestFiles",  testFileName + ".png");
        }

        public static string WriteResultFile(string resultFileName, Bitmap result)
        {
            var executingDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var resultFilePath = Path.Combine(executingDir, "CssTestFiles", resultFileName + "_result.png");

            new Bitmap(result).Save(resultFilePath, ImageFormat.Png);

            return resultFilePath;
        }
    }
}
