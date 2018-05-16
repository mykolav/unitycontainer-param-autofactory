using System;
using System.Linq;
using Unity.Builder;
using Unity.Builder.Selection;
using Unity.ObjectBuilder.BuildPlan.DynamicMethod.Creation;
using Unity.Policy;
using Unity.Resolution;

namespace ParameterizedAutoFactory
{
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

            if (context.CurrentOperation is BuildOperation operation &&
                operation.TypeBeingConstructed == _targetType)
            {
                if (dependencyType == _parameterType)
                {
                    EnsureSingleParameterOfTargetType(context);

                    var resolver = Value.GetResolverPolicy(dependencyType);
                    return resolver;
                }
            }

            return null;
        }

        private void EnsureSingleParameterOfTargetType(IBuilderContext context)
        {
            var selectedConstructor = GetSelectedConstructorOrNull(context);
            if (selectedConstructor != null)
            {
                var constructorParameters = selectedConstructor.Constructor.GetParameters();
                var ctorParametersOfType = constructorParameters
                    .Where(ctorParameter => ctorParameter.ParameterType == _parameterType)
                    .ToList();

                if (ctorParametersOfType.Count > 1)
                {
                    var constructorSignature = DynamicMethodConstructorStrategy
                        .CreateSignatureString(selectedConstructor.Constructor);

                    throw new InvalidOperationException(
                        $"The constructor {constructorSignature} " +
                        $"has {ctorParametersOfType.Count} parameters " +
                        $"of type {_parameterType.FullName}." +
                        $"{Environment.NewLine}" +
                        "Do not know which one you meant to override."
                    );
                }
            }
        }

        private SelectedConstructor GetSelectedConstructorOrNull(
            IBuilderContext context)
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