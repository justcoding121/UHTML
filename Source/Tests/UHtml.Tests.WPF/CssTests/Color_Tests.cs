using UHtml.Tests.WPF.Utility;
using System.Drawing;
using NUnit.Framework;

namespace UHtml.Tests.WPF.CssTests
{
    [TestFixture]
    public class Color_Tests
    {
        [SetUp]
        public void CSSTestsInit()
        {
            IocModule.Register(new SimpleInjector.Container());
        }

        [Test]
        public void Color_Test_01()
        {
            var testFilesDir = TestFileManager.GetTestFile("Color", "01");

            var expectedFile = TestFileManager.GetExpectedImagePath(testFilesDir);
            var resultFile = ImageHelper.GenerateResultImage(testFilesDir, 1600, 1200);

            var expectedImg = Image.FromFile(expectedFile);
            var actualImg = Image.FromFile(resultFile);

            Assert.IsTrue(ImageHelper.AreEqual(expectedImg, actualImg));
        }

    }
}
