using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using EPiGlue.Framework;
using EPiGlue.Helpers;
using EPiServer.Web;
using Perks;

namespace EPiGlue.Handlers
{
    public class HtmlStringFriendlyUrlRewriter : IModelPropertyHandler
    {
        protected readonly FriendlyUrlRewriter _urlRewriter;

        public HtmlStringFriendlyUrlRewriter(FriendlyUrlRewriter urlRewriter)
        {
            Ensure.ArgumentNotNull(urlRewriter, "urlRewriter");

            _urlRewriter = urlRewriter;
        }

        public virtual bool CanHandle(ModelPropertyContext context)
        {
            return context.Property.PropertyType.Is<IHtmlString>() &&
                   context.Property.Has<UIHintAttribute>(with: x => x.UIHint == UIHint.Document);
        }

        public virtual void Process(ModelPropertyContext context)
        {
            var url = ((IHtmlString) context.PropertyValue).IfNotNull(x => x.ToHtmlString());
            var friendlyUrl = _urlRewriter.GetFriendlyUrl(url, context.ExecutionContext.RequestContext, RouteTable.Routes);
            context.PropertyValue = HtmlStringActivator.CreateInstance(context.Property, friendlyUrl);
        }
    }
}
