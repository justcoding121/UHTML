using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UHtml.Adapters;


namespace UHtml.Core.Dom
{
    internal static partial class CssLayoutEngine
    {
        public static void LayoutRecursively(RGraphics g,
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
                                    case "block":
                                        LayoutStaticNoneBlock(g, currentBox, curX, curY,
                                            null, leftLimit,
                                            rightLimit, currentBottom);
                                        break;

                                    case "inline-block":
                                        break;
                                    case "inline":
                                    default:
                                        {
                                            LayoutStaticNoneInline(g,
                                                          currentBox,
                                                          curX, curY,
                                                          currentLine,
                                                          leftLimit, rightLimit,
                                                          currentBottom);
                                        }
                                        break;
                                }
                                break;
                        }
                    }
                    break;

            }
        }
    }
}

