using System;
using System.Collections.Generic;
using System.Globalization;
using MvvmCross;
using MvvmCross.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Poprey.Core.Models.AdditionalServices.Tariffs;
using TariffService = Poprey.Core.Models.AdditionalServices.Tariffs.TariffService;
using AddsTariffsResponse = Poprey.Core.Models.AdditionalServices.Tariffs.AddsTariffsResponse;
using TariffSystem = Poprey.Core.Models.AdditionalServices.Tariffs.TariffSystem;

namespace Poprey.Core.Rest.Converters
{
    public class AdditionalServicesConverter : JsonConverter
    {
        private readonly IMvxLog _mvxLog;
        private const string PlansJsonKey = "plans";

        public AdditionalServicesConverter()
        {
            _mvxLog = Mvx.IoCProvider.Resolve<IMvxLog>();
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            throw new NotImplementedException();
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            var jo = JObject.Load(reader);
            var tariffSystems = new List<TariffSystem>();

            foreach (var property in jo.Properties())
            {
                LogAndIgnoreIfException(() =>
                {
                    var tariffService = ParseTariffSystem(property);
                    tariffSystems.Add(tariffService);
                });
            }

            var tariffResponse = new AddsTariffsResponse { TariffServices = tariffSystems };
            return tariffResponse;
        }

        private TariffSystem ParseTariffSystem(JProperty property)
        {
            var jo = JObject.Parse(property.Value.ToString());

            var tariffSystem = new TariffSystem
            {
                Name = property.Name
            };

            var tariffServices = new List<TariffService>();

            foreach (var prop in jo.Properties())
            {
                LogAndIgnoreIfException(() =>
                {
                    var tariffService = ParseTariffService(prop);
                    tariffServices.Add(tariffService);
                });
            }

            tariffSystem.TariffServices = tariffServices;

            return tariffSystem;
        }

        private TariffService ParseTariffService(JProperty property)
        {
            var jo = JObject.Parse(property.Value.ToString());

            var tariffService = new TariffService
            {
                Name = property.Name
            };

            List<TariffItem> tariffItems = null;

            foreach (var prop in jo.Properties())
            {
                LogAndIgnoreIfException(() =>
                {
                    if (prop.Name == PlansJsonKey)
                    {
                        tariffItems = ParseTariffItems(prop);
                    }
                });
            }

            tariffService.TariffItems = tariffItems;

            return tariffService;
        }

        private List<TariffItem> ParseTariffItems(JProperty property)
        {
            var jo = JObject.Parse(property.Value.ToString());

            var tariffItems = new List<TariffItem>();

            foreach (var prop in jo.Properties())
            {
                LogAndIgnoreIfException(() =>
                {
                    var tariffItem = new TariffItem
                    {
                        Name = int.Parse(prop.Name.Replace(" ", "")),
                        Price = ParseDouble(prop.Value.ToString())
                    };
                    tariffItems.Add(tariffItem);
                });
            }

            return tariffItems;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(AddsTariffsResponse);
        }

        private double ParseDouble(string str)
        {
            str = str.Replace(" ", "");
            str = str.Replace(",", ".");

            double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out var value);

            return value;
        }

        private void LogAndIgnoreIfException(Action action)
        {
            try
            {
                action?.Invoke();
            }
            catch (Exception e)
            {
                _mvxLog.Log(MvxLogLevel.Warn, () => e.Message, e);
            }
        }
    }
}