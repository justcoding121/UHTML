
using UHtml.Tests.WPF.Utility;
using System.Drawing;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UHtml.Tests.WPF.CssTests
{
    [TestClass]
    public class BoxModel_Tests
    {
        [TestInitialize]
        public void CSSTestsInit()
        {
            IocModule.Register(new SimpleInjector.Container());
        }

        [TestMethod]
        public void BoxModel_Test_01()
        {
            var testFilesDir = TestFileManager.GetTestFile("BoxModel", "01");

            var expectedFile = TestFileManager.GetExpectedImagePath(testFilesDir);
            var resultFile = ImageHelper.GenerateResultImage(testFilesDir, 1600, 1600);

            var expectedImg = Image.FromFile(expectedFile);
            var actualImg = Image.FromFile(resultFile);

            Assert.IsTrue(ImageHelper.AreEqual(expectedImg, actualImg));
        }

    }
}
