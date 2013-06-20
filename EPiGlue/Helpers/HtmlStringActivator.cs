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
        public static IHtmlString CreateInstance(PropertyInfo property, string value)
        {
            if (property.PropertyType.IsNot<IHtmlString>())
            {
                return null;
            }

            if (property.PropertyType == typeof(IHtmlString))
            {
                return new HtmlString(value);
            }

            if (property.PropertyType.IsAbstract || property.PropertyType.IsInterface)
            {
                return null; // Can't create instance
            }

            return (IHtmlString) Activator.CreateInstance(property.PropertyType, new object[] {value});
        }
    }
}
