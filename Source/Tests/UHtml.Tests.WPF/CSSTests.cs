using Microsoft.VisualStudio.TestTools.UnitTesting;
using UHtml.WPF;
using System.IO;
using UHtml.Tests.WPF.Utility;
using System.Drawing;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;

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
            var htmlTest = File.ReadAllText(TestFileManager.GetHtmlTestFilePath(TestFiles.Color_1));
            using (Bitmap expectedResultFile = new Bitmap(Image.FromFile(TestFileManager.GetTestImagePath(TestFiles.Color_1))))
            using (Bitmap expectedResult = expectedResultFile.Clone(new Rectangle(0, 0, expectedResultFile.Width, expectedResultFile.Height), PixelFormat.Format32bppRgb))
            {

                var actualResultBitmapSource = HtmlRender.RenderToImage(htmlTest,
                    new System.Windows.Size(expectedResult.Size.Width, expectedResult.Size.Height));

                using (var actualResult = BitmapFromSource(actualResultBitmapSource))
                {
                   
                    TestFileManager.WriteResultFile(TestFiles.Color_1, actualResult);
                }

            }
        }

        private static Bitmap BitmapFromSource(BitmapSource bitmapsource)
        {
            System.Drawing.Bitmap bitmap;
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();

                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new System.Drawing.Bitmap(outStream);
            }
            return bitmap;
        }

        private bool CompareBitmaps(Image left, Image right)
        {
            if (object.Equals(left, right))
                return true;
            if (left == null || right == null)
                return false;
            if (!left.Size.Equals(right.Size) || !left.PixelFormat.Equals(right.PixelFormat))
                return false;

            Bitmap leftBitmap = left as Bitmap;
            Bitmap rightBitmap = right as Bitmap;
            if (leftBitmap == null || rightBitmap == null)
                return true;

            #region Code taking more time for comparison

            for (int col = 0; col < left.Width; col++)
            {
                for (int row = 0; row < left.Height; row++)
                {
                    if (!leftBitmap.GetPixel(col, row).Equals(rightBitmap.GetPixel(col, row)))
                        return false;
                }
            }

            #endregion

            return true;
        }
    }
}
