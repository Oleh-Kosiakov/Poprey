using System;
using Newtonsoft.Json;

namespace Poprey.Core.Models.AdditionalServices.Tariffs
{
    public class TariffItem
    {
        public int Name { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }
    }
}