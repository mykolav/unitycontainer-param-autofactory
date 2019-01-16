using System;
using FluentAssertions;
using ParameterizedAutoFactory.Tests.Support;
using ParameterizedAutoFactory.Tests.Support.InjectedTypes;
using Xunit;
#if UNITY4_0_1
using Microsoft.Practices.Unity;
#elif UNITY5_X
using Unity;
using Unity.Lifetime;
#endif

namespace ParameterizedAutoFactory.Tests.Automatic_factory
{
    public class Resolved_from_child_container
    {
        [Fact]
        public void Is_not_ReferenceEquals_with_autofactory_resolved_from_parent_container()
        {
            // Arrange
            var container = new ContainerBuilder().AddParameterizedAutoFactoryExtension().Build();
            var childContainer = container.CreateChildContainer().AddParameterizedAutoFactoryExtension();

            // Act
            var autofactory0 = container.Resolve<Func<
                TypeWithParameterlessCtor,
                TypeWithCtorWithTwoDependencyParams>>();

            var autofactory1 = childContainer.Resolve<Func<
                TypeWithParameterlessCtor,
                TypeWithCtorWithTwoDependencyParams>>();

            // Assert
            autofactory1.Should().NotBeSameAs(autofactory0);
        }
        
        [Fact]
        public void Creates_product_which_is_not_ReferenceEquals_with_product_of_autofactory_resolved_from_parent_container()
        {
            // Arrange
            var container = new ContainerBuilder().AddParameterizedAutoFactoryExtension().Build();
            container.RegisterType<TypeWithCtorWithTwoDependencyParams>(new HierarchicalLifetimeManager());
            
            var childContainer = container.CreateChildContainer().AddParameterizedAutoFactoryExtension();

            var autofactory0 = container.Resolve<Func<
                TypeWithParameterlessCtor,
                TypeWithCtorWithTwoDependencyParams>>();

            var autofactory1 = childContainer.Resolve<Func<
                TypeWithParameterlessCtor,
                TypeWithCtorWithTwoDependencyParams>>();
            
            // Act
            var product0 = autofactory0(new TypeWithParameterlessCtor());
            var product1 = autofactory1(new TypeWithParameterlessCtor());

            // Assert
            product0.Should().NotBeSameAs(product1);
        }
    }
}