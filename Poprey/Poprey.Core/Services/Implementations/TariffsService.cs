using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Poprey.Core.Analytics.Interfaces;
using Poprey.Core.Models.Analytics;
using Poprey.Core.Rest.Interfaces;
using Poprey.Core.Services.Interfaces;
using InstagramTariffs = Poprey.Core.Models.Instagram.Tariffs;
using AdditionalTariffs = Poprey.Core.Models.AdditionalServices.Tariffs;


namespace Poprey.Core.Services.Implementations
{
    public class TariffsService : ITariffsService
    {
        private readonly IPopreyApiClient _apiClient;
        private readonly IAnalyticsService _analyticsService;

        private List<InstagramTariffs.TariffSystem> _tariffSystems;
        private List<AdditionalTariffs.TariffSystem> _additionalTariffSystems;

        public TariffsService(IPopreyApiClient apiClient, IAnalyticsService analyticsService)
        {
            _apiClient = apiClient;
            _analyticsService = analyticsService;
        }

        public async Task<List<InstagramTariffs.TariffSystem>> GetTariffs(string instagramNickname, CancellationToken ct)
        {
            if (_tariffSystems == null)
            {
                var tariffResponse = await _apiClient.GetTariffs(instagramNickname, ct);
                RemoveDisabledPlans(tariffResponse.TariffServices);
                _tariffSystems = tariffResponse.TariffServices;
            }

            return _tariffSystems;
        }

        private void RemoveDisabledPlans(List<InstagramTariffs.TariffSystem> tariffSystems)
        {
            foreach (var tariffSystem in tariffSystems)
            {
                foreach (var tariffService in tariffSystem.TariffServices)
                {
                    foreach (var tariffType in tariffService.TariffTypes)
                    {
                        tariffType.TariffItems = tariffType.TariffItems.Where(ti => ti.Disabled != 1).ToList();
                    }
                }
            }
        }

        public async Task<List<AdditionalTariffs.TariffSystem>> GetTariffsForAdditionalServices(CancellationToken ct)
        {
            if (_additionalTariffSystems == null)
            {
                var tariffResponse = await _apiClient.GetTariffsForAdditionalServices(ct);
                _additionalTariffSystems = tariffResponse.TariffServices;
            }

            ThrowIfAdditionalTariffsAreInvalid(_additionalTariffSystems);

            return _additionalTariffSystems;
        }

        public List<InstagramTariffs.TariffSystem> GetCachedTariffs()
        {
            return _tariffSystems;
        }

        public List<AdditionalTariffs.TariffSystem> GetCachedTariffsForAdditionalServices()
        {
            return _additionalTariffSystems;
        }

        public bool IsTariffServiceAvailable(string tariffServiceName)
        {
            var tariffItems = _tariffSystems.SelectMany(ts => ts.TariffServices).First(ts => ts.Name == tariffServiceName).TariffTypes.SelectMany(ts => ts.TariffItems).ToList();
            return tariffItems.Any(ti => ti.Disabled == 0 && ti.Price != default);
        }

        public InstagramTariffs.ExtraItem GetInstagramExtraImpressionsForTariff(InstagramTariffs.TariffItem tariffItem, string serviceName)
        {
            var tariffService = _tariffSystems.First(s => s.Name == "Instagram").TariffServices.First(s => s.Name == serviceName);

            if (tariffService.Extras?.Impressions == null)
            {
                return null;
            }

            if (tariffService.Extras.ImpressionsDisabled == 1)
            {
                return null;
            }

            var extraItem = tariffService.Extras.Impressions.FirstOrDefault(ex => ex.Name == tariffItem.Name);

            return extraItem;
        }

        private void ThrowIfAdditionalTariffsAreInvalid(List<AdditionalTariffs.TariffSystem> tariffSystems)
        {
            bool tikTokFailed = false, youTubeFailed = false;

            try
            {
                //Ensuring that youtube service and views & subscribers services exist
                var youtubeSystem = tariffSystems.First(ts => ts.Name == Constants.YoutubeSystemKey);
                // ReSharper disable twice ReturnValueOfPureMethodIsNotUsed
                youtubeSystem.TariffServices.First(ts => ts.Name == Constants.YoutubeViewsServiceKey);
                youtubeSystem.TariffServices.First(ts => ts.Name == Constants.YoutubeSubscribersServiceKey);
            }
            catch (InvalidOperationException e)
            {
                youTubeFailed = true;
                _analyticsService.TrackException(e, RequestImprotance.Critical);
            }

            try
            {
                //Ensuring that tiktok service and views & subscribers services exist
                var tiktokSystem = tariffSystems.First(ts => ts.Name == Constants.TikTokSystemKey);
                // ReSharper disable twice ReturnValueOfPureMethodIsNotUsed
                var system = tiktokSystem.TariffServices.First(ts => ts.Name == Constants.TikTokLikesServiceKey);
                tiktokSystem.TariffServices.First(ts => ts.Name == Constants.TikTokFunsServiceKey);
            }
            catch (InvalidOperationException e)
            {
                tikTokFailed = true;

                _analyticsService.TrackException(e, RequestImprotance.Critical);
            }

            if (youTubeFailed && tikTokFailed)
            {
                var exception = new ServiceException(ServiceResolution.AdditionalTariffsConfigurationIncorrect, "Additional services are currently not available.");
                throw exception;
            }

            if (tikTokFailed && !youTubeFailed)
            {
                var exception = new ServiceException(ServiceResolution.TikTokValidationFailed);
                throw exception;
            }

            if (!tikTokFailed && youTubeFailed)
            {
                var exception = new ServiceException(ServiceResolution.YoutubeValidationFailed);
                throw exception;
            }
        }

    }
}