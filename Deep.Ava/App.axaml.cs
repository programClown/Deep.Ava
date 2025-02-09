using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Deep.Ava.ViewModels;
using Deep.Ava.Views;
using Deep.Navigation.Avaloniaui.Extensions;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;

namespace Deep.Ava;

public class App : Application
{
    public IServiceProvider? AppServiceProvider { get; private set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAvaNavigationSupport()
            .AddSingleton<MainWindow>()
            .AddSingleton<MainView>()
            .AddSingleton<MainViewModel>();
        AppServiceProvider = services.BuildServiceProvider();

        var viewModel = AppServiceProvider.GetRequiredService<MainViewModel>();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Avoid duplicate validations from both Avalonia and the CommunityToolkit. 
            // More info: https://docs.avaloniaui.net/docs/guides/development-guides/data-validation#manage-validationplugins
            DisableAvaloniaDataAnnotationValidation();
            var window = AppServiceProvider.GetRequiredService<MainWindow>();
            window.DataContext = viewModel;
            desktop.MainWindow = window;
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (DataAnnotationsValidationPlugin plugin in dataValidationPluginsToRemove)
        {
            BindingPlugins.DataValidators.Remove(plugin);
        }
    }
}