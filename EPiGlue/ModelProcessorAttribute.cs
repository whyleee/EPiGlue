using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Mvc;
using EPiGlue.Framework;
using Perks;

namespace EPiGlue
{
    public class ModelProcessorAttribute : ActionFilterAttribute
    {
        private readonly IEnumerable<IModelProcessor> _processors;

        public ModelProcessorAttribute(IEnumerable<IModelProcessor> processors)
        {
            Ensure.ArgumentNotNull(processors, "processors");

            _processors = processors;
        }

        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            foreach (var processor in _processors)
            {
                processor.Process(filterContext);
            }
        }
    }
}
