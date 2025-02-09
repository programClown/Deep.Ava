using Avalonia.Controls;
using Avalonia.Interactivity;
using Deep.Navigation.Abstracts;

namespace Deep.Navigation.Avaloniaui.Dialogs
{
    public interface IAvaDialogWindow : IDialogWindow
    {
        //event EventHandler<WindowClosingEventArgs>? Closing;
        //event EventHandler<RoutedEventArgs>? Loaded;
        Task<TResult> ShowDialog<TResult>(Window owner);
        Task ShowDialog(Window owner);
    }
}
