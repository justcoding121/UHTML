using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using System.Windows.Media;
using System.Windows.Threading;
using UHtml.Demo.Common;
using UHtml.WPF;

namespace UHtml.Demo.WPF
{
    /// <summary>
    /// Interaction logic for DemoWindow.xaml
    /// </summary>
    public partial class DemoWindow
    {
        #region Fields/Consts

        /// <summary>
        /// the private font used for the demo
        /// </summary>
        //private readonly PrivateFontCollection _privateFont = new PrivateFontCollection();

        #endregion
        public DemoWindow()
        {
            SamplesLoader.Init("WPF", typeof(HtmlRender).Assembly.GetName().Version.ToString());

            InitializeComponent();

            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            Width = SystemParameters.PrimaryScreenWidth * 0.7;
            Height = SystemParameters.PrimaryScreenHeight * 0.8;

            LoadCustomFonts();
        }

        /// <summary>
        /// Load custom fonts to be used by renderer HTMLs
        /// </summary>
        private static void LoadCustomFonts()
        {
            // load custom font font into private fonts collection
            foreach (FontFamily fontFamily in Fonts.GetFontFamilies(new Uri("pack://application:,,,/"), "./fonts/"))
            {
                // add the fonts to renderer
                HtmlRender.AddFontFamily(fontFamily);
            }
        }

        /// <summary>
        /// Open sample window.
        /// </summary>
        private void OnOpenSampleWindow_click(object sender, RoutedEventArgs e)
        {
            var w = new SampleWindow();
            w.Owner = this;
            w.Width = Width * 0.8;
            w.Height = Height * 0.8;
            w.ShowDialog();
        }

        /// <summary>
        /// Toggle if to show split view of HtmlPanel and WinForms WebBrowser control.
        /// </summary>
        private void OnShowIEView_ButtonClick(object sender, EventArgs e)
        {
            _mainControl.ShowWebBrowserView(_showIEView.IsChecked.GetValueOrDefault(false));
        }

        /// <summary>
        /// Open the current html is external process - the default user browser.
        /// </summary>
        private void OnOpenInExternalView_Click(object sender, EventArgs e)
        {
            _mainControl.UseGeneratedHtml = true;
            _mainControl.UpdateWebBrowserHtml();

            var tmpFile = Path.ChangeExtension(Path.GetTempFileName(), ".htm");
            File.WriteAllText(tmpFile, _mainControl.GetHtml());
            Process.Start(tmpFile);
        }

        /// <summary>
        /// Toggle the use generated html button state.
        /// </summary>
        private void OnUseGeneratedHtml_Click(object sender, EventArgs e)
        {
            _mainControl.UseGeneratedHtml = _useGeneratedHtml.IsChecked.GetValueOrDefault(false);
            _mainControl.UpdateWebBrowserHtml();
        }

        /// <summary>
        /// Open generate image window for the current html.
        /// </summary>
        private void OnGenerateImage_Click(object sender, RoutedEventArgs e)
        {
            var w = new GenerateImageWindow(_mainControl.GetHtml());
            w.Owner = this;
            w.Width = Width * 0.8;
            w.Height = Height * 0.8;
            w.ShowDialog();
        }

        private static void ApplicationDoEvents()
        {
            Application.Current.Dispatcher.Invoke(DispatcherPriority.Background, new Action<bool>(delegate { }), false);
        }
    }
}