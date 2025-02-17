using Avalonia.Controls;

namespace Deep.Shell;

public interface ISelectableHostItems : IHostItems
{
    object? SelectedItem { get; set; }
    event EventHandler<SelectionChangedEventArgs> SelectionChanged;
}