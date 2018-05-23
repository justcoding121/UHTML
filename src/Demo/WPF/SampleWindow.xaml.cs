using System.Windows;
using System.Windows.Input;
using UHtml.Demo.Common;
using Xceed.Wpf.Toolkit.PropertyGrid;

namespace UHtml.Demo.WPF
{
    /// <summary>
    /// Interaction logic for SampleWindow.xaml
    /// </summary>
    public partial class SampleWindow
    {
        public SampleWindow()
        {
            InitializeComponent();

            _htmlLabel.Text = DemoUtils.SampleHtmlLabelText;
            _htmlPanel.Text = DemoUtils.SampleHtmlPanelText;

            _propertyGrid.SelectedObject = _htmlLabel;
        }

        private void OnHtmlControl_click(object sender, MouseButtonEventArgs e)
        {
            _propertyGrid.SelectedObject = sender;
        }

        private void OnPropertyChanged(object sender, PropertyValueChangedEventArgs e)
        {
            var control = (UIElement)_propertyGrid.SelectedObject;
            control.InvalidateMeasure();
            control.InvalidateVisual();
        }
    }
}
