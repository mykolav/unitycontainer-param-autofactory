using System;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using Unity.ParameterizedAutoFactory.Core;

namespace ParameterizedAutoFactory.Unity4;

/// <summary>
/// A <see cref="ResolverOverride"/> class that overrides
/// a parameter based on its type passed to a constructor
/// of the target type.
/// This checks to see
///     1) if the current type being built is the right one
///     2) if the ctor selected to instantiate the current type
///        has only one dependency (i. e. parameter) of the given type.
/// </summary>
internal class ParameterByTypeOverride : ResolverOverride
{
    private readonly InjectionParameterValue _parameterValue;

    private readonly ParameterByTypeOverrideTargetInfo _targetInfo;

    public ParameterByTypeOverride(
        Type targetType,
        Type parameterType,
        object parameterValue)
    {
        _targetInfo = new ParameterByTypeOverrideTargetInfo(
            targetType,
            parameterType,
            DynamicMethodConstructorStrategy.CreateSignatureString);

        _parameterValue = new InjectionParameter(parameterType, parameterValue);
    }

    public override IDependencyResolverPolicy GetResolver(IBuilderContext context, Type dependencyType)
    {
        if (context == null) throw new ArgumentNullException(nameof(context));

        if (!(context.CurrentOperation is BuildOperation operation) ||
            !_targetInfo.TargetTypeMatches(operation.TypeBeingConstructed))
        {
            return null;
        }

        if (!_targetInfo.ParameterTypeMatches(dependencyType))
            return null;

        var selectedConstructor = GetSelectedConstructorOrNull(context);
        // In case we did not find a constructor suitable
        // to be used by UnityContainer to create an instance of _targetType,
        // we do not try to report it ourselves.
        // Instead we just ignore it here and let Unity's code deal with the situation.
        //
        // In case we found the constructor Unity is going to use to create
        // an instance of _targetType, let's make sure the ctor has only
        // one parameter of type _parameterType.
        // If there are multiple parameters of type _parameterType,
        // it's an ambiguous case and we refuse implicitly handling it.
        if (selectedConstructor != null)
            _targetInfo.EnsureSingleParameterOfOverriddenType(selectedConstructor.Constructor);

        var resolver = _parameterValue.GetResolverPolicy(dependencyType);
        return resolver;

    }

    /// <summary>
    /// Hopefully, this method manages to find the same constructor
    /// that UnityContainer is going to use to create an instance of the target type.
    /// </summary>
    private static SelectedConstructor GetSelectedConstructorOrNull(IBuilderContext context)
    {
        var selector = context.Policies.Get<IConstructorSelectorPolicy>(
            context.OriginalBuildKey,
            out var resolverPolicyDestination);

        var selectedConstructor = selector?.SelectConstructor(
            context,
            resolverPolicyDestination);

        return selectedConstructor;
    }
}
