using System.Collections.Generic;
using Newtonsoft.Json;
using Poprey.Core.Rest.Converters;

namespace Poprey.Core.Models.AdditionalServices.Tariffs
{
    [JsonConverter(typeof(AdditionalServicesConverter))]
    public class AddsTariffsResponse
    {
        public List<TariffSystem> TariffServices { get; set; }
    }
}