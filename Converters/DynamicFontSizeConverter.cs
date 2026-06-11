using System;
using System.Globalization;
using Microsoft.Maui.Controls;

namespace N5StudyApp.Converters
{
    public class DynamicFontSizeConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string text)
            {
                // Auto-shrink logic based on character count
                if (text.Length >= 6) return 42; // Very long words (Shrink a lot)
                if (text.Length >= 4) return 54; // Medium words like エンジニア (Shrink a little)
                
                return 72; // Short 1-3 character Kanji (Keep it massive!)
            }
            return 72;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) 
            => throw new NotImplementedException();
    }
}