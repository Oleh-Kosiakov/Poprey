using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Poprey.Core.Models.Analytics;
using Poprey.Core.Rest.Models;

namespace Poprey.Core.Analytics.Interfaces
{
    public interface IAnalyticsService
    {
        void TrackEvent(string eventName, [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "");

        void TrackEvent(string eventName, IDictionary<string, string> parameters);

        void TrackException(Exception e, RequestImprotance severity, [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "");

        void TrackApiException(ApiException e, RequestImprotance severity, [CallerMemberName] string memberName = "",
            [CallerFilePath] string filePath = "");

        void IdentifyUser(string email, IDictionary<string, string> parameters);
    }
}