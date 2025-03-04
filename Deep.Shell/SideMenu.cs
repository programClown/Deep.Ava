using System.Collections;
using System.Diagnostics;
using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Controls.Primitives;
using Avalonia.Controls.Templates;
using Deep.Shell.Data;

namespace Deep.Shell;

public class SideMenu : TemplatedControl
{
    private ListBox _listBox;

    protected override void OnApplyTemplate(TemplateAppliedEventArgs e)
    {
        base.OnApplyTemplate(e);
        _listBox = e.NameScope.Find<ListBox>("PART_Items")
                   ?? throw new KeyNotFoundException("PART_Items not found in SideMenu template");

        SetupUi();
    }

    private void SetupUi()
    {
        _listBox!.ItemsSource ??= new AvaloniaList<object>();
        _listBox!.SelectionChanged += OnSelectionChanged;
    }

    private void OnSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);
        Debug.WriteLine(change.Property.Name);
    }

    #region HeaderTemplate

    public static readonly StyledProperty<IDataTemplate> HeaderTemplateProperty =
        AvaloniaProperty.Register<SideMenu, IDataTemplate>(
            nameof(HeaderTemplate));

    public IDataTemplate HeaderTemplate
    {
        get => GetValue(HeaderTemplateProperty);
        set => SetValue(HeaderTemplateProperty, value);
    }

    #endregion

    #region Header

    public static readonly StyledProperty<object?> HeaderProperty =
        AvaloniaProperty.Register<SideMenu, object?>(
            nameof(Header));

    public object? Header
    {
        get => GetValue(HeaderProperty);
        set => SetValue(HeaderProperty, value);
    }

    #endregion

    #region FooterTemplate

    public static readonly StyledProperty<IDataTemplate> FooterTemplateProperty =
        AvaloniaProperty.Register<SideMenu, IDataTemplate>(
            nameof(FooterTemplate));

    public IDataTemplate FooterTemplate
    {
        get => GetValue(FooterTemplateProperty);
        set => SetValue(FooterTemplateProperty, value);
    }

    #endregion

    #region Footer

    public static readonly StyledProperty<object?> FooterProperty =
        AvaloniaProperty.Register<SideMenu, object?>(
            nameof(Footer));

    public object? Footer
    {
        get => GetValue(FooterProperty);
        set => SetValue(FooterProperty, value);
    }

    #endregion

    #region Items

    private IList<SideMenuItem> _items;

    public static readonly DirectProperty<SideMenu, IList<SideMenuItem>> ItemsProperty =
        AvaloniaProperty.RegisterDirect<SideMenu, IList<SideMenuItem>>(
            nameof(Items),
            o => o.Items,
            (o, v) => o.Items = v);

    public IList<SideMenuItem> Items
    {
        get => _items;
        set => SetAndRaise(ItemsProperty, ref _items, value);
    }

    #endregion

    #region SelectedItem

    private SideMenuItem? _selectedItem;

    public static readonly DirectProperty<SideMenu, SideMenuItem?> SelectedItemProperty =
        AvaloniaProperty.RegisterDirect<SideMenu, SideMenuItem?>(
            nameof(SelectedItem),
            o => o.SelectedItem,
            (o, v) => o.SelectedItem = v);

    public SideMenuItem? SelectedItem
    {
        get => _selectedItem;
        set => SetAndRaise(SelectedItemProperty, ref _selectedItem, value);
    }

    #endregion

    #region ContentsTemplate

    public static readonly StyledProperty<IDataTemplate> ContentsTemplateProperty =
        AvaloniaProperty.Register<SideMenu, IDataTemplate>(
            nameof(ContentsTemplate));

    public IDataTemplate ContentsTemplate
    {
        get => GetValue(ContentsTemplateProperty);
        set => SetValue(ContentsTemplateProperty, value);
    }

    #endregion

    #region Contents

    public static readonly StyledProperty<IList> ContentsProperty =
        AvaloniaProperty.Register<SideMenu, IList>(
            nameof(Contents));

    public IList Contents
    {
        get => GetValue(ContentsProperty);
        set => SetValue(ContentsProperty, value);
    }

    #endregion
}