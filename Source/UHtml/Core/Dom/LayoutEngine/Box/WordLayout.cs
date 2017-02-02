using System;
using UHtml.Adapters;
using UHtml.Core.Utils;

namespace UHtml.Core.Dom
{

    internal static partial class CssLayoutEngine
    {

        /// <summary>
        /// Should update the actual bottom of the box is height is auto
        /// </summary>
        /// <param name="g"></param>
        /// <param name="box"></param>
        /// <param name="curX"></param>
        /// <param name="curY"></param>
        /// <param name="rightLimit"></param>
        private static WordLayoutStatus LayoutWords(RGraphics g,
            CssBox box,
            CssLineBox currentLineBox,
            double curX, double curY,
            double leftLimit, double rightLimit, double currentMaxBottom)
        {

            double localMaxLineBottom = currentMaxBottom;

            if (box.Words.Count > 0)
            {
                if (currentLineBox == null)
                {
                    currentLineBox = new CssLineBox(box);
                }

                if (DomUtils.DoesBoxHasWhitespace(box))
                    curX += box.ActualWordSpacing;

                foreach (var word in box.Words)
                {
                    if ((box.WhiteSpace != CssConstants.NoWrap
                        && box.WhiteSpace != CssConstants.Pre
                        && curX + word.Width > rightLimit
                         && (box.WhiteSpace != CssConstants.PreWrap || !word.IsSpaces))
                        || word.IsLineBreak)
                    {

                        curX = leftLimit;
                        curY = localMaxLineBottom;

                        currentLineBox = new CssLineBox(box);

                    }

                    currentLineBox.ReportExistanceOf(word);

                    word.Left = curX;
                    word.Top = curY;

                    curX = word.Left + word.FullWidth;
                    localMaxLineBottom = Math.Max(localMaxLineBottom, word.Bottom);

                }

                //set x,y location
                if (box.Height == CssConstants.Auto)
                {

                    box.ActualBottom = localMaxLineBottom
                        + box.ActualBorderBottomWidth
                        + box.ActualPaddingBottom;

                }

                currentMaxBottom = box.ActualBottom;


                if (box.Width == CssConstants.Auto)
                {
                    box.ActualRight = curX
                        + box.ActualBorderRightWidth
                        + box.ActualPaddingRight;
                }


            }

            return new WordLayoutStatus()
            {
                CurrentLineBox = currentLineBox,
                CurX = curX,
                CurY = curY,
                CurrentMaxBottom = currentMaxBottom
            };
        }
    }

    internal class WordLayoutStatus
    {
        public double CurX { get; set; }
        public double CurY { get; set; }

        public CssLineBox CurrentLineBox { get; set; }
        public double CurrentMaxBottom { get; internal set; }
    }
}
