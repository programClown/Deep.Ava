using Avalonia;
using Avalonia.Animation;
using Avalonia.Animation.Easings;
using Avalonia.Rendering.Composition;
using Avalonia.Rendering.Composition.Animations;
using Avalonia.VisualTree;

namespace Deep.Shell.Platform;

public abstract class PlatformBasePageTransition : IPageTransition
{
    private readonly double _lastDistance = 0;
    private CompositionAnimationGroup? _bringBackAnimation;
    private CompositionAnimationGroup? _entranceAnimation;
    private CompositionAnimationGroup? _exitAnimation;
    private CompositionAnimationGroup? _sendBackAnimation;

    /// <summary>
    ///     Gets the duration of the animation.
    /// </summary>
    public virtual TimeSpan Duration { get; set; } = TimeSpan.FromSeconds(.25);

    /// <summary>
    ///     Gets or sets element entrance easing.
    /// </summary>
    public virtual Easing Easing { get; set; } = Easing.Parse("0.42, 0.0, 0.58, 1.0");

    protected virtual bool ShouldHideAfterExit { get; set; } = true;

    public async Task Start(Visual? from, Visual? to, bool forward, CancellationToken cancellationToken)
    {
        if (cancellationToken.IsCancellationRequested) return;

        var parent = GetVisualParent(from, to);
        var parentComposition = ElementComposition.GetElementVisual(parent)!;

        var distance = parent.Bounds.Width;
        var toElement = to != null ? ElementComposition.GetElementVisual(to) : null;
        var fromElement = from != null ? ElementComposition.GetElementVisual(from) : null;

        if (distance != _lastDistance)
        {
            _entranceAnimation = null;
            _exitAnimation = null;
            _sendBackAnimation = null;
            _bringBackAnimation = null;
        }

        if (to != null) to.IsVisible = true;
        await RunAnimationAsync(parentComposition, fromElement, toElement, forward, parent.Bounds.Width,
            parent.Bounds.Height, cancellationToken);
        if (from != null && ShouldHideAfterExit) from.IsVisible = false;
    }

    protected abstract CompositionAnimationGroup GetOrCreateEntranceAnimation(CompositionVisual element,
        double distance, double heightDistance);

    protected abstract CompositionAnimationGroup GetOrCreateExitAnimation(CompositionVisual element, double distance,
        double heightDistance);

    protected abstract CompositionAnimationGroup GetOrCreateSendBackAnimation(CompositionVisual element,
        double distance, double heightDistance);

    protected abstract CompositionAnimationGroup GetOrCreateBringBackAnimation(CompositionVisual element,
        double distance, double heightDistance);

    protected virtual Task RunAnimationAsync(
        CompositionVisual parentComposition,
        CompositionVisual? fromElement,
        CompositionVisual? toElement,
        bool forward,
        double widthDistance,
        double heightDistance,
        CancellationToken cancellationToken)
    {
        if (toElement != null)
        {
            var animation = forward
                ? _entranceAnimation ??= GetOrCreateEntranceAnimation(parentComposition, widthDistance, heightDistance)
                : _bringBackAnimation ??=
                    GetOrCreateBringBackAnimation(parentComposition, widthDistance, heightDistance);

            toElement.StartAnimationGroup(animation);
        }

        if (fromElement != null)
        {
            var animation = forward
                ? _sendBackAnimation ??= GetOrCreateSendBackAnimation(parentComposition, widthDistance, heightDistance)
                : _exitAnimation ??= GetOrCreateExitAnimation(parentComposition, widthDistance, heightDistance);

            fromElement.StartAnimationGroup(animation);
        }

        return Task.Delay(Duration, cancellationToken);
    }

    /// <summary>
    ///     Gets the common visual parent of the two control.
    /// </summary>
    /// <param name="from">The from control.</param>
    /// <param name="to">The to control.</param>
    /// <returns>The common parent.</returns>
    /// <exception cref="ArgumentException">
    ///     The two controls do not share a common parent.
    /// </exception>
    /// <remarks>
    ///     Any one of the parameters may be null, but not both.
    /// </remarks>
    protected static Visual GetVisualParent(Visual? from, Visual? to)
    {
        var p1 = (from ?? to)!.GetVisualParent();
        var p2 = (to ?? from)!.GetVisualParent();

        if (p1 != null && p2 != null && p1 != p2)
            throw new ArgumentException("Controls for PageSlide must have same parent.");

        return p1 ?? throw new InvalidOperationException("Cannot determine visual parent.");
    }
}