using System.Globalization;
using System.Windows.Data;
using System;

namespace KGuiV2.Helpers.ValueConverters
{
    internal class InverseBooleanConverter : IValueConverter
    {
        /// <inheritdoc/>
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
            => !System.Convert.ToBoolean(value, culture);

        /// <inheritdoc/>
        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => throw new NotImplementedException();
    }
}