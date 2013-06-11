using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Routing;
using EPiGlue.DataAnnotations;
using EPiGlue.Framework;
using EPiServer.Editor;
using EPiServer.Framework.Serialization;
using EPiServer.ServiceLocation;
using EPiServer.Web.Mvc;
using Perks;

namespace EPiGlue.Handlers
{
    public class PageEditorHtmlHandler : PageEditorFieldHandler
    {
        public override void Process(ModelPropertyContext context)
        {
            var fieldHtml = InsertPageEditorMarkup(context, (IHtmlString) context.PropertyValue);
            SetValue(fieldHtml, context); // TODO: we could have an exception here (different types of property and setting value)
        }

        private IHtmlString InsertPageEditorMarkup(ModelPropertyContext context, IHtmlString fieldHtml)
        {
            var editHtml = fieldHtml is IEditHtmlString ? (IEditHtmlString) fieldHtml : new PageEditorHtmlString(fieldHtml);
            var htmlBuilder = new StringBuilder();
            var editHint = context.Property.Get<EditHintAttribute>() ?? new EditHintAttribute();

            using (var writer = new StringWriter(htmlBuilder))
            {
                using (var editContainer = new MvcEditContainer(
                    requestContext: context.ExecutionContext.RequestContext,
                    epiPropertyKey: PageEditing.DataEPiPropertyName,
                    epiPropertyName: GetFieldEditName(context),
                    editElementName: editHint.EditTag,
                    editElementCssClass: editHint.EditCssClass,
                    writer: writer))
                {
                    Func<string> renderSettingsWriter = null;

                    if (editHint.CssClass.IsNotNullOrEmpty())
                    {
                        var renderSettings = new RouteValueDictionary(new {CssClass = editHint.CssClass});
                        renderSettingsWriter = () => AttrsWriter(renderSettings, PageEditing.DataEPiPropertyRenderSettings);
                    }

                    editContainer.CreateStartElementForEditMode(renderSettingsWriter);
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

        private static string AttrsWriter(RouteValueDictionary routeValues, string attributeName)
        {
            // TODO: decompiled and could be much easier I think
            if (routeValues.Count == 0)
            {
                return null;
            }
            var instance = ServiceLocator.Current.GetInstance<IObjectSerializerFactory>();
            var serializer = instance.GetSerializer("application/json");
            var stringBuilder = new StringBuilder();
            serializer.Serialize(new StringWriter(stringBuilder), routeValues);
            var cssClass = routeValues["cssclass"] as string;
            return string.Format("{0}=\"{1}\"{2}", attributeName, HttpUtility.HtmlAttributeEncode(stringBuilder.ToString()),
                string.IsNullOrEmpty(cssClass) ? string.Empty : string.Format(CultureInfo.InvariantCulture, " class=\"{0}\"", new object[]{cssClass}));
        }
    }
}
