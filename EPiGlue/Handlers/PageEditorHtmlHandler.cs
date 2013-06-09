using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using EPiGlue.DataAnnotations;
using EPiGlue.Framework;
using EPiServer.Editor;
using EPiServer.Web.Mvc;
using Perks;

namespace EPiGlue.Handlers
{
    public class PageEditorHtmlHandler : PageEditorFieldHandler
    {
        public override void Process(ModelPropertyContext context)
        {
            var fieldHtml = InsertPageEditorMarkup(context, (IHtmlString) context.PropertyValue);
            context.Property.SetValue(context.Model, fieldHtml); // TODO: we could have an exception here (different types of property and setting value)
        }

        private IHtmlString InsertPageEditorMarkup(ModelPropertyContext context, IHtmlString fieldHtml)
        {
            var editHtml = fieldHtml is IEditHtmlString ? (IEditHtmlString) fieldHtml : new PageEditorHtmlString(fieldHtml);
            var htmlBuilder = new StringBuilder();
            var editHint = context.Property.Get<EditHintAttribute>();

            using (var writer = new StringWriter(htmlBuilder))
            {
                using (var editContainer = new MvcEditContainer(
                    requestContext: context.ExecutionContext.RequestContext,
                    epiPropertyKey: PageEditing.DataEPiPropertyName,
                    epiPropertyName: GetFieldEditName(context),
                    editElementName: editHint.IfNotNull(x => x.EditTag),
                    editElementCssClass: editHint.IfNotNull(x => x.EditCssClass),
                    writer: writer))
                {
                    editContainer.CreateStartElementForEditMode(null);
                    editHtml.EditorStart = new HtmlString(writer.ToString());
                    htmlBuilder.Clear();
                }

                editHtml.EditorEnd = new HtmlString(writer.ToString());
            }

            return editHtml;
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
