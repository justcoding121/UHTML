using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using UHtml.Core.Entities;
using UHtml.Core.Utils;

namespace UHtml.WPF
{
    /// <summary>
    /// Provides HTML rendering using the text property.<br/>
    /// WPF control that will render html content in it's client rectangle.<br/>
    /// If the layout of the html resulted in its content beyond the client bounds of the panel it will show scrollbars (horizontal/vertical) allowing to scroll the content.<br/>
    /// The control will handle mouse and keyboard events on it to support html text selection, copy-paste and mouse clicks.<br/>
    /// </summary>
    /// <remarks>
    /// See <see cref="HtmlControl"/> for more info.
    /// </remarks>
    public class HtmlPanel : HtmlControl
    {
        #region Fields and Consts

        /// <summary>
        /// the vertical scroll bar for the control to scroll to html content out of view
        /// </summary>
        protected ScrollBar verticalScrollBar;

        /// <summary>
        /// the horizontal scroll bar for the control to scroll to html content out of view
        /// </summary>
        protected ScrollBar horizontalScrollBar;

        #endregion


        static HtmlPanel()
        {
            BackgroundProperty.OverrideMetadata(typeof(HtmlPanel), new FrameworkPropertyMetadata(SystemColors.WindowBrush));

            TextProperty.OverrideMetadata(typeof(HtmlPanel), new PropertyMetadata(null, OnTextProperty_change));
        }

        /// <summary>
        /// Creates a new HtmlPanel and sets a basic css for it's styling.
        /// </summary>
        public HtmlPanel()
        {
            verticalScrollBar = new ScrollBar();
            verticalScrollBar.Orientation = Orientation.Vertical;
            verticalScrollBar.Width = 18;
            verticalScrollBar.Scroll += OnScrollBarScroll;
            AddVisualChild(verticalScrollBar);
            AddLogicalChild(verticalScrollBar);

            horizontalScrollBar = new ScrollBar();
            horizontalScrollBar.Orientation = Orientation.Horizontal;
            horizontalScrollBar.Height = 18;
            horizontalScrollBar.Scroll += OnScrollBarScroll;
            AddVisualChild(horizontalScrollBar);
            AddLogicalChild(horizontalScrollBar);

            htmlContainer.ScrollChange += OnScrollChange;
        }

        /// <summary>
        /// Adjust the scrollbar of the panel on html element by the given id.<br/>
        /// The top of the html element rectangle will be at the top of the panel, if there
        /// is not enough height to scroll to the top the scroll will be at maximum.<br/>
        /// </summary>
        /// <param name="elementId">the id of the element to scroll to</param>
        public virtual void ScrollToElement(string elementId)
        {
            ArgChecker.AssertArgNotNullOrEmpty(elementId, "elementId");

            if (htmlContainer != null)
            {
                var rect = htmlContainer.GetElementRectangle(elementId);
                if (rect.HasValue)
                {
                    ScrollToPoint(rect.Value.Location.X, rect.Value.Location.Y);
                    htmlContainer.HandleMouseMove(this, Mouse.GetPosition(this));
                }
            }
        }


        #region Private methods

        protected override int VisualChildrenCount
        {
            get { return 2; }
        }

        protected override Visual GetVisualChild(int index)
        {
            if (index == 0)
                return verticalScrollBar;
            else if (index == 1)
                return horizontalScrollBar;
            return null;
        }

        /// <summary>
        /// Perform the layout of the html in the control.
        /// </summary>
        protected override Size MeasureOverride(Size constraint)
        {
            Size size = PerformHtmlLayout(constraint);

            // to handle if scrollbar is appearing or disappearing
            bool relayout = false;
            var htmlWidth = HtmlWidth(constraint);
            var htmlHeight = HtmlHeight(constraint);

            if ((verticalScrollBar.Visibility == Visibility.Hidden && size.Height > htmlHeight) ||
                (verticalScrollBar.Visibility == Visibility.Visible && size.Height <= htmlHeight))
            {
                verticalScrollBar.Visibility = verticalScrollBar.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
                relayout = true;
            }

            if ((horizontalScrollBar.Visibility == Visibility.Hidden && size.Width > htmlWidth) ||
                (horizontalScrollBar.Visibility == Visibility.Visible && size.Width <= htmlWidth))
            {
                horizontalScrollBar.Visibility = horizontalScrollBar.Visibility == Visibility.Visible ? Visibility.Hidden : Visibility.Visible;
                relayout = true;
            }

            if (relayout)
                PerformHtmlLayout(constraint);

            if (double.IsPositiveInfinity(constraint.Width) || double.IsPositiveInfinity(constraint.Height))
                constraint = size;

            return constraint;
        }

        /// <summary>
        /// After measurement arrange the scrollbars of the panel.
        /// </summary>
        protected override Size ArrangeOverride(Size bounds)
        {
            var scrollHeight = HtmlHeight(bounds) + Padding.Top + Padding.Bottom;
            scrollHeight = scrollHeight > 1 ? scrollHeight : 1;
            var scrollWidth = HtmlWidth(bounds) + Padding.Left + Padding.Right;
            scrollWidth = scrollWidth > 1 ? scrollWidth : 1;
            verticalScrollBar.Arrange(new Rect(System.Math.Max(bounds.Width - verticalScrollBar.Width - BorderThickness.Right, 0), BorderThickness.Top, verticalScrollBar.Width, scrollHeight));
            horizontalScrollBar.Arrange(new Rect(BorderThickness.Left, System.Math.Max(bounds.Height - horizontalScrollBar.Height - BorderThickness.Bottom, 0), scrollWidth, horizontalScrollBar.Height));

            if (htmlContainer != null)
            {
                if (verticalScrollBar.Visibility == Visibility.Visible)
                {
                    verticalScrollBar.ViewportSize = HtmlHeight(bounds);
                    verticalScrollBar.SmallChange = 25;
                    verticalScrollBar.LargeChange = verticalScrollBar.ViewportSize * .9;
                    verticalScrollBar.Maximum = htmlContainer.ActualSize.Height - verticalScrollBar.ViewportSize;
                }

                if (horizontalScrollBar.Visibility == Visibility.Visible)
                {
                    horizontalScrollBar.ViewportSize = HtmlWidth(bounds);
                    horizontalScrollBar.SmallChange = 25;
                    horizontalScrollBar.LargeChange = horizontalScrollBar.ViewportSize * .9;
                    horizontalScrollBar.Maximum = htmlContainer.ActualSize.Width - horizontalScrollBar.ViewportSize;
                }

                // update the scroll offset because the scroll values may have changed
                UpdateScrollOffsets();
            }

            return bounds;
        }

        /// <summary>
        /// Perform html container layout by the current panel client size.
        /// </summary>
        protected Size PerformHtmlLayout(Size constraint)
        {
            if (htmlContainer != null)
            {
                htmlContainer.MaxSize = new Size(HtmlWidth(constraint), 0);
                htmlContainer.PerformLayout();
                return htmlContainer.ActualSize;
            }
            return Size.Empty;
        }

        /// <summary>
        /// Handle minor case where both scroll are visible and create a rectangle at the bottom right corner between them.
        /// </summary>
        protected override void OnRender(DrawingContext context)
        {
            base.OnRender(context);

            // render rectangle in right bottom corner where both scrolls meet
            if (horizontalScrollBar.Visibility == Visibility.Visible && verticalScrollBar.Visibility == Visibility.Visible)
                context.DrawRectangle(SystemColors.ControlBrush, null, new Rect(BorderThickness.Left + HtmlWidth(RenderSize), BorderThickness.Top + HtmlHeight(RenderSize), verticalScrollBar.Width, horizontalScrollBar.Height));
        }

        /// <summary>
        /// Handle mouse up to set focus on the control. 
        /// </summary>
        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            base.OnMouseUp(e);
            Focus();
        }

        /// <summary>
        /// Handle mouse wheel for scrolling.
        /// </summary>
        protected override void OnMouseWheel(MouseWheelEventArgs e)
        {
            base.OnMouseWheel(e);
            if (verticalScrollBar.Visibility == Visibility.Visible)
            {
                verticalScrollBar.Value -= e.Delta;
                UpdateScrollOffsets();
                e.Handled = true;
            }
        }

        /// <summary>
        /// Handle key down event for selection, copy and scrollbars handling.
        /// </summary>
        protected override void OnKeyDown(KeyEventArgs e)
        {
            base.OnKeyDown(e);

            if (verticalScrollBar.Visibility == Visibility.Visible)
            {
                if (e.Key == Key.Up)
                {
                    verticalScrollBar.Value -= verticalScrollBar.SmallChange;
                    UpdateScrollOffsets();
                    e.Handled = true;
                }
                else if (e.Key == Key.Down)
                {
                    verticalScrollBar.Value += verticalScrollBar.SmallChange;
                    UpdateScrollOffsets();
                    e.Handled = true;
                }
                else if (e.Key == Key.PageUp)
                {
                    verticalScrollBar.Value -= verticalScrollBar.LargeChange;
                    UpdateScrollOffsets();
                    e.Handled = true;
                }
                else if (e.Key == Key.PageDown)
                {
                    verticalScrollBar.Value += verticalScrollBar.LargeChange;
                    UpdateScrollOffsets();
                    e.Handled = true;
                }
                else if (e.Key == Key.Home)
                {
                    verticalScrollBar.Value = 0;
                    UpdateScrollOffsets();
                    e.Handled = true;
                }
                else if (e.Key == Key.End)
                {
                    verticalScrollBar.Value = verticalScrollBar.Maximum;
                    UpdateScrollOffsets();
                    e.Handled = true;
                }
            }

            if (horizontalScrollBar.Visibility == Visibility.Visible)
            {
                if (e.Key == Key.Left)
                {
                    horizontalScrollBar.Value -= horizontalScrollBar.SmallChange;
                    UpdateScrollOffsets();
                    e.Handled = true;
                }
                else if (e.Key == Key.Right)
                {
                    horizontalScrollBar.Value += horizontalScrollBar.SmallChange;
                    UpdateScrollOffsets();
                    e.Handled = true;
                }
            }
        }

        /// <summary>
        /// Get the width the HTML has to render in (not including vertical scroll iff it is visible)
        /// </summary>
        protected override double HtmlWidth(Size size)
        {
            var width = base.HtmlWidth(size) - (verticalScrollBar.Visibility == Visibility.Visible ? verticalScrollBar.Width : 0);
            return width > 1 ? width : 1;
        }

        /// <summary>
        /// Get the width the HTML has to render in (not including vertical scroll iff it is visible)
        /// </summary>
        protected override double HtmlHeight(Size size)
        {
            var height = base.HtmlHeight(size) - (horizontalScrollBar.Visibility == Visibility.Visible ? horizontalScrollBar.Height : 0);
            return height > 1 ? height : 1;
        }

        /// <summary>
        /// On HTML container scroll change request scroll to the requested location.
        /// </summary>
        private void OnScrollChange(object sender, HtmlScrollEventArgs e)
        {
            ScrollToPoint(e.X, e.Y);
        }

        /// <summary>
        /// Set the control scroll offset to the given values.
        /// </summary>
        private void ScrollToPoint(double x, double y)
        {
            horizontalScrollBar.Value = x;
            verticalScrollBar.Value = y;
            UpdateScrollOffsets();
        }

        /// <summary>
        /// On scrollbar scroll update the scroll offsets and invalidate.
        /// </summary>
        private void OnScrollBarScroll(object sender, ScrollEventArgs e)
        {
            UpdateScrollOffsets();
        }

        /// <summary>
        /// Update the scroll offset of the HTML container and invalidate visual to re-render.
        /// </summary>
        private void UpdateScrollOffsets()
        {
            var newScrollOffset = new Point(-horizontalScrollBar.Value, -verticalScrollBar.Value);
            if (!newScrollOffset.Equals(htmlContainer.ScrollOffset))
            {
                htmlContainer.ScrollOffset = newScrollOffset;
                InvalidateVisual();
            }
        }

        /// <summary>
        /// On text property change reset the scrollbars to zero.
        /// </summary>
        private static void OnTextProperty_change(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var panel = d as HtmlPanel;
            if (panel != null)
                panel.horizontalScrollBar.Value = panel.verticalScrollBar.Value = 0;
        }

        #endregion
    }
}