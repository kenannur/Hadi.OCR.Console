namespace Ocr.Server.ConsoleApp.Extension
{
    public static class StringExtensions
    {
        static readonly string[] oldValues = new string[] 
        {
            "\n", "&nbsp;", ".", ",", "?", ":", ";", "-", "&middot;", "(", ")", "&#39;", "&quot;", "&#39", "|", "&quot", "&nbsp"
        };

        internal static string CleanString(this string str)
        {
            foreach (var oldValue in oldValues)
            {
                str = str.Replace(oldValue, " ");
            }
            return str;
        }
    }
}
