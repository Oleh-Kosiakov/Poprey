using Android.App;
using Android.Content.PM;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using MvvmCross.Platforms.Android.Views;

namespace Poprey.Droid
{
    [MvxActivityPresentation]
    [Activity(
        MainLauncher = true,
        Icon = "@mipmap/icon",
        Theme = "@style/AppTheme.Launcher",
        NoHistory = true,
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class SplashScreen : MvxSplashScreenActivity
    {
    }
}