using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using Xamarin.Forms;

namespace RedSpartan.Mvvm.Converters
{
    public class BoolInverterConverter : IValueConverter
    {
        #region IValueConverter Implementation
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() == typeof(bool))
                return !(bool)value;

            throw new InvalidCastException($"Cannot invert type '{value.GetType()}'");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value.GetType() == typeof(bool))
                return !(bool)value;

            throw new InvalidCastException($"Cannot invert type '{value.GetType()}'");
        }

        #endregion IValueConverter Implementation
    }
}
