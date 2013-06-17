using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Perks;

namespace EPiGlue
{
    internal static class StringExtensions
    {
        public static string ToHtml(this IHtmlString htmlString)
        {
            return htmlString.IfNotNull(x => x.ToHtmlString());
        }

        public static IHtmlString ToHtml(this object value)
        {
            return new HtmlString(value.IfNotNull(x => x.ToString()));
        }
    }
}
