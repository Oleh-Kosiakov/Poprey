using Newtonsoft.Json;

namespace Poprey.Core.Models.Bag
{
    public class OrderResponse
    {
        [JsonProperty("price")]
        public double Price { get; set; }

        [JsonProperty("redirect")]
        public string Redirect { get; set; }
    }
}
