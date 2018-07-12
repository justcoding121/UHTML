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
    internal class StaticNoneBlockLayoutProgress
    {
        public double CurX { get; set; }
        public double CurY { get; set; }
        
        public double CurrentRight { get; set; }
        public double CurrentBottom { get; internal set; }

    }

    internal static partial class CssLayoutEngine
    {
        public static StaticNoneBlockLayoutProgress LayoutStaticNoneBlock(RGraphics g,
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

            //child boxes here
            foreach (var box in currentBox.Boxes)
            {
                var result = LayoutRecursively(g, box, layoutCoreStatus.CurX, layoutCoreStatus.CurY,
                     layoutCoreStatus.CurrentLine, currentMaxLeft , currentMaxRight, layoutCoreStatus.CurrentBottom);

                if(result!=null)
                {
                    layoutCoreStatus.CurrentBottom = result.CurrentBottom;
                    layoutCoreStatus.CurX = result.CurX;
                    layoutCoreStatus.CurY = result.CurY;
                }
            }


            SetBlockBoxSize(currentBox, leftLimit, rightLimit, top, layoutCoreStatus.CurrentBottom);

            if (!currentBox.IsFixed)
            {
                var actualWidth = Math.Max(currentBox.GetMinimumWidth() 
                        + currentBox.GetWidthMarginDeep(currentBox), 
                        currentBox.Size.Width < 90999 ? 
                        currentBox.ActualRight - currentBox.HtmlContainer.Root.Location.X 
                        : 0);

                currentBox.HtmlContainer.ActualSize = 
                    CommonUtils.Max(currentBox.HtmlContainer.ActualSize,
                    new RSize(actualWidth, currentBox.ActualBottom 
                    - currentBox.HtmlContainer.Root.Location.Y));
            }

            return new StaticNoneBlockLayoutProgress()
            {
                CurX = layoutCoreStatus.CurX,
                CurY = layoutCoreStatus.CurY,
                CurrentBottom = layoutCoreStatus.CurrentBottom + currentBox.ActualMarginBottom
            };

        }


        /// <summary>
        /// Set Width & Height for Box
        /// </summary>
        /// <param name="box"></param>
        private static void SetBlockBoxSize(CssBox box,
            double leftLimit,
            double rightLimit,
            double top, double bottom)
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
                                : bottom - top
                                + box.ActualPaddingBottom
                                + box.ActualBorderBottomWidth);

        }
    }
}
