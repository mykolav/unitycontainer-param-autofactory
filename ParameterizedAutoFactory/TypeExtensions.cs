using System;
using System.Reflection;

namespace ParameterizedAutoFactory
{
    internal static class TypeExtensions
    {
        public static bool IsFunc(this Type type)
        {
            var isFunc = 
                type.IsDelegate() &&
                type.Name.StartsWith("Func`", StringComparison.Ordinal);
            return isFunc;
        }

        private static bool IsDelegate(this Type type)
        {
            var isDelegate = type.GetTypeInfo().IsSubclassOf(typeof(Delegate));
            return isDelegate;
        }
    }
}