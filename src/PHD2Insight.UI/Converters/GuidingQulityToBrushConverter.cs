using Avalonia.Data.Converters;
using Avalonia.Media;
using PHD2Insight.Analysis.Models;
using System;
using System.Globalization;

namespace PHD2Insight.UI;

public sealed class GuidingQualityToBrushConverter : IValueConverter {
    public object? Convert(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture) {
        return value switch {
            GuidingQuality.Good =>
                new SolidColorBrush(Color.Parse("#4CAF50")),

            GuidingQuality.Acceptable =>
                new SolidColorBrush(Color.Parse("#FFC107")),

            GuidingQuality.Poor =>
                new SolidColorBrush(Color.Parse("#F44336")),

            _ =>
                new SolidColorBrush(Color.Parse("#808080"))
        };
    }

    public object? ConvertBack(
        object? value,
        Type targetType,
        object? parameter,
        CultureInfo culture) {
        throw new NotSupportedException();
    }
}