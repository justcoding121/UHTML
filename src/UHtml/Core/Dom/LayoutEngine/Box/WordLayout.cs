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
            double initialX, double initialY,
            double leftLimit, double rightLimit, double initialBottom)
        {

            double curX = initialX;
            double curY = initialY;

            double maxRight = initialX;
            double maxBottom = initialBottom;

            if (box.Words.Count > 0)
            {
               currentLineBox = currentLineBox ?? new CssLineBox(box);

                if (DomUtils.DoesBoxHasWhitespace(box))
                {
                    curX += box.ActualWordSpacing;
                    maxRight = curX;
                }

                foreach (var word in box.Words)
                {
                    if ((box.WhiteSpace != CssConstants.NoWrap
                        && box.WhiteSpace != CssConstants.Pre
                        && curX + word.Width > rightLimit
                         && (box.WhiteSpace != CssConstants.PreWrap || !word.IsSpaces))
                        || word.IsLineBreak)
                    {

                        curX = leftLimit;
                        curY = maxBottom;

                        currentLineBox = new CssLineBox(box);

                    }

                    currentLineBox.ReportExistanceOf(word);

                    word.Left = curX;
                    word.Top = curY;

                    curX = word.Left + word.FullWidth;
                    maxRight = Math.Max(maxRight, curX);
                    maxBottom = Math.Max(maxBottom, word.Bottom);
                }

                if (box.Height == CssConstants.Auto)
                {
                    //use the maximum bottom hit during word layout
                    box.ActualBottom = maxBottom;
                }
                else
                {
                    //use the fixed height
                    box.ActualBottom = initialY + box.ActualHeight;
                }
                
                if (box.Width == CssConstants.Auto)
                {
                    //use the maximum right hit during word layout
                    box.ActualRight = maxRight;
                }
                else
                {
                    //use the fixed width
                    box.ActualRight = initialX + box.ActualWidth;
                }

            }

            return new WordLayoutStatus()
            {
                CurrentLineBox = currentLineBox,
                CurX = curX,
                CurY = curY,
                CurrentMaxBottom = box.ActualBottom
            };
        }
    }

    internal class WordLayoutStatus
    {
        internal double CurX;
        internal double CurY;

        internal CssLineBox CurrentLineBox;
        internal double CurrentMaxBottom;
    }
}
