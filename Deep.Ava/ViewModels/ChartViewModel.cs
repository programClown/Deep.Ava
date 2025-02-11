using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore.SkiaSharpView.Avalonia;
using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;

namespace Deep.Ava.ViewModels;

public partial class ChartViewModel : ViewModelBase
{
    [ObservableProperty] private object? _chartPage;
    [ObservableProperty] private string? _selectedItem;

    public ChartViewModel()
    {
        Items = new ObservableCollection<string>
        {
            "散点图", "面积图", "饼图", "动态波", "雷达图", "柱状图"
        };
        SelectedItem = Items[0];
    }

    public ObservableCollection<string> Items { get; set; }

    public void Clear()
    {
        SelectedItem = null;
    }


    partial void OnSelectedItemChanged(string? value)
    {
        if (value == null) return;

        ChartPage = value switch
        {
            "散点图" => new CartesianChart
            {
                Width = 600,
                Height = 400,
                Series =
                [
                    new LineSeries<double>
                    {
                        Values = [-2, -1, 3, 5, 3, 4, 6],
                        // Set he Fill property to build an area series
                        // by default the series has a fill color based on your app theme
                        Fill = new SolidColorPaint(SKColors.CornflowerBlue), // mark

                        Stroke = null,
                        GeometryFill = null,
                        GeometryStroke = null
                    }
                ],
                DrawMarginFrame = new DrawMarginFrame
                {
                    Fill = new SolidColorPaint(new SKColor(220, 220, 220)),
                    Stroke = new SolidColorPaint(new SKColor(180, 180, 180), 1)
                }
            },

            //
            // "面积图" => new AreaChartView(),
            // "饼图" => new PieChartView(),
            // "动态波" => new WaveChartView(),
            // "雷达图" => new RadarChartView(),
            // "柱状图" => new BarChartView(),
            _ => null
        };
    }
}