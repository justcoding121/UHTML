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
    internal class StaticNoneBlockStatus
    {
        public double CurX { get; set; }
        public double CurY { get; set; }

        public CssLineBox CurrentLineBox { get; set; }
        public double CurrentMaxBottom { get; internal set; }

        public double CurrentMaxRight { get; set; }
    }

    internal static partial class CssLayoutEngine
    {
        public static StaticNoneBlockStatus LayoutStaticNoneBlock(RGraphics g,
          CssBox currentBox,
          double curX, double curY,
          CssLineBox currentLine,
          double leftLimit, double rightLimit,
          double currentBottom)
        {
            //get previous sibling to adjust margin overlapping on top
            var prevSibling = DomUtils.GetPreviousSibling(currentBox);

            double left;
            double top;


            left = curX + currentBox.ActualMarginLeft;

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

            var layoutCoreStatus = new LayoutCoreStatus()
            {
                CurrentLineBox = currentLine,
                CurX = currentBox.Location.X,
                CurY = currentBox.Location.Y,
                CurrentMaxRight = currentBox.Width != CssConstants.Auto &&
                                  !string.IsNullOrEmpty(currentBox.Width) ?
                                currentBox.Location.X 
                                + currentBox.ActualBorderLeftWidth
                                + currentBox.ActualPaddingLeft
                                + width
                                + currentBox.ActualBorderRightWidth
                                + currentBox.ActualPaddingRight
                                : rightLimit,
                CurrentMaxBottom = currentBottom
            };

            //child boxes here
            foreach (var box in currentBox.Boxes)
            {
                var result = LayoutRecursively(g, box, layoutCoreStatus.CurX, layoutCoreStatus.CurY,
                     layoutCoreStatus.CurrentLineBox, currentBox.Location.X, layoutCoreStatus.CurrentMaxRight, layoutCoreStatus.CurrentMaxBottom);

                if(result!=null)
                {
                    layoutCoreStatus = result;
                }
            }


            SetBlockBoxSize(currentBox, leftLimit, rightLimit, layoutCoreStatus.CurrentMaxBottom - top);

            return new StaticNoneBlockStatus()
            {
                CurrentLineBox = layoutCoreStatus.CurrentLineBox,
                CurX = layoutCoreStatus.CurX,
                CurY = layoutCoreStatus.CurY,
                CurrentMaxBottom = layoutCoreStatus.CurrentMaxBottom, 
                CurrentMaxRight = layoutCoreStatus.CurrentMaxRight
            };

        }


        /// <summary>
        /// Set Width & Height for Box
        /// </summary>
        /// <param name="box"></param>
        private static void SetBlockBoxSize(CssBox box,
            double leftLimit,
            double rightLimit,
            double currentBottom)
        {

            double height = CssValueParser.ParseLength(box.Height, box.ContainingBlock.Size.Height, box);
            double width = CssValueParser.ParseLength(box.Width, box.ContainingBlock.Size.Width, box);

            box.Size = new RSize(box.Width != CssConstants.Auto && !string.IsNullOrEmpty(box.Width) ? width
                                + box.ActualBorderLeftWidth
                                + box.ActualPaddingLeft
                                + box.ActualBorderRightWidth
                                + box.ActualPaddingRight
                                : (rightLimit - leftLimit)
                                - box.ActualMarginLeft
                                - box.ActualMarginRight
                                ,
                                box.Height != CssConstants.Auto && !string.IsNullOrEmpty(box.Height) ? height
                                + box.ActualBorderTopWidth
                                + box.ActualPaddingTop
                                + box.ActualBorderBottomWidth
                                + box.ActualPaddingBottom
                                : currentBottom
                                + box.ActualPaddingBottom
                                + box.ActualBorderBottomWidth
                                + box.ActualMarginBottom);

        }
    }
}
