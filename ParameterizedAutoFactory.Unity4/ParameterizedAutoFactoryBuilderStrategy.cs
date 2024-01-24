using System.Linq;
using Microsoft.Practices.ObjectBuilder2;
using Microsoft.Practices.Unity;
using ParameterizedAutoFactory.Unity4;
using Unity.ParameterizedAutoFactory.Core;

namespace ParameterizedAutoFactory.Unity;

/// <summary>
/// The <see cref="PreBuildUp" /> method of this class gets
/// called while Unity is trying to resolve a dependency of
/// the type's instance being created at that moment.
/// </summary>
internal class ParameterizedAutoFactoryBuilderStrategy : BuilderStrategy
{
    private readonly IUnityContainer _container;
    private readonly ParameterizedAutoFactoryProvider<
        IUnityContainer,
        ResolverOverride,
        ParameterByTypeOverride> _autoFactoryProvider;

    public ParameterizedAutoFactoryBuilderStrategy(IUnityContainer container = null)
    {
        _container = container;
        _autoFactoryProvider = new ParameterizedAutoFactoryProvider<
            IUnityContainer,
            ResolverOverride,
            ParameterByTypeOverride>();
    }

    public override void PreBuildUp(IBuilderContext context)
    {
        var strategy = GetOverridingStrategy(context);
        strategy.DoPreBuildUp(context);
    }

    private void DoPreBuildUp(IBuilderContext context)
    {
        var type = context.OriginalBuildKey.Type;

        const string withoutNameOrAnyName = "";

        var foundOrCreated = _autoFactoryProvider.TryBuildAutoFactory(
            typeOfAutoFactory: type,
            container: _container,
            isRegisteredInContainer: () => _container.IsRegistered(type, withoutNameOrAnyName),
            autoFactory: out var autoFactory);

        if (!foundOrCreated)
            return;

        context.Existing = autoFactory;
        context.BuildComplete = true;
    }

    private ParameterizedAutoFactoryBuilderStrategy GetOverridingStrategy(IBuilderContext context)
        => context.Strategies.OfType<ParameterizedAutoFactoryBuilderStrategy>().Last();
}
