using System;
using MvvmCross;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Poprey.Core.Analytics.Interfaces;
using Poprey.Core.Messages;
using Poprey.Core.Services.Interfaces;
using Poprey.Core.Util;

namespace Poprey.Core.ViewModels
{
    public class MainViewModel : BaseViewModel<object>
    {
        private static readonly Type FirstPageType = typeof(InstagramViewModel);

        public BagViewModelSingleton BagViewModel => Mvx.IoCProvider.Resolve<BagViewModelSingleton>();

        public MainViewModel(IMvxNavigationService navigationService, IAnalyticsService analyticsService, MessageTokenHelper messenger, IPopupService popupService) 
            : base(navigationService,analyticsService, messenger, popupService)
        {
            ShowFirstViewModelCommand = PrepareNavigationCommand(FirstPageType);
            ShowMenuViewModelCommand = PrepareNavigationCommand(typeof(MenuViewModel));

            Messenger.Subscribe<SwitchPageMessage>(SwitchMainPage);
        }

        public static IMvxViewModel InstagramViewModel { get; set; }
        public static IMvxViewModel AdditionalServicesViewModel { get; set; }
        public static IMvxViewModel HashtagViewModel { get; set; }

        private async void SwitchMainPage(SwitchPageMessage switchPageMessage)
        {
            if (switchPageMessage.SwitchToType == typeof(InstagramViewModel))
            {
                if (InstagramViewModel == null)
                {
                   InstagramViewModel = Mvx.IoCProvider.Resolve<InstagramViewModel>();
                }

                await NavigationService.Navigate(InstagramViewModel);
            }
            else if (switchPageMessage.SwitchToType == typeof(HashtagsViewModel))
            {
                if (HashtagViewModel == null)
                {
                  HashtagViewModel = Mvx.IoCProvider.Resolve<HashtagsViewModel>();
                }

                await NavigationService.Navigate(HashtagViewModel);
            }
            else if (switchPageMessage.SwitchToType == typeof(AdditionalServicesViewModel))
            {
                if (AdditionalServicesViewModel == null)
                {
                   AdditionalServicesViewModel = Mvx.IoCProvider.Resolve<AdditionalServicesViewModel>();
                }

                await NavigationService.Navigate(AdditionalServicesViewModel);
            }
        }

        public IMvxAsyncCommand ShowFirstViewModelCommand { get; }
        public IMvxAsyncCommand ShowMenuViewModelCommand { get; }

        private IMvxAsyncCommand PrepareNavigationCommand(Type vmType)
        {
            return new MvxAsyncCommand(async () => await NavigationService.Navigate(vmType));
        }

        public void KeyboardToggled(bool isAppeared)
        {
            BagViewModel.NotifyAboutKeyboard(isAppeared);
        }
    }
}
