using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;
using EPiGlue.Framework;
using EPiGlue.Helpers;
using EPiServer.Core;
using EPiServer.Core.Html.StringParsing;
using EPiServer.DynamicContent.PlugIn;
using EPiServer.Web.Mvc.Html;

namespace EPiGlue.Handlers
{
    public class XhtmlStringHandler : IModelPropertyHandler
    {
        public virtual bool CanHandle(ModelPropertyContext context)
        {
            return context.PropertyValue != null && context.PropertyValue.GetType() == typeof(XhtmlString);
        }

        public virtual void Process(ModelPropertyContext context)
        {
            var value = (XhtmlString) context.PropertyValue;
            var result = new StringBuilder();

            using (var writer = new StringWriter(result))
            {
                RenderXhtmlStringToWriter(value, writer,
                    context.ExecutionContext.Controller,
                    context.ExecutionContext.RequestContext
                );
            }

            context.PropertyValue = HtmlStringActivator.CreateInstance(context.Property.PropertyType, result.ToString());
        }

        private void RenderXhtmlStringToWriter(IHtmlString text, TextWriter writer, ControllerBase controller, RequestContext requestContext)
        {
            var fragments = (IEnumerable<IStringFragment>)((XhtmlString) text).Fragments.GetFilteredFragments();

            var urlHelper = new UrlHelper(requestContext);
            var viewContext = new ViewContext(
                controller.ControllerContext,
                new DynamicPageProperty(),
                controller.ViewData,
                controller.TempData,
                writer
            );

            foreach (var fragment in fragments)
            {
                RenderFragment(fragment, writer, urlHelper, viewContext);
            }
        }

        private void RenderFragment(IStringFragment fragment, TextWriter writer, UrlHelper urlHelper, ViewContext viewContext)
        {
            if (fragment is DynamicContentFragment)
            {
                ((DynamicContentFragment) fragment).Render(viewContext, writer);
            }
            else if (fragment is UrlFragment)
            {
                writer.Write(urlHelper.PageUrl(((UrlFragment) fragment).GetViewFormat()));
            }
            else if (fragment is PersonalizedContentFragment)
            {
                foreach (var personalizedFragment in ((PersonalizedContentFragment) fragment).Fragments.GetFilteredFragments())
                {
                    RenderFragment(personalizedFragment, writer, urlHelper, viewContext);
                }
            }
            else
            {
                writer.Write(fragment.GetViewFormat());
            }
        }
    }
}
