using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Unity.ParameterizedAutoFactory.Core
{
    /// <summary>
    /// This class builds a parameterized auto-factory specified by
    /// a type of kind Func{TArg0, ..., TResult}
    /// </summary>
    internal static class ParameterizedAutoFactoryFactoryFactory
    {
        /// <summary>
        /// We leverage System.Linq.Expressions.Expression to dynamically build
        /// a func similar to the one below.
        /// <code>
        /// Func{TUnityContainer, Param0Type, Param1Type, ..., ParamNType, ProductType}
        /// createParameterizedAutoFactory = (container) =>
        /// {
        ///     return (param0, param1, ..., paramN) =>
        ///     {
        ///         var resolverOverrides = new ResolverOverride[]
        ///         {
        ///             new ParameterByTypeOverride(
        ///                 /*targetType*/ _typeCreatedByAutoFactory, 
        ///                 /*parameterType*/ _autoFactoryParamTypes[0], 
        ///                 /*parameterValue*/ param0),
        ///
        ///             new ParameterByTypeOverride(
        ///                 /*targetType*/ _typeCreatedByAutoFactory, 
        ///                 /*parameterType*/ _autoFactoryParamTypes[1], 
        ///                 /*parameterValue*/ param1),
        ///
        ///             /// ...
        ///
        ///             new ParameterByTypeOverride(
        ///                 /*targetType*/ _typeCreatedByAutoFactory, 
        ///                 /*parameterType*/ _autoFactoryParamTypes[N], 
        ///                 /*parameterValue*/ paramN)
        ///         };

        ///         var resolvedInstance = container.Resolve(
        ///             _typeCreatedByAutoFactory, 
        ///             resolverOverrides);

        ///         return resolvedInstance;
        ///     };
        /// };
        /// </code>
        /// </summary>
        public static Func<TUnityContainer, Delegate> BuildAutoFactoryCreator<
            TUnityContainer,
            TResolverOverride, 
            TParameterByTypeOverride>(Type autoFactoryType)

            where TUnityContainer : class
            where TResolverOverride : class
            where TParameterByTypeOverride : class, TResolverOverride
        {
            EnsureAutoFactoryTypeIsParameterizedFunc(autoFactoryType);

            var epContainer = Expression.Parameter(typeof(TUnityContainer), "container");

            var eParameterizedAutoFactory = BuildAutoFactoryExpression<
                TResolverOverride, 
                TParameterByTypeOverride>(epContainer, autoFactoryType);

            // Now build code equivalent to:
            // container =>
            //     (param0, param1, ..., paramN) =>
            //     {
            //         ...
            //         // container is referenced from this block
            //         ...
            //     }
            var eCreateParameterizedAutoFactory = Expression.Lambda(
                eParameterizedAutoFactory,
                epContainer);

            var createParameterizedAutoFactory = (Func<TUnityContainer, Delegate>)
                eCreateParameterizedAutoFactory.Compile();
            return createParameterizedAutoFactory;
        }

        /// <summary>
        /// We leverage System.Linq.Expressions.Expression to dynamically build
        /// a func similar to the one below.
        /// <code>
        /// Func{Param0Type, Param1Type, ..., ParamNType, ProductType}
        /// parameterizedAutoFactory = (param0, param1, ..., paramN) =>
        /// {
        ///     var resolverOverrides = new ResolverOverride[]
        ///     {
        ///         new ParameterByTypeOverride(
        ///             /*targetType*/ _typeCreatedByAutoFactory, 
        ///             /*parameterType*/ _autoFactoryParamTypes[0], 
        ///             /*parameterValue*/ param0),
        ///
        ///         new ParameterByTypeOverride(
        ///             /*targetType*/ _typeCreatedByAutoFactory, 
        ///             /*parameterType*/ _autoFactoryParamTypes[1], 
        ///             /*parameterValue*/ param1),
        ///
        ///         /// ...
        ///
        ///         new ParameterByTypeOverride(
        ///             /*targetType*/ _typeCreatedByAutoFactory, 
        ///             /*parameterType*/ _autoFactoryParamTypes[N], 
        ///            /*parameterValue*/ paramN)
        ///     };

        ///     var resolvedInstance = container.Resolve(
        ///         _typeCreatedByAutoFactory, 
        ///         resolverOverrides);

        ///     return resolvedInstance;
        /// };
        /// };
        /// </code>
        /// </summary>
        public static Delegate BuildAutoFactory<
            TUnityContainer,
            TResolverOverride, 
            TParameterByTypeOverride>(
                Type autoFactoryType,
                TUnityContainer container)

            where TUnityContainer : class
            where TResolverOverride : class
            where TParameterByTypeOverride : class, TResolverOverride
        {
            EnsureAutoFactoryTypeIsParameterizedFunc(autoFactoryType);

            var ecContainer = Expression.Constant(container, typeof(TUnityContainer));

            var parameterizedAutoFactory = BuildAutoFactoryExpression<
                TResolverOverride, 
                TParameterByTypeOverride>(ecContainer, autoFactoryType).Compile();

            return parameterizedAutoFactory;
        }

        private static LambdaExpression BuildAutoFactoryExpression<
            TResolverOverride, 
            TParameterByTypeOverride>(
                Expression eContainer,
                Type autoFactoryType)

            where TResolverOverride : class
            where TParameterByTypeOverride : class, TResolverOverride
        {
            var autoFactoryGenericArguments = autoFactoryType.GetTypeInfo().GenericTypeArguments;

            var autoFactoryParamTypes = autoFactoryGenericArguments
                .Take(autoFactoryGenericArguments.Length - 1)
                .ToArray();

            var typeCreatedByAutoFactory = autoFactoryGenericArguments.Last();

            var autoFactoryParams = CreateAutoFactoryParameters(autoFactoryParamTypes);
            var resolverOverrides = CreateResolverOverrides(
                typeCreatedByAutoFactory,
                GetParameterByTypeOverrideCtorInfo<TParameterByTypeOverride>(),
                autoFactoryParams);

            // Here we create the following piece of code
            // var resolverOverrides = new ResolverOverride[]
            // {
            //     new ParameterByTypeOverride(...),
            //     // ...
            // };
            //
            // var resolvedInstance = container.Resolve(
            //     _typeCreatedByAutoFactory, 
            //     resolverOverrides);

            var evResolverOverrides = Expression.Variable(
                typeof(TResolverOverride[]),
                "resolverOverrides");

            var autoFactoryBody = Expression.Block(
                new[] {evResolverOverrides},
                Expression.Assign(
                    evResolverOverrides,
                    Expression.NewArrayInit(
                            typeof(TResolverOverride), 
                            resolverOverrides)
                ),
                Expression.Convert(
                    CreateResolveCall(typeCreatedByAutoFactory, eContainer, evResolverOverrides),
                    typeCreatedByAutoFactory
                )
            );

            var eParameterizedAutoFactory = Expression.Lambda(
                autoFactoryBody,
                autoFactoryParams);
            return eParameterizedAutoFactory;
        }

        private static void EnsureAutoFactoryTypeIsParameterizedFunc(Type autoFactoryType)
        {
            if (autoFactoryType.IsParameterizedFunc()) 
                return;

            throw new ArgumentException(
                message: $"{autoFactoryType.FullName} is not a Func<TArg0, ..., TResult>",
                paramName: nameof(autoFactoryType)
            );
        }

        private static MethodCallExpression CreateResolveCall(
            Type typeCreatedByAutoFactory,
            Expression eContainer,
            ParameterExpression evResolverOverrides)
        {
            // We want to call this method
            // object IUnityContainer.Resolve(
            //                          Type type,
            //                          string name,
            //                          params ResolverOverride[] resolverOverrides);

            return Expression.Call(
                eContainer,
                "Resolve",
                typeArguments: null, // a call to non-generic method
                arguments: new Expression[] 
                {
                    Expression.Constant(typeCreatedByAutoFactory, typeof(Type)),
                    Expression.Constant(null, typeof(string)),
                    evResolverOverrides
                }
            );
        }

        private static ConstructorInfo GetParameterByTypeOverrideCtorInfo<TParameterByTypeOverride>()
        {
            return typeof(TParameterByTypeOverride)
                .GetTypeInfo().GetConstructor(new []
                {
                    /*targetType*/ typeof(Type), 
                    /*parameterType*/ typeof(Type), 
                    /*parameterValue*/ typeof(object)
                });
        }

        private static IReadOnlyList<ParameterExpression> CreateAutoFactoryParameters(
            IReadOnlyList<Type> autoFactoryParamTypes)
        {
            var autoFactoryParams = new List<ParameterExpression>();
            for (var i = 0; i < autoFactoryParamTypes.Count; ++i)
            {
                autoFactoryParams.Add(
                    Expression.Parameter(autoFactoryParamTypes[i], $"param{i}"));
            }

            return autoFactoryParams;
        }

        private static IReadOnlyList<Expression> CreateResolverOverrides(
            Type typeCreatedByAutoFactory,
            ConstructorInfo ciParameterByTypeOverride,
            IEnumerable<ParameterExpression> autoFactoryParameters)
        {
            var resolverOverrides = new List<Expression>();
            foreach (var parameter in autoFactoryParameters)
            {
                resolverOverrides.Add(
                    // Dynamically build the code
                    // similar to the following snippet:
                    //
                    // new ParameterByTypeOverride(
                    //     /*targetType*/ _typeCreatedByAutoFactory, 
                    //     /*parameterType*/ _autoFactoryParamTypes[i], 
                    //     /*parameterValue*/ param{i}),
                    Expression.New(
                        ciParameterByTypeOverride, 
                        Expression.Constant(typeCreatedByAutoFactory),
                        Expression.Constant(parameter.Type, typeof(Type)), 
                        Expression.Convert(parameter, typeof(object))
                    )
                );
            }

            return resolverOverrides;
        }
    }
}
