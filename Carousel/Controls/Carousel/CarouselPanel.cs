using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Windows.Foundation;

namespace Uno.Toolkit.UI;

/// <summary>
/// Custom panel that arranges carousel items in a horizontal or vertical strip.
/// Only the selected item is visible at a time; other items are arranged off-screen.
/// </summary>
internal partial class CarouselPanel : Panel
{
    private Orientation _orientation = Orientation.Horizontal;
    private int _selectedIndex;

    internal Orientation Orientation
    {
        get => _orientation;
        set
        {
            if (_orientation != value)
            {
                _orientation = value;
                InvalidateMeasure();
            }
        }
    }

    internal int SelectedIndex
    {
        get => _selectedIndex;
        set
        {
            if (_selectedIndex != value)
            {
                _selectedIndex = value;
                InvalidateArrange();
            }
        }
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        // Each child gets the full available size
        var childSize = new Size(
            double.IsInfinity(availableSize.Width) ? 800 : availableSize.Width,
            double.IsInfinity(availableSize.Height) ? 600 : availableSize.Height);

        foreach (var child in Children)
        {
            child.Measure(childSize);
        }

        // Panel itself takes the size of one item
        return childSize;
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        for (int i = 0; i < Children.Count; i++)
        {
            var child = Children[i];

            if (i == _selectedIndex)
            {
                // Selected item is arranged at (0,0)
                child.Arrange(new Rect(0, 0, finalSize.Width, finalSize.Height));
                child.Opacity = 1.0;
            }
            else
            {
                // Non-selected items are arranged off-screen but still in the visual tree
                // This allows transitions to animate them in/out
                double offsetX = 0;
                double offsetY = 0;

                if (_orientation == Orientation.Horizontal)
                {
                    offsetX = (i - _selectedIndex) * finalSize.Width;
                }
                else
                {
                    offsetY = (i - _selectedIndex) * finalSize.Height;
                }

                child.Arrange(new Rect(offsetX, offsetY, finalSize.Width, finalSize.Height));
                child.Opacity = 0.0;
            }
        }

        return finalSize;
    }
}
