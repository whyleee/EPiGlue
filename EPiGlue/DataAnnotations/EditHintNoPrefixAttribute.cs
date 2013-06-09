using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EPiGlue.DataAnnotations
{
    [AttributeUsage(AttributeTargets.Property)]
    public class EditHintNoPrefixAttribute : Attribute
    {
    }
}
