using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Ursa.Controls;

namespace Deep.Ava.ViewModels;

public partial class SciViewModel : ViewModelBase
{
    [ObservableProperty] private MenuItem? _selectedMenuItem;

    public ObservableCollection<string> TabItems { get; set; } = new()
    {
        "魔鬼斤肉人", "咸蛋超人", "大西瓜",
    };
    
    public ObservableCollection<MenuItem> MenuItems { get; set; } = new()
    {
        new MenuItem { Header = "Introduction" },
        new MenuItem { Header = "Controls", IsSeparator = true },
        new MenuItem { Header = "Badge" },
        new MenuItem { Header = "Banner" },
        new MenuItem { Header = "ButtonGroup" },
        new MenuItem { Header = "Class Input" },
        new MenuItem { Header = "Dialog" },
        new MenuItem { Header = "Divider" },
        new MenuItem { Header = "Drawer" }
    };

    private List<MenuItem> GetLeaves()
    {
        List<MenuItem> items = new();
        foreach (var item in MenuItems) items.AddRange(item.GetLeaves());

        return items;
    }
}

public class MenuItem
{
    private static readonly Random r = new();

    public MenuItem()
    {
        NavigationCommand = new AsyncRelayCommand(OnNavigate);
        IconIndex = r.Next(100);
    }

    public string? Header { get; set; }
    public int IconIndex { get; set; }
    public bool IsSeparator { get; set; }
    public ICommand NavigationCommand { get; set; }

    public ObservableCollection<MenuItem> Children { get; set; } = new();

    private async Task OnNavigate()
    {
        await MessageBox.ShowOverlayAsync(Header ?? string.Empty, "Navigation Result");
    }

    public IEnumerable<MenuItem> GetLeaves()
    {
        if (Children.Count == 0)
        {
            yield return this;
            yield break;
        }

        foreach (var child in Children)
        {
            var items = child.GetLeaves();
            foreach (var item in items) yield return item;
        }
    }
}