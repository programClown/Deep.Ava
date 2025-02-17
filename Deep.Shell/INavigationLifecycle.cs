namespace Deep.Shell;

public interface INavigationLifecycle
{
    Task InitialiseAsync(CancellationToken cancellationToken);
    Task AppearAsync(CancellationToken cancellationToken);
    Task DisappearAsync(CancellationToken cancellationToken);
    Task ArgumentAsync(object args, CancellationToken cancellationToken);
    Task TerminateAsync(CancellationToken cancellationToken);
}