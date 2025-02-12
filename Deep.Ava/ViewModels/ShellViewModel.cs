using System;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Controls.Notifications;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Deep.Ava.Models;
using WindowNotificationManager = Ursa.Controls.WindowNotificationManager;

namespace Deep.Ava.ViewModels;

public partial class ShellViewModel : ViewModelBase
{
    private const int MaxOutputCount = 200;
    [ObservableProperty] private ObservableCollection<ConsoleText> _outputs;


    public ShellViewModel()
    {
        NotificationManager = new WindowNotificationManager(App.TopLevel) { MaxItems = 3 };

        Outputs = new ObservableCollection<ConsoleText>
        {
            new("窗前明月光"),
            new("疑是地上霜"),
            new("举头望明月"),
            new("低头思故乡")
        };
    }

    public WindowNotificationManager? NotificationManager { get; set; }

    [RelayCommand]
    private void ClearOutput()
    {
        Outputs.Clear();
    }

    [RelayCommand]
    private void CopyOutput()
    {
        if (Outputs.Count == 0) return;
        var texs = Outputs.Select(o => o.Text);
        var outStr = string.Join(Environment.NewLine, texs);
        App.Clipboard.SetTextAsync(outStr);
        NotificationManager?.Show(
            new Notification("Welcome", "This is message"),
            showIcon: true,
            showClose: true,
            type: NotificationType.Success);
    }
}