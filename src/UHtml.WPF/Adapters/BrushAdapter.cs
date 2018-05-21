using System.Windows.Media;
using UHtml.Adapters;

namespace UHtml.WPF.Adapters
{
    /// <summary>
    /// Adapter for WPF brushes.
    /// </summary>
    internal sealed class BrushAdapter : RBrush
    {
        /// <summary>
        /// The actual WPF brush instance.
        /// </summary>
        private readonly Brush brush;

        /// <summary>
        /// Init.
        /// </summary>
        public BrushAdapter(Brush brush)
        {
            this.brush = brush;
        }

        /// <summary>
        /// The actual WPF brush instance.
        /// </summary>
        public Brush Brush
        {
            get { return brush; }
        }

        public override void Dispose()
        { }
    }
}