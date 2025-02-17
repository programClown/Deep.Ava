namespace Deep.Shell;

public interface INavigateStrategy
{
    Task<Uri> NavigateAsync(NavigationChain chain, Uri currentUri, string path, CancellationToken cancellationToken);
    Task<Uri?> BackAsync(NavigationChain chain, Uri currentUri, CancellationToken cancellationToken);
}