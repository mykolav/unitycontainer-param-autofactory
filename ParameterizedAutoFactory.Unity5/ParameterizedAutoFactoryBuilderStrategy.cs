using Unity;
using Unity.Builder;
using Unity.ParameterizedAutoFactory.Core;
using Unity.Resolution;
using Unity.Strategies;

namespace ParameterizedAutoFactory.Unity5;

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

    public override void PreBuildUp(ref BuilderContext context)
    {
        var container = context.Container;
        var type = context.Type;

        const string withoutNameOrAnyName = "";
        var foundOrCreated = _autoFactoryProvider.TryGetOrBuildAutoFactoryCreator(
            typeOfAutoFactory: type,
            isRegisteredInContainer: () => container.IsRegistered(type, withoutNameOrAnyName),
            createAutoFactory: out var createAutoFactory);

        if (!foundOrCreated)
            return;

        var autoFactory = createAutoFactory(container);

        context.Existing = autoFactory;
        context.BuildComplete = true;
    }
}
