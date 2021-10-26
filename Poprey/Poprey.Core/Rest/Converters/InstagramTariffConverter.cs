using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using MvvmCross;
using MvvmCross.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Poprey.Core.Models.Instagram.Tariffs;

namespace Poprey.Core.Rest.Converters
{
    public class InstagramTariffConverter : JsonConverter
    {
        private const string ExtraJsonKey = "extra";
        private const string ExtraImpressionsJsonKey = "impressions";
        private const string ExtraReachJsonKey = "reach";
        private const string ExtraSaveJsonKey = "saves";

        private readonly IMvxLog _mvxLog;
        public InstagramTariffConverter()
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

            var tariffResponse = new TariffsResponse { TariffServices = tariffSystems };
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

            var tariffTypes = new List<TariffType>();

            foreach (var prop in jo.Properties())
            {
                LogAndIgnoreIfException(() =>
                {
                    if (prop.Name != ExtraJsonKey)
                    {
                        var tariffType = ParseTariffType(prop);
                        tariffTypes.Add(tariffType);
                    }
                    else
                    {
                        tariffService.Extras = ParseTariffExtras(prop);
                    }
                });
            }

            tariffService.TariffTypes = tariffTypes;

            return tariffService;
        }


        private TariffType ParseTariffType(JProperty property)
        {
            var jo = JObject.Parse(property.Value.ToString());

            var tariffType = new TariffType
            {
                Name = property.Name
            };

            var tariffTypes = new List<TariffItem>();

            foreach (var prop in jo.Properties())
            {
                LogAndIgnoreIfException(() =>
                {
                    var tariffItem = ParseTariffItem(prop);
                    tariffTypes.Add(tariffItem);
                });
            }

            tariffType.TariffItems = tariffTypes;

            return tariffType;
        }

        private TariffItem ParseTariffItem(JProperty property)
        {
            var tariffItem = property.Value.ToObject<TariffItem>();
            tariffItem.Name = ParseInt(property.Name);
            return tariffItem;
        }

        private Extras ParseTariffExtras(JProperty property)
        {
            var jo = JObject.Parse(property.Value.ToString());
            var extras = new Extras();

            var impressionsProperty = jo.Properties().FirstOrDefault(p => p.Name == ExtraImpressionsJsonKey);
            if (impressionsProperty != null)
            {
                var impressionsObject = JObject.Parse(impressionsProperty.Value.ToString());
                extras.Impressions = new List<ExtraItem>();

                foreach (var prop in impressionsObject.Properties())
                {
                    if (prop.Name == "disabled")
                    {
                        extras.ImpressionsDisabled = ParseInt(prop.Value.ToString());
                    }
                    else
                    {
                        var item = new ExtraItem
                        {
                            Name = ParseInt(prop.Name),
                            Price = ParseDouble(prop.Value.ToString())
                        };
                        extras.Impressions.Add(item);
                    }
                }
            }

            var reachProperty = jo.Properties().FirstOrDefault(p => p.Name == ExtraReachJsonKey);
            if (reachProperty != null)
            {
                var reachObject = JObject.Parse(reachProperty.Value.ToString());
                extras.Reaches = new List<ExtraItem>();

                foreach (var prop in reachObject.Properties())
                {
                    if (prop.Name == "disabled")
                    {
                        extras.ReachesDisabled = ParseInt(prop.Value.ToString());
                    }
                    else
                    {
                        var item = new ExtraItem
                        {
                            Name = ParseInt(prop.Name),
                            Price = ParseDouble(prop.Value.ToString())
                        };

                        extras.Reaches.Add(item);
                    }

                }
            }

            var savesProperty = jo.Properties().FirstOrDefault(p => p.Name == ExtraSaveJsonKey);
            if (savesProperty != null)
            {
                var savesObject = JObject.Parse(savesProperty.Value.ToString());
                extras.Saves = new List<ExtraItem>();

                foreach (var prop in savesObject.Properties())
                {
                    if (prop.Name == "disabled")
                    {
                        extras.SavesDisabled = ParseInt(prop.Value.ToString());
                    }
                    else
                    {
                        var item = new ExtraItem
                        {
                            Name = ParseInt(prop.Name),
                            Price = ParseDouble(prop.Value.ToString())
                        };
                        extras.Saves.Add(item);
                    }
                }
            }

            return extras;
        }

        public override bool CanConvert(Type objectType)
        {
            return objectType == typeof(TariffsResponse);
        }

        private double ParseDouble(string str)
        {
            str = str.Replace(" ", "");
            str = str.Replace(",", ".");

            double.TryParse(str, NumberStyles.Any, CultureInfo.InvariantCulture, out var value);

            return value;
        }

        private int ParseInt(string str)
        {
            str = str.Replace(" ", "");

            return int.Parse(str);
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