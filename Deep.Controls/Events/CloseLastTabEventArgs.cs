using Avalonia.Controls;

namespace Deep.Controls.Events;

public class CloseLastTabEventArgs(Window? window) : EventArgs
{
    public Window? Window { get; } = window;
}