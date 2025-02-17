namespace Deep.Shell.Presenters;

public class RemovePresenter : PresenterBase
{
    public override Task PresentAsync(ShellView shellView,
        NavigationChain chain,
        NavigateType navigateType,
        CancellationToken cancellationToken)
    {
        return shellView?.RemoveViewAsync(chain.Instance, navigateType, cancellationToken) ?? Task.CompletedTask;
    }
}