using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Microsoft.UI.Xaml.Shapes;
using Windows.Foundation;
using Windows.UI;

namespace Uno.Toolkit.UI;

/// <summary>
/// Internal control that renders page indicator dots for the carousel.
/// </summary>
internal partial class CarouselIndicator : Panel
{
    private int _itemCount;
    private int _selectedIndex;
    private Brush? _activeBrush;
    private Brush? _inactiveBrush;
    private double _dotSize = 8;
    private double _dotSpacing = 8;
    private Orientation _orientation = Orientation.Horizontal;

    internal int ItemCount
    {
        get => _itemCount;
        set
        {
            if (_itemCount != value)
            {
                _itemCount = value;
                RebuildDots();
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
                UpdateDotStates();
            }
        }
    }

    internal Brush? ActiveBrush
    {
        get => _activeBrush;
        set
        {
            _activeBrush = value;
            UpdateDotStates();
        }
    }

    internal Brush? InactiveBrush
    {
        get => _inactiveBrush;
        set
        {
            _inactiveBrush = value;
            UpdateDotStates();
        }
    }

    internal double DotSize
    {
        get => _dotSize;
        set
        {
            _dotSize = value;
            RebuildDots();
        }
    }

    internal double DotSpacing
    {
        get => _dotSpacing;
        set
        {
            _dotSpacing = value;
            InvalidateMeasure();
        }
    }

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

    private void RebuildDots()
    {
        Children.Clear();

        var defaultActive = new SolidColorBrush(Colors.White);
        var defaultInactive = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255));

        for (int i = 0; i < _itemCount; i++)
        {
            var dot = new Ellipse
            {
                Width = _dotSize,
                Height = _dotSize,
                Fill = i == _selectedIndex
                    ? (_activeBrush ?? defaultActive)
                    : (_inactiveBrush ?? defaultInactive)
            };
            Children.Add(dot);
        }

        InvalidateMeasure();
    }

    private void UpdateDotStates()
    {
        var defaultActive = new SolidColorBrush(Colors.White);
        var defaultInactive = new SolidColorBrush(Color.FromArgb(128, 255, 255, 255));

        for (int i = 0; i < Children.Count; i++)
        {
            if (Children[i] is Ellipse dot)
            {
                dot.Fill = i == _selectedIndex
                    ? (_activeBrush ?? defaultActive)
                    : (_inactiveBrush ?? defaultInactive);

                // Active dot is slightly larger
                dot.Width = i == _selectedIndex ? _dotSize * 1.25 : _dotSize;
                dot.Height = i == _selectedIndex ? _dotSize * 1.25 : _dotSize;
            }
        }

        InvalidateArrange();
    }

    protected override Size MeasureOverride(Size availableSize)
    {
        double totalWidth = 0;
        double totalHeight = 0;
        double maxCross = 0;

        foreach (var child in Children)
        {
            child.Measure(availableSize);

            if (_orientation == Orientation.Horizontal)
            {
                totalWidth += child.DesiredSize.Width;
                maxCross = Math.Max(maxCross, child.DesiredSize.Height);
            }
            else
            {
                totalHeight += child.DesiredSize.Height;
                maxCross = Math.Max(maxCross, child.DesiredSize.Width);
            }
        }

        double spacing = Math.Max(0, Children.Count - 1) * _dotSpacing;

        if (_orientation == Orientation.Horizontal)
        {
            return new Size(totalWidth + spacing, maxCross);
        }
        else
        {
            return new Size(maxCross, totalHeight + spacing);
        }
    }

    protected override Size ArrangeOverride(Size finalSize)
    {
        double offset = 0;

        // Center the dots within the panel
        double totalMain = 0;
        foreach (var child in Children)
        {
            totalMain += _orientation == Orientation.Horizontal
                ? child.DesiredSize.Width
                : child.DesiredSize.Height;
        }
        totalMain += Math.Max(0, Children.Count - 1) * _dotSpacing;

        if (_orientation == Orientation.Horizontal)
        {
            offset = Math.Max(0, (finalSize.Width - totalMain) / 2.0);
        }
        else
        {
            offset = Math.Max(0, (finalSize.Height - totalMain) / 2.0);
        }

        foreach (var child in Children)
        {
            if (_orientation == Orientation.Horizontal)
            {
                double y = (finalSize.Height - child.DesiredSize.Height) / 2.0;
                child.Arrange(new Rect(offset, y, child.DesiredSize.Width, child.DesiredSize.Height));
                offset += child.DesiredSize.Width + _dotSpacing;
            }
            else
            {
                double x = (finalSize.Width - child.DesiredSize.Width) / 2.0;
                child.Arrange(new Rect(x, offset, child.DesiredSize.Width, child.DesiredSize.Height));
                offset += child.DesiredSize.Height + _dotSpacing;
            }
        }

        return finalSize;
    }
}
