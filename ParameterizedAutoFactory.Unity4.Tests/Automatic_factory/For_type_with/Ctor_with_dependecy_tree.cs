using System;
using FluentAssertions;
using Microsoft.Practices.Unity;
using ParameterizedAutoFactory.Tests.Support.InjectedTypes;
using ParameterizedAutoFactory.Unity4.Tests.Support;
using Xunit;

namespace ParameterizedAutoFactory.Tests.Automatic_factory.For_type_with;

public class Ctor_with_dependecy_tree
{
    [Fact]
    public void Overrides_only_param_of_ctor_of_product_type()
    {
        // Arrange
        var container = new ContainerBuilder().AddParameterizedAutoFactoryExtension().Build();
        var typeWithParameterlessCtor = new TypeWithParameterlessCtor();

        // Act
        var create = container.Resolve<Func<
            TypeWithParameterlessCtor,
            TypeWithCtorWithTwoDependencyParams>>();
        var instance = create(typeWithParameterlessCtor);

        // Assert
        instance.TypeWithCtorWithOneDependencyParam
            .TypeWithParameterlessCtor
            .Should().NotBeSameAs(typeWithParameterlessCtor);
    }
}
