using System.Collections.Generic;
using System.IO;
using System.Reflection;

namespace UHtml.Demo.Common
{
    public static class SamplesLoader
    {
        /// <summary>
        /// Samples to test the different features of HTML Renderer that they work correctly
        /// </summary>
        private static readonly List<HtmlSample> _testCssSamples = new List<HtmlSample>();

        /// <summary>
        /// Init.
        /// </summary>
        public static void Init(string platform, string version)
        {
            LoadCssSamples(platform, version);
        }

        /// <summary>
        /// Samples to test the different CSS features of HTML Renderer that they work correctly
        /// </summary>
        public static List<HtmlSample> CssTestSamples
        {
            get { return _testCssSamples; }
        }

        private static void LoadCssSamples(string platform, string version)
        {
            var cssSampleRoot = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), "CssTestFiles");
            var cssTestModels = new DirectoryInfo(cssSampleRoot).GetDirectories();

            foreach (var cssModel in cssTestModels)
            {
                var testDirs = cssModel.GetDirectories();

                foreach (var testDir in testDirs)
                {
                    var testFile = testDir.GetFiles("*.html");

                    if (testFile.Length > 0)
                    {
                        foreach (var file in testFile)
                        {
                            _testCssSamples.Add(new HtmlSample(testDir.Name, cssModel.Name, file.Name, File.ReadAllText(file.FullName)));
                        }
                    }
                }
            }
        }
    }
}