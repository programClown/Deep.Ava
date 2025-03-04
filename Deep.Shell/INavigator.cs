using Avalonia.Animation;

namespace Deep.Shell;

public interface INavigator
{
    Uri CurrentUri { get; }

    INavigationRegistrar Registrar { get; }

    NavigationChain? CurrentChain { get; }

    void RegisterShell(ShellView shellView);

    bool HasItemInStack();

    Task NavigateAsync(string path, CancellationToken cancellationToken = default);
    Task NavigateAsync(string path, object? argument, CancellationToken cancellationToken = default);

    Task NavigateAsync(
        string path,
        NavigateType? navigateType,
        CancellationToken cancellationToken = default);

    Task NavigateAsync(
        string path,
        NavigateType? navigateType,
        object? argument,
        CancellationToken cancellationToken = default);

    Task NavigateAsync(
        string path,
        NavigateType? navigateType,
        object? sender,
        bool withAnimation,
        IPageTransition? overrideTransition = null,
        CancellationToken cancellationToken = default);

    Task NavigateAsync(
        string path,
        NavigateType? navigateType,
        object? argument,
        object? sender,
        bool withAnimation,
        IPageTransition? overrideTransition = null,
        CancellationToken cancellationToken = default);


    Task BackAsync(CancellationToken cancellationToken = default);
    Task BackAsync(object? argument, CancellationToken cancellationToken = default);

    Task BackAsync(
        object? sender,
        bool withAnimation,
        IPageTransition? overrideTransition = null,
        CancellationToken cancellationToken = default);

    Task BackAsync(
        object? argument,
        object? sender,
        bool withAnimation,
        IPageTransition? overrideTransition = null,
        CancellationToken cancellationToken = default);

    Task<NavigateResult> NavigateAndWaitAsync(string path, CancellationToken cancellationToken = default);

    Task<NavigateResult> NavigateAndWaitAsync(
        string path,
        object? argument,
        CancellationToken cancellationToken = default);

    Task<NavigateResult> NavigateAndWaitAsync(
        string path,
        NavigateType navigateType,
        CancellationToken cancellationToken = default);

    Task<NavigateResult> NavigateAndWaitAsync(
        string path,
        object? argument,
        NavigateType navigateType,
        CancellationToken cancellationToken = default);

    Task<NavigateResult> NavigateAndWaitAsync(
        string path,
        object? sender,
        NavigateType navigateType,
        bool withAnimation,
        IPageTransition? overrideTransition = null,
        CancellationToken cancellationToken = default);

    Task<NavigateResult> NavigateAndWaitAsync(
        string path,
        object? argument,
        object? sender,
        NavigateType navigateType,
        bool withAnimation,
        IPageTransition? overrideTransition = null,
        CancellationToken cancellationToken = default);
}