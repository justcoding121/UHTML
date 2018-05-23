namespace UHtml.Core.Entities
{
    /// <summary>
    /// Holds data on link element in HTML.<br/>
    /// Used to expose data outside of HTML Renderer internal structure.
    /// </summary>
    public sealed class LinkElementData<T>
    {
        /// <summary>
        /// the id of the link element if present
        /// </summary>
        private readonly string id;

        /// <summary>
        /// the href data of the link
        /// </summary>
        private readonly string href;

        /// <summary>
        /// the rectangle of element as calculated by html layout
        /// </summary>
        private readonly T rectangle;

        /// <summary>
        /// Init.
        /// </summary>
        public LinkElementData(string id, string href, T rectangle)
        {
            this.id = id;
            this.href = href;
            this.rectangle = rectangle;
        }

        /// <summary>
        /// the id of the link element if present
        /// </summary>
        public string Id
        {
            get { return id; }
        }

        /// <summary>
        /// the href data of the link
        /// </summary>
        public string Href
        {
            get { return href; }
        }

        /// <summary>
        /// the rectangle of element as calculated by html layout
        /// </summary>
        public T Rectangle
        {
            get { return rectangle; }
        }

        /// <summary>
        /// Is the link is directed to another element in the html
        /// </summary>
        public bool IsAnchor
        {
            get { return href.Length > 0 && href[0] == '#'; }
        }

        /// <summary>
        /// Return the id of the element this anchor link is referencing.
        /// </summary>
        public string AnchorId
        {
            get { return IsAnchor && href.Length > 1 ? href.Substring(1) : string.Empty; }
        }

        public override string ToString()
        {
            return string.Format("Id: {0}, Href: {1}, Rectangle: {2}", id, href, rectangle);
        }
    }
}