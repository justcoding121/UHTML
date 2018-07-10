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
        public static StaticNoneInlineBlockLayoutProgress LayoutInlineBlockBoxes(RGraphics g, CssBox blockBox)
        {
            var curX = blockBox.Location.X;
            var curY = blockBox.Location.Y;

            var leftLimit = curX;
            var rightLimit = curX + blockBox.ActualWidth;

            var currentBottom = curX;
            CssLineBox currentLine;

            var layoutCoreStatus = new LayoutProgress()
            {
                CurX = curX,
                CurY = curY,
                CurrentBottom = currentBottom
            };

            foreach (var word in blockBox.Boxes)
            {
                var localLeftLimit = curX;
                double localRightLimit;

                if (word.Width != null)
                {
                    localRightLimit = curX + word.ActualWidth;
                }
                else
                {
                    localRightLimit = rightLimit;
                }

                layoutCoreStatus = LayoutRecursively(g, word, layoutCoreStatus.CurX, layoutCoreStatus.CurY,
                      layoutCoreStatus.CurrentLine, localLeftLimit, localRightLimit, layoutCoreStatus.CurrentBottom);

                curX = layoutCoreStatus.CurX;
                //cur

            }

            throw new NotImplementedException();
        }

        public static StaticNoneInlineBlockLayoutProgress LayoutStaticNoneInlineBlock(RGraphics g,
          CssBox currentBox,
          double curX, double curY,
          CssLineBox currentLine,
          double leftLimit, double rightLimit,
          double currentBottom)
        {
            if (currentLine == null)
            {
                currentLine = new CssLineBox(currentBox.ContainingBlock);
            }

            SetInlineBlockBoxSize(currentBox, curX, curY,
                                                currentLine,
                                                leftLimit, rightLimit,
                                                currentBottom);


            if (curX + currentBox.ActualWidth > rightLimit)
            {
                currentBox.Location = new RPoint(leftLimit + currentBox.ActualMarginLeft, curY + currentBox.ActualHeight);
            }
            else
            {
                currentBox.Location = new RPoint(curX + currentBox.ActualMarginLeft, curY);
            }

            leftLimit = currentBox.Location.X;
            rightLimit = currentBox.Location.X + currentBox.ActualWidth;


            var layoutCoreStatus = new LayoutProgress()
            {
                CurX = currentBox.Location.X,
                CurY = currentBox.Location.Y,
                CurrentBottom = currentBottom
            };


            foreach (var box in currentBox.Boxes)
            {
                var result = LayoutRecursively(g, box, layoutCoreStatus.CurX, layoutCoreStatus.CurY,
                      layoutCoreStatus.CurrentLine, leftLimit, rightLimit, layoutCoreStatus.CurrentBottom);

                if (result != null)
                {
                    layoutCoreStatus.CurX = result.CurX;
                    layoutCoreStatus.CurY = result.CurY;
                    layoutCoreStatus.CurrentBottom = result.CurrentBottom;
                    layoutCoreStatus.CurrentLine = result.CurrentLine;
                }
            }

            return new StaticNoneInlineBlockLayoutProgress()
            {
                CurX = currentBox.ActualRight,
                CurY = layoutCoreStatus.CurY,
                CurrentMaxBottom = Math.Max(layoutCoreStatus.CurrentBottom, currentBox.ActualBottom),
                CurrentLineBox = currentLine
            };
        }

        /// <summary>
        /// Set Width & Height for Box
        /// </summary>
        /// <param name="box"></param>
        private static void SetInlineBlockBoxSize(CssBox box,
          double curX, double curY,
          CssLineBox currentLine,
          double leftLimit, double rightLimit,
          double currentBottom)
        {

            double height = CssValueParser.ParseLength(box.Height, box.ContainingBlock.Size.Height, box);
            double width = CssValueParser.ParseLength(box.Width, box.ContainingBlock.Size.Width, box);

            box.Size = new RSize(box.Width != CssConstants.Auto && !string.IsNullOrEmpty(box.Width) ? width
                                + box.ActualBorderLeftWidth
                                + box.ActualPaddingLeft
                                + box.ActualBorderRightWidth
                                + box.ActualPaddingRight
                                : rightLimit
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
                                + box.ActualBorderBottomWidth);

            if (currentLine != null)
            {
                currentLine.ReportExistanceOfBox(box);
            }
        }
    }
}

