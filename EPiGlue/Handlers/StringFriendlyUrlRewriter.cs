using System.ComponentModel.DataAnnotations;
using System.Web.Routing;
using EPiGlue.Framework;
using EPiGlue.Helpers;
using EPiServer.Web;
using Perks;

namespace EPiGlue.Handlers
{
    public class StringFriendlyUrlRewriter : IModelPropertyHandler
    {
        protected readonly FriendlyUrlRewriter _urlRewriter;

        public StringFriendlyUrlRewriter(FriendlyUrlRewriter urlRewriter)
        {
            Ensure.ArgumentNotNull(urlRewriter, "urlRewriter");

            _urlRewriter = urlRewriter;
        }

        public virtual bool CanHandle(ModelPropertyContext context)
        {
            return context.PropertyValue is string &&
                   context.Property.Has<UIHintAttribute>(with: x => x.UIHint == UIHint.Document);
        }

        public virtual void Process(ModelPropertyContext context)
        {
            var url = (string) context.PropertyValue;
            var friendlyUrl = _urlRewriter.GetFriendlyUrl(url, context.ExecutionContext.RequestContext, RouteTable.Routes);

            context.PropertyValue = friendlyUrl;
        }
    }
}