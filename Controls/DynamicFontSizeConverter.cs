using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace MauiCrud.Converters
{
    public class DynamicFontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                int fontSize = 60;

                if (text.Length > 6)
                {
                    fontSize -= (text.Length - 6) * 5;
                }

                if (fontSize < 20)
                {
                    fontSize = 20;
                }

                return fontSize;
            }

            // Default font size
            return 60;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
