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
           

            foreach (var inlineBox in currentBox.Boxes)
            {
                if (inlineBox.Width != CssConstants.Auto &&
                               !string.IsNullOrEmpty(inlineBox.Width))
                {
                    if (layoutCoreStatus.CurX + inlineBox.ActualWidth > rightLimit)
                    {
                        currentLine = new CssLineBox(currentBox);
                        layoutCoreStatus.CurX = leftLimit;
                    }
                }


                layoutCoreStatus = LayoutRecursively(g, inlineBox, layoutCoreStatus.CurX, layoutCoreStatus.CurY,
                      layoutCoreStatus.CurrentLine, leftLimit, rightLimit, layoutCoreStatus.CurrentBottom);

                currentLine.ReportExistanceOfBox(inlineBox);
            }

            SetBlockBoxSize(currentBox, leftLimit, rightLimit, top, layoutCoreStatus.CurrentBottom);

            return layoutCoreStatus;
        }

        public static StaticNoneInlineBlockLayoutProgress LayoutStaticNoneInlineBlock(RGraphics g,
          CssBox currentBox,
          double curX, double curY,
          double leftLimit, double rightLimit,
          double currentBottom)
        {   
           currentBox.Location = new RPoint(curX + currentBox.ActualMarginLeft, curY);
                
            var layoutCoreStatus = new LayoutProgress()
            {
                CurX = currentBox.Location.X + currentBox.ActualPaddingLeft + currentBox.ActualBorderLeftWidth,
                CurY = currentBox.Location.Y + currentBox.ActualPaddingTop + currentBox.ActualBorderTopWidth,
                CurrentBottom = currentBottom
            };

            var maxRight = 0.0;

            foreach (var box in currentBox.Boxes)
            {

                var result = LayoutRecursively(g, box, 
                    layoutCoreStatus.CurX, layoutCoreStatus.CurY,
                    layoutCoreStatus.CurrentLine,
                    layoutCoreStatus.CurX,
                    rightLimit - currentBox.ActualPaddingRight - currentBox.ActualBorderRightWidth, 
                    layoutCoreStatus.CurrentBottom);

                if (result != null)
                {
                    layoutCoreStatus.CurX = result.CurX;
                    layoutCoreStatus.CurY = result.CurY;
                    layoutCoreStatus.CurrentBottom = result.CurrentBottom;
                    layoutCoreStatus.CurrentLine = result.CurrentLine;
                }

                maxRight = Math.Max(maxRight, box.ActualRight);
            }

            maxRight += currentBox.ActualPaddingRight + currentBox.ActualBorderRightWidth;

            SetInlineBlockBoxSize(currentBox, curX, curY,
                                   curX, maxRight,
                                   layoutCoreStatus.CurrentBottom);

            return new StaticNoneInlineBlockLayoutProgress()
            {
                CurX = currentBox.ActualRight,
                CurY = layoutCoreStatus.CurY,
                CurrentMaxBottom = currentBox.ActualBottom
            };
        }

        /// <summary>
        /// Set Width & Height for Box
        /// </summary>
        /// <param name="box"></param>
        private static void SetInlineBlockBoxSize(CssBox box,
          double startX, double startY,
          double leftEnd, double rightEnd,
          double currentBottom)
        {

            double height = CssValueParser.ParseLength(box.Height, box.ContainingBlock.Size.Height, box);
            double width = CssValueParser.ParseLength(box.Width, box.ContainingBlock.Size.Width, box);


            box.Size = new RSize(box.Width != CssConstants.Auto && !string.IsNullOrEmpty(box.Width) ? width
                                + box.ActualBorderLeftWidth
                                + box.ActualPaddingLeft
                                + box.ActualBorderRightWidth
                                + box.ActualPaddingRight
                                : rightEnd - leftEnd
                                ,
                                box.Height != CssConstants.Auto && !string.IsNullOrEmpty(box.Height) ? height
                                + box.ActualBorderTopWidth
                                + box.ActualPaddingTop
                                + box.ActualBorderBottomWidth
                                + box.ActualPaddingBottom
                                : currentBottom - startY
                                + box.ActualBorderTopWidth
                                + box.ActualPaddingTop
                                + box.ActualPaddingBottom
                                + box.ActualBorderBottomWidth);

        }
    }
}

