using System;
using System.Globalization;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using UHtml.Adapters;
using UHtml.Adapters.Entities;
using UHtml.Core.Utils;
using UHtml.WPF.Utilities;

namespace UHtml.WPF.Adapters
{
    /// <summary>
    /// Adapter for WPF Graphics.
    /// </summary>
    internal sealed class GraphicsAdapter : RGraphics
    {
        #region Fields and Consts

        /// <summary>
        /// The wrapped WPF graphics object
        /// </summary>
        private readonly DrawingContext g;

        /// <summary>
        /// if to release the graphics object on dispose
        /// </summary>
        private readonly bool releaseGraphics;

        #endregion


        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="g">the WPF graphics object to use</param>
        /// <param name="initialClip">the initial clip of the graphics</param>
        /// <param name="releaseGraphics">optional: if to release the graphics object on dispose (default - false)</param>
        public GraphicsAdapter(DrawingContext g, RRect initialClip, bool releaseGraphics = false)
            : base(WpfAdapter.Instance, initialClip)
        {
            ArgChecker.AssertArgNotNull(g, "g");

            this.g = g;
            this.releaseGraphics = releaseGraphics;
        }

        /// <summary>
        /// Init.
        /// </summary>
        public GraphicsAdapter()
            : base(WpfAdapter.Instance, RRect.Empty)
        {
            g = null;
            releaseGraphics = false;
        }

        public override void PopClip()
        {
            g.Pop();
            clipStack.Pop();
        }

        public override void PushClip(RRect rect)
        {
            clipStack.Push(rect);
            g.PushClip(new RectangleGeometry(Utils.Convert(rect)));
        }

        public override void PushClipExclude(RRect rect)
        {
            var geometry = new CombinedGeometry();
            geometry.Geometry1 = new RectangleGeometry(Utils.Convert(clipStack.Peek()));
            geometry.Geometry2 = new RectangleGeometry(Utils.Convert(rect));
            geometry.GeometryCombineMode = GeometryCombineMode.Exclude;

            clipStack.Push(clipStack.Peek());
            g.PushClip(geometry);
        }

        public override Object SetAntiAliasSmoothingMode()
        {
            return null;
        }

        public override void ReturnPreviousSmoothingMode(Object prevMode)
        { }

        public override RSize MeasureString(string str, RFont font)
        {
            double width = 0;
            GlyphTypeface glyphTypeface = ((FontAdapter)font).GlyphTypeface;
            if (glyphTypeface != null)
            {
                for (int i = 0; i < str.Length; i++)
                {
                    if (glyphTypeface.CharacterToGlyphMap.ContainsKey(str[i]))
                    {
                        ushort glyph = glyphTypeface.CharacterToGlyphMap[str[i]];
                        double advanceWidth = glyphTypeface.AdvanceWidths[glyph];
                        width += advanceWidth;
                    }
                    else
                    {
                        width = 0;
                        break;
                    }
                }
            }

            if (width <= 0)
            {
                var formattedText = new FormattedText(str, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, ((FontAdapter)font).Font, 96d / 72d * font.Size, Brushes.Red);
                return new RSize(formattedText.WidthIncludingTrailingWhitespace, formattedText.Height);
            }

            return new RSize(width * font.Size * 96d / 72d, font.Height);
        }

        public override void MeasureString(string str, RFont font, double maxWidth, out int charFit, out double charFitWidth)
        {
            charFit = 0;
            charFitWidth = 0;
            bool handled = false;
            GlyphTypeface glyphTypeface = ((FontAdapter)font).GlyphTypeface;
            if (glyphTypeface != null)
            {
                handled = true;
                double width = 0;
                for (int i = 0; i < str.Length; i++)
                {
                    if (glyphTypeface.CharacterToGlyphMap.ContainsKey(str[i]))
                    {
                        ushort glyph = glyphTypeface.CharacterToGlyphMap[str[i]];
                        double advanceWidth = glyphTypeface.AdvanceWidths[glyph] * font.Size * 96d / 72d;

                        if (!(width + advanceWidth < maxWidth))
                        {
                            charFit = i;
                            charFitWidth = width;
                            break;
                        }
                        width += advanceWidth;
                    }
                    else
                    {
                        handled = false;
                        break;
                    }
                }
            }

            if (!handled)
            {
                var formattedText = new FormattedText(str, CultureInfo.CurrentCulture, FlowDirection.LeftToRight, ((FontAdapter)font).Font, 96d / 72d * font.Size, Brushes.Red);
                charFit = str.Length;
                charFitWidth = formattedText.WidthIncludingTrailingWhitespace;
            }
        }

        public override void DrawString(string str, RFont font, RColor color, RPoint point, RSize size, bool rtl)
        {
            var colorConv = ((BrushAdapter)adapter.GetSolidBrush(color)).Brush;

            bool glyphRendered = false;
            GlyphTypeface glyphTypeface = ((FontAdapter)font).GlyphTypeface;
            if (glyphTypeface != null)
            {
                double width = 0;
                ushort[] glyphs = new ushort[str.Length];
                double[] widths = new double[str.Length];

                int i = 0;
                for (; i < str.Length; i++)
                {
                    ushort glyph;
                    if (!glyphTypeface.CharacterToGlyphMap.TryGetValue(str[i], out glyph))
                        break;

                    glyphs[i] = glyph;
                    width += glyphTypeface.AdvanceWidths[glyph];
                    widths[i] = 96d / 72d * font.Size * glyphTypeface.AdvanceWidths[glyph];
                }

                if (i >= str.Length)
                {
                    point.Y += glyphTypeface.Baseline * font.Size * 96d / 72d;
                    point.X += rtl ? 96d / 72d * font.Size * width : 0;

                    glyphRendered = true;
                    var glyphRun = new GlyphRun(glyphTypeface, rtl ? 1 : 0, false, 96d / 72d * font.Size, glyphs, Utils.ConvertRound(point), widths, null, null, null, null, null, null);
                    g.DrawGlyphRun(colorConv, glyphRun);
                }
            }

            if (!glyphRendered)
            {
                var formattedText = new FormattedText(str, CultureInfo.CurrentCulture, rtl ? FlowDirection.RightToLeft : FlowDirection.LeftToRight, ((FontAdapter)font).Font, 96d / 72d * font.Size, colorConv);
                point.X += rtl ? formattedText.Width : 0;
                g.DrawText(formattedText, Utils.ConvertRound(point));
            }
        }

        public override RBrush GetTextureBrush(RImage image, RRect dstRect, RPoint translateTransformLocation)
        {
            var brush = new ImageBrush(((ImageAdapter)image).Image);
            brush.Stretch = Stretch.None;
            brush.TileMode = TileMode.Tile;
            brush.Viewport = Utils.Convert(dstRect);
            brush.ViewportUnits = BrushMappingMode.Absolute;
            brush.Transform = new TranslateTransform(translateTransformLocation.X, translateTransformLocation.Y);
            brush.Freeze();
            return new BrushAdapter(brush);
        }

        public override RGraphicsPath GetGraphicsPath()
        {
            return new GraphicsPathAdapter();
        }

        public override void Dispose()
        {
            if (releaseGraphics)
                g.Close();
        }


        #region Delegate graphics methods

        public override void DrawLine(RPen pen, double x1, double y1, double x2, double y2)
        {
            x1 = (int)x1;
            x2 = (int)x2;
            y1 = (int)y1;
            y2 = (int)y2;

            var adj = pen.Width;
            if (Math.Abs(x1 - x2) < .1 && Math.Abs(adj % 2 - 1) < .1)
            {
                x1 += .5;
                x2 += .5;
            }
            if (Math.Abs(y1 - y2) < .1 && Math.Abs(adj % 2 - 1) < .1)
            {
                y1 += .5;
                y2 += .5;
            }

            g.DrawLine(((PenAdapter)pen).CreatePen(), new Point(x1, y1), new Point(x2, y2));
        }

        public override void DrawRectangle(RPen pen, double x, double y, double width, double height)
        {
            var adj = pen.Width;
            if (Math.Abs(adj % 2 - 1) < .1)
            {
                x += .5;
                y += .5;
            }
            
            g.DrawRectangle(null, ((PenAdapter)pen).CreatePen(), new Rect(x, y, width, height));
        }

        public override void DrawRectangle(RBrush brush, double x, double y, double width, double height)
        {
            g.DrawRectangle(((BrushAdapter)brush).Brush, null, new Rect(x, y, width, height));
        }

        public override void DrawImage(RImage image, RRect destRect, RRect srcRect)
        {
            CroppedBitmap croppedImage = new CroppedBitmap(((ImageAdapter)image).Image, new Int32Rect((int)srcRect.X, (int)srcRect.Y, (int)srcRect.Width, (int)srcRect.Height));
            g.DrawImage(croppedImage, Utils.ConvertRound(destRect));
        }

        public override void DrawImage(RImage image, RRect destRect)
        {
            g.DrawImage(((ImageAdapter)image).Image, Utils.ConvertRound(destRect));
        }

        public override void DrawPath(RPen pen, RGraphicsPath path)
        {
            g.DrawGeometry(null, ((PenAdapter)pen).CreatePen(), ((GraphicsPathAdapter)path).GetClosedGeometry());
        }

        public override void DrawPath(RBrush brush, RGraphicsPath path)
        {
            g.DrawGeometry(((BrushAdapter)brush).Brush, null, ((GraphicsPathAdapter)path).GetClosedGeometry());
        }

        public override void DrawPolygon(RBrush brush, RPoint[] points)
        {
            if (points != null && points.Length > 0)
            {
                var g = new StreamGeometry();
                using (var context = g.Open())
                {
                    context.BeginFigure(Utils.Convert(points[0]), true, true);
                    for (int i = 1; i < points.Length; i++)
                        context.LineTo(Utils.Convert(points[i]), true, true);
                }
                g.Freeze();

                this.g.DrawGeometry(((BrushAdapter)brush).Brush, null, g);
            }
        }

        #endregion
    }
}