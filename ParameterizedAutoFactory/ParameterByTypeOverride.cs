using System;
using System.Linq;
using System.Reflection;
using Unity.Builder;
using Unity.Builder.Selection;
using Unity.ObjectBuilder.BuildPlan.DynamicMethod.Creation;
using Unity.Policy;
using Unity.Resolution;

namespace ParameterizedAutoFactory
{
    /// <summary>
    /// A <see cref="ResolverOverride"/> class that overrides
    /// a parameter based on its type passed to a constructor
    /// of the target type.
    /// This checks to see if the current type being built is the right one
    /// </summary>
    public class ParameterByTypeOverride : ResolverOverride
    {
        private readonly Type _targetType;
        private readonly Type _parameterType;

        public ParameterByTypeOverride(
            Type targetType,
            Type parameterType,
            object parameterValue)
            : base(null, parameterValue)
        {
            _targetType = targetType;
            _parameterType = parameterType;
        }

        public override IResolverPolicy GetResolver(IBuilderContext context, Type dependencyType)
        {
            if (context == null) throw new ArgumentNullException(nameof(context));

            if (!(context.CurrentOperation is BuildOperation operation) ||
                 operation.TypeBeingConstructed != _targetType)
            {
                return null;
            }

            if (dependencyType != _parameterType)
                return null;

            var selectedConstructor = GetSelectedConstructorOrNull(context);
            // In case we did not find a constructor suitable
            // to be used by UnityContainer to create an instance of _targetType,
            // we do not try to report it ourself.
            // Instead we just ignore it here and let Unity's code deal with the situation.
            // 
            // In case we found the constructor Unity is going to use to create
            // an instance of _targetType, let's make sure the ctor has only
            // one parameter of type _parameterType.
            // If there are multiple parameters of type _parameterType,
            // it's an ambiguous case and we refuse implicitly handling it.
            if (selectedConstructor != null)
                EnsureSingleParameterOfOverriddenType(selectedConstructor.Constructor);

            var resolver = Value.GetResolverPolicy(dependencyType);
            return resolver;

        }

        private void EnsureSingleParameterOfOverriddenType(ConstructorInfo constructor)
        {
            var constructorParameters = constructor.GetParameters();
            var ctorParametersOfType = constructorParameters
                .Where(ctorParameter => ctorParameter.ParameterType == _parameterType)
                .ToList();

            if (ctorParametersOfType.Count <= 1)
                return;

            var constructorSignature = DynamicMethodConstructorStrategy
                .CreateSignatureString(constructor);

            throw new InvalidOperationException(
                $"The constructor {constructorSignature} " +
                $"has {ctorParametersOfType.Count} parameters " +
                $"of type {_parameterType.FullName}." +
                $"{Environment.NewLine}" +
                "Do not know which one you meant to override."
            );
        }

        /// <summary>
        /// Hopefully, this method manages to find the same constructor
        /// that UnityContainer is going to use to create an instance of <see cref="_targetType"/>
        /// </summary>
        private SelectedConstructor GetSelectedConstructorOrNull(IBuilderContext context)
        {
            var selector = context.Policies.GetPolicy<IConstructorSelectorPolicy>(
                context.OriginalBuildKey, 
                out var resolverPolicyDestination);

            var selectedConstructor = selector?.SelectConstructor(
                context, 
                resolverPolicyDestination);

            return selectedConstructor;
        }
    }
}
