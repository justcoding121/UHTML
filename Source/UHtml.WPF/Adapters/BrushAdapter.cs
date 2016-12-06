







// 



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
        private readonly Brush _brush;

        /// <summary>
        /// Init.
        /// </summary>
        public BrushAdapter(Brush brush)
        {
            _brush = brush;
        }

        /// <summary>
        /// The actual WPF brush instance.
        /// </summary>
        public Brush Brush
        {
            get { return _brush; }
        }

        public override void Dispose()
        { }
    }
}