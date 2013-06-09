using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using EPiGlue.Framework;
using EPiServer.Web.Mvc;
using Perks;

namespace EPiGlue.Handlers
{
    public class PageEditorHintsHandler : PageEditorHtmlHandler
    {
        public override bool CanHandle(ModelPropertyContext context)
        {
            return base.CanHandle(context) && context.PropertyValue == null && !IsInBlockPreviewMode(context);
        }

        public override void Process(ModelPropertyContext context)
        {
            var editName = GetFieldEditName(context);
            var editHint = GetFieldEditHint(editName);
            var fieldType = context.Property.PropertyType;

            if (fieldType.Is<IEditHtmlString>())
            {
                var fieldValue = Activator.CreateInstance(fieldType);
                ((IEditHtmlString) fieldValue).DefaultValue = editHint;

                context.PropertyValue = fieldValue;
            }
            else if (fieldType == typeof (IHtmlString))
            {
                context.PropertyValue = editHint;
            }
            else
            {
                // Don't know how to insert the hint, skipping this field..
            }
        }

        private IHtmlString GetFieldEditHint(string editName)
        {
            return new HtmlString("[" + editName.ToFriendlyString() + "]");
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
