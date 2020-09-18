using System.Xml.Linq;

namespace aemarcoCommons.Extensions.TextExtensions
{
    public static class FormattingExtensions
    {
        public static string PrettifyXml(this string xml)
        {
            return XElement.Parse(xml).ToString();
        }




    }
}
