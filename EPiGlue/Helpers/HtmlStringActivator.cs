using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Perks;

namespace EPiGlue.Helpers
{
    internal static class HtmlStringActivator
    {
        public static IHtmlString CreateInstance(Type type, string value)
        {
            if (type.IsNot<IHtmlString>())
            {
                return null;
            }

            if (type == typeof(IHtmlString))
            {
                return new HtmlString(value);
            }

            if (type.IsAbstract || type.IsInterface)
            {
                return null; // Can't create instance
            }

            return (IHtmlString) Activator.CreateInstance(type, new object[] {value});
        }
    }
}
