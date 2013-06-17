using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using EPiServer.Framework;
using EPiServer.Framework.Initialization;

namespace EPiGlue
{
    [InitializableModule]
    [ModuleDependency(typeof(EPiServer.Web.InitializationModule))]
    public class Initializer : IInitializableModule
    {
        private static Func<HtmlHelper, string> _defaultObjectTemplate;

        public void Initialize(InitializationEngine context)
        {
            RegisterHtmlStringAwareObjectTemplate();
        }

        private static void RegisterHtmlStringAwareObjectTemplate()
        {
            var templateHelpers = Type.GetType("System.Web.Mvc.Html.TemplateHelpers, System.Web.Mvc");
            var defTemplates = (Dictionary<string, Func<HtmlHelper, string>>)
                templateHelpers.GetField("_defaultDisplayActions", BindingFlags.Static | BindingFlags.NonPublic)
                .GetValue(null);

            defTemplates[typeof(object).Name] = HtmlStringAwareObjectTemplate;
        }

        private static string HtmlStringAwareObjectTemplate(HtmlHelper html)
        {
            var viewData = html.ViewContext.ViewData;

            if (viewData.Model is IHtmlString)
            {
                return ((IHtmlString) viewData.Model).ToHtmlString();
            }

            return DefaultObjectTemplate(html);
        }

        private static string DefaultObjectTemplate(HtmlHelper html)
        {
            if (_defaultObjectTemplate != null)
            {
                return _defaultObjectTemplate(html);
            }

            var defTemplatesType = Type.GetType("System.Web.Mvc.Html.DefaultDisplayTemplates, System.Web.Mvc");
            var defObjectTemplateMethod = defTemplatesType.GetMethod("ObjectTemplate",
                BindingFlags.Static | BindingFlags.NonPublic, null, new[] {typeof(HtmlHelper)}, null);

            var call = Expression.Call(defObjectTemplateMethod, Expression.Constant(html));
            var param = Expression.Parameter(typeof (HtmlHelper));
            var lambda = Expression.Lambda<Func<HtmlHelper, string>>(call, param);

            _defaultObjectTemplate = lambda.Compile();

            return _defaultObjectTemplate(html);
        }

        public void Uninitialize(InitializationEngine context)
        {
        }

        public void Preload(string[] parameters)
        {
        }
    }
}
