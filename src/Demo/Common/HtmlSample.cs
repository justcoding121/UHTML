namespace UHtml.Demo.Common
{
    /// <summary>
    /// Used to hold a single html sample with its name.
    /// </summary>
    public sealed class HtmlSample
    {
        private readonly string name;
        private readonly string fullName;
        private readonly string html;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public HtmlSample(string name, string category, string fullName, string html)
        {
            this.name = name;
            this.fullName = fullName;
            this.html = html;
            Category = category;
        }

        public string Name
        {
            get { return name; }
        }

        public string FullName
        {
            get { return fullName; }
        }

        public string Html
        {
            get { return html; }
        }

        public string Category { get; set; }
    }
}