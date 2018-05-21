using Newtonsoft.Json;
using System;
using System.Globalization;
using System.Text.RegularExpressions;
using UHtml.Adapters;
using UHtml.Adapters.Entities;
using UHtml.Core.Parse;
using UHtml.Core.Utils;

namespace UHtml.Core.Dom
{
    /// <summary>
    /// Base class for css box to handle the css properties.<br/>
    /// Has field and property for every css property that can be set, the properties add additional parsing like
    /// setting the correct border depending what border value was set (single, two , all four).<br/>
    /// Has additional fields to control the location and size of the box and 'actual' css values for some properties
    /// that require additional calculations and parsing.<br/>
    /// </summary>
    [JsonObject(MemberSerialization.OptIn)]
    internal abstract class CssBoxProperties
    {
        #region CSS Fields

        private string backgroundColor = "transparent";
        private string backgroundGradient = "none";
        private string backgroundGradientAngle = "90";
        private string backgroundImage = "none";
        private string backgroundPosition = "0% 0%";
        private string backgroundRepeat = "repeat";
        private string borderTopWidth = "medium";
        private string borderRightWidth = "medium";
        private string borderBottomWidth = "medium";
        private string borderLeftWidth = "medium";
        private string borderTopColor = "black";
        private string borderRightColor = "black";
        private string borderBottomColor = "black";
        private string borderLeftColor = "black";
        private string borderTopStyle = "none";
        private string borderRightStyle = "none";
        private string borderBottomStyle = "none";
        private string borderLeftStyle = "none";
        private string borderSpacing = "0";
        private string borderCollapse = "separate";
        private string bottom;
        private string color = "black";
        private string content = "normal";
        private string cornerNwRadius = "0";
        private string cornerNeRadius = "0";
        private string cornerSeRadius = "0";
        private string cornerSwRadius = "0";
        private string cornerRadius = "0";
        private string emptyCells = "show";
        private string direction = "ltr";
        private string display = "inline";
        private string fontFamily;
        private string fontSize = "medium";
        private string fontStyle = "normal";
        private string fontVariant = "normal";
        private string fontWeight = "normal";
        private string @float = "none";
        private string clear = "none";
        private string height = "auto";
        private string marginBottom = "0";
        private string marginLeft = "0";
        private string marginRight = "0";
        private string marginTop = "0";
        private string left = "auto";
        private string lineHeight = "normal";
        private string listStyleType = "disc";
        private string listStyleImage = string.Empty;
        private string listStylePosition = "outside";
        private string listStyle = string.Empty;
        private string overflow = "visible";
        private string paddingLeft = "0";
        private string paddingBottom = "0";
        private string paddingRight = "0";
        private string paddingTop = "0";
        private string pageBreakInside = CssConstants.Auto;
        private string right;
        private string textAlign = string.Empty;
        private string textDecoration = string.Empty;
        private string textIndent = "0";
        private string top = "auto";
        private string position = "static";
        private string verticalAlign = "baseline";
        private string width = "auto";
        private string maxWidth = "none";
        private string wordSpacing = "normal";
        private string wordBreak = "normal";
        private string whiteSpace = "normal";
        private string visibility = "visible";

        #endregion

        #region Fields

        /// <summary>
        /// Gets or sets the location of the box
        /// </summary>
        private RPoint location;

        /// <summary>
        /// Gets or sets the size of the box
        /// </summary>
        private RSize size;

        private double actualCornerNw = double.NaN;
        private double actualCornerNe = double.NaN;
        private double actualCornerSw = double.NaN;
        private double actualCornerSe = double.NaN;
        private RColor actualColor = RColor.Empty;
        private double actualBackgroundGradientAngle = double.NaN;
        private double actualHeight = double.NaN;
        private double actualWidth = double.NaN;
        private double actualPaddingTop = double.NaN;
        private double actualPaddingBottom = double.NaN;
        private double actualPaddingRight = double.NaN;
        private double actualPaddingLeft = double.NaN;
        private double actualMarginTop = double.NaN;
        private double collapsedMarginTop = double.NaN;
        private double actualMarginBottom = double.NaN;
        private double actualMarginRight = double.NaN;
        private double actualMarginLeft = double.NaN;
        private double actualBorderTopWidth = double.NaN;
        private double actualBorderLeftWidth = double.NaN;
        private double actualBorderBottomWidth = double.NaN;
        private double actualBorderRightWidth = double.NaN;

        /// <summary>
        /// the width of whitespace between words
        /// </summary>
        private double actualLineHeight = double.NaN;

        private double actualWordSpacing = double.NaN;
        private double actualTextIndent = double.NaN;
        private double actualBorderSpacingHorizontal = double.NaN;
        private double actualBorderSpacingVertical = double.NaN;
        private RColor actualBackgroundGradient = RColor.Empty;
        private RColor actualBorderTopColor = RColor.Empty;
        private RColor actualBorderLeftColor = RColor.Empty;
        private RColor actualBorderBottomColor = RColor.Empty;
        private RColor actualBorderRightColor = RColor.Empty;
        private RColor actualBackgroundColor = RColor.Empty;
        private RFont actualFont;

        #endregion


        #region CSS Properties
        [JsonProperty]
        public string BorderBottomWidth
        {
            get { return borderBottomWidth; }
            set
            {
                borderBottomWidth = value;
                actualBorderBottomWidth = Single.NaN;
            }
        }

        [JsonProperty]
        public string BorderLeftWidth
        {
            get { return borderLeftWidth; }
            set
            {
                borderLeftWidth = value;
                actualBorderLeftWidth = Single.NaN;
            }
        }
        [JsonProperty]
        public string BorderRightWidth
        {
            get { return borderRightWidth; }
            set
            {
                borderRightWidth = value;
                actualBorderRightWidth = Single.NaN;
            }
        }
        [JsonProperty]
        public string BorderTopWidth
        {
            get { return borderTopWidth; }
            set
            {
                borderTopWidth = value;
                actualBorderTopWidth = Single.NaN;
            }
        }
        [JsonProperty]
        public string BorderBottomStyle
        {
            get { return borderBottomStyle; }
            set { borderBottomStyle = value; }
        }
        [JsonProperty]
        public string BorderLeftStyle
        {
            get { return borderLeftStyle; }
            set { borderLeftStyle = value; }
        }
        [JsonProperty]
        public string BorderRightStyle
        {
            get { return borderRightStyle; }
            set { borderRightStyle = value; }
        }
        [JsonProperty]
        public string BorderTopStyle
        {
            get { return borderTopStyle; }
            set { borderTopStyle = value; }
        }
        [JsonProperty]
        public string BorderBottomColor
        {
            get { return borderBottomColor; }
            set
            {
                borderBottomColor = value;
                actualBorderBottomColor = RColor.Empty;
            }
        }
        [JsonProperty]
        public string BorderLeftColor
        {
            get { return borderLeftColor; }
            set
            {
                borderLeftColor = value;
                actualBorderLeftColor = RColor.Empty;
            }
        }
        [JsonProperty]
        public string BorderRightColor
        {
            get { return borderRightColor; }
            set
            {
                borderRightColor = value;
                actualBorderRightColor = RColor.Empty;
            }
        }
        [JsonProperty]
        public string BorderTopColor
        {
            get { return borderTopColor; }
            set
            {
                borderTopColor = value;
                actualBorderTopColor = RColor.Empty;
            }
        }
        [JsonProperty]
        public string BorderSpacing
        {
            get { return borderSpacing; }
            set { borderSpacing = value; }
        }
        [JsonProperty]
        public string BorderCollapse
        {
            get { return borderCollapse; }
            set { borderCollapse = value; }
        }
        [JsonProperty]
        public string CornerRadius
        {
            get { return cornerRadius; }
            set
            {
                MatchCollection r = RegexParserUtils.Match(RegexParserUtils.CssLength, value);

                switch (r.Count)
                {
                    case 1:
                        CornerNeRadius = r[0].Value;
                        CornerNwRadius = r[0].Value;
                        CornerSeRadius = r[0].Value;
                        CornerSwRadius = r[0].Value;
                        break;
                    case 2:
                        CornerNeRadius = r[0].Value;
                        CornerNwRadius = r[0].Value;
                        CornerSeRadius = r[1].Value;
                        CornerSwRadius = r[1].Value;
                        break;
                    case 3:
                        CornerNeRadius = r[0].Value;
                        CornerNwRadius = r[1].Value;
                        CornerSeRadius = r[2].Value;
                        break;
                    case 4:
                        CornerNeRadius = r[0].Value;
                        CornerNwRadius = r[1].Value;
                        CornerSeRadius = r[2].Value;
                        CornerSwRadius = r[3].Value;
                        break;
                }

                cornerRadius = value;
            }
        }
        [JsonProperty]
        public string CornerNwRadius
        {
            get { return cornerNwRadius; }
            set { cornerNwRadius = value; }
        }
        [JsonProperty]
        public string CornerNeRadius
        {
            get { return cornerNeRadius; }
            set { cornerNeRadius = value; }
        }
        [JsonProperty]
        public string CornerSeRadius
        {
            get { return cornerSeRadius; }
            set { cornerSeRadius = value; }
        }
        [JsonProperty]
        public string CornerSwRadius
        {
            get { return cornerSwRadius; }
            set { cornerSwRadius = value; }
        }
        [JsonProperty]
        public string MarginBottom
        {
            get { return marginBottom; }
            set { marginBottom = value; }
        }
        [JsonProperty]
        public string MarginLeft
        {
            get { return marginLeft; }
            set { marginLeft = value; }
        }
        [JsonProperty]
        public string MarginRight
        {
            get { return marginRight; }
            set { marginRight = value; }
        }
        [JsonProperty]
        public string MarginTop
        {
            get { return marginTop; }
            set { marginTop = value; }
        }
        [JsonProperty]
        public string PaddingBottom
        {
            get { return paddingBottom; }
            set
            {
                paddingBottom = value;
                actualPaddingBottom = double.NaN;
            }
        }
        [JsonProperty]
        public string PaddingLeft
        {
            get { return paddingLeft; }
            set
            {
                paddingLeft = value;
                actualPaddingLeft = double.NaN;
            }
        }
        [JsonProperty]
        public string PaddingRight
        {
            get { return paddingRight; }
            set
            {
                paddingRight = value;
                actualPaddingRight = double.NaN;
            }
        }
        [JsonProperty]
        public string PaddingTop
        {
            get { return paddingTop; }
            set
            {
                paddingTop = value;
                actualPaddingTop = double.NaN;
            }
        }
        [JsonProperty]
        public string PageBreakInside
        {
            get { return pageBreakInside; }
            set
            {
                pageBreakInside = value;
            }
        }
        [JsonProperty]
        public string Left
        {
            get { return left; }
            set
            {
                left = value;

                if (Position == CssConstants.Fixed)
                {
                    location = GetActualLocation(Left, Top);
                }
            }
        }
        [JsonProperty]
        public string Top
        {
            get { return top; }
            set {
                top = value;

                if (Position == CssConstants.Fixed)
                {
                    location = GetActualLocation(Left, Top);
                }

            }
        }
        [JsonProperty]
        public string Width
        {
            get { return width; }
            set { width = value; }
        }
        [JsonProperty]
        public string MaxWidth
        {
            get { return maxWidth; }
            set { maxWidth = value; }
        }
        [JsonProperty]
        public string Height
        {
            get { return height; }
            set { height = value; }
        }
        [JsonProperty]
        public string BackgroundColor
        {
            get { return backgroundColor; }
            set { backgroundColor = value; }
        }
        [JsonProperty]
        public string BackgroundImage
        {
            get { return backgroundImage; }
            set { backgroundImage = value; }
        }
        [JsonProperty]
        public string BackgroundPosition
        {
            get { return backgroundPosition; }
            set { backgroundPosition = value; }
        }
        [JsonProperty]
        public string BackgroundRepeat
        {
            get { return backgroundRepeat; }
            set { backgroundRepeat = value; }
        }
        [JsonProperty]
        public string BackgroundGradient
        {
            get { return backgroundGradient; }
            set { backgroundGradient = value; }
        }
        [JsonProperty]
        public string BackgroundGradientAngle
        {
            get { return backgroundGradientAngle; }
            set { backgroundGradientAngle = value; }
        }
        [JsonProperty]
        public string Color
        {
            get { return color; }
            set
            {
                color = value;
                actualColor = RColor.Empty;
            }
        }
        [JsonProperty]
        public string Content
        {
            get { return content; }
            set { content = value; }
        }
        [JsonProperty]
        public string Display
        {
            get { return display; }
            set { display = value; }
        }
        [JsonProperty]
        public string Direction
        {
            get { return direction; }
            set { direction = value; }
        }
        [JsonProperty]
        public string EmptyCells
        {
            get { return emptyCells; }
            set { emptyCells = value; }
        }
        [JsonProperty]
        public string Float
        {
            get { return @float; }
            set { @float = value; }
        }

        [JsonProperty]
        public string Clear
        {
            get { return clear; }
            set { clear = value; }
        }

        [JsonProperty]
        public string Position
        {
            get { return position; }
            set { position = value; }
        }
        [JsonProperty]
        public string LineHeight
        {
            get { return lineHeight; }
            set { lineHeight = string.Format(NumberFormatInfo.InvariantInfo, "{0}px", CssValueParser.ParseLength(value, Size.Height, this, CssConstants.Em)); }
        }
        [JsonProperty]
        public string VerticalAlign
        {
            get { return verticalAlign; }
            set { verticalAlign = value; }
        }
        [JsonProperty]
        public string TextIndent
        {
            get { return textIndent; }
            set { textIndent = NoEms(value); }
        }
        [JsonProperty]
        public string TextAlign
        {
            get { return textAlign; }
            set { textAlign = value; }
        }
        [JsonProperty]
        public string TextDecoration
        {
            get { return textDecoration; }
            set { textDecoration = value; }
        }
        [JsonProperty]
        public string WhiteSpace
        {
            get { return whiteSpace; }
            set { whiteSpace = value; }
        }
        [JsonProperty]
        public string Visibility
        {
            get { return visibility; }
            set { visibility = value; }
        }
        [JsonProperty]
        public string WordSpacing
        {
            get { return wordSpacing; }
            set { wordSpacing = NoEms(value); }
        }
        [JsonProperty]
        public string WordBreak
        {
            get { return wordBreak; }
            set { wordBreak = value; }
        }
        [JsonProperty]
        public string FontFamily
        {
            get { return fontFamily; }
            set { fontFamily = value; }
        }
        [JsonProperty]
        public string FontSize
        {
            get { return fontSize; }
            set
            {
                string length = RegexParserUtils.Search(RegexParserUtils.CssLength, value);

                if (length != null)
                {
                    string computedValue;
                    CssLength len = new CssLength(length);

                    if (len.HasError)
                    {
                        computedValue = "medium";
                    }
                    else if (len.Unit == CssUnit.Ems && GetParent() != null)
                    {
                        computedValue = len.ConvertEmToPoints(GetParent().ActualFont.Size).ToString();
                    }
                    else
                    {
                        computedValue = len.ToString();
                    }

                    fontSize = computedValue;
                }
                else
                {
                    fontSize = value;
                }
            }
        }
        [JsonProperty]
        public string FontStyle
        {
            get { return fontStyle; }
            set { fontStyle = value; }
        }
        [JsonProperty]
        public string FontVariant
        {
            get { return fontVariant; }
            set { fontVariant = value; }
        }
        [JsonProperty]
        public string FontWeight
        {
            get { return fontWeight; }
            set { fontWeight = value; }
        }
        [JsonProperty]
        public string ListStyle
        {
            get { return listStyle; }
            set { listStyle = value; }
        }
        [JsonProperty]
        public string Overflow
        {
            get { return overflow; }
            set { overflow = value; }
        }
        [JsonProperty]
        public string ListStylePosition
        {
            get { return listStylePosition; }
            set { listStylePosition = value; }
        }
        [JsonProperty]
        public string ListStyleImage
        {
            get { return listStyleImage; }
            set { listStyleImage = value; }
        }
        [JsonProperty]
        public string ListStyleType
        {
            get { return listStyleType; }
            set { listStyleType = value; }
        }

        #endregion CSS Propertier
        [JsonProperty]
        /// <summary>
        /// Gets or sets the location of the box
        /// </summary>
        public RPoint Location
        {
            get {
                if (location.IsEmpty && Position == CssConstants.Fixed)
                {
                    var left = Left;
                    var top = Top;

                    location = GetActualLocation(Left, Top);
                }
                return location;
            }
            set {
                location = value;
            }
        }

        /// <summary>
        /// Gets or sets the size of the box
        /// </summary>
        [JsonProperty]
        public RSize Size
        {
            get { return size; }
            set { size = value; }
        }

        /// <summary>
        /// Gets the bounds of the box
        /// </summary>
        [JsonProperty]
        public RRect Bounds
        {
            get { return new RRect(Location, Size); }
        }

        /// <summary>
        /// Gets the width available on the box, counting padding and margin.
        /// </summary>
        [JsonProperty]
        public double AvailableWidth
        {
            get { return Size.Width - ActualBorderLeftWidth - ActualPaddingLeft - ActualPaddingRight - ActualBorderRightWidth; }
        }

        /// <summary>
        /// Gets the right of the box. When setting, it will affect only the width of the box.
        /// </summary>
        [JsonProperty]
        public double ActualRight
        {
            get { return Location.X + Size.Width; }
            set { Size = new RSize(value - Location.X, Size.Height); }
        }

        /// <summary>
        /// Gets or sets the bottom of the box. 
        /// (When setting, alters only the Size.Height of the box)
        /// </summary>
        [JsonProperty]
        public double ActualBottom
        {
            get { return Location.Y + Size.Height; }
            set { Size = new RSize(Size.Width, value - Location.Y); }
        }

        /// <summary>
        /// Gets the left of the client rectangle (Where content starts rendering)
        /// </summary>
        [JsonProperty]
        public double ClientLeft
        {
            get { return Location.X + ActualBorderLeftWidth + ActualPaddingLeft; }
        }

        /// <summary>
        /// Gets the top of the client rectangle (Where content starts rendering)
        /// </summary>
        [JsonProperty]
        public double ClientTop
        {
            get { return Location.Y + ActualBorderTopWidth + ActualPaddingTop; }
        }

        /// <summary>
        /// Gets the right of the client rectangle
        /// </summary>
        [JsonProperty]
        public double ClientRight
        {
            get { return ActualRight - ActualPaddingRight - ActualBorderRightWidth; }
        }

        /// <summary>
        /// Gets the bottom of the client rectangle
        /// </summary>
        [JsonProperty]
        public double ClientBottom
        {
            get { return ActualBottom - ActualPaddingBottom - ActualBorderBottomWidth; }
        }

        /// <summary>
        /// Gets the client rectangle
        /// </summary>
        [JsonProperty]
        public RRect ClientRectangle
        {
            get { return RRect.FromCoordinates(ClientLeft, ClientTop, ClientRight, ClientBottom); }
        }

        /// <summary>
        /// Gets the actual height
        /// </summary>
        public double ActualHeight
        {
            get
            {
                if (double.IsNaN(actualHeight))
                {
                    actualHeight = CssValueParser.ParseLength(Height, Size.Height, this);
                }
                return actualHeight;
            }
        }

        /// <summary>
        /// Gets the actual height
        /// </summary>

        public double ActualWidth
        {
            get
            {
                if (double.IsNaN(actualWidth))
                {
                    actualWidth = CssValueParser.ParseLength(Width, Size.Width, this);
                }
                return actualWidth;
            }
        }

        /// <summary>
        /// Gets the actual top's padding
        /// </summary>
        public double ActualPaddingTop
        {
            get
            {
                if (double.IsNaN(actualPaddingTop))
                {
                    actualPaddingTop = CssValueParser.ParseLength(PaddingTop, Size.Width, this);
                }
                return actualPaddingTop;
            }
        }

        /// <summary>
        /// Gets the actual padding on the left
        /// </summary>
        public double ActualPaddingLeft
        {
            get
            {
                if (double.IsNaN(actualPaddingLeft))
                {
                    actualPaddingLeft = CssValueParser.ParseLength(PaddingLeft, Size.Width, this);
                }
                return actualPaddingLeft;
            }
        }

        /// <summary>
        /// Gets the actual Padding of the bottom
        /// </summary>
        public double ActualPaddingBottom
        {
            get
            {
                if (double.IsNaN(actualPaddingBottom))
                {
                    actualPaddingBottom = CssValueParser.ParseLength(PaddingBottom, Size.Width, this);
                }
                return actualPaddingBottom;
            }
        }

        /// <summary>
        /// Gets the actual padding on the right
        /// </summary>
        public double ActualPaddingRight
        {
            get
            {
                if (double.IsNaN(actualPaddingRight))
                {
                    actualPaddingRight = CssValueParser.ParseLength(PaddingRight, Size.Width, this);
                }
                return actualPaddingRight;
            }
        }

        /// <summary>
        /// Gets the actual top's Margin
        /// </summary>
        public double ActualMarginTop
        {
            get
            {
                if (double.IsNaN(actualMarginTop))
                {
                    if (MarginTop == CssConstants.Auto)
                        MarginTop = "0";
                    var actualMarginTop = CssValueParser.ParseLength(MarginTop, Size.Width, this);
                    if (MarginLeft.EndsWith("%"))
                        return actualMarginTop;
                    this.actualMarginTop = actualMarginTop;
                }
                return actualMarginTop;
            }
        }

        /// <summary>
        /// The margin top value if was effected by margin collapse.
        /// </summary>
        [JsonProperty]
        public double CollapsedMarginTop
        {
            get { return double.IsNaN(collapsedMarginTop) ? 0 : collapsedMarginTop; }
            set { collapsedMarginTop = value; }
        }

        /// <summary>
        /// Gets the actual Margin on the left
        /// </summary>
        public double ActualMarginLeft
        {
            get
            {
                if (double.IsNaN(actualMarginLeft))
                {
                    if (MarginLeft == CssConstants.Auto)
                        MarginLeft = "0";
                    var actualMarginLeft = CssValueParser.ParseLength(MarginLeft, Size.Width, this);
                    if (MarginLeft.EndsWith("%"))
                        return actualMarginLeft;
                    this.actualMarginLeft = actualMarginLeft;
                }
                return actualMarginLeft;
            }
        }

        /// <summary>
        /// Gets the actual Margin of the bottom
        /// </summary>
        public double ActualMarginBottom
        {
            get
            {
                if (double.IsNaN(actualMarginBottom))
                {
                    if (MarginBottom == CssConstants.Auto)
                        MarginBottom = "0";
                    var actualMarginBottom = CssValueParser.ParseLength(MarginBottom, Size.Width, this);
                    if (MarginLeft.EndsWith("%"))
                        return actualMarginBottom;
                    this.actualMarginBottom = actualMarginBottom;
                }
                return actualMarginBottom;
            }
        }

        /// <summary>
        /// Gets the actual Margin on the right
        /// </summary>
        public double ActualMarginRight
        {
            get
            {
                if (double.IsNaN(actualMarginRight))
                {
                    if (MarginRight == CssConstants.Auto)
                        MarginRight = "0";
                    var actualMarginRight = CssValueParser.ParseLength(MarginRight, Size.Width, this);
                    if (MarginLeft.EndsWith("%"))
                        return actualMarginRight;
                    this.actualMarginRight = actualMarginRight;
                }
                return actualMarginRight;
            }
        }

        /// <summary>
        /// Gets the actual top border width
        /// </summary>
        public double ActualBorderTopWidth
        {
            get
            {
                if (double.IsNaN(actualBorderTopWidth))
                {
                    actualBorderTopWidth = CssValueParser.GetActualBorderWidth(BorderTopWidth, this);
                    if (string.IsNullOrEmpty(BorderTopStyle) || BorderTopStyle == CssConstants.None)
                    {
                        actualBorderTopWidth = 0f;
                    }
                }
                return actualBorderTopWidth;
            }
        }

        /// <summary>
        /// Gets the actual Left border width
        /// </summary>
        public double ActualBorderLeftWidth
        {
            get
            {
                if (double.IsNaN(actualBorderLeftWidth))
                {
                    actualBorderLeftWidth = CssValueParser.GetActualBorderWidth(BorderLeftWidth, this);
                    if (string.IsNullOrEmpty(BorderLeftStyle) || BorderLeftStyle == CssConstants.None)
                    {
                        actualBorderLeftWidth = 0f;
                    }
                }
                return actualBorderLeftWidth;
            }
        }

        /// <summary>
        /// Gets the actual Bottom border width
        /// </summary>
        public double ActualBorderBottomWidth
        {
            get
            {
                if (double.IsNaN(actualBorderBottomWidth))
                {
                    actualBorderBottomWidth = CssValueParser.GetActualBorderWidth(BorderBottomWidth, this);
                    if (string.IsNullOrEmpty(BorderBottomStyle) || BorderBottomStyle == CssConstants.None)
                    {
                        actualBorderBottomWidth = 0f;
                    }
                }
                return actualBorderBottomWidth;
            }
        }

        /// <summary>
        /// Gets the actual Right border width
        /// </summary>
        public double ActualBorderRightWidth
        {
            get
            {
                if (double.IsNaN(actualBorderRightWidth))
                {
                    actualBorderRightWidth = CssValueParser.GetActualBorderWidth(BorderRightWidth, this);
                    if (string.IsNullOrEmpty(BorderRightStyle) || BorderRightStyle == CssConstants.None)
                    {
                        actualBorderRightWidth = 0f;
                    }
                }
                return actualBorderRightWidth;
            }
        }

        /// <summary>
        /// Gets the actual top border Color
        /// </summary>
        public RColor ActualBorderTopColor
        {
            get
            {
                if (actualBorderTopColor.IsEmpty)
                {
                    actualBorderTopColor = GetActualColor(BorderTopColor);
                }
                return actualBorderTopColor;
            }
        }

        protected abstract RPoint GetActualLocation(string X, string Y);

        protected abstract RColor GetActualColor(string colorStr);

        /// <summary>
        /// Gets the actual Left border Color
        /// </summary>
        public RColor ActualBorderLeftColor
        {
            get
            {
                if ((actualBorderLeftColor.IsEmpty))
                {
                    actualBorderLeftColor = GetActualColor(BorderLeftColor);
                }
                return actualBorderLeftColor;
            }
        }

        /// <summary>
        /// Gets the actual Bottom border Color
        /// </summary>
        public RColor ActualBorderBottomColor
        {
            get
            {
                if ((actualBorderBottomColor.IsEmpty))
                {
                    actualBorderBottomColor = GetActualColor(BorderBottomColor);
                }
                return actualBorderBottomColor;
            }
        }

        /// <summary>
        /// Gets the actual Right border Color
        /// </summary>
        public RColor ActualBorderRightColor
        {
            get
            {
                if ((actualBorderRightColor.IsEmpty))
                {
                    actualBorderRightColor = GetActualColor(BorderRightColor);
                }
                return actualBorderRightColor;
            }
        }

        /// <summary>
        /// Gets the actual length of the north west corner
        /// </summary>
        public double ActualCornerNw
        {
            get
            {
                if (double.IsNaN(actualCornerNw))
                {
                    actualCornerNw = CssValueParser.ParseLength(CornerNwRadius, 0, this);
                }
                return actualCornerNw;
            }
        }

        /// <summary>
        /// Gets the actual length of the north east corner
        /// </summary>
        public double ActualCornerNe
        {
            get
            {
                if (double.IsNaN(actualCornerNe))
                {
                    actualCornerNe = CssValueParser.ParseLength(CornerNeRadius, 0, this);
                }
                return actualCornerNe;
            }
        }

        /// <summary>
        /// Gets the actual length of the south east corner
        /// </summary>
        public double ActualCornerSe
        {
            get
            {
                if (double.IsNaN(actualCornerSe))
                {
                    actualCornerSe = CssValueParser.ParseLength(CornerSeRadius, 0, this);
                }
                return actualCornerSe;
            }
        }

        /// <summary>
        /// Gets the actual length of the south west corner
        /// </summary>
        public double ActualCornerSw
        {
            get
            {
                if (double.IsNaN(actualCornerSw))
                {
                    actualCornerSw = CssValueParser.ParseLength(CornerSwRadius, 0, this);
                }
                return actualCornerSw;
            }
        }

        /// <summary>
        /// Gets a value indicating if at least one of the corners of the box is rounded
        /// </summary>
        public bool IsRounded
        {
            get { return ActualCornerNe > 0f || ActualCornerNw > 0f || ActualCornerSe > 0f || ActualCornerSw > 0f; }
        }

        /// <summary>
        /// Gets the actual width of whitespace between words.
        /// </summary>
        public double ActualWordSpacing
        {
            get { return actualWordSpacing; }
        }

        /// <summary>
        /// 
        /// Gets the actual color for the text.
        /// </summary>
        public RColor ActualColor
        {
            get
            {
                if (actualColor.IsEmpty)
                {
                    actualColor = GetActualColor(Color);
                }

                return actualColor;
            }
        }

        /// <summary>
        /// Gets the actual background color of the box
        /// </summary>
        public RColor ActualBackgroundColor
        {
            get
            {
                if (actualBackgroundColor.IsEmpty)
                {
                    actualBackgroundColor = GetActualColor(BackgroundColor);
                }

                return actualBackgroundColor;
            }
        }

        /// <summary>
        /// Gets the second color that creates a gradient for the background
        /// </summary>
        public RColor ActualBackgroundGradient
        {
            get
            {
                if (actualBackgroundGradient.IsEmpty)
                {
                    actualBackgroundGradient = GetActualColor(BackgroundGradient);
                }
                return actualBackgroundGradient;
            }
        }

        /// <summary>
        /// Gets the actual angle specified for the background gradient
        /// </summary>
        public double ActualBackgroundGradientAngle
        {
            get
            {
                if (double.IsNaN(actualBackgroundGradientAngle))
                {
                    actualBackgroundGradientAngle = CssValueParser.ParseNumber(BackgroundGradientAngle, 360f);
                }

                return actualBackgroundGradientAngle;
            }
        }

        /// <summary>
        /// Gets the actual font of the parent
        /// </summary>
        public RFont ActualParentFont
        {
            get { return GetParent() == null ? ActualFont : GetParent().ActualFont; }
        }

        /// <summary>
        /// Gets the font that should be actually used to paint the text of the box
        /// </summary>
        public RFont ActualFont
        {
            get
            {
                if (actualFont == null)
                {
                    if (string.IsNullOrEmpty(FontFamily))
                    {
                        FontFamily = CssConstants.DefaultFont;
                    }
                    if (string.IsNullOrEmpty(FontSize))
                    {
                        FontSize = CssConstants.FontSize.ToString(CultureInfo.InvariantCulture) + "pt";
                    }

                    RFontStyle st = RFontStyle.Regular;

                    if (FontStyle == CssConstants.Italic || FontStyle == CssConstants.Oblique)
                    {
                        st |= RFontStyle.Italic;
                    }

                    if (FontWeight != CssConstants.Normal && FontWeight != CssConstants.Lighter && !string.IsNullOrEmpty(FontWeight) && FontWeight != CssConstants.Inherit)
                    {
                        st |= RFontStyle.Bold;
                    }

                    double fsize;
                    double parentSize = CssConstants.FontSize;

                    if (GetParent() != null)
                        parentSize = GetParent().ActualFont.Size;

                    switch (FontSize)
                    {
                        case CssConstants.Medium:
                            fsize = CssConstants.FontSize;
                            break;
                        case CssConstants.XXSmall:
                            fsize = CssConstants.FontSize - 4;
                            break;
                        case CssConstants.XSmall:
                            fsize = CssConstants.FontSize - 3;
                            break;
                        case CssConstants.Small:
                            fsize = CssConstants.FontSize - 2;
                            break;
                        case CssConstants.Large:
                            fsize = CssConstants.FontSize + 2;
                            break;
                        case CssConstants.XLarge:
                            fsize = CssConstants.FontSize + 3;
                            break;
                        case CssConstants.XXLarge:
                            fsize = CssConstants.FontSize + 4;
                            break;
                        case CssConstants.Smaller:
                            fsize = parentSize - 2;
                            break;
                        case CssConstants.Larger:
                            fsize = parentSize + 2;
                            break;
                        default:
                            fsize = CssValueParser.ParseLength(FontSize, parentSize, parentSize, null, true, true);
                            break;
                    }

                    if (fsize <= 1f)
                    {
                        fsize = CssConstants.FontSize;
                    }

                    actualFont = GetCachedFont(FontFamily, fsize, st);
                }
                return actualFont;
            }
        }

        protected abstract RFont GetCachedFont(string fontFamily, double fsize, RFontStyle st);

        /// <summary>
        /// Gets the line height
        /// </summary>
        public double ActualLineHeight
        {
            get
            {
                if (double.IsNaN(actualLineHeight))
                {
                    actualLineHeight = .9f * CssValueParser.ParseLength(LineHeight, Size.Height, this);
                }
                return actualLineHeight;
            }
        }

        /// <summary>
        /// Gets the text indentation (on first line only)
        /// </summary>
        public double ActualTextIndent
        {
            get
            {
                if (double.IsNaN(actualTextIndent))
                {
                    actualTextIndent = CssValueParser.ParseLength(TextIndent, Size.Width, this);
                }

                return actualTextIndent;
            }
        }

        /// <summary>
        /// Gets the actual horizontal border spacing for tables
        /// </summary>
        public double ActualBorderSpacingHorizontal
        {
            get
            {
                if (double.IsNaN(actualBorderSpacingHorizontal))
                {
                    MatchCollection matches = RegexParserUtils.Match(RegexParserUtils.CssLength, BorderSpacing);

                    if (matches.Count == 0)
                    {
                        actualBorderSpacingHorizontal = 0;
                    }
                    else if (matches.Count > 0)
                    {
                        actualBorderSpacingHorizontal = CssValueParser.ParseLength(matches[0].Value, 1, this);
                    }
                }


                return actualBorderSpacingHorizontal;
            }
        }

        /// <summary>
        /// Gets the actual vertical border spacing for tables
        /// </summary>
        public double ActualBorderSpacingVertical
        {
            get
            {
                if (double.IsNaN(actualBorderSpacingVertical))
                {
                    MatchCollection matches = RegexParserUtils.Match(RegexParserUtils.CssLength, BorderSpacing);

                    if (matches.Count == 0)
                    {
                        actualBorderSpacingVertical = 0;
                    }
                    else if (matches.Count == 1)
                    {
                        actualBorderSpacingVertical = CssValueParser.ParseLength(matches[0].Value, 1, this);
                    }
                    else
                    {
                        actualBorderSpacingVertical = CssValueParser.ParseLength(matches[1].Value, 1, this);
                    }
                }
                return actualBorderSpacingVertical;
            }
        }

        /// <summary>
        /// Get the parent of this css properties instance.
        /// </summary>
        /// <returns></returns>
        protected abstract CssBoxProperties GetParent();

        /// <summary>
        /// Gets the height of the font in the specified units
        /// </summary>
        /// <returns></returns>
        public double GetEmHeight()
        {
            return ActualFont.Height;
        }

        /// <summary>
        /// Ensures that the specified length is converted to pixels if necessary
        /// </summary>
        /// <param name="length"></param>
        protected string NoEms(string length)
        {
            var len = new CssLength(length);
            if (len.Unit == CssUnit.Ems)
            {
                length = len.ConvertEmToPixels(GetEmHeight()).ToString();
            }
            return length;
        }

        /// <summary>
        /// Set the style/width/color for all 4 borders on the box.<br/>
        /// if null is given for a value it will not be set.
        /// </summary>
        /// <param name="style">optional: the style to set</param>
        /// <param name="width">optional: the width to set</param>
        /// <param name="color">optional: the color to set</param>
        protected void SetAllBorders(string style = null, string width = null, string color = null)
        {
            if (style != null)
                BorderLeftStyle = BorderTopStyle = BorderRightStyle = BorderBottomStyle = style;
            if (width != null)
                BorderLeftWidth = BorderTopWidth = BorderRightWidth = BorderBottomWidth = width;
            if (color != null)
                BorderLeftColor = BorderTopColor = BorderRightColor = BorderBottomColor = color;
        }

        /// <summary>
        /// Measures the width of whitespace between words (set <see cref="ActualWordSpacing"/>).
        /// </summary>
        protected void MeasureWordSpacing(RGraphics g)
        {
            if (double.IsNaN(ActualWordSpacing))
            {
                actualWordSpacing = CssUtils.WhiteSpace(g, this);
                if (WordSpacing != CssConstants.Normal)
                {
                    string len = RegexParserUtils.Search(RegexParserUtils.CssLength, WordSpacing);
                    actualWordSpacing += CssValueParser.ParseLength(len, 1, this);
                }
            }
        }

        /// <summary>
        /// Inherits inheritable values from specified box.
        /// </summary>
        /// <param name="everything">Set to true to inherit all CSS properties instead of only the ineritables</param>
        /// <param name="p">Box to inherit the properties</param>
        protected void InheritStyle(CssBox p, bool everything)
        {
            if (p != null)
            {
                borderSpacing = p.borderSpacing;
                borderCollapse = p.borderCollapse;
                color = p.color;
                emptyCells = p.emptyCells;
                whiteSpace = p.whiteSpace;
                visibility = p.visibility;
                textIndent = p.textIndent;
                textAlign = p.textAlign;
                verticalAlign = p.verticalAlign;
                fontFamily = p.fontFamily;
                fontSize = p.fontSize;
                fontStyle = p.fontStyle;
                fontVariant = p.fontVariant;
                fontWeight = p.fontWeight;
                listStyleImage = p.listStyleImage;
                listStylePosition = p.listStylePosition;
                listStyleType = p.listStyleType;
                listStyle = p.listStyle;
                lineHeight = p.lineHeight;
                wordBreak = p.WordBreak;
                direction = p.direction;

                if (everything)
                {
                    backgroundColor = p.backgroundColor;
                    backgroundGradient = p.backgroundGradient;
                    backgroundGradientAngle = p.backgroundGradientAngle;
                    backgroundImage = p.backgroundImage;
                    backgroundPosition = p.backgroundPosition;
                    backgroundRepeat = p.backgroundRepeat;
                    borderTopWidth = p.borderTopWidth;
                    borderRightWidth = p.borderRightWidth;
                    borderBottomWidth = p.borderBottomWidth;
                    borderLeftWidth = p.borderLeftWidth;
                    borderTopColor = p.borderTopColor;
                    borderRightColor = p.borderRightColor;
                    borderBottomColor = p.borderBottomColor;
                    borderLeftColor = p.borderLeftColor;
                    borderTopStyle = p.borderTopStyle;
                    borderRightStyle = p.borderRightStyle;
                    borderBottomStyle = p.borderBottomStyle;
                    borderLeftStyle = p.borderLeftStyle;
                    bottom = p.bottom;
                    cornerNwRadius = p.cornerNwRadius;
                    cornerNeRadius = p.cornerNeRadius;
                    cornerSeRadius = p.cornerSeRadius;
                    cornerSwRadius = p.cornerSwRadius;
                    cornerRadius = p.cornerRadius;
                    display = p.display;
                    @float = p.@float;
                    height = p.height;
                    marginBottom = p.marginBottom;
                    marginLeft = p.marginLeft;
                    marginRight = p.marginRight;
                    marginTop = p.marginTop;
                    left = p.left;
                    lineHeight = p.lineHeight;
                    overflow = p.overflow;
                    paddingLeft = p.paddingLeft;
                    paddingBottom = p.paddingBottom;
                    paddingRight = p.paddingRight;
                    paddingTop = p.paddingTop;
                    right = p.right;
                    textDecoration = p.textDecoration;
                    top = p.top;
                    position = p.position;
                    width = p.width;
                    maxWidth = p.maxWidth;
                    wordSpacing = p.wordSpacing;
                }
            }
        }
    }
}