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

        public CssLineBox CurrentLineBox { get; set; }
        public double CurrentMaxBottom { get; internal set; }
    }

    internal static partial class CssLayoutEngine
    {
        public static LayoutProgress LayoutInlineBlockBoxes(RGraphics g,
            CssBox currentBox,
            double curX, double curY,
            double leftLimit, double rightLimit, double currentBottom)
        {
            //get previous sibling to adjust margin overlapping on top
            var prevSibling = DomUtils.GetPreviousSibling(currentBox);

            double left;
            double top;


            left = leftLimit + currentBox.ActualMarginLeft;

            if (prevSibling == null && currentBox.ParentBox != null)
            {
                top = currentBox.ParentBox.ClientTop + currentBox.MarginTopCollapse(prevSibling);
            }
            else
            {
                if (prevSibling != null)
                {
                    top = prevSibling.ActualBottom + currentBox.MarginTopCollapse(prevSibling);
                }
                else
                {
                    top = currentBox.MarginTopCollapse(prevSibling);
                }

            }

            currentBox.Location = new RPoint(left, top);

            double width = CssValueParser.ParseLength(currentBox.Width, currentBox.ContainingBlock.Size.Width, currentBox);

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
                CurrentBottom = currentBottom
            };

            var currentMaxLeft = currentBox.Location.X
                           + currentBox.ActualBorderLeftWidth
                           + currentBox.ActualPaddingLeft;

            var currentMaxRight = currentBox.Width != CssConstants.Auto &&
                               !string.IsNullOrEmpty(currentBox.Width) ?
                             currentBox.Location.X
                             + currentBox.ActualBorderLeftWidth
                             + currentBox.ActualPaddingLeft
                             + width
                             : rightLimit
                             - currentBox.ActualMarginRight
                             - currentBox.ActualPaddingRight
                             - currentBox.ActualBorderRightWidth;

            var maxBottom = 0.0;
            curY = layoutCoreStatus.CurY;

            foreach (var inlineBox in currentBox.Boxes)
            {
                if (inlineBox.Width != CssConstants.Auto &&
                               !string.IsNullOrEmpty(inlineBox.Width))
                {
                    if (layoutCoreStatus.CurX + inlineBox.ActualWidth > rightLimit)
                    {
                        currentLine = new CssLineBox(currentBox);
                        layoutCoreStatus.CurX = leftLimit;
                        curY = maxBottom;
                    }
                }

                layoutCoreStatus = LayoutRecursively(g, inlineBox, layoutCoreStatus.CurX, curY,
                      layoutCoreStatus.CurrentLine, leftLimit, rightLimit, layoutCoreStatus.CurrentBottom);

                if(inlineBox.Display=="inline")
                {
                    curY = layoutCoreStatus.CurY;
                }

                maxBottom = Math.Max(maxBottom, layoutCoreStatus.CurrentBottom);
            }

            SetBlockBoxSize(currentBox, leftLimit, rightLimit, top, layoutCoreStatus.CurrentBottom);

            return layoutCoreStatus;
        }

        public static StaticNoneInlineBlockLayoutProgress LayoutStaticNoneInlineBlock(RGraphics g,
          CssBox currentBox,
          double curX, double curY,
          CssLineBox currentLine,
          double leftLimit, double rightLimit,
          double currentBottom)
        {
            currentBox.Location = new RPoint(curX + currentBox.ActualMarginLeft, curY);

            var boxLeftLimit = currentBox.Location.X + currentBox.ActualPaddingLeft + currentBox.ActualBorderLeftWidth;
            var boxRightLimit = currentBox.Width != CssConstants.Auto && !string.IsNullOrEmpty(currentBox.Width) ?
                boxLeftLimit + CssValueParser.ParseLength(currentBox.Width, currentBox.ContainingBlock.Size.Width, currentBox)
                :  rightLimit - currentBox.ActualPaddingRight - currentBox.ActualBorderRightWidth - currentBox.ActualMarginRight;

            var top = currentBox.Location.Y + currentBox.ActualPaddingTop + currentBox.ActualBorderTopWidth;

            var layoutCoreStatus = new LayoutProgress()
            {
                CurX = boxLeftLimit,
                CurY = top,
                CurrentBottom = currentBottom
            };

            var maxRight = 0.0;

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
                    layoutCoreStatus.CurrentBottom = result.CurrentBottom;
                    layoutCoreStatus.CurrentLine = result.CurrentLine;
                }

                maxRight = Math.Max(maxRight, box.ActualRight);
            }

            SetInlineBlockBoxSize(currentBox,
                                   boxLeftLimit, maxRight,
                                   top, layoutCoreStatus.CurrentBottom);

            currentLine.ReportExistanceOfBox(currentBox);

            return new StaticNoneInlineBlockLayoutProgress()
            {
                CurX = currentBox.ActualRight + currentBox.ActualMarginRight,
                CurY = layoutCoreStatus.CurY,
                CurrentMaxBottom = currentBox.ActualBottom + currentBox.ActualMarginBottom,
                CurrentLineBox = currentLine
            };
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

