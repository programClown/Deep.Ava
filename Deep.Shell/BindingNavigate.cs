using System.ComponentModel;
using System.Windows.Input;
using Avalonia;
using Avalonia.Animation;
using Avalonia.VisualTree;

namespace Deep.Shell;

[TypeConverter(typeof(BindingNavigateConverter))]
public class BindingNavigate : AvaloniaObject, ICommand
{
    private bool _singletonCanExecute = true;
    private EventHandler? _singletonCanExecuteChanged;

    public AvaloniaObject? Sender { get; internal set; }
    public string Path { get; set; }
    public NavigateType? Type { get; set; }
    public IPageTransition? Transition { get; set; }

    public event EventHandler? CanExecuteChanged
    {
        add => _singletonCanExecuteChanged += value;
        remove => _singletonCanExecuteChanged -= value;
    }

    public bool CanExecute(object? parameter)
    {
        return _singletonCanExecute;
    }

    public void Execute(object? parameter)
    {
        ExecuteAsync(parameter, CancellationToken.None);
    }

    public async Task ExecuteAsync(object? parameter, CancellationToken cancellationToken)
    {
        if (Sender is not Visual visual) return;
        if (visual.FindAncestorOfType<ShellView>() is not { } shell) return;

        _singletonCanExecute = false;
        _singletonCanExecuteChanged?.Invoke(this, EventArgs.Empty);
        try
        {
            if (parameter != null)
                await shell.Navigator.NavigateAsync(
                    Path,
                    Type,
                    parameter,
                    Sender,
                    true,
                    Transition,
                    cancellationToken);
            else
                await shell.Navigator.NavigateAsync(
                    Path,
                    Type,
                    Sender,
                    true,
                    Transition,
                    cancellationToken);
        }
        finally
        {
            _singletonCanExecute = true;
            _singletonCanExecuteChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public static implicit operator BindingNavigate(string path)
    {
        return new BindingNavigate
        {
            Path = path
        };
    }
}