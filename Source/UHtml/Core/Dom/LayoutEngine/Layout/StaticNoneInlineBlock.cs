using UHtml.Adapters;
using UHtml.Adapters.Entities;
using UHtml.Core.Parse;
using UHtml.Core.Utils;

namespace UHtml.Core.Dom
{
    internal class StaticNoneInlineBlockStatus
    {
        public double CurX { get; set; }
        public double CurY { get; set; }

        public CssLineBox CurrentLineBox { get; set; }
        public double CurrentMaxBottom { get; internal set; }
    }

    internal static partial class CssLayoutEngine
    {
        public static StaticNoneInlineBlockStatus LayoutStaticNoneInlineBlock(RGraphics g,
          CssBox currentBox,
          double curX, double curY,
          CssLineBox currentLine,
          double leftLimit, double rightLimit,
          double currentBottom)
        {
           
            SetInlineBlockBoxSize(currentBox, curX, curY,
                                                currentLine,
                                                leftLimit, rightLimit,
                                                currentBottom);

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

            return new StaticNoneInlineBlockStatus()
            {
                CurrentLineBox = layoutCoreStatus.CurrentLineBox,
                CurX = layoutCoreStatus.CurX,
                CurY = layoutCoreStatus.CurY,
                CurrentMaxBottom = layoutCoreStatus.CurrentMaxBottom
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

            currentLine.ReportExistanceOf(box);
        }
    }
}

