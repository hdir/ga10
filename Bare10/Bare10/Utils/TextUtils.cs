using System;
using System.Text.RegularExpressions;
using Xamarin.Forms;

namespace Bare10.Utils
{
    public class TextUtils
    {
        public const string REGEX_URL = @"\b(?:https?://|www\.)\S+\b";

        public static FormattedString ParseHtmlAnchors(string text, Color linkColor)
        {
            var formatted = new FormattedString();

            SplitUrlSpans(text, linkColor, ref formatted);

            return formatted;
        }

        private static void SplitUrlSpans(string text, Color linkColor, ref FormattedString fs)
        {
            while (true)
            {
                var match = Regex.Match(text, REGEX_URL);

                if (match.Success)
                {
                    var i = text.IndexOf(match.Value);

                    var url = match.Value;
                    var pre = text.Substring(0, i);
                    var post = text.Substring(i + url.Length, text.Length - (i + url.Length));

                    var spanPre = new Span() { Text = pre, };

                    var tgr = new TapGestureRecognizer();
                    tgr.Tapped += (sender, args) => { Device.OpenUri(new Uri(url)); };

                    var spanLink = new Span()
                    {
                        Text = url,
                        TextDecorations = TextDecorations.Underline,
                        TextColor = linkColor,
                        GestureRecognizers = { tgr }
                    };

                    fs.Spans.Add(spanPre);
                    fs.Spans.Add(spanLink);

                    text = post;
                    continue;
                }

                fs.Spans.Add(new Span() { Text = text, });
                break;
            }
        }
    }
}
