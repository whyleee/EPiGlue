using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace EPiGlue
{
    internal static class TypeExtensions
    {
        public static bool Has<TAttribute>(this Type type) where TAttribute : Attribute
        {
            return type.Get<TAttribute>() != null;
        }

        public static bool Has<TAttribute>(this MemberInfo member) where TAttribute : Attribute
        {
            return member.Get<TAttribute>() != null;
        }

        public static TAttribute Get<TAttribute>(this Type type) where TAttribute : Attribute
        {
            return type.GetCustomAttribute<TAttribute>();
        }

        public static TAttribute Get<TAttribute>(this MemberInfo member) where TAttribute : Attribute
        {
            return member.GetCustomAttribute<TAttribute>();
        }
    }
}
