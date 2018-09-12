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
        public static StaticNoneInlineLayoutProgress LayoutStaticNoneInline(RGraphics g,
          CssBox currentBox,
          double curX, double curY,
          CssLineBox currentLine,
          double leftLimit, double rightLimit,
          double currentBottom)
        {
            currentBox.Location = new RPoint(curX + currentBox.ActualMarginLeft, curY);

            var startX = currentBox.Location.X
                          + currentBox.ActualBorderLeftWidth
                          + currentBox.ActualPaddingLeft;

            var layoutCoreStatus = new LayoutProgress()
            {
                CurrentLine = currentLine,
                CurX = startX,
                CurY = currentBox.Location.Y,
                Bottom = currentBottom
            };

            // handle box that is only a whitespace
            if (currentBox.Text != null 
                && currentBox.Text.IsWhitespace() 
                && !currentBox.IsImage 
                && currentBox.Boxes.Count == 0 
                && currentBox.Words.Count == 0)
            {
                layoutCoreStatus.CurX += currentBox.ActualWordSpacing;

                return new StaticNoneInlineLayoutProgress()
                {
                    CurrentLineBox = layoutCoreStatus.CurrentLine,
                    CurX = layoutCoreStatus.CurX + currentBox.ActualMarginRight,
                    CurY = layoutCoreStatus.CurY,
                    Right  = layoutCoreStatus.Right,
                    Bottom = layoutCoreStatus.Bottom,
                    MaxRight = layoutCoreStatus.MaxRight
                };

            }

            //position words within local max right
            //box bottom should be updated by this method
            //as text wrap to new lines increase bottom
            var status = LayoutWords(g, currentBox, layoutCoreStatus.CurrentLine,
                 layoutCoreStatus.CurX, layoutCoreStatus.CurY, leftLimit, rightLimit, currentBottom);

            layoutCoreStatus.CurX = status.CurX;
            layoutCoreStatus.CurY = status.CurY;
            layoutCoreStatus.Bottom = status.Bottom;
            layoutCoreStatus.Right = status.Right;
            layoutCoreStatus.MaxRight = status.MaxRight;
            layoutCoreStatus.CurrentLine = status.CurrentLineBox;

            var right = layoutCoreStatus.Right;
            var maxRight = layoutCoreStatus.MaxRight;

            if (currentBox.Boxes.Count > 0)
            {
                var top = currentBox.Location.Y;

                LayoutProgress result = null;

                foreach (var box in currentBox.Boxes)
                {
                    result = LayoutRecursively(g, box, layoutCoreStatus.CurX, layoutCoreStatus.CurY,
                          layoutCoreStatus.CurrentLine, leftLimit, rightLimit, layoutCoreStatus.Bottom);

                    if (result != null)
                    {
                        layoutCoreStatus.CurX = result.CurX;
                        layoutCoreStatus.CurY = result.CurY;
                        layoutCoreStatus.Right = result.Right;
                        layoutCoreStatus.MaxRight = result.MaxRight;
                        layoutCoreStatus.Bottom = result.Bottom;
                        layoutCoreStatus.CurrentLine = result.CurrentLine;
                    }

                    right = Math.Max(right, layoutCoreStatus.Right);
                    maxRight = Math.Max(maxRight, layoutCoreStatus.MaxRight);
                }
              
            }

            layoutCoreStatus.CurX +=
                        currentBox.ActualPaddingRight
                        + currentBox.ActualBorderRightWidth
                        + currentBox.ActualMarginRight;

            right = Math.Max(right, layoutCoreStatus.CurX);

            return new StaticNoneInlineLayoutProgress()
            {
                CurrentLineBox = layoutCoreStatus.CurrentLine,
                CurX = layoutCoreStatus.CurX,
                CurY = layoutCoreStatus.CurY,
                Right = right,
                MaxRight = Math.Max(maxRight, right),
                Bottom = layoutCoreStatus.Bottom
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
                                 currentBottom - currentTop
                                + box.ActualBorderTopWidth
                                + box.ActualPaddingTop
                                + box.ActualPaddingBottom
                                + box.ActualBorderBottomWidth);

        }
    }

    internal class StaticNoneInlineLayoutProgress
    {
        public double CurX { get; set; }
        public double CurY { get; set; }

        public double Right { get; internal set; }
        public double MaxRight { get; set; }
        public double Bottom { get; internal set; }

        public CssLineBox CurrentLineBox { get; set; }
    }
}
