







// 



using System;
using System.Diagnostics;
using System.IO;
using UHtml.Core.Entities;

namespace UHtml.Demo.Common
{
    public class DemoUtils
    {
        private const int Iterations = 20;

        /// <summary>
        /// The HTML text used in sample form for HtmlLabel.
        /// </summary>
        public static String SampleHtmlLabelText
        {
            get
            {
                return "This is an <b>HtmlLabel</b> on transparent background with <span style=\"color: red\">colors</span> and links: " +
                       "<a href=\"http://UHtml.codeplex.com/\">HTML Renderer</a>";
            }
        }

        /// <summary>
        /// The HTML text used in sample form for HtmlPanel.
        /// </summary>
        public static String SampleHtmlPanelText
        {
            get
            {
                return "This is an <b>HtmlPanel</b> with <span style=\"color: red\">colors</span> and links: <a href=\"http://UHtml.codeplex.com/\">HTML Renderer</a>" +
                       "<div style=\"font-size: 1.2em; padding-top: 10px;\" >If there is more text than the size of the control scrollbars will appear.</div>" +
                       "<br/>Click me to change my <code>Text</code> property.";
            }
        }

        /// <summary>
        /// Handle stylesheet resolve.
        /// </summary>
        public static void OnStylesheetLoad(object sender, HtmlStylesheetLoadEventArgs e)
        {
            var stylesheet = GetStylesheet(e.Src);
            if (stylesheet != null)
                e.SetStyleSheet = stylesheet;
        }

        /// <summary>
        /// Get stylesheet by given key.
        /// </summary>
        public static string GetStylesheet(string src)
        {
            if (src == "StyleSheet")
            {
                return @"h1, h2, h3 { color: navy; font-weight:normal; }
                    h1 { margin-bottom: .47em }
                    h2 { margin-bottom: .3em }
                    h3 { margin-bottom: .4em }
                    ul { margin-top: .5em }
                    ul li {margin: .25em}
                    body { font:10pt Tahoma }
		            pre  { border:solid 1px gray; background-color:#eee; padding:1em }
                    a:link { text-decoration: none; }
                    a:hover { text-decoration: underline; }
                    .gray    { color:gray; }
                    .example { background-color:#efefef; corner-radius:5px; padding:0.5em; }
                    .whitehole { background-color:white; corner-radius:10px; padding:15px; }
                    .caption { font-size: 1.1em }
                    .comment { color: green; margin-bottom: 5px; margin-left: 3px; }
                    .comment2 { color: green; }";
            }
            return null;
        }

        /// <summary>
        /// Get image by resource key.
        /// </summary>
        public static Stream GetImageStream(string src)
        {
            switch (src.ToLower())
            {
                case "htmlicon":
                    return Resources.Html32;
                case "staricon":
                    return Resources.Favorites32;
                case "fonticon":
                    return Resources.Font32;
                case "commenticon":
                    return Resources.Comment16;
                case "imageicon":
                    return Resources.Image32;
                case "methodicon":
                    return Resources.Method16;
                case "propertyicon":
                    return Resources.Property16;
                case "eventicon":
                    return Resources.Event16;
            }
            return null;
        }
     
    }
}