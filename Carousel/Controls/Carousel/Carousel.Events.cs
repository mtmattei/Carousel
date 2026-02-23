using System;
using Windows.Foundation;

namespace Uno.Toolkit.UI;

/// <summary>
/// Event arguments for the <see cref="Carousel.SelectionChanged"/> event.
/// </summary>
public class CarouselSelectionChangedEventArgs : EventArgs
{
    public CarouselSelectionChangedEventArgs(object? previousItem, object? newItem, int previousIndex, int newIndex)
    {
        PreviousItem = previousItem;
        NewItem = newItem;
        PreviousIndex = previousIndex;
        NewIndex = newIndex;
    }

    /// <summary>Gets the previously selected item.</summary>
    public object? PreviousItem { get; }

    /// <summary>Gets the newly selected item.</summary>
    public object? NewItem { get; }

    /// <summary>Gets the index of the previously selected item.</summary>
    public int PreviousIndex { get; }

    /// <summary>Gets the index of the newly selected item.</summary>
    public int NewIndex { get; }
}

/// <summary>
/// Event arguments for the <see cref="Carousel.SelectionChanging"/> event.
/// Allows cancellation of the selection change.
/// </summary>
public class CarouselSelectionChangingEventArgs : EventArgs
{
    public CarouselSelectionChangingEventArgs(object? previousItem, object? newItem)
    {
        PreviousItem = previousItem;
        NewItem = newItem;
    }

    /// <summary>Gets the previously selected item.</summary>
    public object? PreviousItem { get; }

    /// <summary>Gets the newly selected item.</summary>
    public object? NewItem { get; }

    /// <summary>Set to true to cancel the selection change.</summary>
    public bool Cancel { get; set; }
}

public partial class Carousel
{
    /// <summary>
    /// Occurs when the selected item has changed.
    /// </summary>
    public event TypedEventHandler<Carousel, CarouselSelectionChangedEventArgs>? SelectionChanged;

    /// <summary>
    /// Occurs before the selected item changes. Can be cancelled.
    /// </summary>
    public event TypedEventHandler<Carousel, CarouselSelectionChangingEventArgs>? SelectionChanging;
}
