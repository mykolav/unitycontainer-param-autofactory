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
    public class Ctor_with_two_dependency_params_where
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
            var create = container.Resolve<Func<TypeWithCtorWithTwoDependencyParams>>();
            var instance = create();

            // Assert
            instance.Should().NotBeNull();
            instance.TypeWithParameterlessCtor.Should().NotBeNull();
            instance.TypeWithCtorWithOneDependencyParam.Should().NotBeNull();
        }

        [Fact]
        public void First_param_is_supplied_through_factory()
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
            instance.Should().NotBeNull();
            instance.TypeWithParameterlessCtor.Should().BeSameAs(typeWithParameterlessCtor);
            instance.TypeWithCtorWithOneDependencyParam.Should().NotBeNull();
        }

        [Fact]
        public void Second_param_is_supplied_through_factory()
        {
            // Arrange
            var container = new ContainerBuilder().AddParameterizedAutoFactoryExtension().Build();
            var typeWithCtorWithOneDependencyParam = 
                new TypeWithCtorWithOneDependencyParam(
                    new TypeWithParameterlessCtor()
                );

            // Act
            var create = container.Resolve<Func<
                TypeWithCtorWithOneDependencyParam,
                TypeWithCtorWithTwoDependencyParams>>();
            var instance = create(typeWithCtorWithOneDependencyParam);

            // Assert
            instance.Should().NotBeNull();
            instance.TypeWithParameterlessCtor.Should().NotBeNull();
            instance.TypeWithCtorWithOneDependencyParam
                .Should()
                .BeSameAs(typeWithCtorWithOneDependencyParam);
        }

        [Fact]
        public void Both_params_are_supplied_through_factory()
        {
            // Arrange
            var container = new ContainerBuilder().AddParameterizedAutoFactoryExtension().Build();
            var typeWithParameterlessCtor = new TypeWithParameterlessCtor();
            var typeWithCtorWithOneDependencyParam = 
                new TypeWithCtorWithOneDependencyParam(
                    new TypeWithParameterlessCtor()
                );

            // Act
            var create = container.Resolve<Func<
                TypeWithParameterlessCtor,
                TypeWithCtorWithOneDependencyParam,
                TypeWithCtorWithTwoDependencyParams>>();
            var instance = create(
                typeWithParameterlessCtor,
                typeWithCtorWithOneDependencyParam);

            // Assert
            instance.Should().NotBeNull();
            instance.TypeWithParameterlessCtor.Should().BeSameAs(typeWithParameterlessCtor);
            instance.TypeWithCtorWithOneDependencyParam
                .Should()
                .BeSameAs(typeWithCtorWithOneDependencyParam);
        }
    }
}
