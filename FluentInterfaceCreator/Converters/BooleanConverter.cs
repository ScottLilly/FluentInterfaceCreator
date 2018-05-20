using System;
using System.Collections.Generic;
using System.Globalization;
using System.Windows.Data;

namespace FluentInterfaceCreator.Converters
{
    public class BooleanConverter<T> : IValueConverter
    {
        protected BooleanConverter(T trueVisibility, T falseVisibility)
        {
            True = trueVisibility;
            False = falseVisibility;
        }

        public T True { get; set; }
        public T False { get; set; }

        public virtual object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is bool b && b ? True : False;
        }

        public virtual object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value is T variable && EqualityComparer<T>.Default.Equals(variable, True);
        }
    }
}