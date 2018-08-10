using System;
using System.Collections.Generic;
using UHtml.Adapters;
using UHtml.Adapters.Entities;
using UHtml.Core.Parse;
using UHtml.Core.Utils;

namespace UHtml.Core.Dom
{
    internal class StaticNoneInlineBlockLayoutProgress
    {
        public double CurX { get; set; }
        public double CurY { get; set; }

        public double Right { get; set; }
        public double Bottom { get; internal set; }

        public CssLineBox CurrentLineBox { get; set; }
    }

    internal static partial class CssLayoutEngine
    {
        public static LayoutProgress LayoutInlineBlockBoxes(RGraphics g,
            CssBox currentBox,
            double curX, double curY,
            double leftLimit, double rightLimit, double currentBottom)
        {
            setBlockBoxLocation(currentBox, leftLimit);

            var top = currentBox.Location.Y;

            var currentLine = new CssLineBox(currentBox);

            var layoutCoreStatus = new LayoutProgress()
            {
                CurrentLine = currentLine,
                CurX = currentBox.Location.X
                            + currentBox.ActualBorderLeftWidth
                            + currentBox.ActualPaddingLeft,
                CurY = currentBox.Location.Y
                            + currentBox.ActualBorderTopWidth
                            + currentBox.ActualPaddingTop,
                Bottom = currentBottom
            };


            var maxBottom = 0.0;
            //curY = layoutCoreStatus.CurY;

            foreach (var inlineBox in currentBox.Boxes)
            {
                if (inlineBox.Width != CssConstants.Auto &&
                               !string.IsNullOrEmpty(inlineBox.Width))
                {
                    if (layoutCoreStatus.CurX + inlineBox.ActualWidth > rightLimit)
                    {
                        currentLine = new CssLineBox(currentBox);
                        layoutCoreStatus.CurX = leftLimit;
                        //curY = maxBottom;
                    }
                }

                currentLine = layoutCoreStatus.CurrentLine;

                layoutCoreStatus = LayoutRecursively(g, inlineBox, layoutCoreStatus.CurX, layoutCoreStatus.CurY,
                      layoutCoreStatus.CurrentLine, leftLimit, rightLimit, layoutCoreStatus.Bottom);

                //if (inlineBox.Display == "inline" || currentLine != layoutCoreStatus.CurrentLine)
                //{
                //    curY = layoutCoreStatus.CurY;
                //}

                maxBottom = Math.Max(maxBottom, layoutCoreStatus.Bottom);
            }

            SetBlockBoxSize(currentBox, leftLimit, rightLimit, top, layoutCoreStatus.Bottom);

            //Gets the rectangles for each line-box
            foreach (var linebox in currentBox.LineBoxes)
            {
                ApplyHorizontalAlignment(g, linebox);
                ApplyRightToLeft(currentBox, linebox);
                BubbleRectangles(currentBox, linebox);
                ApplyVerticalAlignment(g, linebox);
                linebox.AssignRectanglesToBoxes();
            }

            return layoutCoreStatus;
        }

        public static StaticNoneInlineBlockLayoutProgress LayoutStaticNoneInlineBlock(RGraphics g,
          CssBox currentBox,
          double curX, double curY,
          CssLineBox currentLine,
          double leftLimit, double rightLimit,
          double maxBottom)
        {
            if (currentBox.Display != CssConstants.None)
            {
                currentBox.RectanglesReset();
                currentBox.MeasureWordsSize(g);
            }

            currentBox.Location = new RPoint(curX + currentBox.ActualMarginLeft, curY);

            var boxLeftLimit = currentBox.Location.X + currentBox.ActualPaddingLeft + currentBox.ActualBorderLeftWidth;
            var boxRightLimit = currentBox.Width != CssConstants.Auto && !string.IsNullOrEmpty(currentBox.Width) ?
                boxLeftLimit + CssValueParser.ParseLength(currentBox.Width, currentBox.ContainingBlock.Size.Width, currentBox)
                : curX + rightLimit - leftLimit - currentBox.ActualPaddingRight - currentBox.ActualBorderRightWidth - currentBox.ActualMarginRight;

            var top = currentBox.Location.Y + currentBox.ActualPaddingTop + currentBox.ActualBorderTopWidth;

            var layoutCoreStatus = new LayoutProgress()
            {
                CurX = boxLeftLimit,
                CurY = top,
                Bottom = maxBottom,
                Right = boxLeftLimit,
            };

            var maxRight = boxLeftLimit;

            foreach (var box in currentBox.Boxes)
            {
                var result = LayoutRecursively(g, box,
                    layoutCoreStatus.CurX, layoutCoreStatus.CurY,
                    layoutCoreStatus.CurrentLine,
                    boxLeftLimit,
                    boxRightLimit,
                    layoutCoreStatus.CurY);

                if (result != null)
                {
                    layoutCoreStatus.CurX = result.CurX;
                    layoutCoreStatus.CurY = result.CurY;
                    layoutCoreStatus.Bottom = result.Bottom;
                    layoutCoreStatus.CurrentLine = result.CurrentLine;
                }

                maxRight = Math.Max(maxRight, result.Right);
            }

            //SetInlineBlockBoxSize(currentBox,
            //                     boxLeftLimit, maxRight,
            //                     top, layoutCoreStatus.Bottom);


            if (layoutCoreStatus.Right + currentBox.ActualPaddingRight
                   + currentBox.ActualBorderRightWidth
                   + currentBox.ActualMarginRight > rightLimit)
            {
                currentLine = new CssLineBox(currentBox);

                layoutCoreStatus.CurX = leftLimit
                    + currentBox.ActualMarginLeft
                    + currentBox.ActualBorderLeftWidth
                    + currentBox.ActualPaddingLeft;

                layoutCoreStatus.CurY = maxBottom;

                var xDiff = currentBox.Location.X - layoutCoreStatus.CurX;
                var yDiff = currentBox.Location.Y - layoutCoreStatus.CurY;

                moveBox(currentBox, xDiff, yDiff);
            }
            else
            {
                currentLine.ReportExistanceOfBox(currentBox);
            }


            //foreach (var linebox in currentBox.LineBoxes)
            //{
            //    ApplyHorizontalAlignment(g, linebox);
            //    ApplyRightToLeft(currentBox, linebox);
            //    BubbleRectangles(currentBox, linebox);
            //    ApplyVerticalAlignment(g, linebox);
            //    linebox.AssignRectanglesToBoxes();
            //}

            maxRight +=
                   currentBox.ActualPaddingRight
                   + currentBox.ActualBorderRightWidth
                   + currentBox.ActualMarginRight;

            var height = currentBox.Height != CssConstants.Auto && !string.IsNullOrEmpty(currentBox.Height) ?
                                CssValueParser.ParseLength(currentBox.Height, currentBox.ContainingBlock.Size.Height, currentBox)
                                : layoutCoreStatus.Bottom - top;

            return new StaticNoneInlineBlockLayoutProgress()
            {
                CurX = maxRight,
                CurY = layoutCoreStatus.CurY,
                Right = maxRight,
                Bottom = top + height
                        + currentBox.ActualPaddingBottom
                        + currentBox.ActualBorderBottomWidth
                        + currentBox.ActualMarginBottom,
                CurrentLineBox = currentLine
            };
        }

        private static void moveBox(CssBox currentBox, double xDiff, double yDiff)
        {
            currentBox.Location = new RPoint(currentBox.Location.X - xDiff, currentBox.Location.Y - yDiff);

            if (currentBox.Words.Count > 0)
            {
                foreach (var word in currentBox.Words)
                {
                    word.Left = word.Left - xDiff;
                    word.Top = word.Top - yDiff;
                }
            }

            foreach (var box in currentBox.Boxes)
            {
                moveBox(box, xDiff, yDiff);
            }
        }

        /// <summary>
        /// Set Width & Height for Box
        /// </summary>
        /// <param name="box"></param>
        private static void SetInlineBlockBoxSize(CssBox box,
          double leftEnd, double rightEnd,
          double currentTop, double currentBottom)
        {

            double height = CssValueParser.ParseLength(box.Height, box.ContainingBlock.Size.Height, box);

            box.Size = new RSize(rightEnd - leftEnd
                                + box.ActualBorderLeftWidth
                                + box.ActualPaddingLeft
                                + box.ActualBorderRightWidth
                                + box.ActualPaddingRight
                                ,
                                box.Height != CssConstants.Auto && !string.IsNullOrEmpty(box.Height) ? height
                                + box.ActualBorderTopWidth
                                + box.ActualPaddingTop
                                + box.ActualBorderBottomWidth
                                + box.ActualPaddingBottom
                                : currentBottom - currentTop
                                + box.ActualBorderTopWidth
                                + box.ActualPaddingTop
                                + box.ActualPaddingBottom
                                + box.ActualBorderBottomWidth);

        }
    }
}

