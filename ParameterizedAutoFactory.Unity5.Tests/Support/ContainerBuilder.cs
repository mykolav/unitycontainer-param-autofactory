using ParameterizedAutoFactory.Unity;
using Unity;
using Unity.Interception;

namespace ParameterizedAutoFactory.Unity5.Tests.Support;

internal class ContainerBuilder
{
    private bool _addParameterizedAutoFactoryExtension;
    private bool _addInterception;

    public ContainerBuilder AddParameterizedAutoFactoryExtension()
    {
        _addParameterizedAutoFactoryExtension = true;
        return this;
    }

    public ContainerBuilder AddInterception()
    {
        _addInterception = true;
        return this;
    }

    public IUnityContainer Build()
    {
        var container = new UnityContainer();

        container.AddNewExtension<Diagnostic>();

        if (_addInterception)
            container.AddNewExtension<Interception>();

        if (_addParameterizedAutoFactoryExtension)
            container.AddParameterizedAutoFactoryExtension();

        return container;
    }
}

internal static class UnityContainerExtensions
{
    internal static IUnityContainer AddParameterizedAutoFactoryExtension(this IUnityContainer container)
        => container.AddNewExtension<UnityParameterizedAutoFactoryExtension>();
}
