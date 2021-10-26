using Newtonsoft.Json;

namespace Poprey.Core.Models.Instagram.Tariffs
{
    public class TariffItem
    {
        public int Name { get; set; }

        [JsonProperty("price")]
        public double Price { get; set; }

        [JsonProperty("discount")]
        public int Discount { get; set; }

        [JsonProperty("discount_proc")]
        public int DiscountPercent { get; set; }

        [JsonProperty("disabled")]
        public int Disabled { get; set; }
    }
}