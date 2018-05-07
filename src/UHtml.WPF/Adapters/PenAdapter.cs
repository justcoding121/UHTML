







// 



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
        private readonly Brush _brush;

        /// <summary>
        /// the width of the pen
        /// </summary>
        private double _width;

        /// <summary>
        /// the dash style of the pen
        /// </summary>
        private DashStyle _dashStyle = DashStyles.Solid;

        /// <summary>
        /// Init.
        /// </summary>
        public PenAdapter(Brush brush)
        {
            _brush = brush;
        }

        public override double Width
        {
            get { return _width; }
            set { _width = value; }
        }

        public override RDashStyle DashStyle
        {
            set
            {
                switch (value)
                {
                    case RDashStyle.Solid:
                        _dashStyle = DashStyles.Solid;
                        break;
                    case RDashStyle.Dash:
                        _dashStyle = DashStyles.Dash;
                        break;
                    case RDashStyle.Dot:
                        _dashStyle = DashStyles.Dot;
                        break;
                    case RDashStyle.DashDot:
                        _dashStyle = DashStyles.DashDot;
                        break;
                    case RDashStyle.DashDotDot:
                        _dashStyle = DashStyles.DashDotDot;
                        break;
                    default:
                        _dashStyle = DashStyles.Solid;
                        break;
                }
            }
        }

        /// <summary>
        /// Create the actual WPF pen instance.
        /// </summary>
        public Pen CreatePen()
        {
            var pen = new Pen(_brush, _width);
            pen.DashStyle = _dashStyle;
            return pen;
        }
    }
}