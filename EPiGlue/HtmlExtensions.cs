using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Mvc.Html;
using System.Web.Routing;
using EPiServer.Web.Mvc.Html;

namespace EPiGlue
{
    public static class HtmlExtensions
    {
        public static IHtmlString For<TModel, TValue>(this HtmlHelper<TModel> html, Expression<Func<TModel, TValue>> expression, object @params = null)
        {
            var value = expression.Compile()(html.ViewData.Model);

            if (!(value is IEditHtmlString))
            {
                return html.PropertyFor(expression, @params);
            }

            SetParams(value, @params);

            return html.DisplayFor(expression, @params);
        }

        public static IHtmlString For<TValue>(this HtmlHelper html, TValue value, object @params = null)
        {
            SetParams(value, @params);

            if (value is IHtmlString)
            {
                return (IHtmlString) value;
            }

            return value.ToHtml();
        }

        private static void SetParams(object value, object @params)
        {
            if (value is ICustomizable)
            {
                ((ICustomizable) value).Attributes = new RouteValueDictionary(@params);
            }
        }
    }
}
