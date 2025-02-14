using Avalonia.Interactivity;
using Deep.Controls.Controls;

namespace Deep.Controls.Events;

public abstract class DragTabItemEventArgs : RoutedEventArgs
{
    protected DragTabItemEventArgs(DragTabItem dragTabItem)
    {
        TabItem = dragTabItem ?? throw new ArgumentNullException(nameof(dragTabItem));
    }

    protected DragTabItemEventArgs(RoutedEvent routedEvent, DragTabItem tabItem)
        : base(routedEvent)
    {
        TabItem = tabItem;
    }

    protected DragTabItemEventArgs(RoutedEvent routedEvent, Interactive source, DragTabItem tabItem)
        : base(routedEvent, source)
    {
        TabItem = tabItem;
    }


    public DragTabItem TabItem { get; }
}