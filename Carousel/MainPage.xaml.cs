using Microsoft.UI;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Controls;
using Microsoft.UI.Xaml.Media;
using Uno.Toolkit.UI;
using Windows.UI;

namespace CarouselDemo;

public sealed partial class MainPage : Page
{
    public MainPage()
    {
        this.InitializeComponent();
        this.Loaded += OnLoaded;
    }

    private void OnLoaded(object sender, RoutedEventArgs e)
    {
        SetupBasicCarousel();
        SetupVerticalCarousel();
        SetupTemplatedCarousel();
        SetupAutoPlayCarousel();
        SetupNoLoopCarousel();
        SetupFadeCarousel();
    }

    private void SetupBasicCarousel()
    {
        var items = new List<SolidColorBrush>
        {
            new SolidColorBrush(Color.FromArgb(255, 103, 80, 164)),   // Primary
            new SolidColorBrush(Color.FromArgb(255, 98, 91, 113)),    // Secondary
            new SolidColorBrush(Color.FromArgb(255, 125, 82, 96)),    // Tertiary
            new SolidColorBrush(Color.FromArgb(255, 79, 55, 139)),    // PrimaryContainer
            new SolidColorBrush(Color.FromArgb(255, 74, 68, 88)),     // SecondaryContainer
        };

        var panels = new List<FrameworkElement>();
        var labels = new string[] { "Primary", "Secondary", "Tertiary", "Container 1", "Container 2" };
        for (int i = 0; i < items.Count; i++)
        {
            var grid = new Grid
            {
                Background = items[i],
            };
            var text = new TextBlock
            {
                Text = labels[i],
                FontSize = 24,
                Foreground = new SolidColorBrush(Colors.White),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            };
            grid.Children.Add(text);
            panels.Add(grid);
        }

        BasicCarousel.ItemsSource = panels;
        BasicCarousel.SelectionChanged += (s, args) =>
        {
            BasicCarouselStatus.Text = $"Item {args.NewIndex + 1} of {panels.Count}";
        };
        BasicCarouselStatus.Text = "Item 1 of 5";
    }

    private void SetupVerticalCarousel()
    {
        var items = new List<FrameworkElement>();
        var colors = new[]
        {
            Color.FromArgb(255, 0, 150, 136),   // Teal
            Color.FromArgb(255, 33, 150, 243),   // Blue
            Color.FromArgb(255, 156, 39, 176),   // Purple
            Color.FromArgb(255, 255, 152, 0),    // Orange
        };
        var names = new[] { "Slide Up", "Keep Going", "Almost There", "Loop Back" };

        for (int i = 0; i < colors.Length; i++)
        {
            var grid = new Grid { Background = new SolidColorBrush(colors[i]) };
            grid.Children.Add(new TextBlock
            {
                Text = names[i],
                FontSize = 22,
                Foreground = new SolidColorBrush(Colors.White),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            });
            items.Add(grid);
        }

        VerticalCarousel.ItemsSource = items;
    }

    private void SetupTemplatedCarousel()
    {
        var slides = new List<SlideData>
        {
            new("&#xE716;", "Welcome", "Get started with the Carousel control",
                new SolidColorBrush(Color.FromArgb(255, 103, 80, 164)),
                new SolidColorBrush(Colors.White)),
            new("&#xE80F;", "Navigate", "Swipe or use buttons to browse",
                new SolidColorBrush(Color.FromArgb(255, 0, 120, 215)),
                new SolidColorBrush(Colors.White)),
            new("&#xE734;", "Favorite", "Mark your favorite items",
                new SolidColorBrush(Color.FromArgb(255, 218, 59, 59)),
                new SolidColorBrush(Colors.White)),
            new("&#xE73E;", "Complete", "You have explored all slides",
                new SolidColorBrush(Color.FromArgb(255, 16, 137, 62)),
                new SolidColorBrush(Colors.White)),
        };

        TemplatedCarousel.ItemsSource = slides;
    }

    private void SetupAutoPlayCarousel()
    {
        var items = new List<FrameworkElement>();
        var colors = new[]
        {
            Color.FromArgb(255, 229, 57, 53),    // Red
            Color.FromArgb(255, 253, 216, 53),    // Yellow
            Color.FromArgb(255, 67, 160, 71),     // Green
            Color.FromArgb(255, 30, 136, 229),    // Blue
        };
        var names = new[] { "Auto 1", "Auto 2", "Auto 3", "Auto 4" };

        for (int i = 0; i < colors.Length; i++)
        {
            var grid = new Grid { Background = new SolidColorBrush(colors[i]) };
            grid.Children.Add(new TextBlock
            {
                Text = names[i],
                FontSize = 22,
                Foreground = new SolidColorBrush(Colors.White),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            });
            items.Add(grid);
        }

        AutoPlayCarousel.ItemsSource = items;
        AutoPlayCarousel.AutoPlayInterval = TimeSpan.FromSeconds(3);
    }

    private void SetupNoLoopCarousel()
    {
        var items = new List<FrameworkElement>();
        var colors = new[]
        {
            Color.FromArgb(255, 121, 85, 72),     // Brown
            Color.FromArgb(255, 96, 125, 139),     // BlueGrey
            Color.FromArgb(255, 158, 158, 158),    // Grey
        };
        var names = new[] { "First (no wrap)", "Middle", "Last (no wrap)" };

        for (int i = 0; i < colors.Length; i++)
        {
            var grid = new Grid { Background = new SolidColorBrush(colors[i]) };
            grid.Children.Add(new TextBlock
            {
                Text = names[i],
                FontSize = 20,
                Foreground = new SolidColorBrush(Colors.White),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            });
            items.Add(grid);
        }

        NoLoopCarousel.ItemsSource = items;
    }

    private void SetupFadeCarousel()
    {
        FadeCarousel.ItemTransition = new FadeTransition();

        var items = new List<FrameworkElement>();
        var colors = new[]
        {
            Color.FromArgb(255, 171, 71, 188),    // Purple
            Color.FromArgb(255, 77, 182, 172),     // Teal
            Color.FromArgb(255, 255, 183, 77),     // Amber
        };
        var names = new[] { "Fade In", "Cross Fade", "Fade Out" };

        for (int i = 0; i < colors.Length; i++)
        {
            var grid = new Grid { Background = new SolidColorBrush(colors[i]) };
            grid.Children.Add(new TextBlock
            {
                Text = names[i],
                FontSize = 22,
                Foreground = new SolidColorBrush(Colors.White),
                HorizontalAlignment = HorizontalAlignment.Center,
                VerticalAlignment = VerticalAlignment.Center
            });
            items.Add(grid);
        }

        FadeCarousel.ItemsSource = items;
    }

    private void OnPreviousClicked(object sender, RoutedEventArgs e) => BasicCarousel.Previous();
    private void OnNextClicked(object sender, RoutedEventArgs e) => BasicCarousel.Next();
    private void OnFadePreviousClicked(object sender, RoutedEventArgs e) => FadeCarousel.Previous();
    private void OnFadeNextClicked(object sender, RoutedEventArgs e) => FadeCarousel.Next();
}

public record SlideData(
    string Icon,
    string Title,
    string Description,
    SolidColorBrush Background,
    SolidColorBrush Foreground);
