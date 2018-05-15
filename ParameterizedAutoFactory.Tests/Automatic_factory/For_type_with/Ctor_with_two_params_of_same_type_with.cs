using System;
using FluentAssertions;
using ParameterizedAutoFactory.Tests.Support;
using ParameterizedAutoFactory.Tests.Support.InjectedTypes;
using Unity;
using Unity.Exceptions;
using Xunit;

namespace ParameterizedAutoFactory.Tests.Automatic_factory.For_type_with
{
    public class Ctor_with_two_params_of_same_type_with
    {
        /// <summary>
        /// Make sure we didn't introduce regressions into Unity's built-in Func resolution.
        /// </summary>
        [Fact]
        public void Zero_params_creates_instance()
        {
            // Arrange
            var container = new ContainerBuilder().AddParameterizedAutoFactoryExtension().Build();

            // Act
            var create = container.Resolve<Func<TypeWithCtorWithTwoDependencyParamsOfSameType>>();
            var instance = create();

            // Assert
            instance.Should().NotBeNull();
            instance.TypeWithParameterlessCtor.Should().NotBeNull();
            instance.TypeWithCtorWithOneDependencyParam0.Should().NotBeNull();
            instance.TypeWithCtorWithOneDependencyParam1.Should().NotBeNull();
        }

        [Fact]
        public void One_param_of_that_type_throws()
        {
            // Arrange
            var container = new ContainerBuilder().AddParameterizedAutoFactoryExtension().Build();
            var param0 = new TypeWithCtorWithOneDependencyParam(new TypeWithParameterlessCtor());

            // Act
            var create = container.Resolve<Func<
                TypeWithCtorWithOneDependencyParam,
                TypeWithCtorWithTwoDependencyParamsOfSameType>>();

            // Assert
            Assert.Throws<ResolutionFailedException>(() => create(param0));
        }

        [Fact]
        public void One_param_of_another_type_creates_instance()
        {
            // Arrange
            var container = new ContainerBuilder().AddParameterizedAutoFactoryExtension().Build();
            var param0 = new TypeWithParameterlessCtor();

            // Act
            var create = container.Resolve<Func<
                TypeWithParameterlessCtor,
                TypeWithCtorWithTwoDependencyParamsOfSameType>>();
            var instance = create(param0);

            // Assert
            instance.Should().NotBeNull();
            instance.TypeWithParameterlessCtor.Should().BeSameAs(param0);
            instance.TypeWithCtorWithOneDependencyParam0.Should().NotBeNull();
            instance.TypeWithCtorWithOneDependencyParam1.Should().NotBeNull();
        }

        [Fact]
        public void Two_params_of_that_type_throws()
        {
            // Arrange
            var container = new ContainerBuilder().AddParameterizedAutoFactoryExtension().Build();
            var param0 = new TypeWithCtorWithOneDependencyParam(new TypeWithParameterlessCtor());
            var param1 = new TypeWithCtorWithOneDependencyParam(new TypeWithParameterlessCtor());

            // Act
            var create = container.Resolve<Func<
                TypeWithCtorWithOneDependencyParam,
                TypeWithCtorWithOneDependencyParam,
                TypeWithCtorWithTwoDependencyParamsOfSameType>>();

            // Assert
            Assert.Throws<ResolutionFailedException>(() => create(param0, param1));
        }
    }
}