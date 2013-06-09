using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using EPiGlue.DataAnnotations;
using EPiGlue.Framework;
using EPiServer.Core;
using EPiServer.Editor;
using EPiServer.Web;
using Perks;

namespace EPiGlue.Handlers
{
    public abstract class PageEditorFieldHandler : IModelPropertyHandler
    {
        public virtual bool CanHandle(ModelPropertyContext context)
        {
            return IsInEditMode(context.ExecutionContext.RequestContext) &&
                   context.Property.PropertyType.Is<IHtmlString>() &&
                   context.Property.PropertyType != typeof(ContentArea) && // TODO: let's skip content areas for now
                   !context.Model.GetType().Has<EditIgnoreAttribute>() &&
                   !context.Property.Has<EditIgnoreAttribute>();
        }

        public abstract void Process(ModelPropertyContext context);

        // TODO: something generic, move out of here
        private bool IsInEditMode(RequestContext requestContext)
        {
            object contextMode;
            if (requestContext.RouteData.DataTokens.TryGetValue("contextmode", out contextMode))
            {
                return (ContextMode) contextMode == ContextMode.Edit;
            }

            return PageEditing.PageIsInEditMode;
        }
    }
}
