namespace Deep.Shell;

public interface IPresenterProvider
{
    IPresenter For(NavigateType type);
    IPresenter Remove();
}