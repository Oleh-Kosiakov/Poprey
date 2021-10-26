using Android.Content;
using MvvmCross;
using MvvmCross.Platforms.Android;
using Poprey.Core.Services.Interfaces;

namespace Poprey.Droid.Services
{
    public class WebBrowserService : IWebBrowserService
    {
        public void OpenWebPage(string webSiteUrl)
        {
            var uri = Android.Net.Uri.Parse(webSiteUrl);
            var intent = new Intent(Intent.ActionView, uri);

            var androidCurrentTopAcitvity = Mvx.IoCProvider.Resolve<IMvxAndroidCurrentTopActivity>().Activity;

            androidCurrentTopAcitvity.StartActivity(intent);
        }
    }
}