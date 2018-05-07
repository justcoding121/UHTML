







// 



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
        private readonly BitmapImage _image;

        /// <summary>
        /// Init.
        /// </summary>
        public ImageAdapter(BitmapImage image)
        {
            _image = image;
        }

        /// <summary>
        /// the underline WPF image.
        /// </summary>
        public BitmapImage Image
        {
            get { return _image; }
        }

        public override double Width
        {
            get { return _image.PixelWidth; }
        }

        public override double Height
        {
            get { return _image.PixelHeight; }
        }

        public override void Dispose()
        {
            if (_image.StreamSource != null)
                _image.StreamSource.Dispose();
        }
    }
}