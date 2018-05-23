using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;

namespace Unity.ParameterizedAutoFactory.Core
{
    /// <summary>
    /// This class builds a parameterized autofactory specified by
    /// a type of kind Func{TArg0, ..., TResult}
    /// </summary>
    public class ParameterizedAutoFactoryCreator<
        TUnityContainer,
        TResolverOverride, 
        TParameterByTypeOverride>

        where TUnityContainer : class
        where TResolverOverride : class
        where TParameterByTypeOverride : class, TResolverOverride
    {
        private readonly TUnityContainer _container;
        private readonly Type[] _autoFactoryParamTypes;
        private readonly Type _typeCreatedByAutoFactory;

        public ParameterizedAutoFactoryCreator(
            TUnityContainer container,
            Type autoFactoryType)
        {
            _container = container;

            if (!autoFactoryType.IsParameterizedFunc())
            {
                throw new ArgumentException(
                    message: $"{autoFactoryType.FullName} is not a Func<TArg0, ..., TResult>",
                    paramName: nameof(autoFactoryType)
                );
            }

            var autoFactoryGenericArguments = autoFactoryType.GetTypeInfo().GenericTypeArguments;

            _autoFactoryParamTypes = autoFactoryGenericArguments
                .Take(autoFactoryGenericArguments.Length - 1)
                .ToArray();

            _typeCreatedByAutoFactory = autoFactoryGenericArguments.Last();
        }

        /// <summary>
        /// We leverage System.Linq.Expressions.Expression to dynamically build
        /// a func similar to the one below.
        /// <code>
        /// Func{Param0Type, Param1Type, ..., ParamNType, ProductType} parameterizedAutoFactory =
        /// (param0, param1, ..., paramN) =>
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
        ///             /*parameterValue*/ paramN)
        ///         
        ///     };

        ///     var resolvedInstance = _container.Resolve(
        ///         _typeCreatedByAutoFactory, 
        ///         resolverOverrides);

        ///     return resolvedInstance;
        /// };
        /// </code>
        /// </summary>
        public object Invoke()
        {
            var autoFactoryParams = CreateAutoFactoryParameters();

            var autoFactoryBody = CreateBodyAutoFactoryBody(
                CreateResolverOverrides(
                    GetParameterByTypeOverrideCtorInfo(), 
                    autoFactoryParams
                )
            );

            var parameterizedAutoFactoryExpression = Expression.Lambda(
                autoFactoryBody,
                autoFactoryParams);

            var parameterizedAutoFactory = parameterizedAutoFactoryExpression.Compile();
            return parameterizedAutoFactory;
        }

        private BlockExpression CreateBodyAutoFactoryBody(IEnumerable<Expression> resolverOverrides)
        {
            var evResolverOverrides = Expression.Variable(
                typeof(TResolverOverride[]),
                "resolverOverrides");

            var autoFactoryBody = Expression.Block(
                new[] {evResolverOverrides},
                Expression.Assign(
                    evResolverOverrides,
                    Expression.NewArrayInit(typeof(TResolverOverride), resolverOverrides)
                ),
                Expression.Convert(
                    CreateResolveCall(evResolverOverrides),
                    _typeCreatedByAutoFactory
                )
            );

            return autoFactoryBody;
        }

        private MethodCallExpression CreateResolveCall(
            ParameterExpression evResolverOverrides)
        {
            // We want to call this method
            // object IUnityContainer.Resolve(
            //                          Type type,
            //                          string name,
            //                          params ResolverOverride[] resolverOverrides);

            return Expression.Call(
                Expression.Constant(_container, _container.GetType()),
                "Resolve",
                typeArguments: null, // a call to non-generic method
                arguments: new Expression[] 
                {
                    Expression.Constant(_typeCreatedByAutoFactory, typeof(Type)),
                    Expression.Constant(null, typeof(string)),
                    evResolverOverrides
                }
            );
        }

        private static ConstructorInfo GetParameterByTypeOverrideCtorInfo()
        {
            return typeof(TParameterByTypeOverride)
                .GetTypeInfo().GetConstructor(new []
                {
                    /*targetType*/ typeof(Type), 
                    /*parameterType*/ typeof(Type), 
                    /*parameterValue*/ typeof(object)
                });
        }

        private IReadOnlyList<ParameterExpression> CreateAutoFactoryParameters()
        {
            var autoFactoryParams = new List<ParameterExpression>();
            for (var i = 0; i < _autoFactoryParamTypes.Length; ++i)
            {
                autoFactoryParams.Add(
                    Expression.Parameter(_autoFactoryParamTypes[i], $"param{i}"));
            }

            return autoFactoryParams;
        }

        private IReadOnlyList<Expression> CreateResolverOverrides(
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
                        Expression.Constant(_typeCreatedByAutoFactory),
                        Expression.Constant(parameter.Type, typeof(Type)), 
                        Expression.Convert(parameter, typeof(object))
                    )
                );
            }

            return resolverOverrides;
        }
    }
}
