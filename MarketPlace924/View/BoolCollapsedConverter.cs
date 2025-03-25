using System;
using Microsoft.UI.Xaml;
using Microsoft.UI.Xaml.Data;

namespace MarketPlace924.View;

public class BoolCollapsedConverter: IValueConverter
{
    public object Convert(object value, Type targetType, object parameter, string language)
    {
        if (value is bool boolValue)
        {
            return boolValue ?Visibility.Collapsed : Visibility.Visible;
        }
        return Visibility.Visible;
    }

    public object ConvertBack(object value, Type targetType, object parameter, string language)
    {
        return value is Visibility visibility && visibility == Visibility.Collapsed;
    }
}