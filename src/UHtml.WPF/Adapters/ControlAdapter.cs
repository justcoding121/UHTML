using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using UHtml.Adapters;
using UHtml.Adapters.Entities;
using UHtml.Core.Utils;
using UHtml.WPF.Utilities;

namespace UHtml.WPF.Adapters
{
    /// <summary>
    /// Adapter for WPF Control for core.
    /// </summary>
    internal sealed class ControlAdapter : RControl
    {
        /// <summary>
        /// the underline WPF control.
        /// </summary>
        private readonly Control control;

        /// <summary>
        /// Init.
        /// </summary>
        public ControlAdapter(Control control)
            : base(WpfAdapter.Instance)
        {
            ArgChecker.AssertArgNotNull(control, "control");

            this.control = control;
        }

        /// <summary>
        /// Get the underline WPF control
        /// </summary>
        public Control Control
        {
            get { return control; }
        }

        public override RPoint MouseLocation
        {
            get { return Utils.Convert(control.PointFromScreen(Mouse.GetPosition(control))); }
        }

        public override bool LeftMouseButton
        {
            get { return Mouse.LeftButton == MouseButtonState.Pressed; }
        }

        public override bool RightMouseButton
        {
            get { return Mouse.RightButton == MouseButtonState.Pressed; }
        }

        public override void SetCursorDefault()
        {
            control.Cursor = Cursors.Arrow;
        }

        public override void SetCursorHand()
        {
            control.Cursor = Cursors.Hand;
        }

        public override void SetCursorIBeam()
        {
            control.Cursor = Cursors.IBeam;
        }

        public override void DoDragDropCopy(object dragDropData)
        {
            DragDrop.DoDragDrop(control, dragDropData, DragDropEffects.Copy);
        }

        public override void MeasureString(string str, RFont font, double maxWidth, out int charFit, out double charFitWidth)
        {
            using (var g = new GraphicsAdapter())
            {
                g.MeasureString(str, font, maxWidth, out charFit, out charFitWidth);
            }
        }

        public override void Invalidate()
        {
            control.InvalidateVisual();
        }
    }
}