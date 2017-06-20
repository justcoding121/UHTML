







// 



namespace UHtml.Demo.Common
{
    /// <summary>
    /// Used to hold a single html sample with its name.
    /// </summary>
    public sealed class HtmlSample
    {
        private readonly string _name;
        private readonly string _fullName;
        private readonly string _html;

        /// <summary>
        /// Initializes a new instance of the <see cref="T:System.Object"/> class.
        /// </summary>
        public HtmlSample(string name, string category, string fullName, string html)
        {
            _name = name;
            _fullName = fullName;
            _html = html;
            Category = category;
        }

        public string Name
        {
            get { return _name; }
        }

        public string FullName
        {
            get { return _fullName; }
        }

        public string Html
        {
            get { return _html; }
        }

        public string Category { get; set; }
    }
}