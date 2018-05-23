using System;
using System.Globalization;
using System.Windows.Data;
using UHtml.Demo.Common;

namespace UHtml.Demo.WPF
{
    public class ToolStripImageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var imageStream = typeof(Resources).Assembly.GetManifestResourceStream("UHtml.Demo.Common.Resources." + parameter + ".png");
            return HtmlRenderingHelper.ImageFromStream(imageStream);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}