using System;
using System.Threading.Tasks;
using MvvmCross;
using MvvmCross.Navigation;
using MvvmCross.ViewModels;
using Plugin.Connectivity;
using Plugin.Connectivity.Abstractions;
using Poprey.Core.Analytics.Interfaces;
using Poprey.Core.Models.Analytics;
using Poprey.Core.Rest;
using Poprey.Core.Rest.Models;
using Poprey.Core.Services;
using Poprey.Core.Services.Interfaces;
using Poprey.Core.Util;

namespace Poprey.Core.ViewModels
{
    public abstract class BaseViewModel<TParam> : MvxViewModel<TParam>, IDisposable
    {
        protected readonly IAnalyticsService AnalyticsService;
        protected readonly IMvxNavigationService NavigationService;
        protected readonly IPopupService PopupService;
        protected readonly CtsHelper CtsHelper;
        protected readonly MessageTokenHelper Messenger;

        public HeaderViewModelSingleton HeaderViewModel { get; set; } = Mvx.IoCProvider.Resolve<HeaderViewModelSingleton>();

        public virtual string PageName { get; }

        public virtual bool ShouldTrackPageOpen => true;

        protected BaseViewModel(IMvxNavigationService navigationService, IAnalyticsService analyticsService, MessageTokenHelper messenger, IPopupService popupService)
        {
            AnalyticsService = analyticsService;
            NavigationService = navigationService;
            Messenger = messenger;
            PopupService = popupService;

            CrossConnectivity.Current.ConnectivityChanged += OnConnectivityStatusChanged;

            CtsHelper = new CtsHelper();
        }

        public override void ViewAppeared()
        {
            TrackPageOpenIfEnabled();
        }

        public override void ViewDestroy(bool viewFinishing = true)
        {
            CtsHelper.CancelAllTokens();
        }

        protected async Task<ServiceResolution> WebRequest(Func<Task> webRequestTask, RequestImprotance requestImprotance = RequestImprotance.Warning)
        {
            try
            {
                PopupService.ShowLoading();

                await webRequestTask.Invoke();

                return ServiceResolution.Success;
            }
            catch (NotConnectedException)
            {
                HeaderViewModel.IsConnected = false;
                return ServiceResolution.NetworkError;
            }
            catch (ApiException e)
            {
                var managedToFind =
                    HttpCodes.ServiceResolutionsHttpErrorsCode.TryGetValue(e.ErrorCode, out var serviceResolution);

                return managedToFind ? serviceResolution : ServiceResolution.UnknownApiError;
            }
            catch (ServiceException e)
            {
                if (!string.IsNullOrEmpty(e.Message))
                {
                    HeaderViewModel.ExplicitlyShowError(e.Message);
                }

                return e.KnownErrorCode;
            }
            catch (Exception e)
            {
                HeaderViewModel.ExplicitlyShowError("Unknown error.");
                AnalyticsService.TrackException(e, requestImprotance);

                return ServiceResolution.UnknownError;
            }
            finally
            {
                PopupService.HideLoading();
            }
        }

        private void OnConnectivityStatusChanged(object sender, ConnectivityChangedEventArgs connectivityChangedEventArgs)
        {
            var isConnected = connectivityChangedEventArgs.IsConnected;
            HeaderViewModel.IsConnected = isConnected;

            var internetStatusAnalyticsString = isConnected
                ? $"On {PageName} restored connection"
                : $"On {PageName} lost connection";

            AnalyticsService.TrackEvent(internetStatusAnalyticsString);
        }

        private void TrackPageOpenIfEnabled()
        {
            if (ShouldTrackPageOpen)
            {
                AnalyticsService.TrackEvent($"{PageName} opened");
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                CrossConnectivity.Current.ConnectivityChanged -= OnConnectivityStatusChanged;
            }
        }

        public override void Prepare(TParam parameter)
        {
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }
}
