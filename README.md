# Parameterized Auto Factory for the Unity IoC Container

A [UnityContainer](https://github.com/unitycontainer) extension inspired by [Autofac's Parameterized Instantiation](http://docs.autofac.org/en/latest/resolve/relationships.html#parameterized-instantiation-func-x-y-b).

# What is it for?

It lets you supply certain parameters of the target type's constructor by passing them to the auto-generated factory. While taking advantage of the container to resolve and inject all the other parameters that you don't want to supply manually.

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
public interface IDialogBoxService { /* ... */ }
public interface IUserListDataSource { /* ... */ }

public class UsersGridWindow
{
    public UsersGridWindow(string windowTitle, 
                           IUserListDataSource userListDataSource, 
                           IDialogBoxService dialogBoxService)
    {
        WindowTitle = windowTitle;
        UserListDataSource = userListDataSource;
        DialogBoxService = dialogBoxService;

        /* ... */
    }

    public string WindowTitle { get; }
    public IUserListDataSource UserListDataSource { get; }
    public IDialogBoxService DialogBoxService { get; }

    /* ... */
}

public class CachedUserListDataSource : IUserListDataSource
{
    public void WarmUp() { /* ... */ }
}

[Fact]
public void Factory_parameters_overrides_matching_constructor_parameters()
{
    // Setup the container
    var container = new UnityContainer()
        .AddNewExtension<UnityParameterizedAutoFactoryExtension>();

    // Here the extension kicks in and generates 
    // a parameterized factory of type Func<string, IUserListDataSource, UsersGridWindow>.
    // Of course, in a real app Func<string, IUserListDataSource, UsersGridWindow> would 
    // likely have been a constructor parameter.
    var createUsersGridWindow = container
        .Resolve<Func<string, IUserListDataSource, UsersGridWindow>>();

    // Now, let's try to show a scenario which illustrates why
    // a parameterized auto-factory can be useful.

    // Create and warm up a cached data source.
    // We can re-use the warmed up cache for any number of UsersGridWindow instances
    // or other windows which depend on IUserListDataSource.
    var cachedUserListDataSource = new CachedUserListDataSource();
    cachedUserListDataSource.WarmUp();

    // Pick window title for this particular window instance.
    // We can pick another title for another window instance.
    const string windowTitle = "Registered users";

    // Create the window.
    var usersGridWindow = createUsersGridWindow(windowTitle, cachedUserListDataSource);

    // Let's make sure the parameters were overridden as expected.
    Assert.Equal(usersGridWindow.WindowTitle, windowTitle); // We overrode this one.
    Assert.Same(usersGridWindow.UserListDataSource, cachedUserListDataSource); // And this one too.

    Assert.NotNull(usersGridWindow.DialogBoxService); // We didn't override DialogBoxService,
                                                      // and so it was resolved from the container.
}
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

The extension is licensed under the MIT license.
