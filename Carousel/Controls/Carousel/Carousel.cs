using System.Collections;
using System.Collections.Specialized;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Controls.Primitives;
using Microsoft.UI.Xaml.Input;
using Microsoft.UI.Xaml.Media;

namespace Uno.Toolkit.UI;

/// <summary>
/// A carousel control that displays items one at a time with navigation,
/// looping, indicators, and transition animations.
/// </summary>
[TemplatePart(Name = PART_ItemsPanel, Type = typeof(CarouselPanel))]
[TemplatePart(Name = PART_IndicatorPanel, Type = typeof(CarouselIndicator))]
[TemplatePart(Name = PART_PreviousButton, Type = typeof(ButtonBase))]
[TemplatePart(Name = PART_NextButton, Type = typeof(ButtonBase))]
[TemplatePart(Name = PART_ClipGrid, Type = typeof(Grid))]
public partial class Carousel : Control
{
    private const string PART_ItemsPanel = "PART_ItemsPanel";
    private const string PART_IndicatorPanel = "PART_IndicatorPanel";
    private const string PART_PreviousButton = "PART_PreviousButton";
    private const string PART_NextButton = "PART_NextButton";
    private const string PART_ClipGrid = "PART_ClipGrid";

    private CarouselPanel? _itemsPanel;
    private CarouselIndicator? _indicatorPanel;
    private ButtonBase? _previousButton;
    private ButtonBase? _nextButton;
    private Grid? _clipGrid;

    private ICarouselTransition _transition = new SlideTransition();
    private DispatcherTimer? _autoPlayTimer;
    private bool _isSynchronizingSelection;
    private int _selectedIndex = -1;
    private IList? _itemsSource;

    public Carousel()
    {
        DefaultStyleKey = typeof(Carousel);
    }

    #region DependencyProperty: ItemsSource

    public static DependencyProperty ItemsSourceProperty { get; } = DependencyProperty.Register(
        nameof(ItemsSource),
        typeof(object),
        typeof(Carousel),
        new PropertyMetadata(null, OnItemsSourceChanged));

    public object? ItemsSource
    {
        get => GetValue(ItemsSourceProperty);
        set => SetValue(ItemsSourceProperty, value);
    }

    private static void OnItemsSourceChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        => ((Carousel)sender).OnItemsSourceChanged(args);

    #endregion

    #region DependencyProperty: ItemTemplate

    public static DependencyProperty ItemTemplateProperty { get; } = DependencyProperty.Register(
        nameof(ItemTemplate),
        typeof(DataTemplate),
        typeof(Carousel),
        new PropertyMetadata(null, OnItemTemplateChanged));

    public DataTemplate? ItemTemplate
    {
        get => (DataTemplate?)GetValue(ItemTemplateProperty);
        set => SetValue(ItemTemplateProperty, value);
    }

    private static void OnItemTemplateChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        => ((Carousel)sender).OnItemTemplateChanged(args);

    #endregion

    #region DependencyProperty: SelectedIndex

    public static DependencyProperty SelectedIndexProperty { get; } = DependencyProperty.Register(
        nameof(SelectedIndex),
        typeof(int),
        typeof(Carousel),
        new PropertyMetadata(-1, OnSelectedIndexChanged));

    public int SelectedIndex
    {
        get => (int)GetValue(SelectedIndexProperty);
        set => SetValue(SelectedIndexProperty, value);
    }

    private static void OnSelectedIndexChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        => ((Carousel)sender).OnSelectedIndexChanged(args);

    #endregion

    #region DependencyProperty: SelectedItem

    public static DependencyProperty SelectedItemProperty { get; } = DependencyProperty.Register(
        nameof(SelectedItem),
        typeof(object),
        typeof(Carousel),
        new PropertyMetadata(null, OnSelectedItemChanged));

    public object? SelectedItem
    {
        get => GetValue(SelectedItemProperty);
        set => SetValue(SelectedItemProperty, value);
    }

    private static void OnSelectedItemChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        => ((Carousel)sender).OnSelectedItemChanged(args);

    #endregion

    #region DependencyProperty: ItemTransition

    public static DependencyProperty ItemTransitionProperty { get; } = DependencyProperty.Register(
        nameof(ItemTransition),
        typeof(ICarouselTransition),
        typeof(Carousel),
        new PropertyMetadata(null, OnItemTransitionChanged));

    public ICarouselTransition? ItemTransition
    {
        get => (ICarouselTransition?)GetValue(ItemTransitionProperty);
        set => SetValue(ItemTransitionProperty, value);
    }

    private static void OnItemTransitionChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
    {
        var carousel = (Carousel)sender;
        carousel._transition = args.NewValue as ICarouselTransition ?? new SlideTransition();
    }

    #endregion

    /// <summary>Gets the current count of items.</summary>
    private int ItemCount => _itemsSource is ICollection c ? c.Count : (_itemsSource != null ? GetEnumerableCount() : 0);

    private int GetEnumerableCount()
    {
        if (_itemsSource is not IEnumerable enumerable) return 0;
        int count = 0;
        foreach (var _ in enumerable) count++;
        return count;
    }

    private object? GetItemAt(int index)
    {
        if (_itemsSource is IList list && index >= 0 && index < list.Count)
        {
            return list[index];
        }
        return null;
    }

    protected override void OnApplyTemplate()
    {
        base.OnApplyTemplate();

        // Detach old event handlers
        if (_previousButton != null) _previousButton.Click -= OnPreviousButtonClick;
        if (_nextButton != null) _nextButton.Click -= OnNextButtonClick;

        // Get template parts
        _clipGrid = GetTemplateChild(PART_ClipGrid) as Grid;
        _itemsPanel = GetTemplateChild(PART_ItemsPanel) as CarouselPanel;
        _indicatorPanel = GetTemplateChild(PART_IndicatorPanel) as CarouselIndicator;
        _previousButton = GetTemplateChild(PART_PreviousButton) as ButtonBase;
        _nextButton = GetTemplateChild(PART_NextButton) as ButtonBase;

        // Attach event handlers
        if (_previousButton != null) _previousButton.Click += OnPreviousButtonClick;
        if (_nextButton != null) _nextButton.Click += OnNextButtonClick;

        // Clip the content area to prevent items from rendering outside bounds
        if (_clipGrid != null)
        {
            _clipGrid.SizeChanged += (s, e) =>
            {
                _clipGrid.Clip = new RectangleGeometry
                {
                    Rect = new Windows.Foundation.Rect(0, 0, e.NewSize.Width, e.NewSize.Height)
                };
            };
        }

        // Sync initial state
        SyncOrientation();
        RebuildItems();
        UpdateAutoPlay();
    }

    private void OnPreviousButtonClick(object sender, RoutedEventArgs e) => Previous();
    private void OnNextButtonClick(object sender, RoutedEventArgs e) => Next();

    /// <summary>
    /// Advances to the next item. Loops if <see cref="IsLoopingEnabled"/> is true.
    /// </summary>
    public void Next()
    {
        var count = ItemCount;
        if (count == 0) return;

        var nextIndex = _selectedIndex + 1;

        if (nextIndex >= count)
        {
            if (IsLoopingEnabled)
            {
                nextIndex = 0;
            }
            else
            {
                return; // At the end, no looping
            }
        }

        GoTo(nextIndex, true);
    }

    /// <summary>
    /// Goes to the previous item. Loops if <see cref="IsLoopingEnabled"/> is true.
    /// </summary>
    public void Previous()
    {
        var count = ItemCount;
        if (count == 0) return;

        var prevIndex = _selectedIndex - 1;

        if (prevIndex < 0)
        {
            if (IsLoopingEnabled)
            {
                prevIndex = count - 1;
            }
            else
            {
                return; // At the start, no looping
            }
        }

        GoTo(prevIndex, true);
    }

    /// <summary>
    /// Navigates to a specific item index.
    /// </summary>
    /// <param name="index">The target index.</param>
    /// <param name="animate">Whether to animate the transition.</param>
    public void GoTo(int index, bool animate = true)
    {
        var count = ItemCount;
        if (count == 0 || index < 0 || index >= count) return;

        if (index == _selectedIndex) return;

        var previousItem = GetItemAt(_selectedIndex);
        var newItem = GetItemAt(index);

        // Raise SelectionChanging (cancellable)
        var changingArgs = new CarouselSelectionChangingEventArgs(previousItem, newItem);
        SelectionChanging?.Invoke(this, changingArgs);
        if (changingArgs.Cancel) return;

        var previousIndex = _selectedIndex;
        var forward = index > previousIndex || (IsLoopingEnabled && previousIndex == count - 1 && index == 0);

        _isSynchronizingSelection = true;
        _selectedIndex = index;
        SelectedIndex = index;
        SelectedItem = newItem;
        _isSynchronizingSelection = false;

        // Animate the transition
        if (animate && _itemsPanel != null)
        {
            var fromElement = previousIndex >= 0 && previousIndex < _itemsPanel.Children.Count
                ? _itemsPanel.Children[previousIndex]
                : null;
            var toElement = index >= 0 && index < _itemsPanel.Children.Count
                ? _itemsPanel.Children[index]
                : null;

            if (toElement != null)
            {
                _transition.OnTransitionStarted(fromElement, toElement, forward);
            }
        }

        // Update panel
        if (_itemsPanel != null)
        {
            _itemsPanel.SelectedIndex = index;
        }

        // Update indicators
        if (_indicatorPanel != null)
        {
            _indicatorPanel.SelectedIndex = index;
        }

        // Update navigation button states
        UpdateNavigationButtonStates();

        // Raise SelectionChanged
        SelectionChanged?.Invoke(this, new CarouselSelectionChangedEventArgs(previousItem, newItem, previousIndex, index));

        // Reset auto-play timer
        ResetAutoPlayTimer();
    }

    private void OnSelectedIndexChanged(DependencyPropertyChangedEventArgs args)
    {
        if (_isSynchronizingSelection) return;

        var newIndex = (int)args.NewValue;
        if (newIndex != _selectedIndex)
        {
            GoTo(newIndex, true);
        }
    }

    private void OnSelectedItemChanged(DependencyPropertyChangedEventArgs args)
    {
        if (_isSynchronizingSelection) return;

        if (args.NewValue != null && _itemsSource is IList list)
        {
            var index = list.IndexOf(args.NewValue);
            if (index >= 0 && index != _selectedIndex)
            {
                GoTo(index, true);
            }
        }
    }

    private void OnItemsSourceChanged(DependencyPropertyChangedEventArgs args)
    {
        // Unsubscribe from old collection notifications
        if (args.OldValue is INotifyCollectionChanged oldNcc)
        {
            oldNcc.CollectionChanged -= OnItemsCollectionChanged;
        }

        _itemsSource = args.NewValue as IList;
        if (_itemsSource == null && args.NewValue is IEnumerable enumerable)
        {
            // Wrap non-IList enumerables
            var list = new System.Collections.Generic.List<object>();
            foreach (var item in enumerable) list.Add(item);
            _itemsSource = list;
        }

        // Subscribe to new collection notifications
        if (args.NewValue is INotifyCollectionChanged newNcc)
        {
            newNcc.CollectionChanged += OnItemsCollectionChanged;
        }

        RebuildItems();

        // Select first item if we have items
        if (ItemCount > 0 && _selectedIndex < 0)
        {
            GoTo(0, false);
        }
    }

    private void OnItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        RebuildItems();
    }

    private void OnItemTemplateChanged(DependencyPropertyChangedEventArgs args)
    {
        RebuildItems();
    }

    private void RebuildItems()
    {
        if (_itemsPanel == null) return;

        _itemsPanel.Children.Clear();

        if (_itemsSource == null)
        {
            if (_indicatorPanel != null) _indicatorPanel.ItemCount = 0;
            return;
        }

        var template = ItemTemplate;
        int index = 0;

        foreach (var item in _itemsSource)
        {
            UIElement element;

            if (template != null)
            {
                var content = template.LoadContent() as FrameworkElement;
                if (content != null)
                {
                    content.DataContext = item;
                    element = content;
                }
                else
                {
                    element = CreateDefaultItemContainer(item);
                }
            }
            else
            {
                element = CreateDefaultItemContainer(item);
            }

            // Initially hide non-selected items
            if (index != _selectedIndex)
            {
                element.Opacity = 0;
            }

            _itemsPanel.Children.Add(element);
            index++;
        }

        // Update indicator
        if (_indicatorPanel != null)
        {
            _indicatorPanel.ItemCount = ItemCount;
            _indicatorPanel.SelectedIndex = Math.Max(0, _selectedIndex);
        }

        // Update panel
        _itemsPanel.SelectedIndex = Math.Max(0, _selectedIndex);

        UpdateNavigationButtonStates();
    }

    private static UIElement CreateDefaultItemContainer(object item)
    {
        return new ContentPresenter
        {
            Content = item,
            HorizontalContentAlignment = HorizontalAlignment.Stretch,
            VerticalContentAlignment = VerticalAlignment.Stretch
        };
    }

    private void SyncOrientation()
    {
        if (_itemsPanel != null)
        {
            _itemsPanel.Orientation = Orientation;
        }

        if (_indicatorPanel != null)
        {
            _indicatorPanel.Orientation = Orientation;
        }
    }

    private void OnOrientationChanged(DependencyPropertyChangedEventArgs args)
    {
        SyncOrientation();
    }

    private void OnIsLoopingEnabledChanged(DependencyPropertyChangedEventArgs args)
    {
        UpdateNavigationButtonStates();
    }

    private void UpdateNavigationButtonStates()
    {
        if (_previousButton != null)
        {
            _previousButton.IsEnabled = IsLoopingEnabled || _selectedIndex > 0;
        }

        if (_nextButton != null)
        {
            _nextButton.IsEnabled = IsLoopingEnabled || _selectedIndex < ItemCount - 1;
        }
    }

    #region Keyboard Navigation

    protected override void OnKeyDown(KeyRoutedEventArgs e)
    {
        base.OnKeyDown(e);

        if (e.Handled) return;

        switch (e.Key)
        {
            case Windows.System.VirtualKey.Left when Orientation == Orientation.Horizontal:
            case Windows.System.VirtualKey.Up when Orientation == Orientation.Vertical:
                Previous();
                e.Handled = true;
                break;

            case Windows.System.VirtualKey.Right when Orientation == Orientation.Horizontal:
            case Windows.System.VirtualKey.Down when Orientation == Orientation.Vertical:
                Next();
                e.Handled = true;
                break;

            case Windows.System.VirtualKey.Home:
                GoTo(0);
                e.Handled = true;
                break;

            case Windows.System.VirtualKey.End:
                var count = ItemCount;
                if (count > 0) GoTo(count - 1);
                e.Handled = true;
                break;
        }
    }

    #endregion

    #region Auto-Play

    private void OnAutoPlayIntervalChanged(DependencyPropertyChangedEventArgs args)
    {
        UpdateAutoPlay();
    }

    private void OnIsAutoPlayEnabledChanged(DependencyPropertyChangedEventArgs args)
    {
        UpdateAutoPlay();
    }

    private void UpdateAutoPlay()
    {
        if (IsAutoPlayEnabled && AutoPlayInterval.HasValue && AutoPlayInterval.Value > TimeSpan.Zero)
        {
            if (_autoPlayTimer == null)
            {
                _autoPlayTimer = new DispatcherTimer();
                _autoPlayTimer.Tick += OnAutoPlayTimerTick;
            }

            _autoPlayTimer.Interval = AutoPlayInterval.Value;
            _autoPlayTimer.Start();
        }
        else
        {
            StopAutoPlay();
        }
    }

    private void StopAutoPlay()
    {
        if (_autoPlayTimer != null)
        {
            _autoPlayTimer.Stop();
            _autoPlayTimer.Tick -= OnAutoPlayTimerTick;
            _autoPlayTimer = null;
        }
    }

    private void ResetAutoPlayTimer()
    {
        if (_autoPlayTimer != null && _autoPlayTimer.IsEnabled)
        {
            _autoPlayTimer.Stop();
            _autoPlayTimer.Start();
        }
    }

    private void OnAutoPlayTimerTick(object? sender, object e)
    {
        Next();
    }

    #endregion

    #region Pointer Interaction (pause auto-play on hover)

    protected override void OnPointerEntered(PointerRoutedEventArgs e)
    {
        base.OnPointerEntered(e);

        if (_autoPlayTimer != null)
        {
            _autoPlayTimer.Stop();
        }
    }

    protected override void OnPointerExited(PointerRoutedEventArgs e)
    {
        base.OnPointerExited(e);

        if (IsAutoPlayEnabled && _autoPlayTimer != null)
        {
            _autoPlayTimer.Start();
        }
    }

    #endregion

    // Clean up when unloaded
    private void OnUnloaded(object sender, RoutedEventArgs e)
    {
        StopAutoPlay();
    }
}
