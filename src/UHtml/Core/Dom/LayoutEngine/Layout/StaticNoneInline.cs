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

            currentBox.Location = new RPoint(curX + currentBox.ActualMarginLeft, curY);

            var layoutCoreStatus = new LayoutProgress()
            {
                CurrentLine = currentLine,
                CurX = currentBox.Location.X
                          + currentBox.ActualBorderLeftWidth
                          + currentBox.ActualPaddingLeft,
                CurY = currentBox.Location.Y,
                CurrentBottom = currentBottom
            };

            //position words within local max right
            //box bottom should be updated by this method
            //as text wrap to new lines increase bottom
            var status = LayoutWords(g, currentBox, currentLine,
                 layoutCoreStatus.CurX, layoutCoreStatus.CurY, leftLimit, rightLimit, currentBottom);

            layoutCoreStatus.CurX = status.CurX;
            layoutCoreStatus.CurY = status.CurY;
            layoutCoreStatus.CurrentBottom = status.CurrentMaxBottom;
            layoutCoreStatus.CurrentLine = status.CurrentLineBox;

            if (currentBox.Boxes.Count > 0)
            {
                var maxRight = 0.0; 
                var top = currentBox.Location.Y;

                foreach (var box in currentBox.Boxes)
                {
                    var result = LayoutRecursively(g, box, layoutCoreStatus.CurX, layoutCoreStatus.CurY,
                          layoutCoreStatus.CurrentLine, leftLimit, rightLimit, layoutCoreStatus.CurrentBottom);

                    if (result != null)
                    {
                        layoutCoreStatus.CurX = result.CurX;
                        layoutCoreStatus.CurY = result.CurY;
                        layoutCoreStatus.CurrentBottom = result.CurrentBottom;
                    }

                    maxRight = Math.Max(maxRight, box.ActualRight);
                }
                
                SetInlineBoxSize(currentBox,
                                    leftLimit, maxRight,
                                    top, layoutCoreStatus.CurrentBottom);

                return new StaticNoneInlineLayoutProgress()
                {
                    CurrentLineBox = currentLine,
                    CurX = layoutCoreStatus.CurX,
                    CurY = layoutCoreStatus.CurY,
                    CurrentBottom = layoutCoreStatus.CurrentBottom
                };
            }


            return new StaticNoneInlineLayoutProgress()
            {
                CurrentLineBox = currentLine,
                CurX = layoutCoreStatus.CurX,
                CurY = layoutCoreStatus.CurY,
                CurrentBottom = layoutCoreStatus.CurrentBottom
            };
        }

        /// <summary>
        /// Set Width & Height for Box
        /// </summary>
        /// <param name="box"></param>
        private static void SetInlineBoxSize(CssBox box,
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
