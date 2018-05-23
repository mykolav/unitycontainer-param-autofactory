using System;
using FluentAssertions;
using Microsoft.Practices.Unity;
using ParameterizedAutoFactory.Unity4.Tests.Support.InjectedTypes;
using Xunit;

namespace ParameterizedAutoFactory.Unity4.Tests
{
    public class Sandbox
    {
        //[Fact]
        public void Frobnicate()
        {
            // Setup the container
            var container = new UnityContainer();
            container.AddNewExtension<UnityParameterizedAutoFactoryExtension>();

            // Act
            var create = container.Resolve<Func<TypeWithParameterlessCtor>>();
            var instance = create();

            // Assert
            instance.Should().NotBeNull();
        }

        public class Widget { }
        public class Gadget { }

        public class Frobnitz
        {
            public Frobnitz(Widget widget, Gadget gadget)
            {
                Widget = widget;
                Gadget = gadget;
            }

            public Widget Widget { get; }
            public Gadget Gadget { get; }
        }

        public class Wombat
        {
            private readonly Func<Gadget, Frobnitz> _createFrobnitz;

            public Wombat(Func<Gadget, Frobnitz> createFrobnitz)
            {
                _createFrobnitz = createFrobnitz;
            }

            public Frobnitz Bork(Gadget gadget)
                => _createFrobnitz(gadget);
        }

        [Fact]
        public void Value_supplied_as_factory_parameter_overrides_corresponding_constructor_parameter()
        {
            // Setup expected values
            var expectedGadget = new Gadget();

            // Setup the container
            var container = new UnityContainer()
                .AddNewExtension<UnityParameterizedAutoFactoryExtension>();

            // Let's try to get the extension to
            // generate a parameterized factory for
            // the resolved instance of class Wombat.
            var wombat = container.Resolve<Wombat>();
            var frobnitz = wombat.Bork(expectedGadget);

            // Assert
            frobnitz.Widget.Should().NotBeNull();
            frobnitz.Gadget.Should().BeSameAs(expectedGadget);
        }
    }
}
