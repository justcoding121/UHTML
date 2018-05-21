using System.Windows.Markup;
using System.Windows.Media;
using UHtml.Adapters;

namespace UHtml.WPF.Adapters
{
    /// <summary>
    /// Adapter for WPF Font family object for core.
    /// </summary>
    internal sealed class FontFamilyAdapter : RFontFamily
    {
        /// <summary>
        /// Default language to get font family name by
        /// </summary>
        private static readonly XmlLanguage xmlLanguage = XmlLanguage.GetLanguage("en-us");

        /// <summary>
        /// the underline win-forms font.
        /// </summary>
        private readonly FontFamily fontFamily;

        /// <summary>
        /// Init.
        /// </summary>
        public FontFamilyAdapter(FontFamily fontFamily)
        {
            this.fontFamily = fontFamily;
        }

        /// <summary>
        /// the underline WPF font family.
        /// </summary>
        public FontFamily FontFamily
        {
            get { return fontFamily; }
        }

        public override string Name
        {
            get
            {
                string name =  fontFamily.FamilyNames[xmlLanguage];
                if (string.IsNullOrEmpty(name))
                {
                    foreach (var familyName in fontFamily.FamilyNames)
                    {
                        return familyName.Value;
                    }
                }
                return name;
            }
        }
    }
}