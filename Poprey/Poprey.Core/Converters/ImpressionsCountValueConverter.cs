using System;
using System.Globalization;
using MvvmCross.Converters;

namespace Poprey.Core.Converters
{
    public class ImpressionsCountValueConverter : MvxValueConverter<int, string>
    {
        protected override string Convert(int value, Type targetType, object parameter, CultureInfo culture)
        {
            var nfi = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
            nfi.NumberGroupSeparator = " ";

            var formattedCount = $"{value.ToString("#,0", nfi)}";

            return $"+ {formattedCount} Impressions";
        }
    }
}