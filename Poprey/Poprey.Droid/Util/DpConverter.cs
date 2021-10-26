using System;
using Android.Content.Res;
using Android.Util;

namespace Poprey.Droid.Util
{
    public static class DpConverter
    {
        public static float ConvertPxToDp(float px, Resources r)
        {
            return TypedValue.ApplyDimension(ComplexUnitType.Dip, px, r.DisplayMetrics);
        }

        public static int ConvertDpToPx(float dp, Resources r)
        {
            return (int) Math.Ceiling(dp * r.DisplayMetrics.Density);
        }
    }
}