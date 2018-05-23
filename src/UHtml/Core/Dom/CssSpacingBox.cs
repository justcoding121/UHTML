using System;
using System.Collections.Generic;
using UHtml.Core.Utils;

namespace UHtml.Core.Dom
{
    /// <summary>
    /// Used to make space on vertical cell combination
    /// </summary>
    internal sealed class CssSpacingBox : CssBox
    {
        #region Fields and Consts

        private readonly CssBox extendedBox;

        /// <summary>
        /// the index of the row where box starts
        /// </summary>
        private readonly int startRow;

        /// <summary>
        /// the index of the row where box ends
        /// </summary>
        private readonly int endRow;

        #endregion

        public CssSpacingBox(CssBox tableBox, ref CssBox extendedBox, int startRow)
            : base(tableBox, new HtmlTag("none", false, new Dictionary<string, string> { { "colspan", "1" } }))
        {
            this.extendedBox = extendedBox;
            Display = CssConstants.None;

            this.startRow = startRow;
            endRow = startRow + Int32.Parse(extendedBox.GetAttribute("rowspan", "1")) - 1;
        }

        public CssBox ExtendedBox
        {
            get { return extendedBox; }
        }

        /// <summary>
        /// Gets the index of the row where box starts
        /// </summary>
        public int StartRow
        {
            get { return startRow; }
        }

        /// <summary>
        /// Gets the index of the row where box ends
        /// </summary>
        public int EndRow
        {
            get { return endRow; }
        }
    }
}