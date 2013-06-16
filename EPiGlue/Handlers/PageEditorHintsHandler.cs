using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using EPiGlue.DataAnnotations;
using EPiGlue.Framework;
using EPiServer.Web.Mvc;
using Perks;

namespace EPiGlue.Handlers
{
    public class PageEditorHintsHandler : PageEditorFieldHandler
    {
        public override bool CanHandle(ModelPropertyContext context)
        {
            return base.CanHandle(context) && !IsInBlockPreviewMode(context);
        }

        protected override bool FilterByValue(ModelPropertyContext context)
        {
            return context.PropertyValue == null;
        }

        public override void Process(ModelPropertyContext context)
        {
            base.Process(context);

            var editName = GetFieldEditName(context);
            var editHint = GetFieldEditHint(editName, context);
            var fieldType = context.Property.PropertyType;

            if (fieldType.Is<IEditHtmlString>())
            {
                var fieldValue = Activator.CreateInstance(fieldType);
                ((IEditHtmlString) fieldValue).DefaultValue = editHint;

                SetValue(fieldValue, context);
            }
            else if (fieldType == typeof (IHtmlString))
            {
                SetValue(editHint, context);
            }
            else
            {
                // Don't know how to insert the hint, skipping this field..
            }
        }

        private IHtmlString GetFieldEditHint(string editName, ModelPropertyContext context)
        {
            var editHint = context.Property.Get<EditHintAttribute>() ?? new EditHintAttribute();
            var cssClassAttr = editHint.CssClass.IfNotNullOrEmpty(css => string.Format(" class=\"{0}\"", css));

            var html = string.Format("<span{0}>[{1}]</span>", cssClassAttr, editName.ToFriendlyString());

            return new HtmlString(html);
        }

        private bool IsInBlockPreviewMode(ModelPropertyContext context)
        {
            if (context.Model.GetType().Name.EndsWith("BlockModel"))
            {
                var httpContext = context.ExecutionContext.RequestContext.HttpContext;
                var contentStack = httpContext.Items[ContentContext.ContentContextKey] as Stack<ContentContext.ContentPropertiesStack>;

                // TODO: check the stask and return true if the parent item is a content area.
                return false;
            }

            return false;
        }
    }
}
