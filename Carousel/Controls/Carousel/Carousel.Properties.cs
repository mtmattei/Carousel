using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;

namespace Uno.Toolkit.UI;

public partial class Carousel
{
    #region DependencyProperty: Orientation

    public static DependencyProperty OrientationProperty { get; } = DependencyProperty.Register(
        nameof(Orientation),
        typeof(Orientation),
        typeof(Carousel),
        new PropertyMetadata(Orientation.Horizontal, OnOrientationChanged));

    /// <summary>
    /// Gets or sets the layout direction of the carousel items.
    /// </summary>
    public Orientation Orientation
    {
        get => (Orientation)GetValue(OrientationProperty);
        set => SetValue(OrientationProperty, value);
    }

    private static void OnOrientationChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        => ((Carousel)sender).OnOrientationChanged(args);

    #endregion

    #region DependencyProperty: IsLoopingEnabled

    public static DependencyProperty IsLoopingEnabledProperty { get; } = DependencyProperty.Register(
        nameof(IsLoopingEnabled),
        typeof(bool),
        typeof(Carousel),
        new PropertyMetadata(true, OnIsLoopingEnabledChanged));

    /// <summary>
    /// Gets or sets whether the carousel wraps from the last item to the first and vice versa.
    /// </summary>
    public bool IsLoopingEnabled
    {
        get => (bool)GetValue(IsLoopingEnabledProperty);
        set => SetValue(IsLoopingEnabledProperty, value);
    }

    private static void OnIsLoopingEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        => ((Carousel)sender).OnIsLoopingEnabledChanged(args);

    #endregion

    #region DependencyProperty: AutoPlayInterval

    public static DependencyProperty AutoPlayIntervalProperty { get; } = DependencyProperty.Register(
        nameof(AutoPlayInterval),
        typeof(object),
        typeof(Carousel),
        new PropertyMetadata(null, OnAutoPlayIntervalChanged));

    /// <summary>
    /// Gets or sets the auto-advance interval. Null means disabled.
    /// </summary>
    public TimeSpan? AutoPlayInterval
    {
        get => GetValue(AutoPlayIntervalProperty) as TimeSpan?;
        set => SetValue(AutoPlayIntervalProperty, value);
    }

    private static void OnAutoPlayIntervalChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        => ((Carousel)sender).OnAutoPlayIntervalChanged(args);

    #endregion

    #region DependencyProperty: IsAutoPlayEnabled

    public static DependencyProperty IsAutoPlayEnabledProperty { get; } = DependencyProperty.Register(
        nameof(IsAutoPlayEnabled),
        typeof(bool),
        typeof(Carousel),
        new PropertyMetadata(false, OnIsAutoPlayEnabledChanged));

    /// <summary>
    /// Gets or sets whether auto-advance is enabled.
    /// </summary>
    public bool IsAutoPlayEnabled
    {
        get => (bool)GetValue(IsAutoPlayEnabledProperty);
        set => SetValue(IsAutoPlayEnabledProperty, value);
    }

    private static void OnIsAutoPlayEnabledChanged(DependencyObject sender, DependencyPropertyChangedEventArgs args)
        => ((Carousel)sender).OnIsAutoPlayEnabledChanged(args);

    #endregion

    #region DependencyProperty: IsSwipeEnabled

    public static DependencyProperty IsSwipeEnabledProperty { get; } = DependencyProperty.Register(
        nameof(IsSwipeEnabled),
        typeof(bool),
        typeof(Carousel),
        new PropertyMetadata(true));

    /// <summary>
    /// Gets or sets whether touch/pointer drag navigation is enabled.
    /// </summary>
    public bool IsSwipeEnabled
    {
        get => (bool)GetValue(IsSwipeEnabledProperty);
        set => SetValue(IsSwipeEnabledProperty, value);
    }

    #endregion

    #region DependencyProperty: IsSnapEnabled

    public static DependencyProperty IsSnapEnabledProperty { get; } = DependencyProperty.Register(
        nameof(IsSnapEnabled),
        typeof(bool),
        typeof(Carousel),
        new PropertyMetadata(true));

    /// <summary>
    /// Gets or sets whether snap-to-item is enabled after drag.
    /// </summary>
    public bool IsSnapEnabled
    {
        get => (bool)GetValue(IsSnapEnabledProperty);
        set => SetValue(IsSnapEnabledProperty, value);
    }

    #endregion

    #region DependencyProperty: SnapPointsAlignment

    public static DependencyProperty SnapPointsAlignmentProperty { get; } = DependencyProperty.Register(
        nameof(SnapPointsAlignment),
        typeof(CarouselSnapPointsAlignment),
        typeof(Carousel),
        new PropertyMetadata(CarouselSnapPointsAlignment.Center));

    /// <summary>
    /// Gets or sets how items align to snap points within the viewport.
    /// </summary>
    public CarouselSnapPointsAlignment SnapPointsAlignment
    {
        get => (CarouselSnapPointsAlignment)GetValue(SnapPointsAlignmentProperty);
        set => SetValue(SnapPointsAlignmentProperty, value);
    }

    #endregion

    #region DependencyProperty: IndicatorVisibility

    public static DependencyProperty IndicatorVisibilityProperty { get; } = DependencyProperty.Register(
        nameof(IndicatorVisibility),
        typeof(Visibility),
        typeof(Carousel),
        new PropertyMetadata(Visibility.Visible));

    /// <summary>
    /// Gets or sets the visibility of the built-in page indicator dots.
    /// </summary>
    public Visibility IndicatorVisibility
    {
        get => (Visibility)GetValue(IndicatorVisibilityProperty);
        set => SetValue(IndicatorVisibilityProperty, value);
    }

    #endregion

    #region DependencyProperty: IndicatorStyle

    public static DependencyProperty IndicatorStyleProperty { get; } = DependencyProperty.Register(
        nameof(IndicatorStyle),
        typeof(Style),
        typeof(Carousel),
        new PropertyMetadata(null));

    /// <summary>
    /// Gets or sets the style applied to indicator dot items.
    /// </summary>
    public Style? IndicatorStyle
    {
        get => (Style?)GetValue(IndicatorStyleProperty);
        set => SetValue(IndicatorStyleProperty, value);
    }

    #endregion
}
