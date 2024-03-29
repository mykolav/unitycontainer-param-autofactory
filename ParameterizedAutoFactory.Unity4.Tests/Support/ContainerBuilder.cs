﻿using Microsoft.Practices.Unity;
using Microsoft.Practices.Unity.InterceptionExtension;
using ParameterizedAutoFactory.Unity;

namespace ParameterizedAutoFactory.Unity4.Tests.Support;

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
