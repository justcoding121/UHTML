using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using UHtml.WPF;

namespace UHtml.Tests.WPF.Utility
{
    public class ImageHelper
    {
        /// <summary>
        /// Renders the given HTML test Files as Image
        /// And returns it file path
        /// </summary>
        /// <param name="testFile"></param>
        /// <param name="width"></param>
        /// <param name="height"></param>
        /// <returns></returns>
        public static string GenerateResultImage(string testFile, int width, int height)
        {
            var testHtml = File.ReadAllText(TestFileManager.GetHtmlTestFilePath(testFile));

            using (var expectedResultFile = new Bitmap(Image.FromFile(TestFileManager.GetExpectedImagePath(testFile))))
            using (var expectedResult = expectedResultFile.Clone(
                new Rectangle(0, 0, expectedResultFile.Width, expectedResultFile.Height),
                PixelFormat.Format32bppRgb))
            {

                var actualResultBitmapSource = HtmlRender.RenderToImage(testHtml,
                    new System.Windows.Size(width, height));

                using (var actualResult = ToBitmap(actualResultBitmapSource))
                {
                   return TestFileManager.WriteResultFile(testFile, actualResult);
                }

            }
        }

        /// <summary>
        /// Convert to Bitmat from BitmapSource
        /// </summary>
        /// <param name="bitmapsource"></param>
        /// <returns></returns>
        private static Bitmap ToBitmap(BitmapSource bitmapsource)
        {
            Bitmap bitmap;
            using (MemoryStream outStream = new MemoryStream())
            {
                var enc = new BmpBitmapEncoder();

                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new Bitmap(outStream);
            }
            return bitmap;
        }

        /// <summary>
        /// Returns true if given two files are same
        /// </summary>
        /// <param name="left"></param>
        /// <param name="right"></param>
        /// <returns></returns>
        public static bool AreEqual(Image left, Image right)
        {
            if (Equals(left, right))
                return true;

            if (left == null || right == null)
                return false;

            if (!left.Size.Equals(right.Size) || !left.PixelFormat.Equals(right.PixelFormat))
                return false;

            var leftBitmap = left as Bitmap;
            var rightBitmap = right as Bitmap;


            if (leftBitmap == null || rightBitmap == null)
                return true;


            for (int col = 0; col < left.Width; col++)
            {
                for (int row = 0; row < left.Height; row++)
                {
                    if (!leftBitmap.GetPixel(col, row).Equals(rightBitmap.GetPixel(col, row)))
                        return false;
                }
            }

            return true;
        }
    }
}
