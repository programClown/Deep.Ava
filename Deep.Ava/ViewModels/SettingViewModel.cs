using System.Collections.Generic;
using System.Linq;
using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using Deep.Toolkit.HardwareInfo;
using Semi.Avalonia;

namespace Deep.Ava.ViewModels;

public partial class SettingViewModel : ViewModelBase
{
    [ObservableProperty] private string _glVersion = $"{HardwareHelper.GetCpuInfoAsync().Result.ProcessorCaption}";
    [ObservableProperty] private string _gpuDeviceDetail = $"{HardwareHelper.IterGpuInfo().FirstOrDefault()}";
    [ObservableProperty] private ThemeVariant? _selectedThemeVariant = ThemeVariant.Default;
    [ObservableProperty] private bool _useCustomAccent;

    public IEnumerable<ThemeVariant> ThemeVariants =>
    [
        ThemeVariant.Default,
        ThemeVariant.Light,
        ThemeVariant.Dark,
        SemiTheme.Aquatic,
        SemiTheme.Desert,
        SemiTheme.Dusk,
        SemiTheme.NightSky
    ];


    partial void OnSelectedThemeVariantChanged(ThemeVariant? value)
    {
        if (value != null) Application.Current.RequestedThemeVariant = value;
    }
}