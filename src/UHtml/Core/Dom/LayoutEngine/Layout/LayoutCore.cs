using System.Diagnostics.CodeAnalysis;
using UHtml.Adapters;
using System.Linq;
using UHtml.Core.Utils;

namespace UHtml.Core.Dom
{
    internal class LayoutProgress
    {
        public double CurX { get; set; }
        public double CurY { get; set; }

        public CssLineBox CurrentLine { get; set; }

        public double Right { get; set; }
        public double Bottom { get; internal set; }

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
            ArgChecker.AssertArgNotNull(g, "g");
            ArgChecker.AssertArgNotNull(currentBox, "currentBox");

            if (currentBox.Display != CssConstants.None)
            {
                currentBox.RectanglesReset();
                currentBox.MeasureWordsSize(g);
                currentBox.LineBoxes.Clear();
            }

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
                                            if (currentBox.boxes.Count > 0
                                                && currentBox.Boxes.Any(x => x.Display == "inline-block"))
                                            {
                                                return LayoutInlineBlockBoxes(g,
                                                    currentBox, curX, curY,
                                                    leftLimit, rightLimit,
                                                    currentBottom);
                                            }
                                            else
                                            {
                                                var staticNoneBlockResult = LayoutStaticNoneBlock(g,
                                                   currentBox,
                                                   curX, curY,
                                                   currentLine,
                                                   leftLimit, rightLimit,
                                                   currentBottom);


                                                return new LayoutProgress()
                                                {
                                                    CurX = staticNoneBlockResult.CurX,
                                                    CurY = staticNoneBlockResult.CurY,
                                                    Right = staticNoneBlockResult.Right,
                                                    Bottom = staticNoneBlockResult.Bottom,

                                                };
                                            }
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
                                                CurX = staticNoneInlineBlockResult.CurX,
                                                CurY = staticNoneInlineBlockResult.CurY,
                                                Right = staticNoneInlineBlockResult.Right,
                                                Bottom = staticNoneInlineBlockResult.Bottom,
                                                CurrentLine = staticNoneInlineBlockResult.CurrentLineBox,
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
                                                CurX = staticNoneInlineResult.CurX,
                                                CurY = staticNoneInlineResult.CurY,
                                                Right = staticNoneInlineResult.Right,
                                                Bottom = staticNoneInlineResult.Bottom,
                                                CurrentLine = staticNoneInlineResult.CurrentLineBox,
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

