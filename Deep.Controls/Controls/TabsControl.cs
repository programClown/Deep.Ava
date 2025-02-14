using System.Collections;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.LogicalTree;
using Avalonia.Threading;
using Deep.Controls.Events;
using Deep.Controls.Extensions;
using Deep.Controls.Panels;

namespace Deep.Controls.Controls;

public class TabsControl : TabControl
{
    #region Constructor

    public TabsControl()
    {
        AddHandler(DragTabItem.DragStarted, ItemDragStarted, handledEventsToo: true);
        AddHandler(DragTabItem.DragDelta, ItemDragDelta);
        AddHandler(DragTabItem.DragCompleted, ItemDragCompleted, handledEventsToo: true);

        _tabsPanel = new TabsPanel(this)
        {
            ItemWidth = TabItemWidth,
            ItemOffset = AdjacentHeaderItemOffset
        };

        _tabsPanel.DragCompleted += TabsPanelOnDragCompleted;

        ItemsPanel = new FuncTemplate<Panel>(() => _tabsPanel);

        LastTabClosedAction = (_, _) => GetThisWindow()?.Close();

        _addItemCommand = new SimpleActionCommand(AddItem);
        _closeItemCommand = new SimpleParamActionCommand(CloseItem);
    }

    #endregion

    #region Constants

    private const double DefaultTabWidth = 140;

    public const double WindowsAndLinuxDefaultLeftThumbWidth = 4d;
    public const double MacOsDefaultLeftThumbWidth = 80d;

    public const double WindowsDefaultRightThumbWidth = 160d;
    public const double MacOsDefaultRightThumbWidth = 50d;

    #endregion


    #region Private Fields

    private readonly TabsPanel _tabsPanel;

    private DragTabItem? _draggedItem;
    private bool _dragging;

    private ICommand _addItemCommand;
    private ICommand _closeItemCommand;

    #endregion


    #region Avalonia Properties

    public static readonly StyledProperty<double> AdjacentHeaderItemOffsetProperty =
        AvaloniaProperty.Register<TabsControl, double>(nameof(AdjacentHeaderItemOffset));


    public static readonly StyledProperty<double> TabItemWidthProperty =
        AvaloniaProperty.Register<TabsControl, double>(nameof(TabItemWidth), DefaultTabWidth);


    public static readonly StyledProperty<bool> ShowDefaultCloseButtonProperty =
        AvaloniaProperty.Register<TabsControl, bool>(nameof(ShowDefaultCloseButton), true);


    public static readonly StyledProperty<bool> ShowDefaultAddButtonProperty =
        AvaloniaProperty.Register<TabsControl, bool>(nameof(ShowDefaultAddButton), true);


    public static readonly StyledProperty<int> FixedHeaderCountProperty =
        AvaloniaProperty.Register<TabsControl, int>(nameof(FixedHeaderCount));


    public static readonly StyledProperty<Func<Task<object>>?> NewItemAsyncFactoryProperty =
        AvaloniaProperty.Register<TabsControl, Func<Task<object>>?>(nameof(NewItemAsyncFactory));


    public static readonly StyledProperty<Func<object>?> NewItemFactoryProperty =
        AvaloniaProperty.Register<TabsControl, Func<object>?>(nameof(NewItemFactory));


    public static readonly StyledProperty<EventHandler<CloseLastTabEventArgs>?> LastTabClosedActionProperty =
        AvaloniaProperty.Register<TabsControl, EventHandler<CloseLastTabEventArgs>?>(nameof(LastTabClosedAction));


    public static readonly StyledProperty<double> LeftThumbWidthProperty =
        AvaloniaProperty.Register<TabsControl, double>(nameof(LeftThumbWidth),
            OperatingSystem.IsMacOS()
                ? MacOsDefaultLeftThumbWidth
                : WindowsAndLinuxDefaultLeftThumbWidth);


    public static readonly StyledProperty<double> RightThumbWidthProperty =
        AvaloniaProperty.Register<TabsControl, double>(nameof(RightThumbWidth),
            OperatingSystem.IsWindows() ? WindowsDefaultRightThumbWidth : MacOsDefaultRightThumbWidth);


    public static readonly DirectProperty<TabsControl, ICommand> AddItemCommandProperty =
        AvaloniaProperty.RegisterDirect<TabsControl, ICommand>(
            nameof(AddItemCommand),
            o => o.AddItemCommand,
            (o, v) => o.AddItemCommand = v);


    public static readonly DirectProperty<TabsControl, ICommand> CloseItemCommandProperty =
        AvaloniaProperty.RegisterDirect<TabsControl, ICommand>(
            nameof(CloseItemCommand),
            o => o.CloseItemCommand,
            (o, v) => o.CloseItemCommand = v);

    #endregion


    #region Public Properties

    public double AdjacentHeaderItemOffset
    {
        get => GetValue(AdjacentHeaderItemOffsetProperty);
        set => SetValue(AdjacentHeaderItemOffsetProperty, value);
    }


    public double TabItemWidth
    {
        get => GetValue(TabItemWidthProperty);
        set => SetValue(TabItemWidthProperty, value);
    }


    public bool ShowDefaultCloseButton
    {
        get => GetValue(ShowDefaultCloseButtonProperty);
        set => SetValue(ShowDefaultCloseButtonProperty, value);
    }


    public bool ShowDefaultAddButton
    {
        get => GetValue(ShowDefaultAddButtonProperty);
        set => SetValue(ShowDefaultAddButtonProperty, value);
    }


    public Func<Task<object>>? NewItemAsyncFactory
    {
        get => GetValue(NewItemAsyncFactoryProperty);
        set => SetValue(NewItemAsyncFactoryProperty, value);
    }


    public Func<object>? NewItemFactory
    {
        get => GetValue(NewItemFactoryProperty);
        set => SetValue(NewItemFactoryProperty, value);
    }


    public EventHandler<CloseLastTabEventArgs>? LastTabClosedAction
    {
        get => GetValue(LastTabClosedActionProperty);
        set => SetValue(LastTabClosedActionProperty, value);
    }


    /// <summary>
    ///     Allows a the first adjacent tabs to be fixed (no dragging, and default close button will not show).
    /// </summary>
    public int FixedHeaderCount
    {
        get => GetValue(FixedHeaderCountProperty);
        set => SetValue(FixedHeaderCountProperty, value);
    }


    public double LeftThumbWidth
    {
        get => GetValue(LeftThumbWidthProperty);
        set => SetValue(LeftThumbWidthProperty, value);
    }


    public double RightThumbWidth
    {
        get => GetValue(RightThumbWidthProperty);
        set => SetValue(RightThumbWidthProperty, value);
    }


    public ICommand AddItemCommand
    {
        get => _addItemCommand;
        private set => SetAndRaise(AddItemCommandProperty, ref _addItemCommand, value);
    }


    public ICommand CloseItemCommand
    {
        get => _closeItemCommand;
        private set => SetAndRaise(CloseItemCommandProperty, ref _closeItemCommand, value);
    }

    #endregion


    #region Protected Methods

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);

        var leftDragWindowThumb = e.NameScope.Get<Thumb>("PART_LeftDragWindowThumb");
        leftDragWindowThumb.DragDelta += WindowDragThumbOnDragDelta;
        leftDragWindowThumb.DoubleTapped += WindowDragThumbOnDoubleTapped;

        var rightDragWindowThumb = e.NameScope.Get<Thumb>("PART_RightDragWindowThumb");
        rightDragWindowThumb.DragDelta += WindowDragThumbOnDragDelta;
        rightDragWindowThumb.DoubleTapped += WindowDragThumbOnDoubleTapped;
    }


    protected override Control CreateContainerForItemOverride(object? item, int index, object? recycleKey)
    {
        return new DragTabItem();
    }


    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == AdjacentHeaderItemOffsetProperty)
            _tabsPanel.ItemOffset = AdjacentHeaderItemOffset;
        else if (change.Property == TabItemWidthProperty) _tabsPanel.ItemWidth = TabItemWidth;
    }

    #endregion


    #region Private Methods

    private void RemoveItem(DragTabItem container)
    {
        var item = ItemFromContainer(container);

        if (item == null)
            return;

        if (ItemsSource is not IList itemsList)
            return;

        var removedItemIndex = itemsList.IndexOf(item);
        var removedItemIsSelected = SelectedItem == item;

        itemsList.Remove(item);

        if (itemsList.Count == 0)
            LastTabClosedAction?.Invoke(this, new CloseLastTabEventArgs(GetThisWindow()));
        else if (removedItemIsSelected)
            SetSelectedNewTab(itemsList, removedItemIndex);
    }


    private void SetSelectedNewTab(IList items, int removedItemIndex)
    {
        SelectedItem = removedItemIndex == items.Count ? items[^1] : items[removedItemIndex];
    }


    private Window? GetThisWindow()
    {
        return this.FindLogicalAncestorOfType<Window>();
    }


    private IEnumerable<DragTabItem> DragTabItems()
    {
        foreach (var item in Items)
        {
            var container = ContainerFromItem(item);

            if (container is DragTabItem dragTabItem)
                yield return dragTabItem;
        }
    }


    private void ItemDragStarted(object? sender, DragTabDragStartedEventArgs e)
    {
        _draggedItem = e.TabItem;

        e.Handled = true;

        _draggedItem.IsSelected = true;

        var item = ItemFromContainer(_draggedItem);

        if (item != null)
        {
            if (item is TabItem tabItem)
                tabItem.IsSelected = true;

            SelectedItem = item;
        }
    }


    private void ItemDragDelta(object? sender, DragTabDragDeltaEventArgs e)
    {
        if (_draggedItem is null)
            throw new Exception($"{nameof(TabsControl)}.{nameof(ItemDragDelta)} - _draggedItem is null");

        if (_draggedItem.LogicalIndex < FixedHeaderCount)
        {
            e.Handled = true;
            return;
        }

        if (!_dragging)
        {
            _dragging = true;
            SetDraggingItem(_draggedItem);
        }

        _draggedItem.X += e.DragDeltaEventArgs.Vector.X;
        _draggedItem.Y += e.DragDeltaEventArgs.Vector.Y;

        Dispatcher.UIThread.Post(() => _tabsPanel.InvalidateMeasure(), DispatcherPriority.Loaded);

        e.Handled = true;
    }


    private void ItemDragCompleted(object? sender, DragTabDragCompletedEventArgs e)
    {
        foreach (var item in DragTabItems())
        {
            item.IsDragging = false;
            item.IsSiblingDragging = false;
        }

        Dispatcher.UIThread.Post(() => _tabsPanel.InvalidateMeasure(), DispatcherPriority.Loaded);

        _dragging = false;
    }


    private void SetDraggingItem(DragTabItem draggedItem)
    {
        foreach (var item in DragTabItems())
        {
            item.IsDragging = false;
            item.IsSiblingDragging = true;
        }

        draggedItem.IsDragging = true;
        draggedItem.IsSiblingDragging = false;
    }


    private void TabsPanelOnDragCompleted()
    {
        MoveTabModelsIfNeeded();

        _draggedItem = null;
    }


    private void MoveTabModelsIfNeeded()
    {
        var item = ItemFromContainer(_draggedItem);

        if (item != null)
        {
            var container = _draggedItem;

            if (ItemsSource is IList list)
                if (container.LogicalIndex != list.IndexOf(item))
                {
                    list.Remove(item);
                    list.Insert(container.LogicalIndex, item);

                    SelectedItem = item;

                    var i = 0;

                    foreach (var dragTabItem in DragTabItems())
                        dragTabItem.LogicalIndex = i++;
                }
        }
    }


    private void WindowDragThumbOnDoubleTapped(object? sender, RoutedEventArgs e)
    {
        var window = this.FindLogicalAncestorOfType<Window>();

        window?.RestoreWindow();
    }


    private void WindowDragThumbOnDragDelta(object? sender, VectorEventArgs e)
    {
        var window = this.FindLogicalAncestorOfType<Window>();

        window?.DragWindow(e.Vector.X, e.Vector.Y);
    }


    private void AddItem()
    {
        if (NewItemAsyncFactory is not null)
        {
            NewItemAsyncFactory.Invoke().ContinueWith(t => { AddItem(t.Result); },
                TaskScheduler.FromCurrentSynchronizationContext());

            return;
        }

        AddItem(NewItemFactory?.Invoke());
    }


    private void AddItem(object? newItem)
    {
        ArgumentNullException.ThrowIfNull(newItem);

        if (ItemsSource is IList itemsList)
            itemsList.Add(newItem);

        SelectedItem = newItem;
    }


    private void CloseItem(object? tabItemSource)
    {
        ArgumentNullException.ThrowIfNull(tabItemSource);

        if (tabItemSource is not DragTabItem tabItem)
            return;

        RemoveItem(tabItem);
    }

    #endregion
}