using System.Windows.Media;
using UHtml.Adapters;

namespace UHtml.WPF.Adapters
{
    /// <summary>
    /// Adapter for WPF Font.
    /// </summary>
    internal sealed class FontAdapter : RFont
    {
        #region Fields and Consts

        /// <summary>
        /// the underline win-forms font.
        /// </summary>
        private readonly Typeface font;

        /// <summary>
        /// The glyph font for the font
        /// </summary>
        private readonly GlyphTypeface glyphTypeface;

        /// <summary>
        /// the size of the font
        /// </summary>
        private readonly double size;

        /// <summary>
        /// the vertical offset of the font underline location from the top of the font.
        /// </summary>
        private readonly double underlineOffset = -1;

        /// <summary>
        /// Cached font height.
        /// </summary>
        private readonly double height = -1;

        /// <summary>
        /// Cached font whitespace width.
        /// </summary>
        private double whitespaceWidth = -1;

        #endregion


        /// <summary>
        /// Init.
        /// </summary>
        public FontAdapter(Typeface font, double size)
        {
            this.font = font;
            this.size = size;
            height = 96d / 72d * this.size * this.font.FontFamily.LineSpacing;
            underlineOffset = 96d / 72d * this.size * (this.font.FontFamily.LineSpacing + font.UnderlinePosition);

            GlyphTypeface typeface;
            if (font.TryGetGlyphTypeface(out typeface))
            {
                glyphTypeface = typeface;
            }
            else
            {
                foreach (var sysTypeface in Fonts.SystemTypefaces)
                {
                    if (sysTypeface.TryGetGlyphTypeface(out typeface))
                        break;
                }
            }
        }

        /// <summary>
        /// the underline win-forms font.
        /// </summary>
        public Typeface Font
        {
            get { return font; }
        }

        public GlyphTypeface GlyphTypeface
        {
            get { return glyphTypeface; }
        }

        public override double Size
        {
            get { return size; }
        }

        public override double UnderlineOffset
        {
            get { return underlineOffset; }
        }

        public override double Height
        {
            get { return height; }
        }

        public override double LeftPadding
        {
            get { return height / 6f; }
        }

        public override double GetWhitespaceWidth(RGraphics graphics)
        {
            if (whitespaceWidth < 0)
            {
                whitespaceWidth = graphics.MeasureString(" ", this).Width;
            }
            return whitespaceWidth;
        }
    }
}