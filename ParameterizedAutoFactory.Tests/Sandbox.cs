using System;
using FluentAssertions;
using ParameterizedAutoFactory.Tests.Support.InjectedTypes;
using Unity;
using Xunit;

namespace ParameterizedAutoFactory.Tests
{
    public class Sandbox
    {
        //[Fact]
        public void Frobnicate()
        {
            // Arrange
            var container = new UnityContainer();

            // Act
            var create = container.Resolve<Func<TypeWithParameterlessCtor>>();
            var instance = create();

            // Assert
            instance.Should().NotBeNull();
        }
    }
}
