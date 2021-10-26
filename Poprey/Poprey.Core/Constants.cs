using System;

namespace Poprey.Core
{
    public static class Constants
    {
        public const string BaseUrl = "https://beta2.poprey.com";

        public const string WebSiteUrl = "https://poprey.com/";

        public const string ContactUsUrl = "https://poprey.com/";

        public const string FaqUrl = "https://poprey.com/faq.php";

        public const string TermsOfServiceUrl = "https://poprey.com/rules.php";

        public const string PrivacyPolicyUrl = "https://poprey.com/rules.php";

        public const string AppCenterDevKey = "d22d0d37-7ab9-4d3f-bf53-fa3c688ebdd9";

        public static readonly TimeSpan CsrfTokenLife = TimeSpan.FromMinutes(60);

        public const int PopupMilliseconds = 3000;

        public const int HashtagPageTickDelay = 2000;

        public const int MaxSavedAccountsNumber = 5;

        public const int NewPostsInitialValueDivider = 5;

        public const int InstagramPostBulkSize = 6;

        public const int NewPostsLimit = 99;

        public const int MaxSelectedPostsPreviews = 9;

        // Services And Systems

        public const string InstagramSystemKey = "Instagram";

        public const string InstagramAutoLikesServiceKey = "Auto-Likes";

        public const string InstagramAutoLikesSubscriptionServiceKey = "Auto-Likes Subs";

        public const string InstagramCommentsServiceKey = "Comments";

        public const string InstagramFollowersServiceKey = "Followers";

        public const string InstagramLikesServiceKey = "Likes";

        public const string InstagramViewsServiceKey = "Views";

        public const string YoutubeSystemKey = "YouTube";

        public const string YoutubeViewsServiceKey = "Views";

        public const string YoutubeSubscribersServiceKey = "Subscribers";

        public const string TikTokSystemKey = "Tik Tok";

        public const string TikTokLikesServiceKey = "Likes";

        public const string TikTokFunsServiceKey = "Followers";

        //Tariff Plans

        public const string InstagramLikesInstant = "instant";

        public const string InstagramLikesGradualSpeed1 = "gradual_speed1";

        public const string InstagramLikesGradualSpeed = "gradual";

        public const string InstagramLikesGradualSpeed2 = "gradual_speed2";

        public const string InstagramLikesGradualSpeed3 = "gradual_speed3";

        public const string InstagramViewsInstant = "instant";

        public const string InstagramViewsGradualSpeed1 = "gradual_speed1";

        public const string InstagramViewsGradualSpeed = "gradual";

        public const string InstagramViewsGradualSpeed2 = "gradual_speed2";

        public const string InstagramViewsGradualSpeed3 = "gradual_speed3";

        public const string InstagramAutoLikesGradual = "gradual";

        public const string InstagramFollowersPremium = "premium";

        public const string InstagramFollowersNormal = "normal";

        public const string InstagramCommentsCustom = "custom";

        public const string InstagramCommentsRandom = "random";
    }
}
