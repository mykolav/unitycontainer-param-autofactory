using System;
using FluentAssertions;
using Microsoft.Practices.Unity;
using ParameterizedAutoFactory.Tests.Support.InjectedTypes;
using ParameterizedAutoFactory.Unity4.Tests.Support;
using Xunit;

namespace ParameterizedAutoFactory.Tests.Automatic_factory.Creates_instance.When_null_supplied;

public class To_type_with_ctor_with_two_dependency_params
{
    [Fact]
    public void As_first_param_through_factory()
    {
        // Arrange
        var container = new ContainerBuilder().AddParameterizedAutoFactoryExtension().Build();

        // Act
        var create = container.Resolve<Func<
            TypeWithParameterlessCtor,
            TypeWithCtorWithTwoDependencyParams>>();
        var instance = create(null);

        // Assert
        instance.Should().NotBeNull();
        instance.TypeWithParameterlessCtor.Should().BeNull();
        instance.TypeWithCtorWithOneDependencyParam.Should().NotBeNull();
    }

    [Fact]
    public void As_second_param_through_factory()
    {
        // Arrange
        var container = new ContainerBuilder().AddParameterizedAutoFactoryExtension().Build();

        // Act
        var create = container.Resolve<Func<
            TypeWithCtorWithOneDependencyParam,
            TypeWithCtorWithTwoDependencyParams>>();
        var instance = create(null);

        // Assert
        instance.Should().NotBeNull();
        instance.TypeWithParameterlessCtor.Should().NotBeNull();
        instance.TypeWithCtorWithOneDependencyParam.Should().BeNull();
    }

    [Fact]
    public void As_both_params_through_factory()
    {
        // Arrange
        var container = new ContainerBuilder().AddParameterizedAutoFactoryExtension().Build();

        // Act
        var create = container.Resolve<Func<
            TypeWithParameterlessCtor,
            TypeWithCtorWithOneDependencyParam,
            TypeWithCtorWithTwoDependencyParams>>();
        var instance = create(null, null);

        // Assert
        instance.Should().NotBeNull();
        instance.TypeWithParameterlessCtor.Should().BeNull();
        instance.TypeWithCtorWithOneDependencyParam.Should().BeNull();
    }
}
