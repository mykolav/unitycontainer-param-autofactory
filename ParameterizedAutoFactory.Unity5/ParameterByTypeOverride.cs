using System;
using System.Linq;
using System.Reflection;
using Unity;
using Unity.Builder;
using Unity.Injection;
using Unity.Processors;
using Unity.Resolution;

namespace ParameterizedAutoFactory.Unity5
{
    /// <summary>
    /// A <see cref="ResolverOverride"/> class that overrides
    /// a parameter based on its type passed to a constructor
    /// of the target type.
    /// This makes sure when the parameter's value is being resolved the code checks
    /// whether the ctor selected to instantiate the current type
    /// has only one parameter of the given type.
    /// </summary>
    internal class ParameterByTypeOverride : ParameterOverride
    {
        public ParameterByTypeOverride(
            Type targetType,
            Type parameterType,
            object parameterValue)
            : base(parameterType, new ParameterByTypeOverrideResolve(parameterType, parameterValue))
        {
            OnType(targetType);
        }
    }

    /// <summary>
    /// This code checks whether the ctor selected to instantiate the target type
    /// has only one parameter of the given type.
    /// </summary>
    internal class ParameterByTypeOverrideResolve : IResolve
    {
        private readonly Type _parameterType;
        private readonly object _parameterValue;

        public ParameterByTypeOverrideResolve(Type parameterType, object parameterValue)
        {
            _parameterType = parameterType;
            _parameterValue = parameterValue;
        }

        public object Resolve<TContext>(ref TContext context) 
            where TContext : IResolveContext
        {
            if (!(context is BuilderContext builderContext))
                return _parameterValue;
            if (!(builderContext.Container is UnityContainer container))
                return _parameterValue;

            // Hopefully, we'll manage to find the same constructor
            // that UnityContainer is going to use to create an instance of the target type.
            var selector = new ConstructorProcessor(policySet: builderContext.Registration, container);

            var selection = selector
                .Select(builderContext.Type, builderContext.Registration)
                .FirstOrDefault();

            ConstructorInfo constructorInfo = null;
            switch (selection)
            {
                case ConstructorInfo ci:
                    constructorInfo = ci;
                    break;

                case MethodBase<ConstructorInfo> im:
                    constructorInfo = im.MemberInfo(builderContext.Type);
                    break;
            }

            // In case we did not find a constructor suitable
            // to be used by UnityContainer to create an instance of the target type,
            // we do not try to report it ourselves.
            // Instead we just ignore it here and let Unity's code deal with the situation.
            // 
            // In case we found the constructor Unity is going to use to create
            // an instance of the target type, let's make sure the ctor has only
            // one parameter of type _parameterType.
            // If there are multiple parameters of type _parameterType,
            // it's an ambiguous case and we refuse implicitly handling it.
            if (constructorInfo != null)
                EnsureSingleParameterOfOverriddenType(constructorInfo);

            return _parameterValue;
        }

        private void EnsureSingleParameterOfOverriddenType(ConstructorInfo constructor)
        {
            var constructorParameters = constructor.GetParameters();
            var ctorParametersOfType = constructorParameters
                .Where(ctorParameter => ctorParameter.ParameterType == _parameterType)
                .ToList();

            if (ctorParametersOfType.Count <= 1)
                return;

            var constructorSignature = CreateSignatureString(constructor);

            throw new ResolutionFailedException(
                type: constructor.DeclaringType,
                name: null,
                message: 
                    $"The constructor {constructorSignature} " +
                    $"has {ctorParametersOfType.Count} parameters " +
                    $"of type {_parameterType.FullName}." +
                    $"{Environment.NewLine}" +
                    "Do not know which one you meant to override."
            );
        }

        private static string CreateSignatureString(ConstructorInfo constructor)
        {
            var parameters = constructor.GetParameters();
            var parameterSignatures = new string[parameters.Length];

            for (int i = 0; i < parameters.Length; i++)
                parameterSignatures[i] = $"{parameters[i].ParameterType.FullName} {parameters[i].Name}";

            string fullName = constructor.DeclaringType.FullName;
            return $"{fullName}({string.Join(", ", parameterSignatures)})";
        }
    }
}
