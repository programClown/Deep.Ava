using System.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;

namespace Deep.Shell;

public static class HostedItemsHelper
{
    public static bool CanBeHosted(Type viewType)
    {
        return viewType.IsSubclassOf(typeof(ItemsControl)) || typeof(IHostItems).IsAssignableFrom(viewType);
    }

    public static bool CanBeHosted(object view)
    {
        return view is ItemsControl or SelectingItemsControl or IHostItems or ISelectableHostItems;
    }

    public static IHostItems? GetHostedItems(object? control)
    {
        if (GetSelectableHostedItems(control) is { } casted) return casted;

        if (control is IHostItems hostedItems)
            return hostedItems;
        if (control is ItemsControl itemsControl)
            return new ItemsControlProxy(itemsControl);

        return null;
    }

    public static ISelectableHostItems? GetSelectableHostedItems(object? control)
    {
        if (control is ISelectableHostItems selectableHostedItem)
            return selectableHostedItem;
        if (control is SelectingItemsControl selectingItemsControl)
            return new SelectingItemsControlProxy(selectingItemsControl);

        return null;
    }

    private class ItemsControlProxy(ItemsControl itemsControl) : IHostItems
    {
        public IEnumerable? ItemsSource
        {
            get => itemsControl.ItemsSource;
            set => itemsControl.ItemsSource = value;
        }

        public ItemCollection Items => itemsControl.Items;
    }

    private class SelectingItemsControlProxy(SelectingItemsControl itemsControl)
        : ItemsControlProxy(itemsControl), ISelectableHostItems
    {
        public object? SelectedItem
        {
            get => itemsControl.SelectedItem;
            set => itemsControl.SelectedItem = value;
        }

        public event EventHandler<SelectionChangedEventArgs>? SelectionChanged
        {
            add => itemsControl.SelectionChanged += value;
            remove => itemsControl.SelectionChanged -= value;
        }
    }
}