using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Perks;

namespace EPiGlue
{
    public class EditableCollection<T> : IEditHtmlString, IEnumerable<T>
    {
        public EditableCollection(IEnumerable<T> col = null)
        {
            Items = col ?? new List<T>();
        }

        public IHtmlString EditorStart { get; set; }

        public IEnumerable<T> Items { get; set; }

        public IHtmlString EditorEnd { get; set; }

        public IHtmlString DefaultValue { get; set; }

        public string ToHtmlString()
        {
            var html = new StringBuilder();
            html.Append(EditorStart.ToHtml());

            var items = Items.Cast<object>().ToList();

            foreach (var item in items)
            {
                if (item is IHtmlString)
                {
                    html.Append(((IHtmlString) item).ToHtml());
                }
                else
                {
                    html.Append(item);
                }
            }

            if (items.IsEmpty())
            {
                html.Append(DefaultValue.ToHtml());
            }

            html.Append(EditorEnd.ToHtml());

            return html.ToString();
        }

        public IEnumerator<T> GetEnumerator()
        {
            return Items.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
