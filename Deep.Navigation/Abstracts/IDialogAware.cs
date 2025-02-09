
namespace Deep.Navigation.Abstracts
{
    public interface IDialogAware
    {
        string Title { get; }
        event Action<IDialogResult>? RequestClose;
        void OnDialogClosed();
        void OnDialogOpened(IDialogParameters? parameters);
    }
}
