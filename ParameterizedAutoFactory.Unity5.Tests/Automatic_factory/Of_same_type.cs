using System;
using FluentAssertions;
using ParameterizedAutoFactory.Unity5.Tests.Support;
using ParameterizedAutoFactory.Unity5.Tests.Support.InjectedTypes;
using Unity;
using Xunit;

namespace ParameterizedAutoFactory.Unity5.Tests.Automatic_factory
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