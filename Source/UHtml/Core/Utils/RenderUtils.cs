using UHtml.Adapters;
using UHtml.Adapters.Entities;
using UHtml.Core.Dom;

namespace UHtml.Core.Utils
{
    /// <summary>
    /// Provides some drawing functionality
    /// </summary>
    internal static class RenderUtils
    {
        /// <summary>
        /// Check if the given color is visible if painted (has alpha and color values)
        /// </summary>
        /// <param name="color">the color to check</param>
        /// <returns>true - visible, false - not visible</returns>
        public static bool IsColorVisible(RColor color)
        {
            return color.A > 0;
        }

        /// <summary>
        /// Clip the region the graphics will draw on by the overflow style of the containing block.<br/>
        /// Recursively travel up the tree to find containing block that has overflow style set to hidden. if not
        /// block found there will be no clipping and null will be returned.
        /// </summary>
        /// <param name="g">the graphics to clip</param>
        /// <param name="box">the box that is rendered to get containing blocks</param>
        /// <returns>true - was clipped, false - not clipped</returns>
        public static bool ClipGraphicsByOverflow(RGraphics g, CssBox box)
        {
            var containingBlock = box.ContainingBlock;
            while (true)
            {
                if (containingBlock.Overflow == CssConstants.Hidden)
                {
                    var prevClip = g.GetClip();
                    var rect = box.ContainingBlock.ClientRectangle;
                    rect.X -= 2; // TODO:a find better way to fix it
                    rect.Width += 2;

                    if (!box.IsFixed)
                        rect.Offset(box.HtmlContainer.ScrollOffset);

                    rect.Intersect(prevClip);
                    g.PushClip(rect);
                    return true;
                }
                else
                {
                    var cBlock = containingBlock.ContainingBlock;
                    if (cBlock == containingBlock)
                        return false;
                    containingBlock = cBlock;
                }
            }
        }

        /// <summary>
        /// Draw image loading icon.
        /// </summary>
        /// <param name="g">the device to draw into</param>
        /// <param name="htmlContainer"></param>
        /// <param name="r">the rectangle to draw icon in</param>
        public static void DrawImageLoadingIcon(RGraphics g, HtmlContainerInt htmlContainer, RRect r)
        {
            g.DrawRectangle(g.GetPen(RColor.LightGray), r.X1 + 3, r.Y1 + 3, 13, 14);
            var image = htmlContainer.Adapter.GetLoadingImage();
            g.DrawImage(image, new RRect(r.X1 + 4, r.Y1 + 4, image.Width, image.Height));
        }

        /// <summary>
        /// Draw image failed to load icon.
        /// </summary>
        /// <param name="g">the device to draw into</param>
        /// <param name="htmlContainer"></param>
        /// <param name="r">the rectangle to draw icon in</param>
        public static void DrawImageErrorIcon(RGraphics g, HtmlContainerInt htmlContainer, RRect r)
        {
            g.DrawRectangle(g.GetPen(RColor.LightGray), r.X1 + 2, r.Y1 + 2, 15, 15);
            var image = htmlContainer.Adapter.GetLoadingFailedImage();
            g.DrawImage(image, new RRect(r.X1 + 3, r.Y1 + 3, image.Width, image.Height));
        }

        /// <summary>
        /// Creates a rounded rectangle using the specified corner radius<br/>
        /// NW-----NE
        ///  |       |
        ///  |       |
        /// SW-----SE
        /// </summary>
        /// <param name="g">the device to draw into</param>
        /// <param name="rect">Rectangle to round</param>
        /// <param name="nwRadius">Radius of the north east corner</param>
        /// <param name="neRadius">Radius of the north west corner</param>
        /// <param name="seRadius">Radius of the south east corner</param>
        /// <param name="swRadius">Radius of the south west corner</param>
        /// <returns>GraphicsPath with the lines of the rounded rectangle ready to be painted</returns>
        public static RGraphicsPath GetRoundRect(RGraphics g, RRect rect, double nwRadius, double neRadius, double seRadius, double swRadius)
        {
            var path = g.GetGraphicsPath();

            path.Start(rect.X1 + nwRadius, rect.Y1);

            path.LineTo(rect.X2 - neRadius, rect.Y);

            if (neRadius > 0f)
                path.ArcTo(rect.X2, rect.Y1 + neRadius, neRadius, RGraphicsPath.Corner.TopRight);

            path.LineTo(rect.X2, rect.Y2 - seRadius);

            if (seRadius > 0f)
                path.ArcTo(rect.X2 - seRadius, rect.Y2, seRadius, RGraphicsPath.Corner.BottomRight);

            path.LineTo(rect.X1 + swRadius, rect.Y2);

            if (swRadius > 0f)
                path.ArcTo(rect.X1, rect.Y2 - swRadius, swRadius, RGraphicsPath.Corner.BottomLeft);

            path.LineTo(rect.X1, rect.Y1 + nwRadius);

            if (nwRadius > 0f)
                path.ArcTo(rect.X1 + nwRadius, rect.Y1, nwRadius, RGraphicsPath.Corner.TopLeft);

            return path;
        }
    }
}