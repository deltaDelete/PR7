using System;
using System.Globalization;
using Avalonia.Data;
using Avalonia.Data.Converters;

namespace App.Utils; 

public class TimeSpanToDateTimeBindingConverter : IValueConverter {
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (value is DateTimeOffset dateTime) {
            return dateTime.TimeOfDay;
        }
        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture) {
        if (value is TimeSpan timeSpan && parameter is DateTimeOffset dateTime) {
            return new DateTimeOffset(
                dateTime.Year,
                dateTime.Month,
                dateTime.Day,
                timeSpan.Hours,
                timeSpan.Minutes,
                timeSpan.Seconds,
                timeSpan.Milliseconds,
                dateTime.Offset
            );
        }
        return new BindingNotification(new InvalidCastException(), BindingErrorType.Error);
    }
}