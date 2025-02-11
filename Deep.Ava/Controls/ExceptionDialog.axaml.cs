using Avalonia.Controls;
using Avalonia.Interactivity;
using Deep.Ava.ViewModels;
using System.Diagnostics.CodeAnalysis;

namespace Deep.Ava.Controls;

public partial class ExceptionDialog : UserControl
{
    public ExceptionDialog()
    {
        InitializeComponent();
    }

    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    private async void CopyButton_OnClick(object? sender, RoutedEventArgs e)
    {
        string? content = (DataContext as ExceptionDialogViewModel)?.FormatAsMarkdown();
        if (content is not null && App.Clipboard is not null)
        {
            await App.Clipboard.SetTextAsync(content);
        }
    }

    [SuppressMessage("ReSharper", "UnusedParameter.Local")]
    private void ExitButton_OnClick(object? sender, RoutedEventArgs e)
    {
    }
}