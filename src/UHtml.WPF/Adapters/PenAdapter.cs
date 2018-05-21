using System.Windows.Media;
using UHtml.Adapters;
using UHtml.Adapters.Entities;

namespace UHtml.WPF.Adapters
{
    /// <summary>
    /// Adapter for WPF pens objects for core.
    /// </summary>
    internal sealed class PenAdapter : RPen
    {
        /// <summary>
        /// The actual WPF brush instance.
        /// </summary>
        private readonly Brush brush;

        /// <summary>
        /// the width of the pen
        /// </summary>
        private double width;

        /// <summary>
        /// the dash style of the pen
        /// </summary>
        private DashStyle dashStyle = DashStyles.Solid;

        /// <summary>
        /// Init.
        /// </summary>
        public PenAdapter(Brush brush)
        {
            this.brush = brush;
        }

        public override double Width
        {
            get { return width; }
            set { width = value; }
        }

        public override RDashStyle DashStyle
        {
            set
            {
                switch (value)
                {
                    case RDashStyle.Solid:
                        dashStyle = DashStyles.Solid;
                        break;
                    case RDashStyle.Dash:
                        dashStyle = DashStyles.Dash;
                        break;
                    case RDashStyle.Dot:
                        dashStyle = DashStyles.Dot;
                        break;
                    case RDashStyle.DashDot:
                        dashStyle = DashStyles.DashDot;
                        break;
                    case RDashStyle.DashDotDot:
                        dashStyle = DashStyles.DashDotDot;
                        break;
                    default:
                        dashStyle = DashStyles.Solid;
                        break;
                }
            }
        }

        /// <summary>
        /// Create the actual WPF pen instance.
        /// </summary>
        public Pen CreatePen()
        {
            var pen = new Pen(brush, width);
            pen.DashStyle = dashStyle;
            return pen;
        }
    }
}