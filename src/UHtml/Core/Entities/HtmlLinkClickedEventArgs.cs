using System;
using System.Collections.Generic;

namespace UHtml.Core.Entities
{
    /// <summary>
    /// Raised when the user clicks on a link in the html.
    /// </summary>
    public sealed class HtmlLinkClickedEventArgs : EventArgs
    {
        /// <summary>
        /// the link href that was clicked
        /// </summary>
        private readonly string link;

        /// <summary>
        /// collection of all the attributes that are defined on the link element
        /// </summary>
        private readonly Dictionary<string, string> attributes;

        /// <summary>
        /// use to cancel the execution of the link
        /// </summary>
        private bool handled;

        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="link">the link href that was clicked</param>
        public HtmlLinkClickedEventArgs(string link, Dictionary<string, string> attributes)
        {
            this.link = link;
            this.attributes = attributes;
        }

        /// <summary>
        /// the link href that was clicked
        /// </summary>
        public string Link
        {
            get { return link; }
        }

        /// <summary>
        /// collection of all the attributes that are defined on the link element
        /// </summary>
        public Dictionary<string, string> Attributes
        {
            get { return attributes; }
        }

        /// <summary>
        /// use to cancel the execution of the link
        /// </summary>
        public bool Handled
        {
            get { return handled; }
            set { handled = value; }
        }

        public override string ToString()
        {
            return string.Format("Link: {0}, Handled: {1}", link, handled);
        }
    }
}