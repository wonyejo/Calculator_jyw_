using System;
using System.Globalization;
using System.Windows.Data;

namespace Calculator_jyw_
{
    public class MultiConverter : IMultiValueConverter
    {
        public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
        {
            string selectedExpression = values[0]?.ToString();
            string resultText = values[1]?.ToString();

            if (!string.IsNullOrEmpty(selectedExpression))
            {
                return $"{selectedExpression} {resultText}";
            }

            return resultText;
        }

        public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
