using System.Diagnostics.CodeAnalysis;
using UHtml.Adapters;


namespace UHtml.Core.Dom
{
    internal class LayoutProgress
    {
        public double CurX { get; set; }
        public double CurY { get; set; }

        public CssLineBox CurrentLine { get; set; }
        public double CurrentBottom { get; internal set; }

    }

    internal static partial class CssLayoutEngine
    {
        [SuppressMessage("ReSharper", "RedundantCaseLabel")]
        public static LayoutProgress LayoutRecursively(RGraphics g,
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

                                            return new LayoutProgress()
                                            {
                                                CurrentBottom = staticNoneBlockResult.CurrentBottom,
                                                CurX = staticNoneBlockResult.CurX,
                                                CurY = staticNoneBlockResult.CurY,
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

                                            return new LayoutProgress()
                                            {
                                                CurrentLine = staticNoneInlineBlockResult.CurrentLineBox,
                                                CurrentBottom = staticNoneInlineBlockResult.CurrentMaxBottom,
                                                CurX = staticNoneInlineBlockResult.CurX,
                                                CurY = staticNoneInlineBlockResult.CurY,
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

                                            return new LayoutProgress()
                                            {
                                                CurrentLine = staticNoneInlineResult.CurrentLineBox,
                                                CurrentBottom = staticNoneInlineResult.CurrentBottom,
                                                CurX = staticNoneInlineResult.CurX,
                                                CurY = staticNoneInlineResult.CurY,
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

