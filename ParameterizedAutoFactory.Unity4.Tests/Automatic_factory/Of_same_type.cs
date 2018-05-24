using System;
using FluentAssertions;
using Microsoft.Practices.Unity;
using ParameterizedAutoFactory.Unity4.Tests.Support;
using ParameterizedAutoFactory.Unity4.Tests.Support.InjectedTypes;
using Xunit;

namespace ParameterizedAutoFactory.Unity4.Tests.Automatic_factory
{
    public class Of_same_type
    {
        [Fact]
        public void Is_resolved_to_same_factory_instance()
        {
            // Arrange
            var container = new ContainerBuilder().AddParameterizedAutoFactoryExtension().Build();

            // Act
            var create0 = container.Resolve<Func<
                TypeWithParameterlessCtor,
                TypeWithCtorWithTwoDependencyParams>>();

            var create1 = container.Resolve<Func<
                TypeWithParameterlessCtor,
                TypeWithCtorWithTwoDependencyParams>>();

            // Assert
            create1.Should().BeSameAs(create0);
        }
    }
}