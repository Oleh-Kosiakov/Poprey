using System;
using System.Globalization;
using MvvmCross.Converters;

namespace Poprey.Core.Converters
{
    public class BagServiceCountValueConverter : MvxValueConverter<int, string>
    {
        protected override string Convert(int value, Type targetType, object parameter, CultureInfo culture)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            return $" — {value.ToString("#,0", nfi)}";
        }
    }
}