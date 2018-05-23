using System;
using UHtml.Adapters.Entities;

namespace UHtml.Core.Entities
{
    /// <summary>
    /// Raised when Html Renderer request scroll to specific location.<br/>
    /// This can occur on document anchor click.
    /// </summary>
    public sealed class HtmlScrollEventArgs : EventArgs
    {
        /// <summary>
        /// the location to scroll to
        /// </summary>
        private readonly RPoint location;

        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="location">the location to scroll to</param>
        public HtmlScrollEventArgs(RPoint location)
        {
            this.location = location;
        }

        /// <summary>
        /// the x location to scroll to
        /// </summary>
        public double X
        {
            get { return location.X; }
        }

        /// <summary>
        /// the x location to scroll to
        /// </summary>
        public double Y
        {
            get { return location.Y; }
        }

        public override string ToString()
        {
            return string.Format("Location: {0}", location);
        }
    }
}