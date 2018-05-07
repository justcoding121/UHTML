using System.Diagnostics.CodeAnalysis;
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
        public double CurrentMaxLeft { get; internal set; }
    }

    internal static partial class CssLayoutEngine
    {
        [SuppressMessage("ReSharper", "RedundantCaseLabel")]
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
                                            var staticNoneBlockResult = LayoutStaticNoneBlock(g,
                                               currentBox,
                                               curX, curY,
                                               currentLine,
                                               leftLimit, rightLimit,
                                               currentBottom);

                                            return new LayoutCoreStatus()
                                            {
                                                CurrentLineBox = staticNoneBlockResult.CurrentLineBox,
                                                CurrentMaxLeft = leftLimit,
                                                CurrentMaxBottom = staticNoneBlockResult.CurrentMaxBottom,
                                                CurX = staticNoneBlockResult.CurX,
                                                CurY = staticNoneBlockResult.CurY,
                                                CurrentMaxRight = staticNoneBlockResult.CurrentMaxRight
                                            };
                                        }


                                    case "inline-block":
                                        {
                                            var staticNoneInlineBlockResult = LayoutStaticNoneInlineBlock(g,
                                                currentBox,
                                                curX, curY,
                                                currentLine,
                                                leftLimit, rightLimit,
                                                currentBottom);

                                            return new LayoutCoreStatus()
                                            {
                                                CurrentLineBox = staticNoneInlineBlockResult.CurrentLineBox,
                                                CurrentMaxLeft = leftLimit,
                                                CurrentMaxBottom = staticNoneInlineBlockResult.CurrentMaxBottom,
                                                CurX = staticNoneInlineBlockResult.CurX,
                                                CurY = staticNoneInlineBlockResult.CurY,
                                                CurrentMaxRight = rightLimit
                                            };
                                        }
                                    case "inline":
                                    default:
                                        {
                                            var staticNoneInlineResult = LayoutStaticNoneInline(g,
                                                currentBox,
                                                curX, curY,
                                                currentLine,
                                                leftLimit, rightLimit,
                                                currentBottom);

                                            return new LayoutCoreStatus()
                                            {
                                                CurrentLineBox = staticNoneInlineResult.CurrentLineBox,
                                                CurrentMaxLeft = leftLimit,
                                                CurrentMaxBottom = staticNoneInlineResult.CurrentMaxBottom,
                                                CurX = staticNoneInlineResult.CurX,
                                                CurY = staticNoneInlineResult.CurY,
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

