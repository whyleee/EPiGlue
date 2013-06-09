using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Perks;

namespace EPiGlue.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Class)]
    public class EditHintPrefixAttribute : Attribute
    {
        public EditHintPrefixAttribute(string editPrefix)
        {
            Ensure.ArgumentNotNullOrEmpty(editPrefix, "editPrefix");

            EditPrefix = editPrefix;
        }

        public string EditPrefix { get; set; }
    }
}
