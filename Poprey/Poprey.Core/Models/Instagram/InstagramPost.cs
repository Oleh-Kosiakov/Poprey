using Newtonsoft.Json;
using Poprey.Core.Rest;

namespace Poprey.Core.Models.Instagram
{
    public class InstagramPost
    {
        [SerializerName("url")]
        [JsonProperty("link")]
        public string PostLink { get; set; }

        [SerializerName("img")]
        [JsonProperty("img")]
        public string PostImageLink { get; set; }

    }
}