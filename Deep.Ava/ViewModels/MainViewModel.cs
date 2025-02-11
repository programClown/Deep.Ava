using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Deep.Ava.Views;
using Deep.Navigation.Abstracts;
using Deep.Navigation.Core;
using Deep.Navigation.Extensions;
using Microsoft.Extensions.Logging;

namespace Deep.Ava.ViewModels;

public partial class MainViewModel : ViewModelBase, IServiceAware
{
    private readonly ILogger<MainViewModel> _logger;
    private readonly NavigationService _navigationService;
    private readonly IRegionManager _regionManager;

    private readonly Dictionary<string, UserControl> _viewDictionary = new()
    {
        ["首页"] = new HomeView
        {
            DataContext = new HomeViewModel()
        },
        ["绘图"] = new ChartView
        {
            DataContext = new ChartViewModel()
        },
        ["科学"] = new SciView
        {
            DataContext = new SciViewModel()
        },
        ["终端"] = new ShellView
        {
            DataContext = new ShellViewModel()
        },
        ["设置"] = new SettingView
        {
            DataContext = new SettingViewModel()
        }
    };


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

        _regionManager.NavigationSubscribe<IRegion>(r => { _logger.LogDebug($"New region : {r.Name}"); }
        );
    }

    public ObservableCollection<IModule> Modules { get; set; }
    public IServiceProvider ServiceProvider { get; }

    [RelayCommand]
    private void ToView(string content)
    {
        if (string.IsNullOrEmpty(content)) return;
        PageContent = _viewDictionary[content];
    }

    #region 导航控制

    private string getNaviName(object value)
    {
        if (value is not StackPanel stackPanel || stackPanel.Children[1] is not TextBlock { Text: not null } textBlock)
            return string.Empty;

        return textBlock.Text;
    }

    partial void OnNavigationSelectedItemChanged(object? value)
    {
        if (value == null) return;
        NavigationFooterSelectedItem = null;
        ToView(getNaviName(value));
    }

    partial void OnNavigationFooterSelectedItemChanged(object? value)
    {
        if (value == null) return;
        NavigationSelectedItem = null;
        ToView(getNaviName(value));
    }

    #endregion
}