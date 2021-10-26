using System;
using System.Globalization;
using MvvmCross.Converters;

namespace Poprey.Core.Converters
{
    public class BagSumValueConverter : MvxValueConverter<double, string>
    {
        protected override string Convert(double value, Type targetType, object parameter, CultureInfo culture)
        {
            return "$" + value.ToString("F");
        }
    }
}