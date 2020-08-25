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


namespace ParameterizedAutoFactory.Tests.Automatic_factory.Creates_instance.Of_type_with
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