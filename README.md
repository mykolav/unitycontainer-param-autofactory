# Parameterized Auto Factory for the Unity IoC Container

A [UnityContainer](https://github.com/unitycontainer) extension inspired by [Autofac's Parameterized Instantiation](http://docs.autofac.org/en/latest/resolve/relationships.html#parameterized-instantiation-func-x-y-b).

# What is it for?

It lets you manually pass certain parameters to the constructor of a dependency which is being instantiated by the autogenerated factory.  
While at the same time taking advantage of the container to inject all the parameters you don't want to pass manually.  

Here is a quick example presented as an [xUnit](https://xunit.github.io/) test.

```csharp
/*  1 */ public class Widget { }
/*  2 */ public class Gadget { }
/*  3 */ 
/*  4 */ public class Frobnitz
/*  5 */ {
/*  6 */     public Frobnitz(Widget widget, Gadget gadget)
/*  7 */     {
/*  8 */         Widget = widget;
/*  9 */         Gadget = gadget;
/* 10 */     }
/* 11 */ 
/* 12 */     public Widget Widget { get; }
/* 13 */     public Gadget Gadget { get; }
/* 14 */ }
/* 15 */ 
/* 16 */ public class Wombat
/* 17 */ {
/* 18 */     private readonly Func<Gadget, Frobnitz> _createFrobnitz;
/* 19 */ 
/* 20 */     public Wombat(Func<Gadget, Frobnitz> createFrobnitz)
/* 21 */     {
/* 22 */         _createFrobnitz = createFrobnitz;
/* 23 */     }
/* 24 */ 
/* 25 */     public Frobnitz Bork(Gadget gadget)
/* 26 */         => _createFrobnitz(gadget);
/* 27 */ }
/* 28 */ 
/* 29 */ [Fact]
/* 30 */ public void Value_supplied_as_factory_parameter_overrides_matching_constructor_parameter()
/* 31 */ {
/* 32 */     // Setup expected values.
/* 33 */     var expectedGadget = new Gadget();
/* 34 */ 
/* 35 */     // Setup the container and add the extension.
/* 36 */     var container = new UnityContainer()
/* 37 */         .AddNewExtension<UnityParameterizedAutoFactoryExtension>();
/* 38 */ 
/* 39 */     // Let's try to get the extension to
/* 40 */     // generate a parameterized factory for
/* 41 */     // the resolved instance of class Wombat.
/* 42 */     var wombat = container.Resolve<Wombat>();
/* 43 */     var frobnitz = wombat.Bork(expectedGadget);
/* 44 */ 
/* 45 */     // Assert
/* 46 */     frobnitz.Widget.Should().NotBeNull();              // Widget was injected by the container.
/* 47 */     frobnitz.Gadget.Should().BeSameAs(expectedGadget); // Gadget was supplied through 
/* 48 */                                                        // the param of _createFrobnitz.
/* 49 */ } 
```

Now, let's break down our example a little.  

- The `Wombat` class has a dependency on `Func<Gadget, Frobnitz>`.  
- When an instance of `Wombat` is being resolved from the container on line 42, `ParameterizedAutoFactory` kicks in and generates an implementation of the `Func<...>` &mdash; which is the *parameterized autofactory*.  
- When invoked on line 26, the generated autofactory implementation `_createFrobnitz` passes its `Gadget` parameter's value to the parameter `gadget` of `Frobnitz`'s constructor (line 6). 
- The `gadget` parameter of `Frobnitz` (line 6) was picked up based on its type matching the `Gadget` type of the autofactory's parameter.
- The `Widget` parameter of `Frobnitz`'s constructor does not match any parameter of the `Func<Gadget, Frobnitz>` autofactory. As a result it is injected by the container (in contrast to being supplied by the factory).
- If the `Wombat` class had a dependency on `Func<Gadget, Widget, Frobnitz>`, both `Frobnitz`'s parameters whould have been supplied by the autofactory generated for this dependency.  


# Download and install

## [Unity v4.0.1](https://github.com/unitycontainer/unity/tree/a370e3cd8c0f9aa5f505e896ef5225f42711d361)

In case [Unity v4.0.1](https://github.com/unitycontainer/unity/tree/a370e3cd8c0f9aa5f505e896ef5225f42711d361) is used in your project, install [ParameterizedAutoFactory.Unity4](https://www.nuget.org/packages/ParameterizedAutoFactory.Unity5) version of this extension.

To install it run the following command in the [NuGet Package Manager Console](https://docs.microsoft.com/en-us/nuget/tools/package-manager-console).

```powershell
Install-Package ParameterizedAutoFactory.Unity4
```

## [Unity v5.x](https://github.com/unitycontainer/unity/tree/v5.x)

In case [Unity v5.x](https://github.com/unitycontainer/unity/tree/v5.x) is used in your project, install [ParameterizedAutoFactory.Unity5](https://www.nuget.org/packages/ParameterizedAutoFactory.Unity5) version of this extension.

To install it run the following command in the [NuGet Package Manager Console](https://docs.microsoft.com/en-us/nuget/tools/package-manager-console).

```powershell
Install-Package ParameterizedAutoFactory.Unity5
```
   
This will download all the binaries, and add necessary references to your project.


# How to use it?

One way to register the extension in container is to use the `Unity.UnityContainerExtensions.AddNewExtension` extension method.

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

# Thank you!

- This extension is inspired by [Autofac's Parameterized Instantiation](http://docs.autofac.org/en/latest/resolve/relationships.html#parameterized-instantiation-func-x-y-b).
- [UnityAutoMoq](https://github.com/thedersen/UnityAutoMoq) gave a great example of extension hooking into Unity's dependencies instantiation/resolution pipeline.

# License

The [ParameterizedAutoFactory](https://github.com/mykolav/unitycontainer-param-autofactory) extension is licensed under the MIT license.  
So it can be used freely in commercial applications.
