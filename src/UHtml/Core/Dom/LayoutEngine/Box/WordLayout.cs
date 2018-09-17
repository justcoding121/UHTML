using System;
using UHtml.Adapters;
using UHtml.Core.Utils;
using System.Linq;

namespace UHtml.Core.Dom
{

    internal static partial class CssLayoutEngine
    {

        /// <summary>
        /// Should update the actual bottom of the box is height is auto
        /// </summary>
        private static WordLayoutStatus LayoutWords(RGraphics g,
            CssBox box,
            CssLineBox currentLineBox,
            double initialX, double initialY,
            double leftLimit, double rightLimit, double initialBottom)
        {

            double curX = initialX;
            double curY = initialY;

            double right = initialX;
            double maxRight = initialX;

            double maxBottom = initialBottom;

            if (box.Words.Count > 0)
            {
                currentLineBox = currentLineBox ?? new CssLineBox(box.ContainingBlock);

                if (DomUtils.DoesBoxHasWhitespace(box))
                {
                    curX += box.ActualWordSpacing;
                    right = curX;
                }

                bool firstNewLine = true;
                for (int i = 0; i < box.Words.Count; i++)
                {
                    var word = box.Words[i];

                    //create new line if needed
                    if ((box.WhiteSpace != CssConstants.NoWrap
                        && box.WhiteSpace != CssConstants.Pre
                        && curX + word.Width > rightLimit
                         && (box.WhiteSpace != CssConstants.PreWrap || !word.IsSpaces))
                         || word.IsLineBreak)
                    {
                        right = rightLimit;
                        curX = leftLimit;

                        if (!currentLineBox.IsEmpty())
                        {
                            curY = alignLine(g, currentLineBox);
                            currentLineBox = new CssLineBox(box.ContainingBlock);
                        }

                        if (firstNewLine)
                        {
                            var longestWordLength = box.Words.Skip(i)
                                                    .Take(box.Words.Count - i)
                                                    .Select(x => x.Width)
                                                    .Max();

                            rightLimit = Math.Max(rightLimit, leftLimit + longestWordLength);
                            firstNewLine = false;
                        }

                    }

                    word.Left = curX;
                    word.Top = curY;

                    curX = word.Left + word.FullWidth;

                    right = Math.Max(right, word.Right);
                    maxRight = Math.Max(maxRight, word.Right);
                    maxBottom = Math.Max(maxBottom, word.Bottom);

                    currentLineBox.ReportExistanceOf(box, word);
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

                maxBottom = box.ActualBottom;
                box.ActualRight = right;

            }

            return new WordLayoutStatus()
            {
                CurrentLineBox = currentLineBox,
                CurX = curX,
                CurY = curY,
                Right = box.ActualRight,
                MaxRight = maxRight,
                Bottom = maxBottom
            };
        }
    }

    internal class WordLayoutStatus
    {
        internal double CurX;
        internal double CurY;

        internal double Right;
        internal double MaxRight;

        internal double Bottom;

        internal CssLineBox CurrentLineBox;
    }
}
