using MvvmCross.Commands;
using MvvmCross.Navigation;
using Poprey.Core.Analytics.Interfaces;
using Poprey.Core.DisplayModels;
using Poprey.Core.Services.Interfaces;
using Poprey.Core.Util;

namespace Poprey.Core.ViewModels
{
    public class ThankYouViewModel : BaseViewModel<ThanYouPageArguments>
    {
        public ThankYouViewModel(IMvxNavigationService navigationService, IAnalyticsService analyticsService, MessageTokenHelper messenger, IPopupService popupService)
            : base(navigationService, analyticsService, messenger, popupService)
        {
        }

        public bool PriceVisible { get; set; }

        public bool SubpriceVisible { get; set; }

        public double Price { get; set; } = 120.56;

        public string HeaderText { get; set; } = "Thank you for\r\n you purchase";

        public string SubheaderText { get; set; } = "Soon you will get your 10 free Followers, come back tomorrow to get new one.\r\nEnjoy it!";

        public string SubpriceText { get; set; } = "You saved on discount";

        public string ComeBackText { get; set; } = "Come back";

        private IMvxCommand _goBackCommand;
        public IMvxCommand GoBackCommand => _goBackCommand = _goBackCommand ?? new MvxAsyncCommand(async () =>
                                                                 {
                                                                     await NavigationService.Close(this);
                                                                 });

        public override void Prepare(ThanYouPageArguments parameter)
        {
            HeaderText = parameter.HeaderText;
            SubheaderText = parameter.SubheaderText;
            Price = parameter.Price;
            SubpriceText = parameter.SubpriceText;
            PriceVisible = parameter.PriceVisible;
            SubpriceVisible = parameter.SubpriceVisible;
        }
    }
}