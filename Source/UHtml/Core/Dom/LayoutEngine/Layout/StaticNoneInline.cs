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
    internal static partial class CssLayoutEngine
    {
        public static void LayoutStaticNoneInline(RGraphics g,
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
            LayoutWords(g, null, currentLine, currentBox,
                 curX, curY, leftLimit, rightLimit, currentBottom);


            foreach (var box in currentBox.Boxes)
            {
                LayoutRecursively(g, box, currentBox.Location.X, currentBox.Location.Y,
                     currentLine, currentBox.Location.X, currentBox.ActualRight, currentBottom);
            }

        }
    }
}
