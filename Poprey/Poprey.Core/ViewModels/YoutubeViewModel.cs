using MvvmCross;
using MvvmCross.Navigation;
using Poprey.Core.Analytics.Interfaces;
using Poprey.Core.Messages;
using Poprey.Core.Services.Interfaces;
using Poprey.Core.Util;
using Poprey.Core.ViewModels.AdditionalServicesMenuItems;

namespace Poprey.Core.ViewModels
{
    public class YoutubeViewModel : BaseViewModel<object>
    {
        public YoutubeViewsItemViewModel YoutubeViewsItemViewModel { get; } = Mvx.IoCProvider.Resolve<YoutubeViewsItemViewModel>();
        public YoutubeSubscribersItemViewModel YoutubeSubscribersItemViewModel { get; } = Mvx.IoCProvider.Resolve<YoutubeSubscribersItemViewModel>();
        public string YoutubeServiceName => "YouTube";

        public YoutubeViewModel(IMvxNavigationService navigationService, IAnalyticsService analyticsService, MessageTokenHelper messenger, IPopupService popupService) 
            : base(navigationService, analyticsService, messenger, popupService)
        {
            MainViewModel.AdditionalServicesViewModel = this;
        }

        public override void ViewAppearing()
        {
            base.ViewAppearing();

            Messenger.Publish(new SetCollapsedBagDataMessage(this)
            {
                IsBagVisible = true
            });

            Messenger.Publish(new IgnoreKeyboardOnBagMessage(this, false));
        }

        public override void ViewDisappearing()
        {
            base.ViewDisappearing();

            Messenger.Publish(new SetCollapsedBagDataMessage(this)
            {
                IsBagVisible = false
            });

            Messenger.Publish(new IgnoreKeyboardOnBagMessage(this, true));
        }
    }
}