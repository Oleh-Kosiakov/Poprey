using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Microsoft.AppCenter.Crashes;
using Poprey.Core.Analytics.Interfaces;
using Poprey.Core.Models.Analytics;
using Poprey.Core.Rest.Models;
using AppCenterAnalytics = Microsoft.AppCenter.Analytics;

namespace Poprey.Core.Analytics.Implementations
{
    public class AnalyticsService : IAnalyticsService
    {
        public void TrackEvent(string eventName, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "")
        {
            AppCenterAnalytics.Analytics.TrackEvent(eventName, new Dictionary<string, string>
            {
                { "method", memberName },
                { "file", filePath }
              });
        }

        public void TrackEvent(string eventName, IDictionary<string, string> parameters)
        {
            AppCenterAnalytics.Analytics.TrackEvent(eventName, parameters);
        }

        public void TrackException(Exception e, RequestImprotance severity, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "")
        {
            Crashes.TrackError(e, new Dictionary<string, string>
            {
                { "severity",severity.ToString()},
                { "method", memberName },
                { "file", filePath}
            });
        }

        public void TrackApiException(ApiException e, RequestImprotance severity, [CallerMemberName] string memberName = "", [CallerFilePath] string filePath = "")
        {
            Crashes.TrackError(e, new Dictionary<string, string>
            {
                {"errorCode", e.ErrorCode.ToString() },
                {"errorText", e.ErrorText },
                { "severity",severity.ToString()},
                { "method", memberName },
                { "file", filePath}
            });
        }

        public void IdentifyUser(string email, IDictionary<string, string> parameters)
        {
            AppCenterAnalytics.Analytics.TrackEvent(email, parameters);
        }
    }
}