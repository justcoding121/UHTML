using Microsoft.VisualStudio.TestTools.UnitTesting;
using UHtml.Tests.WPF.Utility;
using System.Drawing;

namespace UHtml.Tests.WPF
{
    [TestClass]
    public class CSSTests
    {
        [TestInitialize]
        public void CSSTestsInit()
        {
            IocModule.Register(new SimpleInjector.Container());
        }

        [TestMethod]
        public void Color_Test()
        {
            var testFilesDir = TestFileManager.GetTestFile("Color", "01");

            var expectedFile = TestFileManager.GetExpectedImagePath(testFilesDir);
            var resultFile = ImageHelper.GenerateResultImage(testFilesDir, 1600, 1200);

            var expectedImg = Image.FromFile(expectedFile);
            var actualImg = Image.FromFile(resultFile);

            Assert.IsTrue(ImageHelper.AreEqual(expectedImg, actualImg));
        }

        [TestMethod]
        public void BoxModel_Test()
        {
            var testFilesDir = TestFileManager.GetTestFile("BoxModel", "01");

            var expectedFile = TestFileManager.GetExpectedImagePath(testFilesDir);
            var resultFile = ImageHelper.GenerateResultImage(testFilesDir, 1600, 1200);

            var expectedImg = Image.FromFile(expectedFile);
            var actualImg = Image.FromFile(resultFile);

            Assert.IsTrue(ImageHelper.AreEqual(expectedImg, actualImg));
        }

    }
}
