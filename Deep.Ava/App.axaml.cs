using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core.Plugins;
using Avalonia.Input.Platform;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Deep.Ava.ViewModels;
using Deep.Ava.Views;
using Deep.Navigation.Avaloniaui.Extensions;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Microsoft.Extensions.DependencyInjection;
using SkiaSharp;

namespace Deep.Ava;

public class App : Application
{
    public IServiceProvider? AppServiceProvider { get; private set; }

    [NotNull] public static Visual? VisualRoot { get; internal set; }

    public static IStorageProvider? StorageProvider { get; internal set; }
    public static TopLevel TopLevel => TopLevel.GetTopLevel(VisualRoot)!;

    [NotNull] public static IClipboard? Clipboard { get; internal set; }

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);

        LiveCharts.Configure(config =>
            config.HasGlobalSKTypeface(SKFontManager.Default.MatchCharacter('汉')));
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

            VisualRoot = window;
            StorageProvider = window.StorageProvider;
            Clipboard = window.Clipboard ?? throw new NullReferenceException("Clipboard is null");
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleView)
        {
            var view = AppServiceProvider.GetRequiredService<MainView>();
            view.DataContext = viewModel;
            singleView.MainView = view;

            VisualRoot = view.Parent as MainWindow;
            StorageProvider = (view.Parent as MainWindow)?.StorageProvider;
            Clipboard = (view.Parent as MainWindow)?.Clipboard ?? throw new NullReferenceException("Clipboard is null");
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void DisableAvaloniaDataAnnotationValidation()
    {
        // Get an array of plugins to remove
        var dataValidationPluginsToRemove =
            BindingPlugins.DataValidators.OfType<DataAnnotationsValidationPlugin>().ToArray();

        // remove each entry found
        foreach (var plugin in dataValidationPluginsToRemove) BindingPlugins.DataValidators.Remove(plugin);
    }
}