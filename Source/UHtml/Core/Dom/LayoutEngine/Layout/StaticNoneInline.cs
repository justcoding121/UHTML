using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHtml.Adapters;
using UHtml.Adapters.Entities;
using UHtml.Core.Utils;

namespace UHtml.Core.Dom
{
    internal class StaticNoneInlineStatus
    {
        public double CurX { get; set; }
        public double CurY { get; set; }

        public CssLineBox CurrentLineBox { get; set; }
        public double CurrentMaxBottom { get; internal set; }
    }

    internal static partial class CssLayoutEngine
    {
        public static StaticNoneInlineStatus LayoutStaticNoneInline(RGraphics g,
          CssBox currentBox,
          double curX, double curY,
          CssLineBox currentLine,
          double leftLimit, double rightLimit,
          double currentBottom)
        {
            ArgChecker.AssertArgNotNull(g, "g");
            ArgChecker.AssertArgNotNull(currentBox, "blockBox");

            if (currentBox.Display != CssConstants.None)
            {
                currentBox.RectanglesReset();
                currentBox.MeasureWordsSize(g);
            }

            //position words within local max right
            //box bottom should be updated by this method
            //as text wrap to new lines increase bottom
            var status = LayoutWords(g, currentBox, currentLine,
                 curX, curY, leftLimit, rightLimit, currentBottom);


            var layoutCoreStatus = new LayoutCoreStatus()
            {
                CurrentLineBox = status.CurrentLineBox,
                CurX = status.CurX,
                CurY = status.CurY,
                CurrentMaxBottom = status.CurrentMaxBottom
            };

            foreach (var box in currentBox.Boxes)
            {
                var result = LayoutRecursively(g, box, layoutCoreStatus.CurX, layoutCoreStatus.CurY,
                      layoutCoreStatus.CurrentLineBox, leftLimit, rightLimit, layoutCoreStatus.CurrentMaxBottom);

                if (result != null)
                {
                    layoutCoreStatus = result;
                }
            }

            return new StaticNoneInlineStatus()
            {
                CurrentLineBox = layoutCoreStatus.CurrentLineBox,
                CurX = layoutCoreStatus.CurX,
                CurY = layoutCoreStatus.CurY,
                CurrentMaxBottom = layoutCoreStatus.CurrentMaxBottom
            };
        }
    }
}
