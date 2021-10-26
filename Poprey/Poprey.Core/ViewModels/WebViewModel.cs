using System.Net;
using MvvmCross.Navigation;
using Poprey.Core.Analytics.Interfaces;
using Poprey.Core.Rest.Interfaces;
using Poprey.Core.Services.Interfaces;
using Poprey.Core.Util;

namespace Poprey.Core.ViewModels
{
    public class WebViewModel : BaseViewModel<string>
    {
        public WebViewModel(IMvxNavigationService navigationService, IAnalyticsService analyticsService, MessageTokenHelper messenger, IPopupService popupService, IRestClient restClient) 
            : base(navigationService, analyticsService, messenger, popupService)
        {
            CookieContainer = restClient.GetCookieContainer();
        }

        public override void Prepare(string parameter)
        {
            UrlToOpen = parameter;
        }

        public string UrlToOpen { get; set; }

        public CookieContainer CookieContainer { get; set; }
    }
}