using System;
using System.Linq;
using System.Reflection;

namespace Unity.ParameterizedAutoFactory.Core;

/// <summary>
/// This class helps override a parameter of the target type's constructor based on that parameter's type.
/// It contains code that is possible to share across Unity v4.x and Unity v5.x.
/// </summary>
internal class ParameterByTypeOverrideTargetInfo
{
    private readonly Type _targetType;
    private readonly TypeInfo _targetTypeInfo;
    private readonly Type _parameterType;
    private readonly Func<ConstructorInfo, string> _createSignatureString;

    public ParameterByTypeOverrideTargetInfo(
        Type targetType,
        Type parameterType,
        Func<ConstructorInfo, string> createSignatureString)
    {
        _targetType = targetType;
        _targetTypeInfo = targetType.GetTypeInfo();
        _parameterType = parameterType;
        _createSignatureString = createSignatureString;
    }

    public bool TargetTypeMatches(Type type)
    {
        if (_targetType == type)
            return true;

        // The branch of code below covers scenario where
        // the autofactory's result type is an interface.
        // E. g., `Func<TArg0, ..., IService>`
        // and Unity resolved `IService` to `ServiceImpl1`
        var matches =
            _targetTypeInfo.IsInterface
            && _targetTypeInfo.IsAssignableFrom(type.GetTypeInfo());

        return matches;
    }
    public bool ParameterTypeMatches(Type type) => _parameterType == type;

    public void EnsureSingleParameterOfOverriddenType(ConstructorInfo constructor)
    {
        var constructorParameters = constructor.GetParameters();
        var ctorParametersOfType = constructorParameters
            .Where(ctorParameter => ctorParameter.ParameterType == _parameterType)
            .ToList();

        if (ctorParametersOfType.Count <= 1)
            return;

        var constructorSignature = _createSignatureString(constructor);

        throw new InvalidOperationException(
            $"The constructor {constructorSignature} " +
            $"has {ctorParametersOfType.Count} parameters " +
            $"of type {_parameterType.FullName}." +
            $"{Environment.NewLine}" +
            "Do not know which one you meant to override."
        );
    }
}
