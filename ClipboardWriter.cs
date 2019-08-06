using System;
using System.Windows;
using System.Text;

namespace HtmlFauxmat
{
    /// <summary>
    /// Sliced out of http://blogs.msdn.com/jmstall/archive/2007/01/21/html-clipboard.aspx
    /// </summary>
    public static class ClipboardWriter
    {
        // The string contains index references to other spots in the string, so we need placeholders so we can compute the offsets.
        // The <<<<<<<_ strings are just placeholders. We'll backpatch them actual values afterwards.
        // The string layout (<<<) also ensures that it can't appear in the body of the html because the <
        // character must be escaped.
        const string CLIPBOARD_HEADER =
@"Format:HTML Format
Version:1.0
StartHTML:<<<<<<<1
EndHTML:<<<<<<<2
StartFragment:<<<<<<<3
EndFragment:<<<<<<<4
StartSelection:<<<<<<<3
EndSelection:<<<<<<<3
";
        const string HTML_START_FRAGMENT = "<html><body><!--StartFragment-->";
        const string HTML_END_FRAGMENT = @"<!--EndFragment--></body></html>";

        const string TO_8_DIGITS = "{0,8}";
        
        /// <summary>
        /// Clears clipboard and copy a HTML fragment to the clipboard, providing additional meta-information.
        /// </summary>
        /// <param name="htmlFragment">a html fragment</param>
        /// <param name="sourceUrl">optional Source URL of the HTML document, for resolving relative links (can be null)</param>
        public static void RawHtmlToClipboard(string htmlFragment, Uri sourceUrl = null)
        {
            var sb = new StringBuilder();

            // Build the CF_HTML header. See format specification here:
            // https://docs.microsoft.com/en-us/windows/win32/dataxchg/html-clipboard-format

            sb.Append(CLIPBOARD_HEADER);

            if (sourceUrl != null)
            {
                sb.Append($"SourceURL:{sourceUrl}");
            }

            int startHTML = sb.Length;

            sb.Append(HTML_START_FRAGMENT);
            int fragmentStart = sb.Length;

            sb.Append(htmlFragment);
            int fragmentEnd = sb.Length;

            sb.Append(HTML_END_FRAGMENT);
            int endHTML = sb.Length;

            // Backpatch offsets
            sb.Replace("<<<<<<<1", String.Format(TO_8_DIGITS, startHTML));
            sb.Replace("<<<<<<<2", String.Format(TO_8_DIGITS, endHTML));
            sb.Replace("<<<<<<<3", String.Format(TO_8_DIGITS, fragmentStart));
            sb.Replace("<<<<<<<4", String.Format(TO_8_DIGITS, fragmentEnd));

            // Finally copy to clipboard.
            string data = sb.ToString();
            Clipboard.Clear();
            Clipboard.SetText(data, TextDataFormat.Html);
        }
    }
}
