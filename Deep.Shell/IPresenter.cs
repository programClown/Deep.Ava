namespace Deep.Shell;

public interface IPresenter
{
    Task PresentAsync(ShellView shellView, NavigationChain chain, NavigateType navigateType,
        CancellationToken cancellationToken);
}