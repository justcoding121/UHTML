using System;
using System.Linq;
using System.Collections.Generic;
using UHtml.Adapters;
using UHtml.Adapters.Entities;
using UHtml.Core.Parse;
using UHtml.Core.Utils;

namespace UHtml.Core.Dom
{ 
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
            curY = layoutCoreStatus.CurY;

            foreach (var inlineBox in currentBox.Boxes)
            {
                if (inlineBox.Width != CssConstants.Auto && !string.IsNullOrEmpty(inlineBox.Width) 
                    && layoutCoreStatus.CurX + inlineBox.ActualWidth > rightLimit)
                {
                    curY = alignLine(g, currentLine);
                    layoutCoreStatus.CurX = leftLimit;
  
                    layoutCoreStatus.Bottom = curY;
                    layoutCoreStatus.Right = leftLimit;

                    currentLine = new CssLineBox(currentBox);
                    layoutCoreStatus.CurrentLine = currentLine;
                }
                else
                {
                    currentLine = layoutCoreStatus.CurrentLine;
                }


                layoutCoreStatus = LayoutRecursively(g, inlineBox, layoutCoreStatus.CurX, curY,
                      layoutCoreStatus.CurrentLine, leftLimit, rightLimit, layoutCoreStatus.Bottom);

                if (inlineBox.Display == "inline" || currentLine != layoutCoreStatus.CurrentLine)
                {
                    curY = layoutCoreStatus.CurY;
                }

                maxBottom = Math.Max(maxBottom, layoutCoreStatus.Bottom);
            }

            if(layoutCoreStatus.CurrentLine!=null)
            {
                layoutCoreStatus.Bottom = alignLine(g, layoutCoreStatus.CurrentLine);
                layoutCoreStatus.CurrentLine = null;
            }

            SetBlockBoxSize(currentBox, leftLimit, rightLimit, top, layoutCoreStatus.Bottom);


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

            currentBox.Location = new RPoint(curX + currentBox.ActualMarginLeft, curY + currentBox.ActualMarginTop);

            var boxLeftLimit = currentBox.Location.X + currentBox.ActualPaddingLeft + currentBox.ActualBorderLeftWidth;
            var boxRightLimit = currentBox.Width != CssConstants.Auto && !string.IsNullOrEmpty(currentBox.Width) ?
                boxLeftLimit + CssValueParser.ParseLength(currentBox.Width, currentBox.ContainingBlock.Size.Width, currentBox)
                : curX + rightLimit - leftLimit - currentBox.ActualPaddingRight - currentBox.ActualBorderRightWidth - currentBox.ActualMarginRight;

            var top = currentBox.Location.Y;

            var layoutCoreStatus = new LayoutProgress()
            {
                CurX = boxLeftLimit,
                CurY = top + currentBox.ActualBorderTopWidth + currentBox.ActualPaddingTop,
                Bottom = maxBottom,
                Right = boxLeftLimit,
            };

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
                    layoutCoreStatus.Right = result.Right;
                    layoutCoreStatus.Bottom = result.Bottom;
                    layoutCoreStatus.CurrentLine = result.CurrentLine;
                }

            }

            if (layoutCoreStatus.CurrentLine != null)
            {
                maxBottom = Math.Max(maxBottom, alignLine(g, layoutCoreStatus.CurrentLine));
            }


            if (layoutCoreStatus.Right + currentBox.ActualPaddingRight
                   + currentBox.ActualBorderRightWidth
                   + currentBox.ActualMarginRight > rightLimit)
            {

                maxBottom = alignLine(g, currentLine);
                currentLine = new CssLineBox(currentBox.ContainingBlock);

                var xDiff = currentBox.Location.X - currentBox.ActualMarginLeft - leftLimit;
                var yDiff = currentBox.Location.Y - currentBox.ActualMarginBottom - maxBottom;

                moveBox(currentBox, xDiff, yDiff);

                layoutCoreStatus.CurX -= xDiff;
                layoutCoreStatus.CurY -= yDiff;

                layoutCoreStatus.Right -= xDiff;
                layoutCoreStatus.Bottom -= yDiff;

                boxLeftLimit -= xDiff;
                top -= yDiff;
            }
         
            currentLine.ReportExistanceOfBox(currentBox);

            setInlineBlockBoxSize(currentBox, boxLeftLimit, layoutCoreStatus.Right, top, layoutCoreStatus.Bottom);

            layoutCoreStatus.Right +=
                   currentBox.ActualPaddingRight
                   + currentBox.ActualBorderRightWidth
                   + currentBox.ActualMarginRight;


            return new StaticNoneInlineBlockLayoutProgress()
            {
                CurX = layoutCoreStatus.Right,
                CurY = layoutCoreStatus.CurY,
                Right = layoutCoreStatus.Right,
                Bottom = Math.Max(currentBox.ActualBottom, currentBox.ContentBottom) + currentBox.ActualMarginBottom,
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
        private static void setInlineBlockBoxSize(CssBox box,
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

            box.ContentBottom = currentBottom;

        }

        private static double alignLine(RGraphics g, CssLineBox linebox)
        {
            var currentBox = linebox.OwnerBox;

            ApplyHorizontalAlignment(g, linebox);
            ApplyRightToLeft(currentBox, linebox);
            BubbleRectangles(currentBox, linebox);
            return ApplyVerticalAlignment(g, linebox);
            //linebox.AssignRectanglesToBoxes();
        }
    }

    internal class StaticNoneInlineBlockLayoutProgress
    {
        public double CurX { get; set; }
        public double CurY { get; set; }

        public double Right { get; set; }
        public double Bottom { get; internal set; }

        public CssLineBox CurrentLineBox { get; set; }
    }
}

