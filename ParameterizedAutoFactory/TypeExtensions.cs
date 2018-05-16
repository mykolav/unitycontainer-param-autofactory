using System;
using System.Reflection;

namespace ParameterizedAutoFactory
{
    internal static class TypeExtensions
    {
        /// <summary>
        /// Checks whether <paramref name="type"/> is
        /// a func of the form Func{TArg0, TArg1, ..., TArgN, TResult}
        /// </summary>
        /// <param name="type">The type to inspect</param>
        /// <returns>
        /// true if <paramref name="type"/> is a func of the expected form.
        /// false -- otherwise
        /// </returns>
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