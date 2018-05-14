using System;
using FluentAssertions;
using ParameterizedAutoFactory.Tests.Support.InjectedTypes;
using Unity;
using Xunit;

namespace ParameterizedAutoFactory.Tests.Automatic_factory_creates_instance_of
{
    public class Type_with_ctor_with_one_simple_param_where
    {
        [Fact]
        public void One_param_is_supplied_through_factory()
        {
            // Arrange
            var container = new UnityContainer();
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