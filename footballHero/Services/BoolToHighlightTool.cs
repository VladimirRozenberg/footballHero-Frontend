using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;

namespace footballHero.Services;

public class BoolToHighlightConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is true ? new SolidColorBrush(Color.Parse("#38c6a1")) : Brushes.Transparent;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        => throw new NotSupportedException();
}