using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Unity;
using Unity.Builder;
using Unity.Builder.Strategy;
using Unity.Resolution;

namespace ParameterizedAutoFactory
{
    public class ParameterizedAutoFactoryBuilderStrategy : BuilderStrategy
    {
        private readonly UnityContainer _container;

        public ParameterizedAutoFactoryBuilderStrategy(UnityContainer container)
        {
            _container = container;
        }

        public override void PreBuildUp(IBuilderContext context)
        {
            var type = context.OriginalBuildKey.Type;

            if (_container.Registrations.Any(r => r.RegisteredType == type))
                return;

            if (!type.IsParameterizedFunc())
                return;

            context.Existing = CreateAutoFactory(type);
            context.BuildComplete = true;
        }


        private object CreateAutoFactory(Type autoFactoryType)
        {
            var autoFactoryGenericArguments = autoFactoryType.GetGenericArguments();
            var autoFactoryParamTypes = autoFactoryGenericArguments.Take(autoFactoryGenericArguments.Length - 1).ToArray();
            var typeCreatedByAutoFactory = autoFactoryGenericArguments.Last();

            //Func<int, string, object> parameterizedAutoFactory0 = (param0, param1) =>
            //{
            //    var resolverOverrides0 = new ResolverOverride[]
            //    {
            //        new ParameterByTypeOverride(
            //            /*targetType*/ typeCreatedByAutoFactory, 
            //            /*parameterType*/ autoFactoryParamTypes[0], 
            //            /*parameterValue*/ param0),

            //        new ParameterByTypeOverride(
            //            /*targetType*/ typeCreatedByAutoFactory, 
            //            /*parameterType*/ autoFactoryParamTypes[1], 
            //            /*parameterValue*/ param1)
            //    };

            //    var resolvedInstance = _container.Resolve(
            //        typeCreatedByAutoFactory, 
            //        resolverOverrides0);

            //    return resolvedInstance;
            //};

            var ciParameterByTypeOverride = typeof(ParameterByTypeOverride)
                .GetConstructor(new []
                {
                    /*targetType*/ typeof(Type), 
                    /*parameterType*/ typeof(Type), 
                    /*parameterValue*/ typeof(object)
                });
            
            var autoFactoryParams = new List<ParameterExpression>();
            var resolverOverrides = new List<Expression>();
            for (var i = 0; i < autoFactoryParamTypes.Length; ++i)
            {
                var paramType = autoFactoryParamTypes[i];

                autoFactoryParams.Add(Expression.Parameter(paramType, $"param{i}"));

                resolverOverrides.Add(
                    //new ParameterByTypeOverride(
                    //    /*targetType*/ typeCreatedByAutoFactory, 
                    //    /*parameterType*/ autoFactoryParamTypes[i], 
                    //    /*parameterValue*/ param{i}),
                    Expression.New(
                        ciParameterByTypeOverride, 
                        Expression.Constant(typeCreatedByAutoFactory),
                        Expression.Constant(paramType, typeof(Type)), 
                        Expression.Convert(autoFactoryParams[i], typeof(object))
                    )
                );
            }

            var evResolverOverrides = Expression.Variable(
                typeof(ResolverOverride[]), 
                "resolverOverrides");

            var autoFactoryBody = Expression.Block(
                new [] { evResolverOverrides },

                Expression.Assign(
                    evResolverOverrides,
                    Expression.NewArrayInit(typeof(ResolverOverride), resolverOverrides)
                ),

                Expression.Convert(
                    Expression.Call(
                        typeof(UnityContainerExtensions),
                        "Resolve",
                        typeArguments: null, // a call to non-generic method
                        arguments: new Expression[] 
                        {
                            Expression.Constant(_container, _container.GetType()),
                            Expression.Constant(typeCreatedByAutoFactory, typeof(Type)),
                            evResolverOverrides
                        }
                    ),

                    typeCreatedByAutoFactory
                )
            );

            var parameterizedAutoFactoryExpression = Expression.Lambda(
                autoFactoryBody,
                autoFactoryParams);

            var parameterizedAutoFactory = parameterizedAutoFactoryExpression.Compile();
            return parameterizedAutoFactory;
        }
    }
}
