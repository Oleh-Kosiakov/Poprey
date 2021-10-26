using System.Collections.Generic;
using Newtonsoft.Json;
using Poprey.Core.Rest.Converters;
using Poprey.Core.Util;

namespace Poprey.Core.Models.Instagram.Tariffs
{
    [JsonConverter(typeof(InstagramTariffConverter))]
    public class TariffsResponse
    {
        public List<TariffSystem> TariffServices { get; set; }
    }
}