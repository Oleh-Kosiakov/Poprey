using System.Collections.Generic;
using System.Threading.Tasks;
using MvvmCross.Commands;
using MvvmCross.Navigation;
using Poprey.Core.Analytics.Interfaces;
using Poprey.Core.Messages;
using Poprey.Core.Models.AdditionalServices.Tariffs;
using Poprey.Core.Services;
using Poprey.Core.Services.Interfaces;
using Poprey.Core.Util;

namespace Poprey.Core.ViewModels
{
    public class AdditionalServicesViewModel : BaseViewModel<object>
    {
        private readonly ITariffsService _tariffsService;
        private List<TariffSystem> _tariffSystems;

        public AdditionalServicesViewModel(IMvxNavigationService navigationService, IAnalyticsService analyticsService,
                                            MessageTokenHelper messenger, IPopupService popupService, ITariffsService tariffsService)
                                            : base(navigationService, analyticsService, messenger, popupService)
        {
            _tariffsService = tariffsService;
            MainViewModel.AdditionalServicesViewModel = this;
        }
        private async Task<ServiceResolution> LoadTariffItems()
        {
            if (_tariffSystems != null)
            {
                return ServiceResolution.Success;
            }

            var responseCode = await WebRequest(async () =>
            {
                _tariffSystems = await _tariffsService.GetTariffsForAdditionalServices(CtsHelper.CreateCts().Token);
            });

            return responseCode;
        }

        #region Labels

        public string YoutubeLabelText => "YouTube";
        public string YoutubeServicesLabelText => "Views, Subscribers";

        public string TikTokLabelText => "Tik Tok";
        public string TikTokServicesLabelText => "Likes, Funs";

        #endregion

        #region Commands

        private IMvxCommand _youtubeSelectedCommand;
        public IMvxCommand YoutubeSelectedCommand => _youtubeSelectedCommand = _youtubeSelectedCommand ?? new MvxAsyncCommand(async () =>
        {
            var statusCode = await LoadTariffItems();

            if (statusCode == ServiceResolution.YoutubeValidationFailed)
            {
                HeaderViewModel.ExplicitlyShowError("Youtube services are currently not available.");
            }

            if (statusCode != ServiceResolution.Success && statusCode != ServiceResolution.TikTokValidationFailed)
            {
                return;
            }

            Messenger.Publish(new EmptifyBagMessage(this));

            await NavigationService.Navigate<YoutubeViewModel>();
        });

        private IMvxCommand _tikTokSelectedCommand;
        public IMvxCommand TikTokSelectedCommand => _tikTokSelectedCommand = _tikTokSelectedCommand ?? new MvxAsyncCommand(async () =>
        {
            var statusCode = await LoadTariffItems();

            if (statusCode == ServiceResolution.TikTokValidationFailed)
            {
                HeaderViewModel.ExplicitlyShowError("TikTok services are currently not available.");
            }

            if (statusCode != ServiceResolution.Success && statusCode != ServiceResolution.YoutubeValidationFailed)
            {
                return;
            }

            Messenger.Publish(new EmptifyBagMessage(this));

            await NavigationService.Navigate<TikTokViewModel>();
        });

        #endregion
    }
}