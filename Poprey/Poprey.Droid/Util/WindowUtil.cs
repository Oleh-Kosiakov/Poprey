using Android.Content;
using Android.Graphics;
using Android.Runtime;
using Android.Util;
using Android.Views;

namespace Poprey.Droid.Util
{
    public static class WindowUtil
    {
        public static int GetFullScreenHeight(Context context)
        {
            var wm = context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            var display = wm.DefaultDisplay;
            var point = new Point();
            display.GetSize(point);
            var screenHeight = point.Y;

            return screenHeight;
        }

        public static int GetRealScreenHeight(Context context)
        {
            var wm = context.GetSystemService(Context.WindowService).JavaCast<IWindowManager>();
            var display = wm.DefaultDisplay;
            var displayMetrics = new DisplayMetrics();

            display.GetRealMetrics(displayMetrics);

            return displayMetrics.HeightPixels;
        }
    }
}