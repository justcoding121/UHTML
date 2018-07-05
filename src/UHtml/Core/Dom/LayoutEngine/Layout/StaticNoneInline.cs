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
    internal class StaticNoneInlineLayoutProgress
    {
        public double CurX { get; set; }
        public double CurY { get; set; }

        public double CurrentBottom { get; internal set; }
        public CssLineBox CurrentLineBox { get; set; }
    }

    internal static partial class CssLayoutEngine
    {
        public static StaticNoneInlineLayoutProgress LayoutStaticNoneInline(RGraphics g,
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
            var status = LayoutWords(g, currentBox, null,
                 curX, curY, leftLimit, rightLimit, currentBottom);


            var layoutCoreStatus = new LayoutProgress()
            {
                CurX = status.CurX,
                CurY = status.CurY,
                CurrentBottom = status.CurrentMaxBottom,
                CurrentLine = status.CurrentLineBox
            };

            foreach (var box in currentBox.Boxes)
            {
                var result = LayoutRecursively(g, box, layoutCoreStatus.CurX, layoutCoreStatus.CurY,
                      layoutCoreStatus.CurrentLine, leftLimit, rightLimit, layoutCoreStatus.CurrentBottom);

                if (result != null)
                {
                    layoutCoreStatus.CurrentBottom = result.CurrentBottom;
                }
            }

            if(currentLine==null)
            {
                currentLine = new CssLineBox(currentBox.ContainingBlock);
            }

            currentLine.ReportExistanceOfBox(currentBox);

            return new StaticNoneInlineLayoutProgress()
            {
                CurrentLineBox = currentLine,
                CurX = layoutCoreStatus.CurX,
                CurY = layoutCoreStatus.CurY,
                CurrentBottom = layoutCoreStatus.CurrentBottom
            };
        }
    }
}
