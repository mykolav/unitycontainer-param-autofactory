using System;
using FluentAssertions;
using ParameterizedAutoFactory.Tests.Support;
using ParameterizedAutoFactory.Tests.Support.InjectedTypes;
using Unity;
using Xunit;

namespace ParameterizedAutoFactory.Tests.Automatic_factory_creates_instance_of
{
    public class Type_with_ctor_with_one_dependency_param_where
    {
        /// <summary>
        /// Make sure we didn't introduce regressions into Unity's built-in Func resolution.
        /// </summary>
        [Fact]
        public void Zero_params_are_supplied_through_factory()
        {
            // Arrange
            var container = new ContainerBuilder().AddParameterizedAutoFactoryExtension().Build();

            // Act
            var create = container.Resolve<Func<TypeWithCtorWithOneDependencyParam>>();
            var instance = create();

            // Assert
            instance.Should().NotBeNull();
            instance.TypeWithParameterlessCtor.Should().NotBeNull();
        }

        [Fact]
        public void One_param_is_supplied_through_factory()
        {
            // Arrange
            var container = new ContainerBuilder().AddParameterizedAutoFactoryExtension().Build();
            var typeWithParameterlessCtor = new TypeWithParameterlessCtor();

            // Act
            var create = container.Resolve<Func<
                TypeWithParameterlessCtor,
                TypeWithCtorWithOneDependencyParam>>();

            var instance = create(typeWithParameterlessCtor);

            // Assert
            instance.Should().NotBeNull();
            instance.TypeWithParameterlessCtor.Should().BeSameAs(typeWithParameterlessCtor);
        }
    }

}