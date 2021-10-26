namespace Poprey.Core.Services
{
    public enum ServiceResolution
    {
        //Generic
        Success,
        NetworkError,
        UnknownApiError,
        UnknownError,
        UnknownServiceError,

        //Hashtags
        SimilarHashtagsNotFound,

        //Instagram
        EmailIsIncorrect,
        ShowCustomErrorMessage,
        PassedTariffIsNotCorrect,
        AccountClosedOrDoesNotExists,
        NoPostsInTheAccount,
        SuchAccountAlreadyAdded,
        InstagramAccountsLimitReached,
        UserIsBanned,
        TestWasUsedDuringLast24Hours,
        TestOrdersOverallLimitReached,

        //Additional services
        AdditionalTariffsConfigurationIncorrect,
        TikTokValidationFailed,
        YoutubeValidationFailed
    }
}