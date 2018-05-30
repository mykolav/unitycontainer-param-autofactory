using System;
using FluentAssertions;
using ParameterizedAutoFactory.Tests.Support;
using ParameterizedAutoFactory.Tests.Support.InjectedTypes;
using Xunit;
#if UNITY4_0_1
using Microsoft.Practices.Unity;
#elif UNITY5_X
using Unity;
#endif

namespace ParameterizedAutoFactory.Tests.Automatic_factory
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