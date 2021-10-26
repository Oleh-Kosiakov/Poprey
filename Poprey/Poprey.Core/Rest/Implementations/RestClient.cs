using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using MvvmCross.Base;
using MvvmCross.Logging;
using Plugin.Connectivity;
using Poprey.Core.Analytics.Interfaces;
using Poprey.Core.Models.Analytics;
using Poprey.Core.Rest.Interfaces;
using Poprey.Core.Rest.Models;

namespace Poprey.Core.Rest.Implementations
{
    public class RestClient : IRestClient
    {
        private CookieCollection _cookies;
        private DateTime _lastDownloadedCsrfTokenDateTime = DateTime.MinValue;
        private string _csfrToken;

        private readonly IMvxJsonConverter _jsonConverter;
        private readonly IMvxLog _mvxLog;
        private readonly IAnalyticsService _analyticsService;

        public RestClient(IMvxJsonConverter jsonConverter, IMvxLog mvxLog, IAnalyticsService analyticsService)
        {
            _jsonConverter = jsonConverter;
            _mvxLog = mvxLog;
            _analyticsService = analyticsService;

         
        }

        public async Task<TResult> MakeApiCall<TResult>(CancellationToken ct, string url, HttpMethod method, object data = null) where TResult : class
        {
            if (!url.Contains("http"))
                url = $"{Constants.BaseUrl}/{url}";

            if (!CrossConnectivity.Current.IsConnected)
            {
                throw new NotConnectedException($"No internet during request {url}");
            }

            //Refresh CSFR each 59 mins
            if (NeedsToRefreshCsfrToken())
            {
                await RefreshCsfrToken();
            }

            if (ct.IsCancellationRequested)
            {
                throw new TaskCanceledException();
            }

            var container = new CookieContainer();
            container.Add(_cookies);

            var httpClientHandler = new HttpClientHandler
            {
                CookieContainer = container,
                UseCookies = true
            };

            using (var httpClient = new HttpClient(httpClientHandler))
            {
                using (var request = new HttpRequestMessageBuilder(new Uri(url), method).WithObjectContent(data, _csfrToken))
                {
                    var response = new HttpResponseMessage();
                    try
                    {
                        response = await httpClient.SendAsync(request, ct).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _mvxLog.ErrorException("MakeApiCall failed", ex);
                        throw;
                    }

                    var stringSerialized = await response.Content.ReadAsStringAsync().ConfigureAwait(false);

                    // deserialize content
                    var deserializedObject = _jsonConverter.DeserializeObject<ApiResponse<TResult>>(stringSerialized);

                    if (deserializedObject.ErrorCode != null)
                    {
                        throw new ApiException(deserializedObject.ErrorCode.Value, deserializedObject.ErrorText, _csfrToken);
                    }

                    RefreshCsrfToken(deserializedObject.CsrfToken);

                    return deserializedObject.Data;
                }
            }
        }

        public CookieContainer GetCookieContainer()
        {
            var cc = new CookieContainer();
            cc.Add(_cookies);

            return cc;
        }

        private void RefreshCsrfToken(string csrfToken)
        {
            _csfrToken = csrfToken;
            _lastDownloadedCsrfTokenDateTime = DateTime.UtcNow;
        }

        private bool NeedsToRefreshCsfrToken()
        {
            return _lastDownloadedCsrfTokenDateTime + Constants.CsrfTokenLife <= DateTime.UtcNow + TimeSpan.FromMinutes(1) || _csfrToken == null;
        }

        private async Task RefreshCsfrToken()
        {
            var uri = new Uri($"{Constants.BaseUrl}/{ApiEndpoints.CsfrTokenEndpoint}");
            var cookieContainer = new CookieContainer();
            var handler = new HttpClientHandler {CookieContainer = cookieContainer, UseCookies = true};

            using (var httpClient = new HttpClient(handler))
            {
                using (var request = new HttpRequestMessageBuilder(uri, HttpMethod.Post)
                                        .WithContent(new StringContent("get=1", Encoding.UTF8, "application/x-www-form-urlencoded")))
                {
                    var response = new HttpResponseMessage();
                    try
                    {
                        response = await httpClient.SendAsync(request).ConfigureAwait(false);
                    }
                    catch (Exception ex)
                    {
                        _mvxLog.ErrorException("Csfr token refresh failed!", ex);
                        _analyticsService.TrackException(ex, RequestImprotance.Critical);

                        throw;
                    }

                    var stringSerialized = await response.Content.ReadAsStringAsync().ConfigureAwait(false);
                    _cookies = cookieContainer.GetCookies(uri);

                    // deserialize content
                    var deserializedObject = _jsonConverter.DeserializeObject<ApiResponse<object>>(stringSerialized);
                    RefreshCsrfToken(deserializedObject.CsrfToken);
                }
            }
        }
    }
}
