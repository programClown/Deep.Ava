using System.ComponentModel;

namespace Deep.Navigation.Abstracts
{
    public interface IDialogWindow
    {
        string? Title { get; set; }
        object? Content { get; set; }
        object? DataContext { get; set; }
        event EventHandler Closed;
        void Close();
        void Show();
    }
}
