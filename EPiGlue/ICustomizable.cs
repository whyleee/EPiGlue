using System.Collections.Generic;

namespace EPiGlue
{
    public interface ICustomizable
    {
        IDictionary<string, object> Attributes { get; set; }
    }
}