using System.Collections.Generic;
using Poprey.Core.Services;

namespace Poprey.Core.Rest
{
    public static class HttpCodes
    {
        public static Dictionary<int, ServiceResolution> ServiceResolutionsHttpErrorsCode { get; } =
            new Dictionary<int, ServiceResolution>
            {
                [200] = ServiceResolution.Success,

                [999] = ServiceResolution.SimilarHashtagsNotFound,

                [101] = ServiceResolution.PassedTariffIsNotCorrect,
                [102] = ServiceResolution.EmailIsIncorrect,
                [203] = ServiceResolution.AccountClosedOrDoesNotExists,
                [202] = ServiceResolution.NoPostsInTheAccount,
                
                [301] = ServiceResolution.UserIsBanned,
                [303] = ServiceResolution.TestWasUsedDuringLast24Hours,
                [304] = ServiceResolution.TestOrdersOverallLimitReached
            };
    }
}