# Parameterized Auto Factory for the Unity IoC Container

A [UnityContainer](https://github.com/unitycontainer) extension inspired by [Autofac's Parameterized Instantiation](http://docs.autofac.org/en/latest/resolve/relationships.html#parameterized-instantiation-func-x-y-b).

# What is it for?

It lets you override certain parameters of the target type's constructor by passing them to the autogenerated factory.  
While taking advantage of the container to resolve and inject all the parameters you don't want to override.

In other words. Unity generates a factory `Func` for constructor parameters of the form `Func<IDependency>`.
```csharp
public class TargetType
{
    // Unity will generate createDependency for us.
    public TargetType(Func<IDependency> createDependency) { /* ... */ }
}
```

This extension takes it one step further. It automatically uses parameter overrides when 
- it sees a dependency of the form `Func<IDependencyOfDependency, IDependency>`
- and the concrete class implementing `IDependency` needs a parameter of type `IDependencyOfDependency`.  

This only kicks in if `Func<IDependencyOfDependency, IDependency>` is not explicitely registered.

Here is a quick example presented as an [xUnit](https://xunit.github.io/) test.

```csharp
/*  1 */ public interface IDialogBoxService { /* ... */ }
/*  2 */ public interface IUserListDataSource { /* ... */ }
/*  3 */ 
/*  4 */ public class UsersGridWindow
/*  5 */ {
/*  6 */     public UsersGridWindow(string windowTitle, 
/*  7 */                            IUserListDataSource userListDataSource, 
/*  8 */                            IDialogBoxService dialogBoxService)
/*  9 */     {
/* 10 */         WindowTitle = windowTitle;
/* 11 */         UserListDataSource = userListDataSource;
/* 12 */         DialogBoxService = dialogBoxService;
/* 13 */ 
/* 14 */         /* ... */
/* 15 */     }
/* 16 */ 
/* 17 */     public string WindowTitle { get; }
/* 18 */     public IUserListDataSource UserListDataSource { get; }
/* 19 */     public IDialogBoxService DialogBoxService { get; }
/* 20 */ 
/* 21 */     /* ... */
/* 22 */ }
/* 23 */ 
/* 24 */ public class CachedUserListDataSource : IUserListDataSource
/* 25 */ {
/* 26 */     public void WarmUp() { /* ... */ }
/* 27 */ }
/* 28 */ 
/* 29 */ [Fact]
/* 30 */ public void Factory_parameters_override_matching_constructor_parameters()
/* 31 */ {
/* 32 */     // Setup the container
/* 33 */     var container = new UnityContainer()
/* 34 */         .AddNewExtension<UnityParameterizedAutoFactoryExtension>();
/* 35 */ 
/* 36 */     // Setup expected values
/* 37 */     var expectedGadget = new Gadget();
/* 38 */ 
/* 39 */     // Here the extension kicks in and generates 
/* 40 */     // a parameterized factory of type Func<string, IUserListDataSource, UsersGridWindow>.
/* 41 */     // Of course, in a real app Func<string, IUserListDataSource, UsersGridWindow> would 
/* 42 */     // likely have been a constructor parameter.
/* 43 */     var createUsersGridWindow = container
/* 44 */         .Resolve<Func<string, IUserListDataSource, UsersGridWindow>>();
/* 45 */ 
/* 46 */     // Now, let's try to show a scenario which illustrates why
/* 47 */     // a parameterized auto-factory can be useful.
/* 48 */ 
/* 49 */     // Create and warm up a cached data source.
/* 50 */     // We can reuse the warmed-up cache for any number of UsersGridWindow instances
/* 51 */     // or other windows which depend on IUserListDataSource.
/* 52 */     var cachedUserListDataSource = new CachedUserListDataSource();
/* 53 */     cachedUserListDataSource.WarmUp();
/* 54 */ 
/* 55 */     // Pick window title for this particular window instance.
/* 56 */     // We can pick another title for another window instance.
/* 57 */     const string windowTitle = "Registered users";
/* 58 */ 
/* 59 */     // Create the window.
/* 60 */     var usersGridWindow = createUsersGridWindow(windowTitle, cachedUserListDataSource);
/* 61 */ 
/* 62 */     // Let's make sure the parameters were overridden as expected.
/* 63 */     Assert.Equal(usersGridWindow.WindowTitle, windowTitle); // We overrode this one.
/* 64 */     Assert.Same(usersGridWindow.UserListDataSource, cachedUserListDataSource); // And this one too.
/* 65 */ 
/* 66 */     Assert.NotNull(usersGridWindow.DialogBoxService); // We didn't override DialogBoxService,
/* 67 */                                                       // and so it was resolved from the container.
/* 68 */ }
```

# Download and install

## [Unity v4.0.1](https://github.com/unitycontainer/unity/tree/a370e3cd8c0f9aa5f505e896ef5225f42711d361)

In case [Unity v4.0.1](https://github.com/unitycontainer/unity/tree/a370e3cd8c0f9aa5f505e896ef5225f42711d361) is used in your project, install [ParameterizedAutoFactory.Unity4](https://www.nuget.org/packages/ParameterizedAutoFactory.Unity4) version of this extension.

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
