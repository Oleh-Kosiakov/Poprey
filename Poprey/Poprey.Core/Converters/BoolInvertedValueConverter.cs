using System;
using System.Globalization;
using MvvmCross.Converters;

namespace Poprey.Core.Converters
{
    public class BoolInvertedValueConverter : MvxValueConverter<bool, bool>
    {
        protected override bool Convert(bool value, Type targetType, object parameter, CultureInfo culture)
        {
            return !value;
        }
    }
}