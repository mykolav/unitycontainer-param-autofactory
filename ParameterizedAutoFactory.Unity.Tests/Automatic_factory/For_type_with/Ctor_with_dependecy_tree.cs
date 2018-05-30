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

namespace ParameterizedAutoFactory.Tests.Automatic_factory.For_type_with
{
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
}
