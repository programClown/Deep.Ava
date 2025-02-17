namespace Deep.Shell;

public interface INavigationUpdateStrategy
{
    event EventHandler<HostItemChangeEventArgs> HostItemChanged;

    Task UpdateChangesAsync(
        ShellView shellView,
        NavigationStackChanges changes,
        NavigateType navigateType,
        object? argument,
        bool hasArgument,
        CancellationToken cancellationToken);
}