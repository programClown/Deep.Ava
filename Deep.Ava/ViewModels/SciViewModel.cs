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
    public Func<object> NewItemFactory => AddItem;

    public ObservableCollection<TabItemModel> TabItems { get; set; } = new()
    {
        new TabItemModel("Tab 1", "This is the content of Tab 1."),
        new TabItemModel("Tab 2", "This is the content of Tab 2."),
        new TabItemModel("Tab 3", "This is the content of Tab 3.")
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

    private object AddItem()
    {
        return new TabItemModel("Tab new",
                "Tab content")
            ;
    }

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

public class TabItemModel(string header, string content)
{
    public string Header { get; set; } = header;
    public string Content { get; set; } = content;
}