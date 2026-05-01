using System;
using Avalonia.Media;
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
    public ThemeWatcher ThemeWatcher { get; }

    private readonly SolidColorPaint _xAxisLabelsPaint = new() { Color = SKColors.Gray };
    private readonly SolidColorPaint _yAxisLabelsPaint = new() { Color = SKColors.Gray };

    public static SolidColorPaint TooltipTextPaint { get; } = new() { Color = SKColors.Black };

    public DashboardViewModel(PageManager pageManager, ThemeWatcher themeWatcher)
    {
        _pageManager = pageManager;
        ThemeWatcher = themeWatcher;
        ThemeWatcher.ThemeChanged += (_, colors) =>
        {
            UpdateAxesLabelPaints(colors);
            UpdateSeriesFill(colors.PrimaryColor);
        };

        XAxes =
        [
            new Axis
            {
                Labels = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"],
                LabelsPaint = _xAxisLabelsPaint,
                TextSize = 12,
                MinStep = 1
            }
        ];

        YAxes =
        [
            new Axis
            {
                Labeler = Labelers.Currency,
                LabelsPaint = _yAxisLabelsPaint,
                TextSize = 12,
                MinStep = 1500,
                ShowSeparatorLines = false
            }
        ];
    }

    [RelayCommand]
    private void NextPage()
    {
        _pageManager.Navigate<ThemeViewModel>();
    }

    private void UpdateSeriesFill(Color primary)
    {
        var color = new SKColor(primary.R, primary.G, primary.B, primary.A);
        if (Series.Length > 0) ((ColumnSeries<double>)Series[0]).Fill = new SolidColorPaint(color);
    }

    private void UpdateAxesLabelPaints(ThemeColors colors)
    {
        var foreground = new SKColor(
            colors.ForegroundColor.R,
            colors.ForegroundColor.G,
            colors.ForegroundColor.B,
            colors.ForegroundColor.A);

        _xAxisLabelsPaint.Color = foreground;
        _yAxisLabelsPaint.Color = foreground;
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
}