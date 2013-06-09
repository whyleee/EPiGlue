using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Perks;

namespace EPiGlue
{
    public class PageEditorHtmlString : IEditHtmlString
    {
        public PageEditorHtmlString(IHtmlString fieldHtml)
        {
            FieldHtml = fieldHtml;
        }

        public IHtmlString EditorStart { get; set; }

        public IHtmlString FieldHtml { get; set; }

        public IHtmlString EditorEnd { get; set; }

        public IHtmlString DefaultValue { get; set; }

        public string ToHtmlString()
        {
            var fieldHtml = FieldHtml.ToHtml().IfNotNullOrEmpty() ?? DefaultValue.ToHtml();

            return string.Concat(EditorStart.ToHtml(), fieldHtml, EditorEnd.ToHtml());
        }
    }
}
