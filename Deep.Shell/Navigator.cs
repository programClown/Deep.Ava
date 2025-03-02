using System.Diagnostics;
using Avalonia.Animation;

namespace Deep.Shell;

public partial class Navigator : INavigator
{
    private readonly INavigateStrategy _navigateStrategy;
    private readonly NavigationStack _stack;
    private readonly INavigationUpdateStrategy _updateStrategy;
    private readonly INavigationViewLocator _viewLocator;
    private readonly Dictionary<NavigationChain, TaskCompletionSource<NavigateResult>> _waitingList = new();

    private bool _navigating;
    private ShellView? _shellView;

    public Navigator(
        INavigationRegistrar navigationRegistrar,
        INavigateStrategy navigateStrategy,
        INavigationUpdateStrategy updateStrategy,
        INavigationViewLocator viewLocator)
    {
        Registrar = navigationRegistrar;
        _navigateStrategy = navigateStrategy;
        _updateStrategy = updateStrategy;
        _viewLocator = viewLocator;
        _stack = new NavigationStack(viewLocator);

        _updateStrategy.HostItemChanged += UpdateStrategyOnHostItemChanged;
    }

    public ShellView ShellView => _shellView ?? throw new ArgumentNullException(nameof(ShellView));

    public Uri CurrentUri => _stack.Current?.Uri ?? Registrar.RootUri;

    public NavigationChain? CurrentChain => _stack.Current;

    public INavigationRegistrar Registrar { get; }

    public void RegisterShell(ShellView shellView)
    {
        if (_shellView != null) throw new ArgumentException("Register shell can call only once");
        _shellView = shellView;
    }

    public bool HasItemInStack()
    {
        var current = _stack.Current?.Back;
        while (current != null)
        {
            if (current is not HostNavigationChain)
                return true;

            current = current.Back;
        }

        return false;
    }

    public Task NavigateAsync(string path, CancellationToken cancellationToken = default)
    {
        return NavigateAsync(path, null, null, false, null, true, null, cancellationToken);
    }

    public Task NavigateAsync(string path, object? argument, CancellationToken cancellationToken = default)
    {
        return NavigateAsync(path, null, argument, true, null, true, null, cancellationToken);
    }

    public Task NavigateAsync(
        string path,
        NavigateType? navigateType,
        CancellationToken cancellationToken = default)
    {
        return NavigateAsync(path, navigateType, null, false, null, true, null, cancellationToken);
    }

    public Task NavigateAsync(
        string path,
        NavigateType? navigateType,
        object? argument,
        CancellationToken cancellationToken = default)
    {
        return NavigateAsync(path, navigateType, argument, true, null, true, null, cancellationToken);
    }

    public Task BackAsync(CancellationToken cancellationToken = default)
    {
        return BackAsync(null, false, null, true, null, cancellationToken);
    }

    public Task BackAsync(object? argument, CancellationToken cancellationToken = default)
    {
        return BackAsync(argument, true, null, true, null, cancellationToken);
    }

    public Task<NavigateResult> NavigateAndWaitAsync(string path, CancellationToken cancellationToken = default)
    {
        return NavigateAndWaitAsync(path, null, null, false, null, true, null, cancellationToken);
    }

    public Task<NavigateResult> NavigateAndWaitAsync(
        string path,
        object? argument,
        CancellationToken cancellationToken = default)
    {
        return NavigateAndWaitAsync(path, null, argument, true, null, true, null, cancellationToken);
    }

    public Task<NavigateResult> NavigateAndWaitAsync(
        string path,
        NavigateType navigateType,
        CancellationToken cancellationToken = default)
    {
        return NavigateAndWaitAsync(path, navigateType, null, false, null, true, null, cancellationToken);
    }

    public Task<NavigateResult> NavigateAndWaitAsync(
        string path,
        object? argument,
        NavigateType navigateType,
        CancellationToken cancellationToken = default)
    {
        return NavigateAndWaitAsync(path, navigateType, argument, true, null, true, null, cancellationToken);
    }

    public Task NavigateAsync(
        string path,
        NavigateType? navigateType,
        object? sender,
        bool withAnimation = true,
        IPageTransition? overrideTransition = null,
        CancellationToken cancellationToken = default)
    {
        return NavigateAndWaitAsync(path, navigateType, null, false, sender, withAnimation, overrideTransition,
            cancellationToken);
    }

    public Task NavigateAsync(
        string path,
        NavigateType? navigateType,
        object? argument,
        object? sender,
        bool withAnimation,
        IPageTransition? overrideTransition = null,
        CancellationToken cancellationToken = default)
    {
        return NavigateAndWaitAsync(path, navigateType, argument, true, sender, withAnimation, overrideTransition,
            cancellationToken);
    }

    public Task BackAsync(
        object? sender,
        bool withAnimation = true,
        IPageTransition? overrideTransition = null,
        CancellationToken cancellationToken = default)
    {
        return BackAsync(null, false, sender, withAnimation, overrideTransition, cancellationToken);
    }

    public Task BackAsync(
        object? argument,
        object? sender,
        bool withAnimation,
        IPageTransition? overrideTransition = null,
        CancellationToken cancellationToken = default)
    {
        return BackAsync(argument, true, sender, withAnimation, overrideTransition, cancellationToken);
    }

    public Task<NavigateResult> NavigateAndWaitAsync(
        string path,
        object? sender,
        NavigateType navigateType,
        bool withAnimation = true,
        IPageTransition? overrideTransition = null,
        CancellationToken cancellationToken = default)
    {
        return NavigateAndWaitAsync(path, navigateType, null, false, sender, withAnimation, overrideTransition,
            cancellationToken);
    }

    public Task<NavigateResult> NavigateAndWaitAsync(
        string path,
        object? argument,
        object? sender,
        NavigateType navigateType,
        bool withAnimation,
        IPageTransition? overrideTransition = null,
        CancellationToken cancellationToken = default)
    {
        return NavigateAndWaitAsync(path, navigateType, argument, true, sender, withAnimation, overrideTransition,
            cancellationToken);
    }

    private async Task NotifyAsync(
        Uri origin,
        Uri newUri,
        object? argument,
        bool hasArgument,
        object? sender,
        NavigateType? navigateType,
        bool withAnimation,
        IPageTransition? overrideTransition,
        CancellationToken cancellationToken = default)
    {
        if (!Registrar.TryGetNode(newUri.AbsolutePath, out var node))
        {
            Debug.WriteLine("Warning: Cannot find the path");
            return;
        }

        var finalNavigateType =
            !origin.AbsolutePath.Equals(newUri.AbsolutePath) &&
            Registrar.TryGetNode(origin.AbsolutePath, out var originalNode)
                ? navigateType ?? originalNode.Navigate
                : navigateType ?? node.Navigate;

        var fromPage = _stack.Current?.Instance as INavigatorLifecycle;
        if (fromPage != null)
        {
            var args = new NaviagatingEventArgs
            {
                Sender = sender,
                From = CurrentUri,
                FromUri = origin,
                ToUri = newUri,
                Argument = argument,
                Navigate = finalNavigateType,
                WithAnimation = withAnimation,
                OverrideTransition = overrideTransition
            };

            await fromPage.OnNavigatingAsync(args, cancellationToken);
            if (args.Cancel) return;

            //Check for overrides

            if (argument != args.Argument)
            {
                argument = args.Argument;
                hasArgument = true;
            }

            finalNavigateType = args.Navigate;
            withAnimation = args.WithAnimation;
            overrideTransition = args.OverrideTransition;
        }

        _navigating = true;

        var stackChanges = _stack.Push(
            node,
            finalNavigateType,
            newUri);

        foreach (var newChain in stackChanges.NewNavigationChains) SetupPage(newChain);

        await _updateStrategy.UpdateChangesAsync(
            ShellView,
            stackChanges,
            finalNavigateType,
            argument,
            hasArgument,
            cancellationToken);

        CheckWaitingList(stackChanges, argument, hasArgument);

        if (fromPage != null)
        {
            var args = new NaviagateEventArgs
            {
                Sender = sender,
                From = fromPage,
                To = _stack.Current?.Instance,
                FromUri = origin,
                ToUri = newUri,
                Argument = argument,
                Navigate = finalNavigateType,
                WithAnimation = withAnimation,
                OverrideTransition = overrideTransition
            };

            await fromPage.OnNavigateAsync(args, cancellationToken);
        }

        _navigating = false;
    }

    private void SetupPage(NavigationChain chain)
    {
        if (chain.Instance is not Page page) return;

        page.Shell = ShellView;
        page.Chain = chain;
    }

    private async Task SwitchHostedItem(
        NavigationChain old,
        NavigationChain chain,
        bool withAnimation,
        IPageTransition? overrideTransition,
        CancellationToken cancellationToken = default)
    {
        var newUri =
            await _navigateStrategy.NavigateAsync(_stack.Current, CurrentUri, chain.Uri.AbsolutePath,
                cancellationToken);
        if (CurrentUri.AbsolutePath != newUri.AbsolutePath)
            await NotifyAsync(newUri, newUri, null, false, null, NavigateType.HostedItemChange, withAnimation,
                overrideTransition, cancellationToken);
    }

    private async Task NavigateAsync(
        string path,
        NavigateType? navigateType,
        object? argument,
        bool hasArgument,
        object? sender,
        bool withAnimation,
        IPageTransition? overrideTransition,
        CancellationToken cancellationToken = default)
    {
        var originalUri = new Uri(CurrentUri, path);
        var newUri = await _navigateStrategy.NavigateAsync(_stack.Current, CurrentUri, path, cancellationToken);
        if (CurrentUri.AbsolutePath != newUri.AbsolutePath)
            await NotifyAsync(originalUri, newUri, argument, hasArgument, sender, navigateType, withAnimation,
                overrideTransition, cancellationToken);
    }

    private async Task BackAsync(
        object? argument,
        bool hasArgument,
        object? sender,
        bool withAnimation,
        IPageTransition? overrideTransition,
        CancellationToken cancellationToken = default)
    {
        var newUri = await _navigateStrategy.BackAsync(_stack.Current, CurrentUri, cancellationToken);
        if (newUri != null && CurrentUri.AbsolutePath != newUri.AbsolutePath)
            await NotifyAsync(newUri, newUri, argument, hasArgument, sender, NavigateType.Pop, withAnimation,
                overrideTransition, cancellationToken);
    }

    private async Task<NavigateResult> NavigateAndWaitAsync(
        string path,
        NavigateType? navigateType,
        object? argument,
        bool hasArgument,
        object? sender,
        bool withAnimation,
        IPageTransition? overrideTransition,
        CancellationToken cancellationToken = default)
    {
        var originalUri = new Uri(CurrentUri, path);
        var newUri = await _navigateStrategy.NavigateAsync(_stack.Current, CurrentUri, path, cancellationToken);
        if (CurrentUri.AbsolutePath == newUri.AbsolutePath)
            return new NavigateResult(false, null); // Or maybe we should throw exception.

        await NotifyAsync(originalUri, newUri, argument, hasArgument, sender, navigateType, withAnimation,
            overrideTransition, cancellationToken);
        var chain = _stack.Current;

        if (!_waitingList.TryGetValue(chain, out var tcs))
            _waitingList[chain] = tcs = new TaskCompletionSource<NavigateResult>();

        try
        {
            return await tcs.Task;
        }
        finally
        {
            _waitingList.Remove(chain);
        }
    }

    private void CheckWaitingList(
        NavigationStackChanges navigationStackChanges,
        object? argument,
        bool hasArgument)
    {
        if (navigationStackChanges.Removed == null) return;
        foreach (var chain in navigationStackChanges.Removed)
            if (_waitingList.TryGetValue(chain, out var tcs))
                tcs.TrySetResult(new NavigateResult(hasArgument, argument));
    }

    private void UpdateStrategyOnHostItemChanged(object? sender, HostItemChangeEventArgs e)
    {
        if (e.OldChain != null && e.NewChain != e.OldChain && !_navigating)
            _ = SwitchHostedItem(e.OldChain, e.NewChain, true, null);
    }
}