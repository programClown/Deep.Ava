using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Deep.Navigation.Abstracts;
using Deep.Navigation.Core;
using Deep.Navigation.Extensions;
using Microsoft.Extensions.Logging;

namespace Deep.Ava.ViewModels;

public partial class MainViewModel : ViewModelBase, IServiceAware
{
    readonly private ILogger<MainViewModel> _logger;
    readonly private NavigationService _navigationService;
    readonly private IRegionManager _regionManager;


    [ObservableProperty] private IModule? _module;
    [ObservableProperty] private double _navigationBarWidth = 150;
    [ObservableProperty] private object? _navigationFooterSelectedItem;
    [ObservableProperty] private object? _navigationSelectedItem;
    [ObservableProperty] private UserControl? _pageContent;

    public MainViewModel(IEnumerable<IModule> modules,
        IServiceProvider serviceProvider,
        NavigationService navigationService,
        IRegionManager regionManager,
        ILogger<MainViewModel> logger)
    {
        ServiceProvider = serviceProvider;
        _navigationService = navigationService;
        _regionManager = regionManager;
        _logger = logger;

        // default view
        _navigationService.RequestViewNavigation("ContentRegion", "ViewAlpha");
        Modules = new ObservableCollection<IModule>(modules);

        _regionManager.NavigationSubscribe<NavigationContext>(n =>
            {
                _logger.LogDebug($"Request to : {n.RegionName}.{n.TargetViewName}");
            }
        );

        _regionManager.NavigationSubscribe<IRegion>(r =>
            {
                _logger.LogDebug($"New region : {r.Name}");
            }
        );
    }

    public ObservableCollection<IModule> Modules { get; set; }
    public IServiceProvider ServiceProvider { get; }

    #region 导航控制

    partial void OnNavigationSelectedItemChanged(object? value)
    {
        NavigationFooterSelectedItem = null;
    }

    partial void OnNavigationFooterSelectedItemChanged(object? value)
    {
        NavigationSelectedItem = null;
    }

    #endregion

    [RelayCommand]
    private void ToView(string content)
    {
        var viewName = content;
        var requestNew = false;
        if (viewName.EndsWith(".RequestNew"))
        {
            viewName = content.Replace(".RequestNew", string.Empty);
            requestNew = true;
        }

        _navigationService.RequestViewNavigation("ContentRegion", viewName, requestNew);
    }
}