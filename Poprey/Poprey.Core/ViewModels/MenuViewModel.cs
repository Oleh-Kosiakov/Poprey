using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Poprey.Core.Analytics.Interfaces;
using Poprey.Core.DisplayModels;
using Poprey.Core.Resources;
using Poprey.Core.Services.Interfaces;
using Poprey.Core.Util;

namespace Poprey.Core.ViewModels
{
    public class MenuViewModel : BaseViewModel<object>
    {
        private readonly IWebBrowserService _webBrowserService;
        private readonly MenuItem _visitSiteItem = new MenuItem { Text = Strings.MenuVisitSite, Url = Constants.WebSiteUrl };

        public MvxObservableCollection<MenuItem> MenuItemsCollection { get; } =
            new MvxObservableCollection<MenuItem>
            {
                new MenuItem {Text = Strings.MenuContactUs, Url = Constants.ContactUsUrl},
                new MenuItem {Text = Strings.MenuFaq, Url = Constants.FaqUrl},
                new MenuItem {Text = Strings.MenuTermsOfService, Url = Constants.TermsOfServiceUrl},
                new MenuItem {Text = Strings.MenuPrivacyPolicy, Url =Constants.PrivacyPolicyUrl}
            };

        public string VisitSiteItemText { get; } = Strings.MenuVisitSite;
        public string PopreyText { get; } = Strings.Poprey;

        public MenuViewModel(IMvxNavigationService navigationService, IAnalyticsService analyticsService,
                             IWebBrowserService webBrowserService, MessageTokenHelper messenger, IPopupService popupService)
            : base(navigationService, analyticsService, messenger, popupService)
        {
            _webBrowserService = webBrowserService;
        }

        private IMvxCommand _menuItemClickedCommand;
        public IMvxCommand MenuItemClickedCommand => _menuItemClickedCommand ?? (_menuItemClickedCommand = new MvxCommand<MenuItem>(
                                                        menuItem =>
                                                        {
                                                            _webBrowserService.OpenWebPage(menuItem.Url);
                                                        }));


        private IMvxCommand _visitSiteCommand;
        public IMvxCommand VisitSiteCommand => _visitSiteCommand ?? (_visitSiteCommand = new MvxCommand(
                                                         () =>
                                                         {
                                                             _webBrowserService.OpenWebPage(_visitSiteItem.Url);
                                                         }));
    }
}
