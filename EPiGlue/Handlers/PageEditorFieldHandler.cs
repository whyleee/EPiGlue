using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using EPiGlue.DataAnnotations;
using EPiGlue.Framework;
using EPiServer.Core;
using EPiServer.Editor;
using EPiServer.Framework.DataAnnotations;
using EPiServer.Framework.Web;
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

        // TODO: something generic, move out of here
        protected virtual bool IsInEditMode(ModelPropertyContext context)
        {
            var editMode = false;
            var modeDetected = false;
            var requestContext = context.ExecutionContext.RequestContext;

            object contextMode;
            if (requestContext.RouteData.DataTokens.TryGetValue("contextmode", out contextMode))
            {
                editMode = (ContextMode) contextMode == ContextMode.Edit;
                modeDetected = true;
            }

            if (!modeDetected)
            {
                editMode = PageEditing.PageIsInEditMode;
            }

            // skip processing for block previews to render them in ordinary view mode
            if (editMode && IsInBlockPreviewMode(context))
            {
                return false;
            }

            return editMode;
        }

        private bool IsInBlockPreviewMode(ModelPropertyContext context)
        {
            if (!context.Model.GetType().Name.EndsWith("BlockModel"))
            {
                return false;
            }

            var parentViewContext = context.ExecutionContext.ParentActionViewContext;

            if (parentViewContext == null)
            {
                return false;
            }

            var controllerType = parentViewContext.Controller.GetType();

            return !controllerType.Has<TemplateDescriptorAttribute>(
                with: attr => attr.Tags.Contains(RenderingTags.Preview)
            );
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
