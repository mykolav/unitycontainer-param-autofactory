using System;
using System.Linq;
using System.Reflection;

namespace Unity.ParameterizedAutoFactory.Core
{
    public static class TypeExtensions
    {
        /// <summary>
        /// This method finds a constructor that has
        ///     1) has excactly the same number of params as the number of elements in <see cref="paramTypes" />
        ///     2) parameters of the given types
        ///     3) are in the same order they are in <see cref="paramTypes" />
        /// This method is built-in into netstandard-1.5,
        /// but we're out of luck as we have to target netstandard-1.1
        /// </summary>
        public static ConstructorInfo GetConstructor(
            this TypeInfo typeInfo,
            Type[] paramTypes)
        {
            bool Matches(ConstructorInfo ci)
            {
                var parameters = ci.GetParameters();
                if (parameters.Length != paramTypes.Length)
                    return false;

                for (var i = 0; i < parameters.Length; ++i)
                {
                    if (parameters[i].ParameterType != paramTypes[i])
                        return false;
                }

                return true;
            }

            var constructorInfo = typeInfo.DeclaredConstructors.Single(Matches);
            return constructorInfo;
        }

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
                type.GetTypeInfo().GenericTypeArguments.Length > 1;
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