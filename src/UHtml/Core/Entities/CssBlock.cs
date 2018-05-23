using System.Collections.Generic;
using UHtml.Core.Utils;

namespace UHtml.Core.Entities
{
    /// <summary>
    /// Represents a block of CSS property values.<br/>
    /// Contains collection of key-value pairs that are CSS properties for specific css class.<br/>
    /// Css class can be either custom or html tag name.
    /// </summary>
    /// <remarks>
    /// To learn more about CSS blocks visit CSS spec: http://www.w3.org/TR/CSS21/syndata.html#block
    /// </remarks>
    public sealed class CssBlock
    {
        #region Fields and Consts

        /// <summary>
        /// the name of the css class of the block
        /// </summary>
        private readonly string @class;

        /// <summary>
        /// the CSS block properties and values
        /// </summary>
        private readonly Dictionary<string, string> properties;

        /// <summary>
        /// additional selectors to used in hierarchy (p className1 > className2)
        /// </summary>
        private readonly List<CssBlockSelectorItem> selectors;

        /// <summary>
        /// is the css block has :hover pseudo-class
        /// </summary>
        private readonly bool hover;

        #endregion

        /// <summary>
        /// Creates a new block from the block's source
        /// </summary>
        /// <param name="class">the name of the css class of the block</param>
        /// <param name="properties">the CSS block properties and values</param>
        /// <param name="selectors">optional: additional selectors to used in hierarchy</param>
        /// <param name="hover">optional: is the css block has :hover pseudo-class</param>
        public CssBlock(string @class, Dictionary<string, string> properties, List<CssBlockSelectorItem> selectors = null, bool hover = false)
        {
            ArgChecker.AssertArgNotNullOrEmpty(@class, "@class");
            ArgChecker.AssertArgNotNull(properties, "properties");

            this.@class = @class;
            this.selectors = selectors;
            this.properties = properties;
            this.hover = hover;
        }

        /// <summary>
        /// the name of the css class of the block
        /// </summary>
        public string Class
        {
            get { return @class; }
        }

        /// <summary>
        /// additional selectors to used in hierarchy (p className1 > className2)
        /// </summary>
        public List<CssBlockSelectorItem> Selectors
        {
            get { return selectors; }
        }

        /// <summary>
        /// Gets the CSS block properties and its values
        /// </summary>
        public IDictionary<string, string> Properties
        {
            get { return properties; }
        }

        /// <summary>
        /// is the css block has :hover pseudo-class
        /// </summary>
        public bool Hover
        {
            get { return hover; }
        }

        /// <summary>
        /// Merge the other block properties into this css block.<br/>
        /// Other block properties can overwrite this block properties.
        /// </summary>
        /// <param name="other">the css block to merge with</param>
        public void Merge(CssBlock other)
        {
            ArgChecker.AssertArgNotNull(other, "other");

            foreach (var prop in other.properties.Keys)
            {
                properties[prop] = other.properties[prop];
            }
        }

        /// <summary>
        /// Create deep copy of the CssBlock.
        /// </summary>
        /// <returns>new CssBlock with same data</returns>
        public CssBlock Clone()
        {
            return new CssBlock(@class, new Dictionary<string, string>(properties), selectors != null ? new List<CssBlockSelectorItem>(selectors) : null);
        }

        /// <summary>
        /// Check if the two css blocks are the same (same class, selectors and properties).
        /// </summary>
        /// <param name="other">the other block to compare to</param>
        /// <returns>true - the two blocks are the same, false - otherwise</returns>
        public bool Equals(CssBlock other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;
            if (!Equals(other.@class, @class))
                return false;

            if (!Equals(other.properties.Count, properties.Count))
                return false;

            foreach (var property in properties)
            {
                if (!other.properties.ContainsKey(property.Key))
                    return false;
                if (!Equals(other.properties[property.Key], property.Value))
                    return false;
            }

            if (!EqualsSelector(other))
                return false;

            return true;
        }

        /// <summary>
        /// Check if the selectors of the css blocks is the same.
        /// </summary>
        /// <param name="other">the other block to compare to</param>
        /// <returns>true - the selectors on blocks are the same, false - otherwise</returns>
        public bool EqualsSelector(CssBlock other)
        {
            if (ReferenceEquals(null, other))
                return false;
            if (ReferenceEquals(this, other))
                return true;

            if (other.Hover != Hover)
                return false;
            if (other.selectors == null && selectors != null)
                return false;
            if (other.selectors != null && selectors == null)
                return false;

            if (other.selectors != null && selectors != null)
            {
                if (!Equals(other.selectors.Count, selectors.Count))
                    return false;

                for (int i = 0; i < selectors.Count; i++)
                {
                    if (!Equals(other.selectors[i].Class, selectors[i].Class))
                        return false;
                    if (!Equals(other.selectors[i].DirectParent, selectors[i].DirectParent))
                        return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Check if the two css blocks are the same (same class, selectors and properties).
        /// </summary>
        /// <param name="obj">the other block to compare to</param>
        /// <returns>true - the two blocks are the same, false - otherwise</returns>
        public override bool Equals(object obj)
        {
            if (ReferenceEquals(null, obj))
                return false;
            if (ReferenceEquals(this, obj))
                return true;
            if (obj.GetType() != typeof(CssBlock))
                return false;
            return Equals((CssBlock)obj);
        }

        /// <summary>
        /// Serves as a hash function for a particular type. 
        /// </summary>
        /// <returns>A hash code for the current <see cref="T:System.Object"/>.</returns>
        public override int GetHashCode()
        {
            unchecked
            {
                return ((@class != null ? @class.GetHashCode() : 0) * 397) ^ (properties != null ? properties.GetHashCode() : 0);
            }
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        public override string ToString()
        {
            var str = @class + " { ";
            foreach (var property in properties)
            {
                str += string.Format("{0}={1}; ", property.Key, property.Value);
            }
            return str + " }";
        }
    }
}