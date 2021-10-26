using Newtonsoft.Json;
using Plugin.Settings;
using Plugin.Settings.Abstractions;

namespace Poprey.Core
{
    public class AppSettings
    {
        /// <summary>
        /// This is the Settings static class that can be used in your Core solution or in any
        /// of your client applications. All settings are laid out the same exact way with getters
        /// and setters. 
        /// </summary>

        private static readonly string SettingsDefault = string.Empty;

        private static ISettings PopreySettings => CrossSettings.Current;

        public static class Keys
        {
            public const string InstagramAccountsKey = "InstagramAccountsSettingsKey";
            public const string InstagramTenFollowersLastUsedDatesKey = "InstagramTenFollowersLastUsedDatesSettingsKey";
        }

        public static TModel GetRecordsForModel<TModel>(string key)
        {
            var serializedObject = PopreySettings.GetValueOrDefault(key, SettingsDefault);

            return JsonConvert.DeserializeObject<TModel>(serializedObject);
        }

        public static void SetRecordsForModel<TModel>(TModel model, string key)
        {
            var serializedObject = JsonConvert.SerializeObject(model);
            PopreySettings.AddOrUpdateValue(key, serializedObject);
        }
    }
}
