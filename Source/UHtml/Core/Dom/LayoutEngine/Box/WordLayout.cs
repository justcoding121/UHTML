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
        private static void LayoutWords(RGraphics g,
            CssBox closestBlockAncestor, ref CssLineBox currentLineBox, CssBox box,
            double startX, double startY,
            ref double curX, ref double curY,
            double rightLimit, ref double currentMaxBottom)
        {

            double localMaxLineBottom = currentMaxBottom;

            if (box.Words.Count > 0)
            {
                box.FirstHostingLineBox = currentLineBox;

                if (DomUtils.DoesBoxHasWhitespace(box))
                    curX += box.ActualWordSpacing;

                foreach (var word in box.Words)
                {

#if DEBUG
                    if (word.Text == "100%")
                    {

                    }
#endif
                    if ((box.WhiteSpace != CssConstants.NoWrap
                        && box.WhiteSpace != CssConstants.Pre
                        && curX + word.Width > rightLimit
                         && (box.WhiteSpace != CssConstants.PreWrap || !word.IsSpaces))
                        || word.IsLineBreak)
                    {

                        curX = startX;
                        curY = localMaxLineBottom;

                        currentLineBox = new CssLineBox(closestBlockAncestor);

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

                box.LastHostingLineBox = currentLineBox;


            }

        }
    }
}
