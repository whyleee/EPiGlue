using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perks;

namespace EPiGlue.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EditHintAttribute : Attribute
    {
        public EditHintAttribute(string editName)
        {
            Ensure.ArgumentNotNullOrEmpty(editName, "editName");

            EditName = editName;
        }

        public EditHintAttribute()
        {
        }

        public string EditName { get; set; }

        public string EditTag { get; set; }

        public string EditCssClass { get; set; }
    }
}
