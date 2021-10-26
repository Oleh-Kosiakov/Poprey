using System;
using System.Globalization;
using Android.Views;
using MvvmCross.Converters;

namespace Poprey.Droid.Converters
{
    public class InvertedGoneVisibilityValueConverter : MvxValueConverter<bool, ViewStates>
    {
        protected override ViewStates Convert(bool value, Type targetType, object parameter, CultureInfo culture)
        {
            return value ? ViewStates.Gone : ViewStates.Visible;
        }
    }
}