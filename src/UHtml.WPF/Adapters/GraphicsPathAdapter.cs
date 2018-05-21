using System.Windows;
using System.Windows.Media;
using UHtml.Adapters;

namespace UHtml.WPF.Adapters
{
    /// <summary>
    /// Adapter for WPF graphics path object for core.
    /// </summary>
    internal sealed class GraphicsPathAdapter : RGraphicsPath
    {
        /// <summary>
        /// The actual WPF graphics geometry instance.
        /// </summary>
        private readonly StreamGeometry geometry = new StreamGeometry();

        /// <summary>
        /// The context used in WPF geometry to render path
        /// </summary>
        private readonly StreamGeometryContext geometryContext;

        public GraphicsPathAdapter()
        {
            geometryContext = geometry.Open();
        }

        public override void Start(double x, double y)
        {
            geometryContext.BeginFigure(new Point(x, y), true, false);
        }

        public override void LineTo(double x, double y)
        {
            geometryContext.LineTo(new Point(x, y), true, true);
        }

        public override void ArcTo(double x, double y, double size, Corner corner)
        {
            geometryContext.ArcTo(new Point(x, y), new Size(size, size), 0, false, SweepDirection.Clockwise, true, true);
        }

        /// <summary>
        /// Close the geometry to so no more path adding is allowed and return the instance so it can be rendered.
        /// </summary>
        public StreamGeometry GetClosedGeometry()
        {
            geometryContext.Close();
            geometry.Freeze();
            return geometry;
        }

        public override void Dispose()
        { }
    }
}