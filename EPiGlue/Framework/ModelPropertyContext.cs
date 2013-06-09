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

        public object PropertyValue { get; set; }

        public ResultExecutingContext ExecutionContext { get; set; }

        public bool StopProcessing { get; set; }
    }
}
