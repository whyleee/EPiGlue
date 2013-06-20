using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace EPiGlue.Framework
{
    public class ModelPropertyContext
    {
        public object Model { get; set; }

        public PropertyInfo Property { get; set; }

        private object _propertyValue;
        public object PropertyValue
        {
            get { return _propertyValue; }
            set
            {
                _propertyValue = value;
                Property.SetValue(Model, value);
            }
        }

        public ResultExecutingContext ExecutionContext { get; set; }

        public bool StopProcessing { get; set; }
    }
}
