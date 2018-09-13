using System.IO;
using System.Windows;
using System.Windows.Media.Imaging;
using UHtml.Demo.Common;
using UHtml.WPF;
using Microsoft.Win32;
using System;

namespace UHtml.Demo.WPF
{
    /// <summary>
    /// Interaction logic for GenerateImageWindow.xaml
    /// </summary>
    public partial class GenerateImageWindow
    {
        private readonly string _html;
        private BitmapFrame _generatedImage;

        public GenerateImageWindow(string html)
        {
            _html = html;

            InitializeComponent();

            Loaded += (sender, args) => GenerateImage();
        }

        private void OnSaveToFile_click(object sender, RoutedEventArgs e)
        {
            var saveDialog = new SaveFileDialog();
            saveDialog.Filter = "Images|*.png;*.bmp;*.jpg;*.tif;*.gif;*.wmp;";
            saveDialog.FileName = "image";
            saveDialog.DefaultExt = ".png";

            var dialogResult = saveDialog.ShowDialog(this);
            if (dialogResult.GetValueOrDefault())
            {
                var encoder = HtmlRenderingHelper.GetBitmapEncoder(Path.GetExtension(saveDialog.FileName));
                encoder.Frames.Add(_generatedImage);
                using (FileStream stream = new FileStream(saveDialog.FileName, FileMode.OpenOrCreate))
                    encoder.Save(stream);
            }
        }

        private void OnGenerateImage_Click(object sender, RoutedEventArgs e)
        {
            GenerateImage();
        }

        private void GenerateImage()
        {
            //if (_imageBoxBorder.RenderSize.Width > 0 && _imageBoxBorder.RenderSize.Height > 0)
            //{
            //    _generatedImage = HtmlRender.RenderToImage(_html, _imageBoxBorder.RenderSize, null, DemoUtils.OnStylesheetLoad, HtmlRenderingHelper.OnImageLoad);
            //    _imageBox.Source = _generatedImage;
            //}

            throw new NotImplementedException();
        }
    }
}