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
            return IsInEditMode(context) && FilterByType(context) && FilterIgnored(context) && FilterByValue(context);
        }

        public abstract void Process(ModelPropertyContext context);

        protected virtual bool FilterIgnored(ModelPropertyContext context)
        {
            return !context.Model.GetType().Has<EditIgnoreAttribute>() && !context.Property.Has<EditIgnoreAttribute>();
        }

        protected virtual bool FilterByType(ModelPropertyContext context)
        {
            return context.Property.PropertyType.Is<IHtmlString>() &&
                   context.Property.PropertyType != typeof (ContentArea); // TODO: let's skip content areas for now
        }

        protected virtual bool FilterByValue(ModelPropertyContext context)
        {
            return true;
        }

        protected virtual void SetValue(object value, ModelPropertyContext context)
        {
            context.PropertyValue = value;
            context.Property.SetValue(context.Model, context.PropertyValue);
        }

        // TODO: something generic, move out of here
        protected virtual bool IsInEditMode(ModelPropertyContext context)
        {
            var requestContext = context.ExecutionContext.RequestContext;

            object contextMode;
            if (requestContext.RouteData.DataTokens.TryGetValue("contextmode", out contextMode))
            {
                return (ContextMode) contextMode == ContextMode.Edit;
            }

            return PageEditing.PageIsInEditMode;
        }
    }
}
