using Avalonia.Controls;

namespace Deep.Navigation.Avaloniaui.Dialogs;

public partial class DefaultDialogWindow : Window, IAvaDialogWindow
{
    public readonly static string Key = nameof(DefaultDialogWindow);
    public DefaultDialogWindow()
    {
        InitializeComponent();
    }
}