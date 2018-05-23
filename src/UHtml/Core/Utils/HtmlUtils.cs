using System;
using System.Collections.Generic;

namespace UHtml.Core.Utils
{
    internal static class HtmlUtils
    {
        #region Fields and Consts

        /// <summary>
        /// List of html tags that don't have content
        /// </summary>
        private static readonly List<string> list = new List<string>(
            new[]
            {
                "area", "base", "basefont", "br", "col",
                "frame", "hr", "img", "input", "isindex",
                "link", "meta", "param"
            }
            );

        /// <summary>
        /// the html encode\decode pairs
        /// </summary>
        private static readonly KeyValuePair<string, string>[] encodeDecode = new[]
        {
            new KeyValuePair<string, string>("&lt;", "<"),
            new KeyValuePair<string, string>("&gt;", ">"),
            new KeyValuePair<string, string>("&quot;", "\""),
            new KeyValuePair<string, string>("&amp;", "&"),
        };

        /// <summary>
        /// the html decode only pairs
        /// </summary>
        private static readonly Dictionary<string, char> decodeOnly = new Dictionary<string, char>(StringComparer.OrdinalIgnoreCase);

        #endregion


        /// <summary>
        /// Init.
        /// </summary>
        static HtmlUtils()
        {
            decodeOnly["nbsp"] = ' ';
            decodeOnly["rdquo"] = '"';
            decodeOnly["lsquo"] = '\'';
            decodeOnly["apos"] = '\'';

            // ISO 8859-1 Symbols
            decodeOnly["iexcl"] = Convert.ToChar(161);
            decodeOnly["cent"] = Convert.ToChar(162);
            decodeOnly["pound"] = Convert.ToChar(163);
            decodeOnly["curren"] = Convert.ToChar(164);
            decodeOnly["yen"] = Convert.ToChar(165);
            decodeOnly["brvbar"] = Convert.ToChar(166);
            decodeOnly["sect"] = Convert.ToChar(167);
            decodeOnly["uml"] = Convert.ToChar(168);
            decodeOnly["copy"] = Convert.ToChar(169);
            decodeOnly["ordf"] = Convert.ToChar(170);
            decodeOnly["laquo"] = Convert.ToChar(171);
            decodeOnly["not"] = Convert.ToChar(172);
            decodeOnly["shy"] = Convert.ToChar(173);
            decodeOnly["reg"] = Convert.ToChar(174);
            decodeOnly["macr"] = Convert.ToChar(175);
            decodeOnly["deg"] = Convert.ToChar(176);
            decodeOnly["plusmn"] = Convert.ToChar(177);
            decodeOnly["sup2"] = Convert.ToChar(178);
            decodeOnly["sup3"] = Convert.ToChar(179);
            decodeOnly["acute"] = Convert.ToChar(180);
            decodeOnly["micro"] = Convert.ToChar(181);
            decodeOnly["para"] = Convert.ToChar(182);
            decodeOnly["middot"] = Convert.ToChar(183);
            decodeOnly["cedil"] = Convert.ToChar(184);
            decodeOnly["sup1"] = Convert.ToChar(185);
            decodeOnly["ordm"] = Convert.ToChar(186);
            decodeOnly["raquo"] = Convert.ToChar(187);
            decodeOnly["frac14"] = Convert.ToChar(188);
            decodeOnly["frac12"] = Convert.ToChar(189);
            decodeOnly["frac34"] = Convert.ToChar(190);
            decodeOnly["iquest"] = Convert.ToChar(191);
            decodeOnly["times"] = Convert.ToChar(215);
            decodeOnly["divide"] = Convert.ToChar(247);

            // ISO 8859-1 Characters
            decodeOnly["Agrave"] = Convert.ToChar(192);
            decodeOnly["Aacute"] = Convert.ToChar(193);
            decodeOnly["Acirc"] = Convert.ToChar(194);
            decodeOnly["Atilde"] = Convert.ToChar(195);
            decodeOnly["Auml"] = Convert.ToChar(196);
            decodeOnly["Aring"] = Convert.ToChar(197);
            decodeOnly["AElig"] = Convert.ToChar(198);
            decodeOnly["Ccedil"] = Convert.ToChar(199);
            decodeOnly["Egrave"] = Convert.ToChar(200);
            decodeOnly["Eacute"] = Convert.ToChar(201);
            decodeOnly["Ecirc"] = Convert.ToChar(202);
            decodeOnly["Euml"] = Convert.ToChar(203);
            decodeOnly["Igrave"] = Convert.ToChar(204);
            decodeOnly["Iacute"] = Convert.ToChar(205);
            decodeOnly["Icirc"] = Convert.ToChar(206);
            decodeOnly["Iuml"] = Convert.ToChar(207);
            decodeOnly["ETH"] = Convert.ToChar(208);
            decodeOnly["Ntilde"] = Convert.ToChar(209);
            decodeOnly["Ograve"] = Convert.ToChar(210);
            decodeOnly["Oacute"] = Convert.ToChar(211);
            decodeOnly["Ocirc"] = Convert.ToChar(212);
            decodeOnly["Otilde"] = Convert.ToChar(213);
            decodeOnly["Ouml"] = Convert.ToChar(214);
            decodeOnly["Oslash"] = Convert.ToChar(216);
            decodeOnly["Ugrave"] = Convert.ToChar(217);
            decodeOnly["Uacute"] = Convert.ToChar(218);
            decodeOnly["Ucirc"] = Convert.ToChar(219);
            decodeOnly["Uuml"] = Convert.ToChar(220);
            decodeOnly["Yacute"] = Convert.ToChar(221);
            decodeOnly["THORN"] = Convert.ToChar(222);
            decodeOnly["szlig"] = Convert.ToChar(223);
            decodeOnly["agrave"] = Convert.ToChar(224);
            decodeOnly["aacute"] = Convert.ToChar(225);
            decodeOnly["acirc"] = Convert.ToChar(226);
            decodeOnly["atilde"] = Convert.ToChar(227);
            decodeOnly["auml"] = Convert.ToChar(228);
            decodeOnly["aring"] = Convert.ToChar(229);
            decodeOnly["aelig"] = Convert.ToChar(230);
            decodeOnly["ccedil"] = Convert.ToChar(231);
            decodeOnly["egrave"] = Convert.ToChar(232);
            decodeOnly["eacute"] = Convert.ToChar(233);
            decodeOnly["ecirc"] = Convert.ToChar(234);
            decodeOnly["euml"] = Convert.ToChar(235);
            decodeOnly["igrave"] = Convert.ToChar(236);
            decodeOnly["iacute"] = Convert.ToChar(237);
            decodeOnly["icirc"] = Convert.ToChar(238);
            decodeOnly["iuml"] = Convert.ToChar(239);
            decodeOnly["eth"] = Convert.ToChar(240);
            decodeOnly["ntilde"] = Convert.ToChar(241);
            decodeOnly["ograve"] = Convert.ToChar(242);
            decodeOnly["oacute"] = Convert.ToChar(243);
            decodeOnly["ocirc"] = Convert.ToChar(244);
            decodeOnly["otilde"] = Convert.ToChar(245);
            decodeOnly["ouml"] = Convert.ToChar(246);
            decodeOnly["oslash"] = Convert.ToChar(248);
            decodeOnly["ugrave"] = Convert.ToChar(249);
            decodeOnly["uacute"] = Convert.ToChar(250);
            decodeOnly["ucirc"] = Convert.ToChar(251);
            decodeOnly["uuml"] = Convert.ToChar(252);
            decodeOnly["yacute"] = Convert.ToChar(253);
            decodeOnly["thorn"] = Convert.ToChar(254);
            decodeOnly["yuml"] = Convert.ToChar(255);

            // Math Symbols Supported by HTML
            decodeOnly["forall"] = Convert.ToChar(8704);
            decodeOnly["part"] = Convert.ToChar(8706);
            decodeOnly["exist"] = Convert.ToChar(8707);
            decodeOnly["empty"] = Convert.ToChar(8709);
            decodeOnly["nabla"] = Convert.ToChar(8711);
            decodeOnly["isin"] = Convert.ToChar(8712);
            decodeOnly["notin"] = Convert.ToChar(8713);
            decodeOnly["ni"] = Convert.ToChar(8715);
            decodeOnly["prod"] = Convert.ToChar(8719);
            decodeOnly["sum"] = Convert.ToChar(8721);
            decodeOnly["minus"] = Convert.ToChar(8722);
            decodeOnly["lowast"] = Convert.ToChar(8727);
            decodeOnly["radic"] = Convert.ToChar(8730);
            decodeOnly["prop"] = Convert.ToChar(8733);
            decodeOnly["infin"] = Convert.ToChar(8734);
            decodeOnly["ang"] = Convert.ToChar(8736);
            decodeOnly["and"] = Convert.ToChar(8743);
            decodeOnly["or"] = Convert.ToChar(8744);
            decodeOnly["cap"] = Convert.ToChar(8745);
            decodeOnly["cup"] = Convert.ToChar(8746);
            decodeOnly["int"] = Convert.ToChar(8747);
            decodeOnly["there4"] = Convert.ToChar(8756);
            decodeOnly["sim"] = Convert.ToChar(8764);
            decodeOnly["cong"] = Convert.ToChar(8773);
            decodeOnly["asymp"] = Convert.ToChar(8776);
            decodeOnly["ne"] = Convert.ToChar(8800);
            decodeOnly["equiv"] = Convert.ToChar(8801);
            decodeOnly["le"] = Convert.ToChar(8804);
            decodeOnly["ge"] = Convert.ToChar(8805);
            decodeOnly["sub"] = Convert.ToChar(8834);
            decodeOnly["sup"] = Convert.ToChar(8835);
            decodeOnly["nsub"] = Convert.ToChar(8836);
            decodeOnly["sube"] = Convert.ToChar(8838);
            decodeOnly["supe"] = Convert.ToChar(8839);
            decodeOnly["oplus"] = Convert.ToChar(8853);
            decodeOnly["otimes"] = Convert.ToChar(8855);
            decodeOnly["perp"] = Convert.ToChar(8869);
            decodeOnly["sdot"] = Convert.ToChar(8901);

            // Greek Letters Supported by HTML
            decodeOnly["Alpha"] = Convert.ToChar(913);
            decodeOnly["Beta"] = Convert.ToChar(914);
            decodeOnly["Gamma"] = Convert.ToChar(915);
            decodeOnly["Delta"] = Convert.ToChar(916);
            decodeOnly["Epsilon"] = Convert.ToChar(917);
            decodeOnly["Zeta"] = Convert.ToChar(918);
            decodeOnly["Eta"] = Convert.ToChar(919);
            decodeOnly["Theta"] = Convert.ToChar(920);
            decodeOnly["Iota"] = Convert.ToChar(921);
            decodeOnly["Kappa"] = Convert.ToChar(922);
            decodeOnly["Lambda"] = Convert.ToChar(923);
            decodeOnly["Mu"] = Convert.ToChar(924);
            decodeOnly["Nu"] = Convert.ToChar(925);
            decodeOnly["Xi"] = Convert.ToChar(926);
            decodeOnly["Omicron"] = Convert.ToChar(927);
            decodeOnly["Pi"] = Convert.ToChar(928);
            decodeOnly["Rho"] = Convert.ToChar(929);
            decodeOnly["Sigma"] = Convert.ToChar(931);
            decodeOnly["Tau"] = Convert.ToChar(932);
            decodeOnly["Upsilon"] = Convert.ToChar(933);
            decodeOnly["Phi"] = Convert.ToChar(934);
            decodeOnly["Chi"] = Convert.ToChar(935);
            decodeOnly["Psi"] = Convert.ToChar(936);
            decodeOnly["Omega"] = Convert.ToChar(937);
            decodeOnly["alpha"] = Convert.ToChar(945);
            decodeOnly["beta"] = Convert.ToChar(946);
            decodeOnly["gamma"] = Convert.ToChar(947);
            decodeOnly["delta"] = Convert.ToChar(948);
            decodeOnly["epsilon"] = Convert.ToChar(949);
            decodeOnly["zeta"] = Convert.ToChar(950);
            decodeOnly["eta"] = Convert.ToChar(951);
            decodeOnly["theta"] = Convert.ToChar(952);
            decodeOnly["iota"] = Convert.ToChar(953);
            decodeOnly["kappa"] = Convert.ToChar(954);
            decodeOnly["lambda"] = Convert.ToChar(955);
            decodeOnly["mu"] = Convert.ToChar(956);
            decodeOnly["nu"] = Convert.ToChar(957);
            decodeOnly["xi"] = Convert.ToChar(958);
            decodeOnly["omicron"] = Convert.ToChar(959);
            decodeOnly["pi"] = Convert.ToChar(960);
            decodeOnly["rho"] = Convert.ToChar(961);
            decodeOnly["sigmaf"] = Convert.ToChar(962);
            decodeOnly["sigma"] = Convert.ToChar(963);
            decodeOnly["tau"] = Convert.ToChar(964);
            decodeOnly["upsilon"] = Convert.ToChar(965);
            decodeOnly["phi"] = Convert.ToChar(966);
            decodeOnly["chi"] = Convert.ToChar(967);
            decodeOnly["psi"] = Convert.ToChar(968);
            decodeOnly["omega"] = Convert.ToChar(969);
            decodeOnly["thetasym"] = Convert.ToChar(977);
            decodeOnly["upsih"] = Convert.ToChar(978);
            decodeOnly["piv"] = Convert.ToChar(982);

            // Other Entities Supported by HTML
            decodeOnly["OElig"] = Convert.ToChar(338);
            decodeOnly["oelig"] = Convert.ToChar(339);
            decodeOnly["Scaron"] = Convert.ToChar(352);
            decodeOnly["scaron"] = Convert.ToChar(353);
            decodeOnly["Yuml"] = Convert.ToChar(376);
            decodeOnly["fnof"] = Convert.ToChar(402);
            decodeOnly["circ"] = Convert.ToChar(710);
            decodeOnly["tilde"] = Convert.ToChar(732);
            decodeOnly["ndash"] = Convert.ToChar(8211);
            decodeOnly["mdash"] = Convert.ToChar(8212);
            decodeOnly["lsquo"] = Convert.ToChar(8216);
            decodeOnly["rsquo"] = Convert.ToChar(8217);
            decodeOnly["sbquo"] = Convert.ToChar(8218);
            decodeOnly["ldquo"] = Convert.ToChar(8220);
            decodeOnly["rdquo"] = Convert.ToChar(8221);
            decodeOnly["bdquo"] = Convert.ToChar(8222);
            decodeOnly["dagger"] = Convert.ToChar(8224);
            decodeOnly["Dagger"] = Convert.ToChar(8225);
            decodeOnly["bull"] = Convert.ToChar(8226);
            decodeOnly["hellip"] = Convert.ToChar(8230);
            decodeOnly["permil"] = Convert.ToChar(8240);
            decodeOnly["prime"] = Convert.ToChar(8242);
            decodeOnly["Prime"] = Convert.ToChar(8243);
            decodeOnly["lsaquo"] = Convert.ToChar(8249);
            decodeOnly["rsaquo"] = Convert.ToChar(8250);
            decodeOnly["oline"] = Convert.ToChar(8254);
            decodeOnly["euro"] = Convert.ToChar(8364);
            decodeOnly["trade"] = Convert.ToChar(153);
            decodeOnly["larr"] = Convert.ToChar(8592);
            decodeOnly["uarr"] = Convert.ToChar(8593);
            decodeOnly["rarr"] = Convert.ToChar(8594);
            decodeOnly["darr"] = Convert.ToChar(8595);
            decodeOnly["harr"] = Convert.ToChar(8596);
            decodeOnly["crarr"] = Convert.ToChar(8629);
            decodeOnly["lceil"] = Convert.ToChar(8968);
            decodeOnly["rceil"] = Convert.ToChar(8969);
            decodeOnly["lfloor"] = Convert.ToChar(8970);
            decodeOnly["rfloor"] = Convert.ToChar(8971);
            decodeOnly["loz"] = Convert.ToChar(9674);
            decodeOnly["spades"] = Convert.ToChar(9824);
            decodeOnly["clubs"] = Convert.ToChar(9827);
            decodeOnly["hearts"] = Convert.ToChar(9829);
            decodeOnly["diams"] = Convert.ToChar(9830);
        }

        /// <summary>
        /// Is the given html tag is single tag or can have content.
        /// </summary>
        /// <param name="tagName">the tag to check (must be lower case)</param>
        /// <returns>true - is single tag, false - otherwise</returns>
        public static bool IsSingleTag(string tagName)
        {
            return list.Contains(tagName);
        }

        /// <summary>
        /// Decode html encoded string to regular string.<br/>
        /// Handles &lt;, &gt;, "&amp;.
        /// </summary>
        /// <param name="str">the string to decode</param>
        /// <returns>decoded string</returns>
        public static string DecodeHtml(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                str = DecodeHtmlCharByCode(str);

                str = DecodeHtmlCharByName(str);

                foreach (var encPair in encodeDecode)
                {
                    str = str.Replace(encPair.Key, encPair.Value);
                }
            }
            return str;
        }

        /// <summary>
        /// Encode regular string into html encoded string.<br/>
        /// Handles &lt;, &gt;, "&amp;.
        /// </summary>
        /// <param name="str">the string to encode</param>
        /// <returns>encoded string</returns>
        public static string EncodeHtml(string str)
        {
            if (!string.IsNullOrEmpty(str))
            {
                for (int i = encodeDecode.Length - 1; i >= 0; i--)
                {
                    str = str.Replace(encodeDecode[i].Value, encodeDecode[i].Key);
                }
            }
            return str;
        }


        #region Private methods

        /// <summary>
        /// Decode html special charecters encoded using char entity code (&#8364;)
        /// </summary>
        /// <param name="str">the string to decode</param>
        /// <returns>decoded string</returns>
        private static string DecodeHtmlCharByCode(string str)
        {
            var idx = str.IndexOf("&#", StringComparison.OrdinalIgnoreCase);
            while (idx > -1)
            {
                bool hex = str.Length > idx + 3 && char.ToLower(str[idx + 2]) == 'x';
                var endIdx = idx + 2 + (hex ? 1 : 0);

                long num = 0;
                while (endIdx < str.Length && CommonUtils.IsDigit(str[endIdx], hex))
                    num = num * (hex ? 16 : 10) + CommonUtils.ToDigit(str[endIdx++], hex);
                endIdx += (endIdx < str.Length && str[endIdx] == ';') ? 1 : 0;

                str = str.Remove(idx, endIdx - idx);
                str = str.Insert(idx, Convert.ToChar(num).ToString());

                idx = str.IndexOf("&#", idx + 1);
            }
            return str;
        }

        /// <summary>
        /// Decode html special charecters encoded using char entity name (&#euro;)
        /// </summary>
        /// <param name="str">the string to decode</param>
        /// <returns>decoded string</returns>
        private static string DecodeHtmlCharByName(string str)
        {
            var idx = str.IndexOf('&');
            while (idx > -1)
            {
                var endIdx = str.IndexOf(';', idx);
                if (endIdx > -1 && endIdx - idx < 8)
                {
                    var key = str.Substring(idx + 1, endIdx - idx - 1);
                    char c;
                    if (decodeOnly.TryGetValue(key, out c))
                    {
                        str = str.Remove(idx, endIdx - idx + 1);
                        str = str.Insert(idx, c.ToString());
                    }
                }

                idx = str.IndexOf('&', idx + 1);
            }
            return str;
        }

        #endregion
    }
}