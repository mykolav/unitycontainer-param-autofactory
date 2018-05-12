using System;
using FluentAssertions;
using ParameterizedAutoFactory.Tests.Support.InjectedTypes;
using Unity;
using Xunit;

namespace ParameterizedAutoFactory.Tests.Automatic_factory_creates_non_null_instance_of
{
    public class Type_with_ctor_with
    {
        [Fact]
        public void Zero_params()
        {
            // Arrange
            var container = new UnityContainer();

            // Act
            var create = container.Resolve<Func<TypeWithParameterlessCtor>>();
            var instance = create();

            // Assert
            instance.Should().NotBeNull();
        }

        [Fact]
        public void One_param()
        {
            // Arrange
            // Act
            // Assert
            throw new NotImplementedException();
        }

        [Fact]
        public void Two_params()
        {
            // Arrange
            // Act
            // Assert
            throw new NotImplementedException();
        }
    }
}