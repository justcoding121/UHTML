using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace UHtml.Tests.WPF.Utility
{
    public class TestFileManager
    {
        public static string GetHtmlTestFilePath(string testFileName)
        {
            var executingDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            return Path.Combine(executingDir, "TestFiles", testFileName, testFileName + ".html");
        }

        internal static string GetTestImagePath(string testFileName)
        {
            var executingDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            return Path.Combine(executingDir, "TestFiles", testFileName, testFileName + ".png");
        }

        internal static void WriteResultFile(string resultFileName, Bitmap result)
        {
            var executingDir = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var resultDir = Path.Combine(executingDir, "TestFiles", resultFileName);

            if(!Directory.Exists(resultDir))
            {
                Directory.CreateDirectory(resultDir);
            }

            var resultFilePath = Path.Combine(resultDir, "Result_"+ resultFileName + ".png");

            new Bitmap(result).Save(resultFilePath, ImageFormat.Png);
        }
    }
}
