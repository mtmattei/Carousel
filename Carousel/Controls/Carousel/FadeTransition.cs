using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media.Animation;

namespace Uno.Toolkit.UI;

/// <summary>
/// Cross-fade transition between carousel items.
/// </summary>
public class FadeTransition : ICarouselTransition
{
    private Storyboard? _storyboard;

    public void OnTransitionStarted(UIElement? from, UIElement to, bool forward)
    {
        _storyboard?.Stop();

        var storyboard = new Storyboard();
        var duration = new Duration(TimeSpan.FromMilliseconds(350));

        var fadeInAnim = new DoubleAnimation
        {
            From = 0.0,
            To = 1.0,
            Duration = duration,
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        Storyboard.SetTarget(fadeInAnim, to);
        Storyboard.SetTargetProperty(fadeInAnim, "Opacity");
        storyboard.Children.Add(fadeInAnim);

        if (from != null)
        {
            var fadeOutAnim = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = duration,
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
            };
            Storyboard.SetTarget(fadeOutAnim, from);
            Storyboard.SetTargetProperty(fadeOutAnim, "Opacity");
            storyboard.Children.Add(fadeOutAnim);
        }

        _storyboard = storyboard;
        storyboard.Begin();
    }

    public void OnTransitionCompleted(UIElement? from, UIElement to)
    {
        if (from != null)
        {
            from.Opacity = 1.0;
        }

        to.Opacity = 1.0;
    }
}
