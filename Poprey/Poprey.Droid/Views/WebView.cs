using System.Net;
using Android.App;
using Android.Content.PM;
using Android.Webkit;
using MvvmCross.Binding.BindingContext;
using MvvmCross.Platforms.Android.Presenters.Attributes;
using Poprey.Core;
using Poprey.Core.ViewModels;

namespace Poprey.Droid.Views
{
    [MvxActivityPresentation]
    [Activity(Label = "Poprey",
        Theme = "@style/AppTheme",
        Name = "poprey.droid.views.WebView",
        ScreenOrientation = ScreenOrientation.Portrait)]
    public class WebView : BaseAppCompatActivity<WebViewModel>
    {
        protected override int ContentViewId => Resource.Layout.WebView;

        private Android.Webkit.WebView _webView;

        protected override void InitComponents()
        {
            _webView = FindViewById<Android.Webkit.WebView>(Resource.Id.web_view);
        }

        protected override void ApplyBindings()
        {
            var bindingSet = this.CreateBindingSet<WebView, WebViewModel>();

            bindingSet.Bind(this).For(v => v.CookieContainer).To(vm => vm.CookieContainer).OneTime();

            bindingSet.Apply();
        }

        public CookieContainer CookieContainer
        {
            get => null;
            set
            {
                var cookieManager = CookieManager.Instance;
                cookieManager.SetAcceptCookie(true);
                cookieManager.RemoveAllCookie();
                var cookies = value.GetCookies(new System.Uri(Constants.BaseUrl));

                for (var i = 0; i < cookies.Count; i++)
                {
                    var cookieValue = cookies[i].Value;
                    var cookieDomain = cookies[i].Domain;
                    var cookieName = cookies[i].Name;
                    cookieManager.SetCookie(cookieDomain, cookieName + "=" + cookieValue);
                }
            }
        }

        protected override void OnResume()
        {
            base.OnResume();

            _webView.LoadUrl(ViewModel.UrlToOpen);
        }
    }
}