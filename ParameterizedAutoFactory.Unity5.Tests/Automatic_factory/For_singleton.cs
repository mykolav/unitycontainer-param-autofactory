using System;
using FluentAssertions;
using ParameterizedAutoFactory.Tests.Support.InjectedTypes;
using ParameterizedAutoFactory.Unity5.Tests.Support;
using Xunit;
using Unity;
using Unity.Lifetime;

namespace ParameterizedAutoFactory.Tests.Automatic_factory
{
    public class For_singleton
    {
        [Fact]
        public void Produces_same_instance_of_product_for_same_args()
        {
            // Arrange
            var container = new ContainerBuilder().AddParameterizedAutoFactoryExtension().Build();
            container.RegisterType<TypeWithCtorWithTwoDependencyParams>(new ContainerControlledLifetimeManager());

            var autofactory0 = container.Resolve<Func<
                TypeWithParameterlessCtor,
                TypeWithCtorWithTwoDependencyParams>>();
            
            // Act
            var factoryArgument = new TypeWithParameterlessCtor();
            var product0 = autofactory0(factoryArgument);
            var product1 = autofactory0(factoryArgument);

            // Assert
            product0.Should().BeSameAs(product1);
        }
        
        
        // <remarks>
        // <example>
        // <code>
        // container.RegisterSingleton{TypeWithCtorWithTwoDependencyParams}();
        // var product0 = container.Resolve{TypeWithCtorWithTwoDependencyParams}(new DependencyOverride{TypeWithParameterlessCtor}(new TypeWithParameterlessCtor()));
        // var product1 = container.Resolve{TypeWithCtorWithTwoDependencyParams}(new DependencyOverride{TypeWithParameterlessCtor}(new TypeWithParameterlessCtor()));
        // </code>
        // </example>
        // As a result of executing the code snippet above <c>product0</c> and <c>product1</c> reference the same object.
        // I believe such a behavior to be a design flaw in Unity.
        // It should either:
        // - give different instances or
        // - throw an exception on attempt to resolve a singleton with a dependency override
        //   that is different from the one supplied when resolving for the first time.
        // <para/>
        // Consistent with the current Unity's behavior, the following test fails.
        // </remarks>
        // [Fact]
        // public void Produces_different_instance_of_product_for_different_args()
        // {
        //     // Arrange
        //     var container = new ContainerBuilder().AddParameterizedAutoFactoryExtension().Build();
        //     container.RegisterSingleton<TypeWithCtorWithTwoDependencyParams>();
        // 
        //     var autofactory0 = container.Resolve<Func<
        //         TypeWithParameterlessCtor,
        //         TypeWithCtorWithTwoDependencyParams>>();
        //     
        //     // Act
        //     var product0 = autofactory0(new TypeWithParameterlessCtor());
        //     var product1 = autofactory0(new TypeWithParameterlessCtor());
        // 
        //     // Assert
        //     product0.Should().NotBeSameAs(product1);
        // }
    }
}