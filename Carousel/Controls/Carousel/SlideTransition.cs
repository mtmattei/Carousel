using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Media.Animation;

namespace Uno.Toolkit.UI;

/// <summary>
/// Default slide transition that translates items in/out horizontally or vertically.
/// </summary>
public class SlideTransition : ICarouselTransition
{
    private Storyboard? _storyboard;

    public void OnTransitionStarted(UIElement? from, UIElement to, bool forward)
    {
        _storyboard?.Stop();

        var storyboard = new Storyboard();
        var duration = new Duration(TimeSpan.FromMilliseconds(300));

        // Ensure the "to" element has a RenderTransform
        if (to.RenderTransform is not TranslateTransform)
        {
            to.RenderTransform = new TranslateTransform();
        }

        // Slide the "to" element in from the right (forward) or left (backward)
        var slideInOffset = forward ? 400.0 : -400.0;
        var slideInAnim = new DoubleAnimation
        {
            From = slideInOffset,
            To = 0,
            Duration = duration,
            EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut }
        };
        Storyboard.SetTarget(slideInAnim, to);
        Storyboard.SetTargetProperty(slideInAnim, "(UIElement.RenderTransform).(TranslateTransform.X)");
        storyboard.Children.Add(slideInAnim);

        // Fade in the "to" element
        var fadeInAnim = new DoubleAnimation
        {
            From = 0.0,
            To = 1.0,
            Duration = duration
        };
        Storyboard.SetTarget(fadeInAnim, to);
        Storyboard.SetTargetProperty(fadeInAnim, "Opacity");
        storyboard.Children.Add(fadeInAnim);

        // Slide and fade out the "from" element
        if (from != null)
        {
            if (from.RenderTransform is not TranslateTransform)
            {
                from.RenderTransform = new TranslateTransform();
            }

            var slideOutOffset = forward ? -400.0 : 400.0;
            var slideOutAnim = new DoubleAnimation
            {
                From = 0,
                To = slideOutOffset,
                Duration = duration,
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseIn }
            };
            Storyboard.SetTarget(slideOutAnim, from);
            Storyboard.SetTargetProperty(slideOutAnim, "(UIElement.RenderTransform).(TranslateTransform.X)");
            storyboard.Children.Add(slideOutAnim);

            var fadeOutAnim = new DoubleAnimation
            {
                From = 1.0,
                To = 0.0,
                Duration = duration
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
            if (from.RenderTransform is TranslateTransform tt)
            {
                tt.X = 0;
            }
        }

        to.Opacity = 1.0;
        if (to.RenderTransform is TranslateTransform toTt)
        {
            toTt.X = 0;
        }
    }
}
