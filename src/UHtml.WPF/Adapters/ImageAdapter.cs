using System.Windows.Media.Imaging;
using UHtml.Adapters;

namespace UHtml.WPF.Adapters
{
    /// <summary>
    /// Adapter for WPF Image object for core.
    /// </summary>
    internal sealed class ImageAdapter : RImage
    {
        /// <summary>
        /// the underline WPF image.
        /// </summary>
        private readonly BitmapImage image;

        /// <summary>
        /// Init.
        /// </summary>
        public ImageAdapter(BitmapImage image)
        {
            this.image = image;
        }

        /// <summary>
        /// the underline WPF image.
        /// </summary>
        public BitmapImage Image
        {
            get { return image; }
        }

        public override double Width
        {
            get { return image.PixelWidth; }
        }

        public override double Height
        {
            get { return image.PixelHeight; }
        }

        public override void Dispose()
        {
            if (image.StreamSource != null)
                image.StreamSource.Dispose();
        }
    }
}