﻿using System;
using FluentAssertions;
using ParameterizedAutoFactory.Unity;
using Unity;
using Unity.Resolution;
using Xunit;

namespace ParameterizedAutoFactory.Unity5.Tests;

public class Sandbox
{
    public interface IDialogBoxService { }
    public interface IUserListDataSource { }

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

    //[Fact]
    internal void Factory_parameters_override_matching_constructor_parameters()
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
        var cachedUserListDataSource = new CachedUserListDataSource();
        cachedUserListDataSource.WarmUp();

        // Pick window title for this particular window instance.
        // We can pick another title for another window instance.
        const string windowTitle = "Registered users";

        // Create the window.
        var usersGridWindow = createUsersGridWindow(windowTitle, cachedUserListDataSource);

        // Let's make sure the parameters were overriden as expected.
        Assert.Equal(expected: windowTitle, usersGridWindow.WindowTitle); // We overrode this one.
        Assert.Same(expected: cachedUserListDataSource, usersGridWindow.UserListDataSource); // And this one too.

        Assert.NotNull(usersGridWindow.DialogBoxService); // We didn't override DialogBoxService,
        // and so it was resolved from the container.
    }

    public interface INavigationService { }
    public interface IPageDialogService { }

    public class NavigationService : INavigationService { }
    public class PageDialogService : IPageDialogService { }

    public class Page
    {
        public Page(INavigationService navigationService, IPageDialogService pageDialogService)
        {
            NavigationServcie = navigationService;
            PageDialogService = pageDialogService;
        }

        public INavigationService NavigationServcie { get; }
        public IPageDialogService PageDialogService { get; }
    }

    public class Frame
    {
        public Frame(Page page, INavigationService navigationService, IPageDialogService pageDialogService)
        {
            Page = page;
            NavigationService = navigationService;
            PageDialogService = pageDialogService;
        }

        public Page Page { get; }
        public INavigationService NavigationService { get; }
        public IPageDialogService PageDialogService { get; }
    }

    [Fact]
    public void CanCreatePageUsingParameterOverride()
    {
        // Arrange
        var container = new UnityContainer()
            .AddNewExtension<UnityParameterizedAutoFactoryExtension>()
            .RegisterType<INavigationService, NavigationService>()
            .RegisterType<IPageDialogService, PageDialogService>();

        var overriddenNavigationService = new NavigationService();
        var overriddenPageDialogService = new PageDialogService();

        var overrides = new[]
        {
            new ParameterOverride(
                parameterType: typeof(INavigationService),
                parameterValue: overriddenNavigationService
            ).OnType<Frame>(),

            new ParameterOverride(
                parameterType: typeof(IPageDialogService),
                parameterValue: overriddenPageDialogService
            ).OnType<Frame>(),
        };

        // Act
        var frame = container.Resolve<Frame>(overrides);

        // Assert
        Assert.Same(expected: overriddenNavigationService, frame.NavigationService);
        Assert.Same(expected: overriddenPageDialogService, frame.PageDialogService);

        Assert.NotSame(expected: overriddenNavigationService, frame.Page.NavigationServcie);
        Assert.NotSame(expected: overriddenPageDialogService, frame.Page.PageDialogService);
    }

    [Fact]
    public void CanCreatePageUsingParameterByTypeOverride()
    {
        // Arrange
        var container = new UnityContainer()
            .AddNewExtension<UnityParameterizedAutoFactoryExtension>()
            .RegisterType<INavigationService, NavigationService>()
            .RegisterType<IPageDialogService, PageDialogService>();

        var overriddenNavigationService = new NavigationService();
        var overriddenPageDialogService = new PageDialogService();

        var overrides = new ResolverOverride[]
        {
            new ParameterByTypeOverride(
                targetType: typeof(Frame),
                parameterType: typeof(INavigationService),
                parameterValue: overriddenNavigationService),

            new ParameterByTypeOverride(
                targetType: typeof(Frame),
                parameterType: typeof(IPageDialogService),
                parameterValue: overriddenPageDialogService),
        };

        // Act
        var frame = container.Resolve<Frame>(overrides);

        // Assert
        Assert.Same(expected: overriddenNavigationService, frame.NavigationService);
        Assert.Same(expected: overriddenPageDialogService, frame.PageDialogService);

        Assert.NotSame(expected: overriddenNavigationService, frame.Page.NavigationServcie);
        Assert.NotSame(expected: overriddenPageDialogService, frame.Page.PageDialogService);
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

    public class Gizmo
    {
        public Gizmo(Frobnitz frobnitz, Widget widget, Gadget gadget)
        {
            Frobnitz = frobnitz;
            Widget = widget;
            Gadget = gadget;
        }

        public Frobnitz Frobnitz { get; }
        public Widget Widget { get; }
        public Gadget Gadget { get; }
    }

    [Fact]
    public void BorkGizmo()
    {
        // Arrange
        var container = new UnityContainer()
            .AddNewExtension<UnityParameterizedAutoFactoryExtension>();

        var overriddenWidget = new Widget();
        var overriddenGadget = new Gadget();

        var overrides = new[]
        {
            new ParameterOverride(typeof(Widget), overriddenWidget).OnType<Gizmo>(),
            new ParameterOverride(typeof(Gadget), overriddenGadget).OnType<Gizmo>(),
        };

        // Act
        var gizmo = container.Resolve<Gizmo>(overrides);

        // Assert
        Assert.Same(gizmo.Widget, overriddenWidget);
        Assert.Same(gizmo.Gadget, overriddenGadget);

        Assert.NotSame(gizmo.Frobnitz.Widget, overriddenWidget);
        Assert.NotSame(gizmo.Frobnitz.Gadget, overriddenGadget);
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
