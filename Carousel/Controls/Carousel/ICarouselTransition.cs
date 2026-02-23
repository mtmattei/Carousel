using Microsoft.UI.Xaml;

namespace Uno.Toolkit.UI;

/// <summary>
/// Defines a pluggable transition animation for carousel item changes.
/// </summary>
public interface ICarouselTransition
{
    /// <summary>
    /// Called when a transition between two items begins.
    /// </summary>
    /// <param name="from">The element being navigated away from. May be null on first load.</param>
    /// <param name="to">The element being navigated to.</param>
    /// <param name="forward">True if navigating forward (next), false if backward (previous).</param>
    void OnTransitionStarted(UIElement? from, UIElement to, bool forward);

    /// <summary>
    /// Called when the transition animation completes.
    /// </summary>
    /// <param name="from">The element that was navigated away from.</param>
    /// <param name="to">The element that was navigated to.</param>
    void OnTransitionCompleted(UIElement? from, UIElement to);
}
