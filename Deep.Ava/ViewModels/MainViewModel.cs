using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Deep.Navigation.Abstracts;
using Deep.Navigation.Core;
using Deep.Navigation.Extensions;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Deep.Ava.ViewModels;

public partial class MainViewModel : ViewModelBase, IServiceAware
{
    private readonly ILogger<MainViewModel> _logger;
    private readonly NavigationService _navigationService;
    private readonly IRegionManager _regionManager;

    [ObservableProperty]
    private IModule? _module;

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
        });
        _regionManager.NavigationSubscribe<IRegion>(r =>
        {
            _logger.LogDebug($"New region : {r.Name}");
        });
    }

    public ObservableCollection<IModule> Modules { get; set; }
    public IServiceProvider ServiceProvider { get; }

    [RelayCommand]
    private void ToView(string content)
    {
        string viewName = content;
        bool requestNew = false;
        if (viewName.EndsWith(".RequestNew"))
        {
            viewName = content.Replace(".RequestNew", string.Empty);
            requestNew = true;
        }
        _navigationService.RequestViewNavigation("ContentRegion", viewName, requestNew);
    }
}