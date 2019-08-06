using System.Runtime.InteropServices;

using RGiesecke.DllExport;

namespace HtmlFauxmat
{
    public static class Exports
    {
        [DllExport(CallingConvention = CallingConvention.Cdecl)]
        public static void CF_HTML(string html)
        {
            ClipboardWriter.RawHtmlToClipboard(html);
        }
    }
}
