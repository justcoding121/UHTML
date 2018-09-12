using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHtml.Adapters;
using UHtml.Adapters.Entities;
using UHtml.Core.Parse;
using UHtml.Core.Utils;

namespace UHtml.Core.Dom
{
    internal static partial class CssLayoutEngine
    {
        private static void setBlockBoxLocation(CssBox currentBox, double leftLimit)
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
        }

        public static StaticNoneBlockLayoutProgress LayoutStaticNoneBlock(RGraphics g,
          CssBox currentBox,
          double curX, double curY,
          double leftLimit, double rightLimit,
          double currentBottom)
        {

            setBlockBoxLocation(currentBox, leftLimit);

            var top = currentBox.Location.Y;

            double width = CssValueParser.ParseLength(currentBox.Width, rightLimit - leftLimit, currentBox);

            var layoutCoreStatus = new LayoutProgress()
            {
                CurX = currentBox.Location.X
                            + currentBox.ActualBorderLeftWidth
                            + currentBox.ActualPaddingLeft,
                CurY = currentBox.Location.Y
                            + currentBox.ActualBorderTopWidth
                            + currentBox.ActualPaddingTop,
                Bottom = currentBottom
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

            var maxRight = 0.0;
            //child boxes here
            foreach (var box in currentBox.Boxes)
            {
                var result = LayoutRecursively(g, box, layoutCoreStatus.CurX, layoutCoreStatus.CurY,
                     layoutCoreStatus.CurrentLine, currentMaxLeft, currentMaxRight, layoutCoreStatus.Bottom);

                if(result!=null)
                {
                    layoutCoreStatus = result;
                    maxRight = Math.Max(maxRight, result.MaxRight);
                }
            }

            if (layoutCoreStatus.CurrentLine != null && !layoutCoreStatus.CurrentLine.IsEmpty())
            {
                layoutCoreStatus.Bottom = alignLine(g, layoutCoreStatus.CurrentLine);
                layoutCoreStatus.CurrentLine = null;
            }

            SetBlockBoxSize(currentBox, leftLimit, rightLimit, top, layoutCoreStatus.Bottom);

            var right = currentBox.ActualRight + currentBox.ActualMarginRight;
            maxRight = Math.Max(maxRight, right);

            if (currentBox.ParentBox == null)
            {
                var actualWidth = maxRight - currentBox.HtmlContainer.Root.Location.X;

                currentBox.HtmlContainer.ActualSize =
                    new RSize(actualWidth, 
                    layoutCoreStatus.Bottom 
                    + currentBox.ActualMarginBottom 
                    + currentBox.ActualPaddingBottom
                    + currentBox.ActualBorderBottomWidth
                    - currentBox.HtmlContainer.Root.Location.Y);
            }


            return new StaticNoneBlockLayoutProgress()
            {
                CurX = layoutCoreStatus.CurX,
                CurY = layoutCoreStatus.CurY,
                Right = right,
                MaxRight = maxRight,
                Bottom = currentBox.ActualBottom + currentBox.ActualMarginBottom
            };

        }


        /// <summary>
        /// Set Width & Height for Box
        /// </summary>
        /// <param name="box"></param>
        private static void SetBlockBoxSize(CssBox box,
            double left,
            double right,
            double top, double bottom)
        {

            double height = CssValueParser.ParseLength(box.Height, box.ContainingBlock.Size.Height, box);
            double width = CssValueParser.ParseLength(box.Width, box.ContainingBlock.Size.Width, box);

            box.Size = new RSize(box.Width != CssConstants.Auto && !string.IsNullOrEmpty(box.Width) ? width
                                + box.ActualBorderLeftWidth
                                + box.ActualPaddingLeft
                                + box.ActualBorderRightWidth
                                + box.ActualPaddingRight
                                : (right - left)
                                - box.ActualMarginLeft
                                - box.ActualMarginRight
                                ,
                                box.Height != CssConstants.Auto && !string.IsNullOrEmpty(box.Height) ? height
                                + box.ActualBorderTopWidth
                                + box.ActualPaddingTop
                                + box.ActualBorderBottomWidth
                                + box.ActualPaddingBottom
                                : bottom - top
                                + box.ActualPaddingBottom
                                + box.ActualBorderBottomWidth);

        }
    }

    internal class StaticNoneBlockLayoutProgress
    {
        public double CurX { get; set; }
        public double CurY { get; set; }

        public double Right { get; set; }
        public double MaxRight { get; set; }

        public double Bottom { get; internal set; }

    }
}
