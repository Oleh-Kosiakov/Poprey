using System;
using MvvmCross;
using MvvmCross.Navigation;
using Poprey.Core.Analytics.Interfaces;
using Poprey.Core.Messages;
using Poprey.Core.Models.Instagram;
using Poprey.Core.Resources;
using Poprey.Core.Services.Interfaces;
using Poprey.Core.Util;
using Poprey.Core.ViewModels.InstagramMenuItems;

namespace Poprey.Core.ViewModels
{
    public class InstagramMenuViewModel : BaseViewModel<InstagramAccount>
    {
        private ITariffsService _tariffsService;

        public string InstagramServiceName => Strings.Instagram;

        public InstagramLikeMenuItemViewModel InstagramLikeMenuItemViewModel { get; } 
        public InstagramFollowersMenuItemViewModel InstagramFollowersMenuItemViewModel { get; }
        public InstagramViewsMenuItemViewModel InstagramViewsMenuItemViewModel { get; } 
        public InstagramAutoMenuItemViewModel InstagramAutoMenuItemViewModel { get; }
        public InstagramCommentsItemViewModel InstagramCommentsItemViewModel { get; } 

        public InstagramMenuViewModel(IMvxNavigationService navigationService, IAnalyticsService analyticsService, MessageTokenHelper messenger, IPopupService popupService, ITariffsService tariffsService)
                                     : base(navigationService, analyticsService, messenger, popupService)
        {
            _tariffsService = tariffsService;

            MainViewModel.InstagramViewModel = this;

            InstagramLikeMenuItemViewModel = IsLikesAvailable ? Mvx.IoCProvider.Resolve<InstagramLikeMenuItemViewModel>() : null;
            InstagramFollowersMenuItemViewModel = IsFollowersAvailable ? Mvx.IoCProvider.Resolve<InstagramFollowersMenuItemViewModel>() : null;
            InstagramViewsMenuItemViewModel = IsViewsAvailable ? Mvx.IoCProvider.Resolve<InstagramViewsMenuItemViewModel>() : null;
            InstagramAutoMenuItemViewModel = IsAutoLikesAvailable ? Mvx.IoCProvider.Resolve<InstagramAutoMenuItemViewModel>() : null;
            InstagramCommentsItemViewModel = IsCommentsAvailable ? Mvx.IoCProvider.Resolve<InstagramCommentsItemViewModel>() : null;
        }
        public override void Prepare(InstagramAccount parameter)
        {
            if (InstagramLikeMenuItemViewModel != null)
            {
                InstagramLikeMenuItemViewModel.InstagramAccount = parameter;
            }

            if (InstagramFollowersMenuItemViewModel != null)
            {
                InstagramFollowersMenuItemViewModel.InstagramAccount = parameter;
            }

            if (InstagramViewsMenuItemViewModel != null)
            {
                InstagramViewsMenuItemViewModel.InstagramAccount = parameter;
            }

            if (InstagramAutoMenuItemViewModel != null)
            {
                InstagramAutoMenuItemViewModel.InstagramAccount = parameter;
            }

            if (InstagramCommentsItemViewModel != null)
            {
                InstagramCommentsItemViewModel.InstagramAccount = parameter;
            }

            Mvx.IoCProvider.Resolve<BagViewModelSingleton>().InstagramAccount = parameter;
        }

        public override void ViewAppearing()
        {
            base.ViewAppearing();

            Messenger.Publish(new SetCollapsedBagDataMessage(this)
            {
                IsBagVisible = true
            });

            Messenger.Publish( new IgnoreKeyboardOnBagMessage(this, false));
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

        public bool IsLikesAvailable => _tariffsService.IsTariffServiceAvailable(Constants.InstagramLikesServiceKey);
        public bool IsFollowersAvailable => _tariffsService.IsTariffServiceAvailable(Constants.InstagramFollowersServiceKey);
        public bool IsViewsAvailable => _tariffsService.IsTariffServiceAvailable(Constants.InstagramViewsServiceKey);
        public bool IsAutoLikesAvailable => _tariffsService.IsTariffServiceAvailable(Constants.InstagramAutoLikesServiceKey) || _tariffsService.IsTariffServiceAvailable(Constants.InstagramAutoLikesSubscriptionServiceKey);
        public bool IsCommentsAvailable => _tariffsService.IsTariffServiceAvailable(Constants.InstagramCommentsServiceKey);
    }
}
