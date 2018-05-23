using Newtonsoft.Json;
using System.Collections.Generic;
using UHtml.Core.Utils;

namespace UHtml.Core.Dom
{
    [JsonObject(MemberSerialization.OptIn)]
    internal sealed class HtmlTag
    {
        #region Fields and Consts

        /// <summary>
        /// the name of the html tag
        /// </summary>
        private readonly string name;

        /// <summary>
        /// if the tag is single placed; in other words it doesn't have a separate closing tag;
        /// </summary>
        private readonly bool isSingle;

        /// <summary>
        /// collection of attributes and their value the html tag has
        /// </summary>
        private readonly Dictionary<string, string> attributes;

        #endregion

        /// <summary>
        /// Init.
        /// </summary>
        /// <param name="name">the name of the html tag</param>
        /// <param name="isSingle">if the tag is single placed; in other words it doesn't have a separate closing tag;</param>
        /// <param name="attributes">collection of attributes and their value the html tag has</param>
        public HtmlTag(string name, bool isSingle, Dictionary<string, string> attributes = null)
        {
            ArgChecker.AssertArgNotNullOrEmpty(name, "name");

            this.name = name;
            this.isSingle = isSingle;
            this.attributes = attributes;
        }

        /// <summary>
        /// Gets the name of this tag
        /// </summary>
        [JsonProperty]
        public string Name
        {
            get { return name; }
        }

        /// <summary>
        /// Gets collection of attributes and their value the html tag has
        /// </summary>
        public Dictionary<string, string> Attributes
        {
            get { return attributes; }
        }

        /// <summary>
        /// Gets if the tag is single placed; in other words it doesn't have a separate closing tag; <br/>
        /// e.g. &lt;br&gt;
        /// </summary>
        public bool IsSingle
        {
            get { return isSingle; }
        }

        /// <summary>
        /// is the html tag has attributes.
        /// </summary>
        /// <returns>true - has attributes, false - otherwise</returns>
        public bool HasAttributes()
        {
            return attributes != null && attributes.Count > 0;
        }

        /// <summary>
        /// Gets a boolean indicating if the attribute list has the specified attribute
        /// </summary>
        /// <param name="attribute">attribute name to check if exists</param>
        /// <returns>true - attribute exists, false - otherwise</returns>
        public bool HasAttribute(string attribute)
        {
            return attributes != null && attributes.ContainsKey(attribute);
        }

        /// <summary>
        /// Get attribute value for given attribute name or null if not exists.
        /// </summary>
        /// <param name="attribute">attribute name to get by</param>
        /// <param name="defaultValue">optional: value to return if attribute is not specified</param>
        /// <returns>attribute value or null if not found</returns>
        public string TryGetAttribute(string attribute, string defaultValue = null)
        {
            return attributes != null && attributes.ContainsKey(attribute) ? attributes[attribute] : defaultValue;
        }

        public override string ToString()
        {
            return string.Format("<{0}>", name);
        }
    }
}