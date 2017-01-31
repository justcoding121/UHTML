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
        public static void LayoutStaticNoneBlock(RGraphics g,
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

            //child boxes here
            foreach (var box in currentBox.Boxes)
            {
                LayoutRecursively(g, box, currentBox.Location.X, currentBox.Location.Y,
                     null, currentBox.Location.X, currentBox.ActualRight, currentBox.Location.Y);
            }
          

            SetBlockBoxSize(currentBox, leftLimit, rightLimit, currentBottom - top);

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
