using System;
using FluentAssertions;
using Microsoft.Practices.Unity;
using ParameterizedAutoFactory.Unity4.Tests.Support;
using ParameterizedAutoFactory.Unity4.Tests.Support.InjectedTypes;
using Xunit;

namespace ParameterizedAutoFactory.Unity4.Tests.Automatic_factory.Creates_instance.Of_type_with
{
    public class Ctor_with_one_simple_param_where
    {
        [Fact]
        public void One_param_is_supplied_through_factory()
        {
            // Arrange
            var container = new ContainerBuilder().AddParameterizedAutoFactoryExtension().Build();
            var param0 = 9001;

            // Act
            var create = container.Resolve<Func<
                int, 
                TypeWithCtorWithOneSimpleParam>>();

            var instance = create(param0);

            // Assert
            instance.Should().NotBeNull();
            instance.Param0.Should().Be(param0);
        }
    }
}