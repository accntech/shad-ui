using System;
using Avalonia.Media;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace ShadUI.Demo.ViewModels;

[Page("dashboard")]
public sealed partial class DashboardViewModel : ViewModelBase, INavigable
{
    private readonly PageManager _pageManager;
    private bool _disposed;
    public ThemeWatcher ThemeWatcher { get; }

    [ObservableProperty]
    private SolidColorPaint _tooltipTextPaint = new(SKColors.Black);

    [ObservableProperty]
    private SolidColorPaint _tooltipBackgroundPaint = new(SKColors.White);

    public DashboardViewModel(PageManager pageManager, ThemeWatcher themeWatcher)
    {
        _pageManager = pageManager;
        ThemeWatcher = themeWatcher;
        UpdateTooltipPaints(ThemeWatcher.ThemeColors);
        ThemeWatcher.ThemeChanged += OnThemeChanged;

        XAxes =
        [
            new Axis
            {
                Labels = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
                LabelsPaint = new SolidColorPaint(SKColors.Gray),
                TextSize = 12,
                MinStep = 1
            }
        ];

        YAxes =
        [
            new Axis
            {
                Labeler = Labelers.Currency,
                LabelsPaint = new SolidColorPaint(SKColors.Gray),
                TextSize = 12,
                MinStep = 1500,
                ShowSeparatorLines = false
            }
        ];
    }

    private void OnThemeChanged(object? sender, ThemeColors colors)
    {
        UpdateAxesLabelPaints(colors);
        UpdateSeriesFill(colors.PrimaryColor);
        UpdateTooltipPaints(colors);
    }

    [RelayCommand]
    private void NextPage()
    {
        _pageManager.Navigate<ThemeViewModel>();
    }

    private void UpdateSeriesFill(Color primary)
    {
        var color = new SKColor(primary.R, primary.G, primary.B, primary.A);
        if (Series.Length == 0) return;

        var series = (ColumnSeries<double>)Series[0];
        series.Fill = new SolidColorPaint(color);
    }

    private void UpdateAxesLabelPaints(ThemeColors colors)
    {
        var foreground = new SKColor(
            colors.ForegroundColor.R,
            colors.ForegroundColor.G,
            colors.ForegroundColor.B,
            colors.ForegroundColor.A);

        XAxes[0].LabelsPaint = new SolidColorPaint(foreground);
        YAxes[0].LabelsPaint = new SolidColorPaint(foreground);
    }

    private void UpdateTooltipPaints(ThemeColors colors)
    {
        var text = new SKColor(
            colors.PrimaryForegroundColor.R,
            colors.PrimaryForegroundColor.G,
            colors.PrimaryForegroundColor.B,
            colors.PrimaryForegroundColor.A);
        var background = new SKColor(
            colors.PrimaryColor.R,
            colors.PrimaryColor.G,
            colors.PrimaryColor.B,
            colors.PrimaryColor.A);

        TooltipTextPaint = new SolidColorPaint(text);
        TooltipBackgroundPaint = new SolidColorPaint(background);
    }

    public ISeries[] Series { get; set; } =
    [
        new ColumnSeries<double>
        {
            Values = GenerateRandomValues(),
            Fill = new SolidColorPaint(SKColors.Transparent)
        }
    ];

    private static double[] GenerateRandomValues()
    {
        var random = new Random();

        var values = new double[12];
        for (var i = 0; i < values.Length; i++)
        {
            values[i] = random.Next(1000, 5000);
        }

        return values;
    }

    public Axis[] XAxes { get; set; }

    public Axis[] YAxes { get; set; }

    public void Initialize()
    {
        ((ColumnSeries<double>)Series[0]).Values = GenerateRandomValues();
        var primary = ThemeWatcher.ThemeColors.PrimaryColor;
        UpdateSeriesFill(primary);
    }

    public override void Dispose()
    {
        if (_disposed) return;

        ThemeWatcher.ThemeChanged -= OnThemeChanged;

        _disposed = true;
        base.Dispose();
    }
}
