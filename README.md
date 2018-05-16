# Parameterized Auto Factory for the Unity IoC Container

A [UnityContainer](https://github.com/unitycontainer) extension inspired by [Autofac's Parameterized Instantiation](http://docs.autofac.org/en/latest/resolve/relationships.html#parameterized-instantiation-func-x-y-b)

# What is it for?

Here is a quick example presented as an [xUnit](https://xunit.github.io/) test.

```csharp
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
public void Value_supplied_as_factory_parameter_overrides_matching_constructor_parameter()
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

```

Now let's break down our example a little.  
[TODO]

# How to use it?

One way to register the extension in container is to use the `Unity.UnityContainerExtensions.AddNewExtension` extension method

```csharp
var container = new UnityContainer()
    .AddNewExtension<UnityParameterizedAutoFactoryExtension>();

```

A call to `Unity.UnityContainerExtensions.AddNewExtension` parameterized by `UnityParameterizedAutoFactoryExtension` is approximately equivalent to executing the snippet below. 
```csharp
var extension = (UnityParameterizedAutoFactoryExtension)container.Resolve(
    typeof(UnityParameterizedAutoFactoryExtension));
container.AddExtension(extension);

```

# License

The [ParameterizedAutoFactory](https://github.com/mykolav/unitycontainer-param-autofactory) extension is licensed under the MIT license.  
So it can be used freely in commercial applications.
