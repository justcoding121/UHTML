using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHtml.Adapters;


namespace UHtml.Core.Dom
{
    internal class LayoutCoreStatus
    {
        public double CurX { get; set; }
        public double CurY { get; set; }

        public CssLineBox CurrentLineBox { get; set; }
        public double CurrentMaxBottom { get; internal set; }
        public double CurrentMaxRight { get; internal set; }
    }

    internal static partial class CssLayoutEngine
    {
        public static LayoutCoreStatus LayoutRecursively(RGraphics g,
          CssBox currentBox,
          double curX, double curY,
          CssLineBox currentLine,
          double leftLimit, double rightLimit,
          double currentBottom)
        {
            switch (currentBox.Position)
            {
                case "absolute":
                    break;
                case "fixed":
                    break;
                case "relative":
                    break;

                case "initial":
                case "static":
                default:
                    {
                        switch (currentBox.Float)
                        {
                            case "left":
                                break;
                            case "right":
                                break;

                            case "none":
                            default:
                                switch (currentBox.Display)
                                {
                                    case "none":
                                        break;
                                    case "block":
                                        {
                                            var staticNoneBlockStatus = LayoutStaticNoneBlock(g,
                                               currentBox,
                                               curX, curY,
                                               currentLine,
                                               leftLimit, rightLimit,
                                               currentBottom);

                                            return new LayoutCoreStatus()
                                            {
                                                CurrentLineBox = staticNoneBlockStatus.CurrentLineBox,
                                                CurrentMaxBottom = staticNoneBlockStatus.CurrentMaxBottom,
                                                CurX = staticNoneBlockStatus.CurX,
                                                CurY = staticNoneBlockStatus.CurY,
                                                CurrentMaxRight = staticNoneBlockStatus.CurrentMaxRight
                                            };
                                        }


                                    case "inline-block":
                                        break;
                                    case "inline":
                                    default:
                                        {
                                            var staticNoneInlineStatus = LayoutStaticNoneInline(g,
                                                currentBox,
                                                curX, curY,
                                                currentLine,
                                                leftLimit, rightLimit,
                                                currentBottom);

                                            return new LayoutCoreStatus()
                                            {
                                                CurrentLineBox = staticNoneInlineStatus.CurrentLineBox,
                                                CurrentMaxBottom = staticNoneInlineStatus.CurrentMaxBottom,
                                                CurX = staticNoneInlineStatus.CurX,
                                                CurY = staticNoneInlineStatus.CurY,
                                                CurrentMaxRight = rightLimit
                                            };
                                        }
                                }
                                break;
                        }
                    }
                    break;

            }

            return null;
        }
    }
}

