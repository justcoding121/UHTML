using System;
using System.Globalization;
using UHtml.Core.Parse;
using UHtml.Core.Utils;

namespace UHtml.Core.Dom
{
    /// <summary>
    /// Represents and gets info about a CSS Length
    /// </summary>
    /// <remarks>
    /// http://www.w3.org/TR/CSS21/syndata.html#length-units
    /// </remarks>
    internal sealed class CssLength
    {
        #region Fields

        private readonly double number;
        private readonly bool isRelative;
        private readonly CssUnit unit;
        private readonly string length;
        private readonly bool isPercentage;
        private readonly bool hasError;

        #endregion


        /// <summary>
        /// Creates a new CssLength from a length specified on a CSS style sheet or fragment
        /// </summary>
        /// <param name="length">Length as specified in the Style Sheet or style fragment</param>
        public CssLength(string length)
        {
            this.length = length;
            this.number = 0f;
            unit = CssUnit.None;
            isPercentage = false;

            //Return zero if no length specified, zero specified
            if (string.IsNullOrEmpty(length) || length == "0")
                return;

            //If percentage, use ParseNumber
            if (length.EndsWith("%"))
            {
                this.number = CssValueParser.ParseNumber(length, 1);
                isPercentage = true;
                return;
            }

            //If no units, has error
            if (length.Length < 3)
            {
                double.TryParse(length, out this.number);
                hasError = true;
                return;
            }

            //Get units of the length
            string u = length.Substring(length.Length - 2, 2);

            //Number of the length
            string number = length.Substring(0, length.Length - 2);

            //TODO: Units behave different in paper and in screen!
            switch (u)
            {
                case CssConstants.Em:
                    unit = CssUnit.Ems;
                    isRelative = true;
                    break;
                case CssConstants.Ex:
                    unit = CssUnit.Ex;
                    isRelative = true;
                    break;
                case CssConstants.Px:
                    unit = CssUnit.Pixels;
                    isRelative = true;
                    break;
                case CssConstants.Mm:
                    unit = CssUnit.Milimeters;
                    break;
                case CssConstants.Cm:
                    unit = CssUnit.Centimeters;
                    break;
                case CssConstants.In:
                    unit = CssUnit.Inches;
                    break;
                case CssConstants.Pt:
                    unit = CssUnit.Points;
                    break;
                case CssConstants.Pc:
                    unit = CssUnit.Picas;
                    break;
                default:
                    hasError = true;
                    return;
            }

            if (!double.TryParse(number, NumberStyles.Number, NumberFormatInfo.InvariantInfo, out this.number))
            {
                hasError = true;
            }
        }


        #region Props

        /// <summary>
        /// Gets the number in the length
        /// </summary>
        public double Number
        {
            get { return number; }
        }

        /// <summary>
        /// Gets if the length has some parsing error
        /// </summary>
        public bool HasError
        {
            get { return hasError; }
        }


        /// <summary>
        /// Gets if the length represents a precentage (not actually a length)
        /// </summary>
        public bool IsPercentage
        {
            get { return isPercentage; }
        }


        /// <summary>
        /// Gets if the length is specified in relative units
        /// </summary>
        public bool IsRelative
        {
            get { return isRelative; }
        }

        /// <summary>
        /// Gets the unit of the length
        /// </summary>
        public CssUnit Unit
        {
            get { return unit; }
        }

        /// <summary>
        /// Gets the length as specified in the string
        /// </summary>
        public string Length
        {
            get { return length; }
        }

        #endregion


        #region Methods

        /// <summary>
        /// If length is in Ems, returns its value in points
        /// </summary>
        /// <param name="emSize">Em size factor to multiply</param>
        /// <returns>Points size of this em</returns>
        /// <exception cref="InvalidOperationException">If length has an error or isn't in ems</exception>
        public CssLength ConvertEmToPoints(double emSize)
        {
            if (HasError)
                throw new InvalidOperationException("Invalid length");
            if (Unit != CssUnit.Ems)
                throw new InvalidOperationException("Length is not in ems");

            return new CssLength(string.Format("{0}pt", Convert.ToSingle(Number * emSize).ToString("0.0", NumberFormatInfo.InvariantInfo)));
        }

        /// <summary>
        /// If length is in Ems, returns its value in pixels
        /// </summary>
        /// <param name="pixelFactor">Pixel size factor to multiply</param>
        /// <returns>Pixels size of this em</returns>
        /// <exception cref="InvalidOperationException">If length has an error or isn't in ems</exception>
        public CssLength ConvertEmToPixels(double pixelFactor)
        {
            if (HasError)
                throw new InvalidOperationException("Invalid length");
            if (Unit != CssUnit.Ems)
                throw new InvalidOperationException("Length is not in ems");

            return new CssLength(string.Format("{0}px", Convert.ToSingle(Number * pixelFactor).ToString("0.0", NumberFormatInfo.InvariantInfo)));
        }

        /// <summary>
        /// Returns the length formatted ready for CSS interpreting.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            if (HasError)
            {
                return string.Empty;
            }
            else if (IsPercentage)
            {
                return string.Format(NumberFormatInfo.InvariantInfo, "{0}%", Number);
            }
            else
            {
                string u = string.Empty;

                switch (Unit)
                {
                    case CssUnit.None:
                        break;
                    case CssUnit.Ems:
                        u = "em";
                        break;
                    case CssUnit.Pixels:
                        u = "px";
                        break;
                    case CssUnit.Ex:
                        u = "ex";
                        break;
                    case CssUnit.Inches:
                        u = "in";
                        break;
                    case CssUnit.Centimeters:
                        u = "cm";
                        break;
                    case CssUnit.Milimeters:
                        u = "mm";
                        break;
                    case CssUnit.Points:
                        u = "pt";
                        break;
                    case CssUnit.Picas:
                        u = "pc";
                        break;
                }

                return string.Format(NumberFormatInfo.InvariantInfo, "{0}{1}", Number, u);
            }
        }

        #endregion
    }
}