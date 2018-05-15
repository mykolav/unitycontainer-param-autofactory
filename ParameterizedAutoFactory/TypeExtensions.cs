using System;
using System.Reflection;

namespace ParameterizedAutoFactory
{
    internal static class TypeExtensions
    {
        public static bool IsParameterizedFunc(this Type type)
        {
            var isParameterizedFunc =
                type.IsFunc() &&
                type.GetGenericArguments().Length > 1;
            return isParameterizedFunc;
        }

        private static bool IsFunc(this Type type)
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