using System;
using System.Collections;
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

        public virtual void Process(ModelPropertyContext context)
        {
            if (context.Property.PropertyType.Is<IEnumerable>())
            {
                if (context.PropertyValue == null || !context.PropertyValue.GetType().Is(typeof (EditableCollection<>)))
                {
                    var itemType = context.Property.PropertyType.GetCollectionItemType();
                    var collectionTemplate = typeof (EditableCollection<>).MakeGenericType(itemType);

                    context.PropertyValue = Activator.CreateInstance(collectionTemplate, context.PropertyValue);
                }
            }
        }

        protected virtual bool FilterIgnored(ModelPropertyContext context)
        {
            return !context.Model.GetType().Has<EditIgnoreAttribute>() && !context.Property.Has<EditIgnoreAttribute>();
        }

        protected virtual bool FilterByType(ModelPropertyContext context)
        {
            return (context.Property.PropertyType.Is<IHtmlString>() &&
                    context.Property.PropertyType != typeof (ContentArea)) || // TODO: let's skip content areas for now
                    context.Property.PropertyType.GetCollectionItemType().IfNotNull(t => t.Is<IHtmlString>()) ||
                   (context.PropertyValue != null && context.PropertyValue.GetType().Is(typeof (EditableCollection<>)));
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

        protected virtual string GetFieldEditName(ModelPropertyContext context)
        {
            var field = context.Property;
            var editNameHint = field.Get<EditHintAttribute>().IfNotNull(x => x.EditName);

            if (editNameHint.IsNotNullOrEmpty())
            {
                return editNameHint;
            }

            if (field.Has<EditHintNoPrefixAttribute>())
            {
                return field.Name;
            }

            var editPrefix = context.Model.GetType().Get<EditHintPrefixAttribute>().IfNotNull(x => x.EditPrefix);

            return editPrefix + field.Name;
        }
    }
}
