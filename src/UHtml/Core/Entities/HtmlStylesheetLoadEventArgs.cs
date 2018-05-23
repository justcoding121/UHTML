using System;
using System.Collections.Generic;

namespace UHtml.Core.Entities
{
    /// <summary>
    /// Invoked when a stylesheet is about to be loaded by file path or URL in 'link' element.<br/>
    /// Allows to overwrite the loaded stylesheet by providing the stylesheet data manually, or different source (file or URL) to load from.<br/>
    /// Example: The stylesheet 'href' can be non-valid URI string that is interpreted in the overwrite delegate by custom logic to pre-loaded stylesheet object<br/>
    /// If no alternative data is provided the original source will be used.<br/>
    /// </summary>
    public sealed class HtmlStylesheetLoadEventArgs : EventArgs
    {
        #region Fields and Consts

        /// <summary>
        /// the source of the stylesheet as found in the HTML (file path or URL)
        /// </summary>
        private readonly string src;

        /// <summary>
        /// collection of all the attributes that are defined on the link element
        /// </summary>
        private readonly Dictionary<string, string> attributes;

        /// <summary>
        /// provide the new source (file path or URL) to load stylesheet from
        /// </summary>
        private string setSrc;

        /// <summary>
        /// provide the stylesheet to load
        /// </summary>
        private string setStyleSheet;

        /// <summary>
        /// provide the stylesheet data to load
        /// </summary>
        private CssData setStyleSheetData;

        #endregion

        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="src">the source of the image (file path or URL)</param>
        /// <param name="attributes">collection of all the attributes that are defined on the image element</param>
        internal HtmlStylesheetLoadEventArgs(string src, Dictionary<string, string> attributes)
        {
            this.src = src;
            this.attributes = attributes;
        }

        /// <summary>
        /// the source of the stylesheet as found in the HTML (file path or URL)
        /// </summary>
        public string Src
        {
            get { return src; }
        }

        /// <summary>
        /// collection of all the attributes that are defined on the link element
        /// </summary>
        public Dictionary<string, string> Attributes
        {
            get { return attributes; }
        }

        /// <summary>
        /// provide the new source (file path or URL) to load stylesheet from
        /// </summary>
        public string SetSrc
        {
            get { return setSrc; }
            set { setSrc = value; }
        }

        /// <summary>
        /// provide the stylesheet to load
        /// </summary>
        public string SetStyleSheet
        {
            get { return setStyleSheet; }
            set { setStyleSheet = value; }
        }

        /// <summary>
        /// provide the stylesheet data to load
        /// </summary>
        public CssData SetStyleSheetData
        {
            get { return setStyleSheetData; }
            set { setStyleSheetData = value; }
        }
    }
}