using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Perks;

namespace EPiGlue.Framework
{
    public class PropertyHandlerChainModelProcessor : PropertyModelProcessor
    {
        private readonly Settings _settings;
        private readonly IList<IModelPropertyHandler> _handlers;

        public PropertyHandlerChainModelProcessor(IEnumerable<IModelPropertyHandler> handlers, Settings settings = null)
        {
            Ensure.ArgumentNotNull(handlers, "handlers");

            _handlers = handlers.ToList();
            _settings = settings ?? new Settings();
        }

        public class Settings
        {
            public Settings()
            {
                ShouldBeHandled = mt => true;
            }

            public Func<Type, bool> ShouldBeHandled { get; set; }
        }

        protected override bool CanHandle(Type modelType)
        {
            return _settings.ShouldBeHandled(modelType);
        }

        protected override void ProcessModelProperty(object model, PropertyInfo property, object value)
        {
            var context = new ModelPropertyContext
                {
                    Model = model,
                    Property = property,
                    PropertyValue = value,
                    ExecutionContext = Context
                };

            foreach (var handler in _handlers.Where(h => h.CanHandle(context)))
            {
                handler.Process(context);

                if (context.StopProcessing)
                {
                    break;
                }
            }
        }
    }
}
