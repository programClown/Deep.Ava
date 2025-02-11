using Avalonia.Controls;
using Avalonia.Interactivity;
using Ursa.Controls;

namespace Deep.Ava.Views;

public partial class MainView : UserControl
{
    public MainView()
    {
        InitializeComponent();
    }

    private void SelectionList_Loaded(object? sender, RoutedEventArgs e)
    {
        if (sender is SelectionList selectionList)
        {
            selectionList.SelectedIndex = 0;
        }
    }
}