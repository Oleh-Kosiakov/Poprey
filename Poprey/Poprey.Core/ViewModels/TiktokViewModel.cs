using MvvmCross;
using MvvmCross.Navigation;
using Poprey.Core.Analytics.Interfaces;
using Poprey.Core.Messages;
using Poprey.Core.Services.Interfaces;
using Poprey.Core.Util;
using Poprey.Core.ViewModels.AdditionalServicesMenuItems;

namespace Poprey.Core.ViewModels
{
    public class TikTokViewModel : BaseViewModel<object>
    {
        public TikTokLikesItemViewModel TikTokLikesItemViewModel { get; } = Mvx.IoCProvider.Resolve<TikTokLikesItemViewModel>();
        public TikTokFunsItemViewModel TikTokFunsItemViewModel { get; } = Mvx.IoCProvider.Resolve<TikTokFunsItemViewModel>();
        public string TikTokServiceName => "TikTok";

        public TikTokViewModel(IMvxNavigationService navigationService, IAnalyticsService analyticsService, MessageTokenHelper messenger, IPopupService popupService) 
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
