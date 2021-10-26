using System;
using System.Globalization;
using Android.Text;
using MvvmCross.Converters;

namespace Poprey.Droid.Converters
{
    public class FromHtmlValueConverter : MvxValueConverter<string, ISpanned>
    {
        protected override ISpanned Convert(string value, Type targetType, object parameter, CultureInfo culture)
        {
            return Html.FromHtml(value);
        }
    }
}